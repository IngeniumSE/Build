// Copyright (c) 2021 Ingenium Software Engineering. All rights reserved.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace Build.Tasks
{
	using Cake.Common;
	using Cake.Common.IO;
	using Cake.Common.Tools.NuGet;
	using Cake.Common.Tools.NuGet.Push;
	using Cake.Common.Tools.NuGet.Sources;
	using Cake.Core;
	using Cake.Frosting;

	/// <summary>
	/// Publishes any generated packages to a NuGet feed.
	/// </summary>
	[TaskName("Publish")]
	[IsDependentOn(typeof(CleanArtefacts))]
	[IsDependentOn(typeof(PackProjects))]
	public class PublishPackages : BuildTask
	{
		public PublishPackages(BuildServices services) : base(services) { }

		public override bool ShouldRun(ICakeContext context)
			=> context.HasArgument(CommonArguments.Publish)
			&& context.HasArgument(CommonArguments.Feed)
			&& context.HasArgument(CommonArguments.Source)
			&& context.HasArgument(CommonArguments.Token);

		protected override void RunCore(BuildContext context)
		{
			string feed = context.Argument<string>(CommonArguments.Feed);
			string source = context.Argument<string>(CommonArguments.Source);
			string token = context.Argument<string>(CommonArguments.Token);
			string username = context.Argument<string>(CommonArguments.Username);

			// Get the generated packages.
			var packages = context.GetFiles(context.ArtefactsPath + "**/*.nupkg");

			bool addedSource = false;
			var sourceSettings = new NuGetSourcesSettings
			{
				UserName = username,
				Password = token,
				StorePasswordInClearText = true
			};

			{
				if (context.FileExists(context.NuGetConfigPath))
				{
					sourceSettings.ConfigFile = context.NuGetConfigPath;
				}

				if (!context.NuGetHasSource(feed, sourceSettings))
				{
					context.NuGetAddSource(source, feed, sourceSettings);
					addedSource = true;
				}
			}

			foreach (var package in packages)
			{
				var pushSettings = new NuGetPushSettings
				{
					//ArgumentCustomization = c => c.Append($"--api-key {token}"),
					Source = source,
				};

				context.NuGetPush(package, pushSettings);
			}

			if (addedSource)
			{
				context.NuGetRemoveSource(source, feed, sourceSettings);
			}
		}
	}
}