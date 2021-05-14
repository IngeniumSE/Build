// Copyright (c) 2021 Ingenium Software Engineering. All rights reserved.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace Build.Tasks
{
	using Cake.Frosting;

	/// <summary>
	/// Packs any avaible apps and libraries.
	/// </summary>
	[TaskName("Pack")]
	public class PackProjects : BuildTask
	{
		/// <inheritdoc />
		protected override void RunCore(BuildContext context)
		{
			foreach (var project in context.Projects[BuildType.Library])
			{
				context.PackProject(project);
			}

			foreach (var project in context.Projects[BuildType.Application])
			{
				context.PackProject(project);
			}
		}
	}
}
