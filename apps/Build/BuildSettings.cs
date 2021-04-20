// Copyright (c) 2021 Ingenium Software Engineering. All rights reserved.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace Build
{
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;

	/// <summary>
	/// Represents settings for the build.
	/// </summary>
	public class BuildSettings
	{
		static readonly IReadOnlyCollection<string> NoExcludedProjects
			= new ReadOnlyCollection<string>(new List<string>());

		/// <summary>
		/// Initializes a new instance of the <see cref="BuildSettings" />
		/// </summary>
		public BuildSettings(
			IEnumerable<string>? excludedProjects = default)
		{
			ExcludedProjects = excludedProjects is null
				? NoExcludedProjects
				: new ReadOnlyCollection<string>(excludedProjects.ToList());
		}

		/// <summary>
		/// Gets the set of excluded projects.
		/// </summary>
		public IReadOnlyCollection<string> ExcludedProjects { get; }
	}
}
