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
	[IsDependentOn(typeof(ResolveVerson))]
	[IsDependentOn(typeof(CleanArtefacts))]
	public class BuildProjects : BuildTask
	{
		public BuildProjects(BuildServices services) : base(services) { }

		/// <inheritdoc />
		protected override void RunCore(BuildContext context)
		{
			void BuildAvailableProjects(IEnumerable<BuildProject> projects)
			{
				var hooks = Services.GetHooks<IBuildHook>();

				foreach (var project in projects)
				{
					foreach (var hook in hooks)
					{
						hook.BeforeBuild(context, project);
					}

					context.BuildProject(project);

					foreach (var hook in hooks)
					{
						hook.AfterBuild(context, project);
					}

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
