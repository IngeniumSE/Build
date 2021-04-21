// Copyright (c) 2021 Ingenium Software Engineering. All rights reserved.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace Build.Tasks
{
	using System.Collections.Generic;

	using Cake.Frosting;

	/// <summary>
	/// Performs a build of available projects.
	/// </summary>
	[TaskName("Build")]
	public class BuildProjects : BuildTask
	{
		/// <inheritdoc />
		protected override void RunCore(BuildContext context)
		{
			void BuildAvailableProjects(IEnumerable<BuildProject> projects)
			{
				foreach (var project in projects)
				{
					context.BuildProject(project);

					project.MarkAsBuilt();
				}
			}

			BuildAvailableProjects(context.Projects[BuildType.Library]);
			BuildAvailableProjects(context.Projects[BuildType.Application]);
			BuildAvailableProjects(context.Projects[BuildType.Test]);
			BuildAvailableProjects(context.Projects[BuildType.Data]);
			BuildAvailableProjects(context.Projects[BuildType.Content]);
		}
	}
}
