// Copyright (c) 2021 Ingenium Software Engineering. All rights reserved.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace Build
{
	using Cake.Frosting;

	public static class Program
	{
		public static int Main(string[] args)
		{
			return new CakeHost()
					.UseContext<BuildContext>()
					.UseDiscoveredTasks()
					.UseDiscoveredHooks()
					.Run(args);
		}
	}
}
