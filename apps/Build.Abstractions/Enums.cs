// Copyright (c) 2021 Ingenium Software Engineering. All rights reserved.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace Build
{
	/// <summary>
	/// Defines the supported build engines.
	/// </summary>
	public enum BuildEngine
	{
		/// <summary>
		/// Use MSBuild directly.
		/// </summary>
		MSBuild,

		/// <summary>
		/// Use the .NET SDK.
		/// </summary>
		DotNetSdk
	}

	/// <summary>
	/// Defines the supported project types.
	/// </summary>
	public enum BuildType
	{
		/// <summary>
		/// A content-only project.
		/// </summary>
		Content,

		/// <summary>
		/// A deployable application.
		/// </summary>
		Application,

		/// <summary>
		/// A data project.
		/// </summary>
		Data,

		/// <summary>
		/// A library package.
		/// </summary>
		Library,

		/// <summary>
		/// A unit tes project.
		/// </summary>
		Test
	}

	/// <summary>
	/// Defines the supported application types.
	/// </summary>
	public enum ApplicationType
	{
		/// <summary>
		/// A console application.
		/// </summary>
		Console,

		/// <summary>
		/// A web application.
		/// </summary>
		Web
	}

	/// <summary>
	/// Defines the supported package types.
	/// </summary>
	public enum PackageType
	{
		/// <summary>
		/// Pack as a NuGet package.
		/// </summary>
		NuGet,

		/// <summary>
		/// Pack as a zip file.
		/// </summary>
		Zip,

		/// <summary>
		/// Pack as a Docker image.
		/// </summary>
		Docker,

		/// <summary>
		/// Do not pack the project.
		/// </summary>
		None
	}

	/// <summary>
	/// Represents the supported test frameworks.
	/// </summary>
	public enum TestFramework
	{
		/// <summary>
		/// An NUnit test project.
		/// </summary>
		NUnit,

		/// <summary>
		/// An XUnit test project.
		/// </summary>
		XUnit,

		/// <summary>
		/// A VSTest test project.
		/// </summary>
		VSTest
	}
}
