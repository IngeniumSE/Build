// Copyright (c) 2021 Ingenium Software Engineering. All rights reserved.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace Build
{
	using Cake.Common.IO;
	using Cake.Common.Tools.DotNetCore;
	using Cake.Common.Tools.DotNetCore.Pack;
	using Cake.Core;
	using Cake.Core.IO;
	using Cake.FileHelpers;

	using IOPath = System.IO.Path;

	/// <summary>
	/// Providers helpers for for packing projects.
	/// </summary>
	public static class PackHelpers
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="project"></param>
		public static void PackProject(
			this BuildContext context,
			BuildProject project)
		{
			if (project.PackageType == PackageType.NuGet)
			{
				string specFile = $"{IOPath.GetFileNameWithoutExtension(project.ProjectFilePath.FullPath)}.nuspec";
				var specFilePath = context.MakeAbsolute(
					new FilePath(IOPath.Combine(project.ProjectFilePath.GetDirectory().FullPath, specFile)));
				var tempFilePath = context.MakeAbsolute(new FilePath(BuildPaths.ArtefactsPath + specFile));

				bool useCustomNuSpec = context.FileExists(specFilePath);
				if (useCustomNuSpec)
				{
					context.CopyFile(specFilePath, tempFilePath);
					context.ReplaceTextInFiles(tempFilePath.FullPath, "$(version)", context.Version?.PackageVersion ?? "0.0.0");
					context.ReplaceTextInFiles(tempFilePath.FullPath, "$(configuration)", context.Configuration);
				}

				context.DotNetCorePack(project.ProjectFilePath.FullPath, new DotNetCorePackSettings
				{
					ArgumentCustomization = args =>
					{
						if (context.Version is not null)
						{
							args.Append($"/p:SemVer={context.Version.PackageVersion}");
						}

						if (useCustomNuSpec)
						{
							args.Append($"/p:NuspecFile={tempFilePath.FullPath}");
							args.Append($"/p:NuspecBasePath={context.MakeAbsolute((DirectoryPath)(project.ProjectFilePath.GetDirectory().FullPath))}");
						}

						return args;
					},
					Configuration = context.Configuration,
					OutputDirectory = context.ArtefactsPath,
					NoBuild = project.HasBuilt
				});
			}
		}
	}
}
