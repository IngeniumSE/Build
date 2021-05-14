// Copyright (c) 2021 Ingenium Software Engineering. All rights reserved.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace Build.Tasks
{
	using Cake.Frosting;

	/// <summary>
	/// Executes any unit tests for available test projects.
	/// </summary>
	[TaskName("Test")]
	public class TestProjects : BuildTask
	{
		public TestProjects(BuildServices services) : base(services) { }

		/// <inheritdoc />
		protected override void RunCore(BuildContext context)
		{
			foreach (var project in context.Projects[BuildType.Test])
			{
				context.TestProject(project);
			}
		}
	}
}
