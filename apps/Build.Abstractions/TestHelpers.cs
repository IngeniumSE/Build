// Copyright (c) 2021 Ingenium Software Engineering. All rights reserved.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace Build
{
	using System.Collections.Generic;
	using System.Linq;

	using Cake.Common.IO;
	using Cake.Common.Tools.DotNetCore;
	using Cake.Common.Tools.DotNetCore.Test;
	using Cake.Common.Tools.VSTest;
	using Cake.Common.Xml;
	using Cake.Core;
	using Cake.Core.IO;

	/// <summary>
	/// Provides helper methods for testing projects.
	/// </summary>
	public static class TestHelpers
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
				context.BuildProject(project);

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
