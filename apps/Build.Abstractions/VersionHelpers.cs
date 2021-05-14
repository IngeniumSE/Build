// Copyright (c) 2021 Ingenium Software Engineering. All rights reserved.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace Build
{
	using System;
	using System.Collections.Generic;

	using Cake.Common.Xml;
	using Cake.Core.IO;
	using Cake.MinVer;

	/// <summary>
	/// Provides helpers for resolving versions.
	/// </summary>
	public static class VersionHelpers
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
		/// Gets the MinVer settings from the parent repo.
		/// </summary>
		/// <param name="context">The build context.</param>
		/// <returns>The MinVer settings.</returns>
		public static MinVerSettings GetMinVerSettings(this BuildContext context)
		{
			if (context is null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			var settings = new MinVerSettings();
			var props = context.RootPath.CombineWithFilePath(new FilePath("./Directory.Build.props"));
			if (!context.FileSystem.Exist(props))
			{
				return settings;
			}

			settings.AutoIncrement = GetAutoIncrement(context, props);
			settings.BuildMetadata = GetString(context, props, "MinVerBuildMetadata");
			settings.DefaultPreReleasePhase = GetString(context, props, "MinVerDefaultPreReleasePhase");
			settings.MinimumMajorMinor = GetString(context, props, "MinVerMinimumMajorMinor");
			settings.TagPrefix = GetString(context, props, "MinVerTagPrefix");
			settings.Verbosity = GetVerbosity(context, props);

			return settings;
		}

		static MinVerAutoIncrement? GetAutoIncrement(BuildContext context, FilePath props)
		{
			string? value = GetProperty(context, props, "MinVerAutoIncrement");
			return Enum.TryParse<MinVerAutoIncrement>(value, out var autoIncrement) 
				? (MinVerAutoIncrement?)autoIncrement 
				: default;
		}

		static MinVerVerbosity? GetVerbosity(BuildContext context, FilePath props)
		{
			string? value = GetProperty(context, props, "MinVerVerbosity");
			return Enum.TryParse<MinVerVerbosity>(value, out var verbosity)
				? (MinVerVerbosity?)verbosity
				: default;
		}

		static string? GetString(BuildContext context, FilePath props, string propName)
			=> GetProperty(context, props, propName);

		static string? GetProperty(BuildContext context, FilePath props, string propName)
			=> context.XmlPeek(props, $"/Project/PropertyGroup/{propName}", StandardXmlPeekSettings)
				?? context.XmlPeek(props, $"/msbuild:Project/msbuild:PropertyGroup/msbuild:{propName}", StandardXmlPeekSettings);
	}
}
