// Copyright (c) 2021 Ingenium Software Engineering. All rights reserved.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace Build
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Cake.Common.IO;
	using Cake.Core;
	using Cake.Core.IO;
	using Cake.Frosting;

	/// <summary>
	/// Represents a build context.
	/// </summary>
	public class BuildContext : FrostingContext
	{
		/// <summary>
		/// Initiailises a new instance of <see cref="Build"/>
		/// </summary>
		/// <param name="context">The Cake context.</param>
		public BuildContext(ICakeContext context)
				: base(context)
		{
			Configuration = context.Arguments.GetArgument("configuration") ?? "Release";
			Version = context.Arguments.GetArgument("version");

			(RootPath, SolutionPath) = GetRootPath(context, context.Environment.ApplicationRoot);

			Projects = context.GetBuildProjects(RootPath, new BuildSettings());

			ArtefactsPath = RootPath.Combine(new DirectoryPath(BuildPaths.ArtefactsPath));
			TestResultsPath = RootPath.Combine(new DirectoryPath(BuildPaths.TestResultsPath));
		}

		/// <summary>
		/// Gets the configuration.
		/// </summary>
		public new string Configuration { get; }

		/// <summary>
		/// Gets the resolved set of projects to build.
		/// </summary>
		public IReadOnlyDictionary<BuildType, IReadOnlyCollection<BuildProject>> Projects { get; }

		/// <summary>
		/// Gets the artefacts path.
		/// </summary>
		public DirectoryPath ArtefactsPath { get; }

		/// <summary>
		/// Gets the resolved root path.
		/// </summary>
		public DirectoryPath RootPath { get; }

		/// <summary>
		/// Gets the resolved solution path.
		/// </summary>
		public FilePath SolutionPath { get; }

		/// <summary>
		/// Gets the test results path.
		/// </summary>
		public DirectoryPath TestResultsPath { get; }

		/// <summary>
		/// Gets or sets the version.
		/// </summary>
		public string? Version { get; set; }

		static (DirectoryPath, FilePath) GetRootPath(ICakeContext context, DirectoryPath path)
		{
			// Try and find the .git directory.
			var (isRoot, solution) = IsRoot(context, path);
			if (isRoot)
			{
				return (path, solution!);
			}

			// Walk up the tree until we find a repo root.
			var parent = path.Combine("./../").Collapse();
			return GetRootPath(context, parent);
		}

		static (bool, FilePath?) IsRoot(ICakeContext context, DirectoryPath path)
		{
			/*
			 * We consider the root to be wherever the solution file is located.
			 */

			var solution = path.Combine("*.sln");
			var file = context.GetFiles(solution.FullPath).FirstOrDefault();
			if (file is object && 
				string.Equals("Build", file.GetFilenameWithoutExtension().Segments.Last(), StringComparison.OrdinalIgnoreCase))
			{
				file = default;
			}

			return (file is not null, file);

		}
	}
}
