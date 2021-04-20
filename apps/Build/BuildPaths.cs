// Copyright (c) 2021 Ingenium Software Engineering. All rights reserved.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace Build
{
	/// <summary>
	/// Provides path conventions.
	/// </summary>
	public static class BuildPaths
	{
		/// <summary>
		/// Gets the application path convention.
		/// </summary>
		public const string ApplicationPathConvention = "/apps/**/*.csproj";

		/// <summary>
		/// Gets the content path convention.
		/// </summary>
		public const string ContentPathConvention = "/content/**/*.csproj";

		/// <summary>
		/// Gets the data path convention.
		/// </summary>
		public const string DataPathConvention = "/data/**/*.sqlproj";

		/// <summary>
		/// Gets the library path convention.
		/// </summary>
		public const string LibraryPathConvention = "/libs/**/*.csproj";

		/// <summary>
		/// Gets the test path convention.
		/// </summary>
		public const string TestPathConvention = "/tests/**/*.csproj";
	}
}
