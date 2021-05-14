// Copyright (c) 2021 Ingenium Software Engineering. All rights reserved.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace Build
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Diagnostics;
	using System.Linq;

	using Cake.Core.IO;

	/// <summary>
	/// Represents a build project.
	/// </summary>
	[DebuggerDisplay("{DebuggerToString(),nq}")]
	public class BuildProject
	{
		/// <summary>
		/// Represents a build project.
		/// </summary>
		/// <param name="name">The build project name.</param>
		/// <param name="targetFrameworks">The set of target frameworks.</param>
		/// <param name="projectFilePath">The project file path.</param>
		/// <param name="rootPath">The root directory path.</param>
		/// <param name="buildType">The build type.</param>
		/// <param name="buildEngine">The build engine.</param>
		/// <param name="applicationType">The application type.</param>
		/// <param name="packageType">The package type.</param>
		/// <param name="testFramework">The test framework.</param>
		public BuildProject(
			string name,
			IEnumerable<string> targetFrameworks,
			FilePath projectFilePath,
			DirectoryPath rootPath,
			BuildType buildType,
			BuildEngine buildEngine = BuildEngine.DotNetSdk,
			ApplicationType applicationType = ApplicationType.Console,
			TestFramework? testFramework = default,
			PackageType packageType = PackageType.None)
		{
			if (targetFrameworks is null)
			{
				throw new ArgumentNullException(nameof(targetFrameworks));
			}

			Name = name ?? throw new ArgumentNullException(nameof(name));
			TargetFrameworks = new ReadOnlyCollection<string>(targetFrameworks.ToList());
			ProjectFilePath = projectFilePath ?? throw new ArgumentNullException(nameof(projectFilePath));
			RootPath = rootPath ?? throw new ArgumentNullException(nameof(rootPath));
			BuildType = buildType;
			BuildEngine = buildEngine;
			ApplicationType = applicationType;
			TestFramework = testFramework;
			PackageType = packageType;
		}

		/// <summary>
		/// Gets the application type.
		/// </summary>
		public ApplicationType ApplicationType { get; }

		/// <summary>
		/// Gets the build engine.
		/// </summary>
		public BuildEngine BuildEngine { get; }

		/// <summary>
		/// Gets the build type.
		/// </summary>
		public BuildType BuildType { get; }

		/// <summary>
		/// Gets whether the project has been built.
		/// </summary>
		public bool HasBuilt { get; private set; }

		/// <summary>
		/// Gets the project name.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// Gets the package type.
		/// </summary>
		public PackageType PackageType { get; }

		/// <summary>
		/// Gets the project file path.
		/// </summary>
		public FilePath ProjectFilePath { get; }

		/// <summary>
		/// Gets the solution file path.
		/// </summary>
		public DirectoryPath RootPath { get; }

		/// <summary>
		/// Gets the set of target frameworks.
		/// </summary>
		public IReadOnlyCollection<string> TargetFrameworks { get; }

		/// <summary>
		/// Gets the test framework.
		/// </summary>
		public TestFramework? TestFramework { get; }

		/// <summary>
		/// Marks the project as built.
		/// </summary>
		public void MarkAsBuilt() => HasBuilt = true;

		string DebuggerToString()
		{
			switch (BuildType)
			{
				case BuildType.Application:
					return $"App: {Name} ({BuildEngine}, {ApplicationType}, {PackageType}) @ {ProjectFilePath.FullPath}";

				case BuildType.Content:
					return $"Content: {Name} ({BuildEngine}, {PackageType}) @ {ProjectFilePath.FullPath}";

				case BuildType.Data:
					return $"Data: {Name} ({BuildEngine}) @ {ProjectFilePath.FullPath}";

				case BuildType.Library:
					return $"Library: {Name} ({BuildEngine}) @ {ProjectFilePath.FullPath}";

				case BuildType.Test:
					return $"Test: {Name} ({BuildEngine}, {TestFramework}) @ {ProjectFilePath.FullPath}";
			}

			return $"Unknown: {Name} ({BuildEngine}) @ {ProjectFilePath.FullPath}";
		}
	}
}
