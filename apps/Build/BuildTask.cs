// Copyright (c) 2021 Ingenium Software Engineering. All rights reserved.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace Build
{
	using Cake.Core;
	using Cake.Frosting;

	/// <summary>
	/// Represents a build task.
	/// </summary>
	public abstract class BuildTask : FrostingTask
	{
		/// <inheritdoc  />
		public override void Run(ICakeContext context)
		{
			RunCore((BuildContext)context);
		}

		/// <summary>
		/// Runs the task.
		/// </summary>
		/// <param name="context">The build context.</param>
		protected virtual void RunCore(BuildContext context) { }
	}
}
