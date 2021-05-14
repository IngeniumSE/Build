// Copyright (c) 2021 Ingenium Software Engineering. All rights reserved.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace Build
{
	using System.Threading.Tasks;

	/// <summary>
	/// Defines the required contract for implementing a build hook.
	/// </summary>
	public interface IBuildHook : ITaskHook
	{
		/// <summary>
		/// Executes before the build starts.
		/// </summary>
		/// <param name="context">The build context.</param>
		/// <param name="project">The build project.</param>
		void BeforeBuild(BuildContext context, BuildProject project);

		/// <summary>
		/// Executes after the build completes.
		/// </summary>
		/// <param name="context">The build context.</param>
		/// <param name="project">The build project.</param>
		void AfterBuild(BuildContext context, BuildProject project);
	}

	/// <summary>
	/// Defines the required contract for implementing an asynchronous build hook.
	/// </summary>
	public interface IAsyncBuildHook : ITaskHook
	{
		/// <summary>
		/// Executes before the build starts.
		/// </summary>
		/// <param name="context">The build context.</param>
		/// <param name="project">The build project.</param>
		Task BeforeBuildAsync(BuildContext context, BuildProject project);

		/// <summary>
		/// Executes after the build completes.
		/// </summary>
		/// <param name="context">The build context.</param>
		/// <param name="project">The build project.</param>
		Task AfterBuildAsync(BuildContext context, BuildProject project);
	}

	/// <summary>
	/// Provides a base implementation of a build hook.
	/// </summary>
	public abstract class BuildHook : IBuildHook
	{
		/// <inheritdoc />
		public virtual void AfterBuild(BuildContext context, BuildProject project) { }

		/// <inheritdoc />
		public virtual void BeforeBuild(BuildContext context, BuildProject project) { }
	}

	/// <summary>
	/// Provides a base implementation of an asynchronous build hook.
	/// </summary>
	public abstract class AsyncBuildHook : IAsyncBuildHook
	{
		/// <inheritdoc />
		public virtual Task AfterBuildAsync(BuildContext context, BuildProject project)
			=> Task.CompletedTask;

		/// <inheritdoc />
		public virtual Task BeforeBuildAsync(BuildContext context, BuildProject project)
			=> Task.CompletedTask;
	}
}
