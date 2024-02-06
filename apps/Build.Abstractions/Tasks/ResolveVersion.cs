// Copyright (c) 2021 Ingenium Software Engineering. All rights reserved.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace Build.Tasks
{
	using Cake.Core;
	using Cake.Core.Tooling;
	using Cake.Frosting;
	using Cake.MinVer;

	using Spectre.Console;

	/// <summary>
	/// Performs a build of available projects.
	/// </summary>
	[TaskName("Version")]
	public class ResolveVersion : BuildTask
	{
		public ResolveVersion(BuildServices services) : base(services) { }

		/// <inheritdoc />
		protected override void RunCore(BuildContext context)
		{
			var settings = VersionHelpers.GetMinVerSettings(context)
				.WithArgumentCustomization(c => c.Append(context.RootPath.FullPath));

			context.Version = context.MinVer(settings);

			var table = new Table();
			table.AddColumn("Element");
			table.AddColumn("Value");

			table.Border(TableBorder.Rounded);

			table.AddRow("Repo", context.RootPath.FullPath);
			table.AddRow("Version", context.Version.Version);
			table.AddRow("Package version", context.Version.PackageVersion);
			table.AddRow("Assembly version", context.Version.AssemblyVersion);
			table.AddRow("File version", context.Version.FileVersion);
			table.AddRow("Informational version", context.Version.InformationalVersion);
			table.AddRow("Is pre-release?", context.Version.IsPreRelease.ToString());
			table.AddRow("Pre-release", context.Version.PreRelease ?? "");

			AnsiConsole.Write(table);
		}
	}
}
