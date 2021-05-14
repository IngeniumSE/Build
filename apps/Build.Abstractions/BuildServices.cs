// Copyright (c) 2021 Ingenium Software Engineering. All rights reserved.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace Build
{
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;

	using Microsoft.Extensions.DependencyInjection;

	/// <summary>
	/// Represents build services.
	/// </summary>
	public class BuildServices
	{
		readonly IServiceScopeFactory _serviceScopeFactory;

		/// <summary>
		/// Initialises a new instance of <see cref="BuildServices"/>
		/// </summary>
		/// <param name="hooks">The service scope factory.</param>
		public BuildServices(IServiceScopeFactory serviceScopeFactory)
		{
			_serviceScopeFactory = serviceScopeFactory;
		}

		/// <summary>
		/// Gets the avilable hooks.
		/// </summary>
		/// <typeparam name="THook">The hook type.</typeparam>
		/// <returns>The set of hooks.</returns>
		public IEnumerable<THook> GetHooks<THook>()
			where THook : ITaskHook
		{
			using var scope = 
		}
	}
}
