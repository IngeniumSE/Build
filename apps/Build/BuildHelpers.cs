// Copyright (c) 2021 Ingenium Software Engineering. All rights reserved.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace Build
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;

	using Cake.Common.IO;
	using Cake.Common.Tools.DotNetCore;
	using Cake.Common.Tools.DotNetCore.Build;
	using Cake.Common.Tools.DotNetCore.Publish;
	using Cake.Common.Tools.DotNetCore.Test;
	using Cake.Common.Tools.MSBuild;
	using Cake.Common.Tools.NuGet;
	using Cake.Common.Tools.NuGet.Restore;
	using Cake.Common.Tools.VSTest;
	using Cake.Common.Xml;
	using Cake.Core;
	using Cake.Core.IO;
	using Cake.FileHelpers;

	/// <summary>
	/// Provides helper methods for building projects.
	/// </summary>
	public static class BuildHelpers
	{
		static readonly XmlPeekSettings StandardXmlPeekSettings = new XmlPeekSettings()
		{
			SuppressWarning = true,
			Namespaces = new Dictionary<string, string>
			{
				["msbuild"] = "http://schemas.microsoft.com/developer/msbuild/2003"
			}
		};

		/// <summary>
		/// Builds the target project.
		/// </summary>
		/// <param name="context">The build context.</param>
		/// <param name="project">The project to build.</param>
		/// <param name="publish">States whether to publish the output.</param>
		/// <param name="outputPath">The output path for publishing.</param>
		public static void BuildProject(
			this BuildContext context,
			BuildProject project,
			bool publish = false,
			DirectoryPath? outputPath = default)
		{
			context = context ?? throw new ArgumentNullException(nameof(context));
			project = project ?? throw new ArgumentNullException(nameof(project));

			if (project.BuildEngine == BuildEngine.MSBuild)
			{
				// MA - Force a NuGet restore
				context.NuGetRestore(project.ProjectFilePath, new NuGetRestoreSettings());

				var settings = new MSBuildSettings()
				{
					Configuration = context.Configuration,
					ArgumentCustomization = args =>
					{
						if (context.Version is not null)
						{
							args.Append($"/p:SemVer={context.Version}");
						}

						if (publish && outputPath is not null)
						{
							if (project.ApplicationType == ApplicationType.Web)
							{
								args.Append($"/p:_PackageTempDir={outputPath.FullPath}");
							}
							else
							{
								args.Append($"/p:OutputPath={outputPath.FullPath}");
							}
						}

						if (context.SolutionPath is not null)
						{
							args.Append($"/p:SolutionDir={context.SolutionPath.FullPath}");
						}

						return args;
					}
				};

				if (publish && outputPath is not null && project.ApplicationType == ApplicationType.Web)
				{
					settings.Targets.Add("PipelinePreDeployCopyAllFilesToOneFolder");
				}

				context.MSBuild(project.ProjectFilePath.FullPath, settings);
			}
			else
			{
				foreach (string targetFramework in project.TargetFrameworks)
				{
					if (publish && outputPath is not null)
					{
						var settings = new DotNetCorePublishSettings()
						{
							Configuration = context.Configuration,
							ArgumentCustomization = args =>
							{
								if (context.Version is not null)
								{
									args.Append($"/p:SemVer={context.Version}");
								}

								if (context.SolutionPath is not null)
								{
									args.Append($"/p:SolutionDir={context.SolutionPath.FullPath}");
								}

								return args;
							},
							Framework = targetFramework
						};

						context.DotNetCorePublish(project.ProjectFilePath.FullPath, settings);
					}
					else
					{
						var settings = new DotNetCoreBuildSettings()
						{
							Configuration = context.Configuration,
							ArgumentCustomization = args =>
							{
								if (context.Version is not null)
								{
									args.Append($"/p:SemVer={context.Version}");
								}

								if (context.SolutionPath is not null)
								{
									args.Append($"/p:SolutionDir={context.SolutionPath.FullPath}");
								}

								return args;
							},
							Framework = targetFramework
						};

						context.DotNetCoreBuild(project.ProjectFilePath.FullPath, settings);
					}
				}
			}
		}

		/// <summary>
		/// Tests the target project.
		/// </summary>
		/// <param name="context">The build context.</param>
		/// <param name="project">The project to build.</param>
		public static void TestProject(
			this BuildContext context,
			BuildProject project)
		{
			string name = project.ProjectFilePath.GetFilenameWithoutExtension().Segments.Last();
			string resultsFile = $"{name}.xml";

			if (project.BuildEngine == BuildEngine.MSBuild)
			{
				BuildProject(context, project);

				resultsFile = FilePath.FromString($"./{resultsFile}").MakeAbsolute(context.TestResultsPath).FullPath;

				var settings = new VSTestSettings
				{
					ToolPath = context.Tools.Resolve("vstest.console.exe"),
					TestAdapterPath = GetTestAdapterLocation(context, project),
					Logger = $"trx;LogFileName={resultsFile}"
				};

				context.VSTest(project.ProjectFilePath.GetDirectory() + $"/bin/{context.Configuration}/**/*.Tests.dll", settings);
			}
			else
			{
				foreach (string framework in project.TargetFrameworks)
				{
					resultsFile = $"{name}-{framework}.xml";

					var settings = new DotNetCoreTestSettings
					{
						NoBuild = project.HasBuilt,
						Configuration = context.Configuration,
						Framework = framework,
						ResultsDirectory = context.TestResultsPath,
						Logger = $"trx;LogFileName={resultsFile}",
						ArgumentCustomization = args =>
						{
							args.Append($"/p:SolutionDir={context.SolutionPath.FullPath}");
							return args;
						}
					};

					context.DotNetCoreTest(project.ProjectFilePath.FullPath, settings);
				}
			}
		}

		/// <summary>
		/// Gets the build projects available in the current repository.
		/// </summary>
		/// <param name="context">The Cake context.</param>
		/// <param name="root">The repository path.</param>
		/// <param name="settings">The build settings.</param>
		/// <returns>The set of build projects.</returns>
		public static IReadOnlyDictionary<BuildType, IReadOnlyCollection<BuildProject>> GetBuildProjects(
			this ICakeContext context,
			DirectoryPath root,
			BuildSettings? settings = default)
		{
			IReadOnlyCollection<BuildProject> GetBuildProjectsInternal(string path, BuildType type)
			{
				var fullPath = root.Combine(new DirectoryPath(path));

				List<BuildProject> projects = new();
				foreach (var project in context.GetFiles(fullPath.FullPath))
				{
					if (!IsProjectExcluded(context, project, root, settings))
					{
						projects.Add(GetBuildProject(context, type, project, root));
					}
				}

				return new ReadOnlyCollection<BuildProject>(projects.ToList());
			}

			Dictionary<BuildType, IReadOnlyCollection<BuildProject>> projects = new();

			projects.Add(BuildType.Application, GetBuildProjectsInternal(BuildPaths.ApplicationPathConvention, BuildType.Application));
			projects.Add(BuildType.Content, GetBuildProjectsInternal(BuildPaths.ContentPathConvention, BuildType.Content));
			projects.Add(BuildType.Data, GetBuildProjectsInternal(BuildPaths.DataPathConvention, BuildType.Data));
			projects.Add(BuildType.Library, GetBuildProjectsInternal(BuildPaths.LibraryPathConvention, BuildType.Library));
			projects.Add(BuildType.Test, GetBuildProjectsInternal(BuildPaths.TestPathConvention, BuildType.Test));

			return new ReadOnlyDictionary<BuildType, IReadOnlyCollection<BuildProject>>(projects);
		}

		static BuildProject GetBuildProject(ICakeContext context, BuildType type, FilePath projectFilePath, DirectoryPath root)
		{
			string? sdk = context.XmlPeek(projectFilePath, "/Project/@Sdk");
			string? targetFrameworkProperty = GetMsBuildPropertyValue(context, projectFilePath, "TargetFramework");
			string? targetFrameworksProperty = GetMsBuildPropertyValue(context, projectFilePath, "TargetFrameworks");

			var engine = (sdk is null ? BuildEngine.MSBuild : BuildEngine.DotNetSdk);
			if (Enum.TryParse<BuildEngine>(
						GetMsBuildPropertyValue(context, projectFilePath, "Ing_ForceBuildEngine", checkNamespace: false),
						out var forceEngine))
			{
				engine = forceEngine;
			}

			var targetFrameworks = Array.Empty<string>();
			if (targetFrameworksProperty is { Length: > 0 })
			{
				targetFrameworks = targetFrameworksProperty.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
			}
			else if (targetFrameworkProperty is { Length: > 0 })
			{
				targetFrameworks = new[] { targetFrameworkProperty };
			}

			TestFramework? testFramework = default;
			if (type == BuildType.Test)
			{
				testFramework = GetTestFramework(context, projectFilePath, root);
			}

			var applicationType = ApplicationType.Console;
			if (engine == BuildEngine.MSBuild)
			{
				if (context.FindTextInFiles(new[] { projectFilePath }, "Microsoft.WebApplication.targets").Any())
				{
					applicationType = ApplicationType.Web;
				}
			}

			string? packageTypeProperty = GetMsBuildPropertyValue(context, projectFilePath, "PackageType");
			string? isPackable = GetMsBuildPropertyValue(context, projectFilePath, "IsPackable");
			PackageType packageType;
			if (!Enum.TryParse(packageTypeProperty, out packageType))
			{
				packageType = PackageType.None;
			}

			if (type == BuildType.Library)
			{
				if (string.Equals("true", isPackable, StringComparison.OrdinalIgnoreCase) ||
						isPackable is null)
				{
					packageType = PackageType.Nuget;
				}
			}

			if (type == BuildType.Library)
			{
				string? isPackableProperty = GetMsBuildPropertyValue(context, projectFilePath, "IsPackable");
				if (string.Equals(isPackableProperty, "false", StringComparison.OrdinalIgnoreCase))
				{
					packageType = PackageType.None;
				}
			}

			return new BuildProject(
				projectFilePath.GetFilenameWithoutExtension().Segments.Last(),
				targetFrameworks,
				projectFilePath,
				root,
				type,
				engine,
				applicationType,
				testFramework,
				packageType);
		}

		static string? GetMsBuildPropertyValue(ICakeContext context, FilePath path, string property, bool checkNamespace = true)
		{
			string? value;
			if (checkNamespace)
			{
				value = context.XmlPeek(path, $"/msbuild:Project/msbuild:PropertyGroup/msbuild:{property}", StandardXmlPeekSettings);
				if (value is { Length: > 0 })
				{
					return value;
				}
			}

			value = context.XmlPeek(path, $"/Project/PropertyGroup/{property}", StandardXmlPeekSettings);
			return value;
		}

		static bool IsProjectExcluded(
			ICakeContext context,
			FilePath path,
			DirectoryPath root,
			BuildSettings? settings)
		{
			if (settings is not { ExcludedProjects: { Count: > 0 } })
			{
				return false;
			}

			foreach (var exclusion in settings.ExcludedProjects)
			{
				if (exclusion.Contains("*"))
				{
					var files = context.GetFiles(root + exclusion);
					foreach (var file in files)
					{
						if (PathComparer.Default.Equals(path, file))
						{
							return true;
						}
					}
				}
				else
				{
					var file = ((FilePath)exclusion).MakeAbsolute(root);
					if (PathComparer.Default.Equals(path, file))
					{
						return true;
					}
				}
			}

			return false;
		}

		static TestFramework GetTestFramework(ICakeContext context, FilePath projectFilePath, DirectoryPath root)
		{
			if (MatchesPackageOrReference(context, projectFilePath, "NUnit")
					|| MatchesTestFrameworkSetting(context, projectFilePath, "nunit"))
			{
				return TestFramework.NUnit;
			}

			var files = context.GetFiles(root + "/**/Directory.Build.targets");
			foreach (var file in files)
			{
				if (MatchesPackageOrReference(context, file, "NUnit"))
				{
					return TestFramework.NUnit;
				}
			}

			files = context.GetFiles(root + "/**/Directory.Build.props");
			foreach (var file in files)
			{
				if (MatchesTestFrameworkSetting(context, file, "nunit"))
				{
					return TestFramework.NUnit;
				}
			}

			return TestFramework.XUnit;
		}

		static bool MatchesPackageOrReference(ICakeContext context, FilePath path, string package)
		 => GetMsBuildPropertyValue(context, path, $"PackageReference[@Include='{package}']") is not null
				|| GetMsBuildPropertyValue(context, path, $"Reference[@Include='{package}']") is not null;

		static bool MatchesTestFrameworkSetting(ICakeContext context, FilePath path, string setting)
			=> string.Equals(
				GetMsBuildPropertyValue(context, path, "Ing_TestFramework"),
				setting,
				StringComparison.OrdinalIgnoreCase);

		static DirectoryPath GetTestAdapterLocation(BuildContext context, BuildProject project)
		{
			if (project.TestFramework == TestFramework.NUnit)
			{
				return context.GetDirectories($"./tools/NUnit3TestAdapter.3.16.1/build/{(project.BuildEngine == BuildEngine.MSBuild ? "net35" : "netcoreapp2.1")}/")
					.FirstOrDefault();
			}

			return context.GetDirectories("./tools/xunit.runner.visualstudio.2.4.0/_common/")
				.FirstOrDefault();
		}
	}
}
