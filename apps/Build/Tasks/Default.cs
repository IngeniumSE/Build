﻿// Copyright (c) 2021 Ingenium Software Engineering. All rights reserved.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace Build.Tasks
{
	using Cake.Frosting;

	/// <summary>
	/// The default (entry point) task for the build.
	/// </summary>
	[TaskName("Default")]
	[Dependency(typeof(BuildProjects))]
	[Dependency(typeof(TestProjects))]
	public class DefaultTask : BuildTask
	{
		/// <inheritdoc />
		protected override void RunCore(BuildContext context)
		{
		}
	}
}
