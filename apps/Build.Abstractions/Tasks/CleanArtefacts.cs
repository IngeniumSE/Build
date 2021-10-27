// Copyright (c) 2021 Ingenium Software Engineering. All rights reserved.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace Build.Tasks
{
	/// <summary>
	/// Cleans the artefacts path before a build commences.
	/// </summary>
	public class CleanArtefacts : BuildTask
	{
		public CleanArtefacts(BuildServices services) : base(services) { }

		/// <inheritdoc />
		protected override void RunCore(BuildContext context)
		{
			if (Directory.Exists(context.ArtefactsPath.FullPath))
			{
				Directory.Delete(context.ArtefactsPath.FullPath, true);
			}

			Directory.CreateDirectory(context.ArtefactsPath.FullPath);
		}
	}
}
