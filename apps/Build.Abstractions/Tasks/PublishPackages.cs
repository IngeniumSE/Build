// Copyright (c) 2021 Ingenium Software Engineering. All rights reserved.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace Build.Tasks
{
	using Cake.Common;
	using Cake.Common.IO;
	using Cake.Common.Tools.DotNet;
	using Cake.Common.Tools.DotNet.NuGet.Push;
	using Cake.Common.Tools.DotNet.NuGet.Source;
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
		const string NUGET_FEED = "https://api.nuget.org/v3/index.json";

		public PublishPackages(BuildServices services) : base(services) { }

		public override bool ShouldRun(ICakeContext context)
			=> context.HasArgument(CommonArguments.Publish)
			&& (
				// Either we're publishing to the public feed using --publish --nuget --token {token}
				(context.HasArgument(CommonArguments.NuGet) && context.HasArgument(CommonArguments.Token))
				// Or we're publishing to a custom feed using --publish --source {source} --feed {feed} --username {username} --token {token}
				|| (context.HasArgument(CommonArguments.Feed)
						&& context.HasArgument(CommonArguments.Source)
						&& context.HasArgument(CommonArguments.Token)
				)
			);

		protected override void RunCore(BuildContext context)
		{
			bool isCustomFeed = !context.HasArgument(CommonArguments.NuGet);

			string feed = context.Argument(CommonArguments.Feed, "");
			string source = context.Argument(CommonArguments.Source, "");
			string token = context.Argument(CommonArguments.Token, "");
			string username = context.Argument(CommonArguments.Username, "");

			bool addedSource = false;
			DotNetNuGetAddSourceSettings? sourceSettings = null;
			if (isCustomFeed)
			{
				sourceSettings = new DotNetNuGetAddSourceSettings
				{
					Source = feed,
					UserName = username,
					Password = token,
					StorePasswordInClearText = true
				};

				{
					if (context.FileExists(context.NuGetConfigPath))
					{
						sourceSettings.ConfigFile = context.NuGetConfigPath;
					}

					if (!context.DotNetNuGetHasSource(source, sourceSettings))
					{
						context.DotNetNuGetAddSource(source, sourceSettings);
						addedSource = true;
					}
				}
			}

			// Get the generated packages.
			var packages = context.GetFiles(context.ArtefactsPath + "**/*.nupkg");
			foreach (var package in packages)
			{
				var pushSettings = new DotNetNuGetPushSettings
				{
					ApiKey = token,
					Source = isCustomFeed ? feed : NUGET_FEED,
				};

				context.DotNetNuGetPush(package, pushSettings);
			}

			if (addedSource)
			{
				context.DotNetNuGetRemoveSource(source, sourceSettings);
			}
		}
	}
}