// Copyright (c) 2021 Ingenium Software Engineering. All rights reserved.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace Build.Tasks
{
	using System;
	using System.Collections.Generic;

	using Cake.Frosting;

	using Spectre.Console;

	/// <summary>
	/// Outputs useful debugging information about the build.
	/// </summary>
	[TaskName("ListProjects")]
	public class ListProjects : BuildTask
	{
		/// <inheritdoc />
		protected override void RunCore(BuildContext context)
		{
			var table = new Table();
			table.AddColumn("Name");
			table.AddColumn("Type");
			table.AddColumn("Engine");
			table.AddColumn("Target frameworks");
			table.AddColumn("Other info");

			table.Border(TableBorder.Rounded);
			table.Expand();

			int rows =
				WriteRows(table, context.Projects[BuildType.Application],
					p => $"App type: {p.ApplicationType}, Package: {p.PackageType}") +
				WriteRows(table, context.Projects[BuildType.Content],
					p => string.Empty) +
				WriteRows(table, context.Projects[BuildType.Data],
					p => string.Empty) +
				WriteRows(table, context.Projects[BuildType.Library],
					p => $"Package: {p.PackageType}") +
				WriteRows(table, context.Projects[BuildType.Test],
					p => $"Test framework: {p.TestFramework}");

			if (rows == 0)
			{
				AnsiConsole.MarkupLine("[orange3]There are no projects to build[/]");
			}
			else
			{
				AnsiConsole.Render(table);
			}
		}

		int WriteRows(
			Table table,
			IReadOnlyCollection<BuildProject> projects,
			Func<BuildProject, string> otherInfoThunk)
		{
			int rows = 0;

			foreach (var project in projects)
			{
				table.AddRow(
					project.Name,
					project.BuildType.ToString(),
					project.BuildEngine.ToString(),
					string.Join(",", project.TargetFrameworks),
					otherInfoThunk(project));

				rows++;
			}

			return rows;
		}
	}
}
