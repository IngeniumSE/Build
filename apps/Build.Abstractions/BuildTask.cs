// Copyright (c) 2021 Ingenium Software Engineering. All rights reserved.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace Build
{
	using System.Threading.Tasks;

	using Cake.Core;
	using Cake.Frosting;

	/// <summary>
	/// Represents a build task.
	/// </summary>
	public abstract class BuildTask : FrostingTask
	{
		readonly BuildServices _services;

		/// <summary>
		/// Initialises a new instance of <see cref="BuildTask"/>
		/// </summary>
		/// <param name="services"></param>
		public BuildTask(BuildServices services)
		{
			_services = services;
		}

		/// <summary>
		/// Gets the build services.
		/// </summary>
		protected BuildServices Services => _services;

		/// <inheritdoc  />
		public override void Run(ICakeContext context)
			=> RunCore((BuildContext)context);

		/// <summary>
		/// Runs the task.
		/// </summary>
		/// <param name="context">The build context.</param>
		protected virtual void RunCore(BuildContext context) { }
	}

	/// <summary>
	/// Represents an asynchronous build task.
	/// </summary>
	public abstract class AsyncBuildTask : AsyncFrostingTask
	{
		readonly BuildServices _services;

		/// <summary>
		/// Initialises a new instance of <see cref="BuildTask"/>
		/// </summary>
		/// <param name="services"></param>
		public AsyncBuildTask(BuildServices services)
		{
			_services = services;
		}

		/// <summary>
		/// Gets the build services.
		/// </summary>
		protected BuildServices Services => _services;

		/// <inheritdoc  />
		public override Task RunAsync(ICakeContext context)
			=> RunCoreAsync((BuildContext)context);

		/// <summary>
		/// Runs the task.
		/// </summary>
		/// <param name="context">The build context.</param>
		protected virtual Task RunCoreAsync(BuildContext context)
			=> Task.CompletedTask;
	}
}
