// Copyright (c) 2021 Ingenium Software Engineering. All rights reserved.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace Build
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	using Cake.Frosting;

	using Microsoft.Extensions.DependencyModel;
	using Microsoft.Extensions.DependencyInjection;

	/// <summary>
	/// Provides static extension methods for the Cake host.
	/// </summary>
	public static class CakeHostExtensions
	{
		static Lazy<IReadOnlyCollection<Assembly>> _assembliesThunk;

		static CakeHostExtensions()
		{
			_assembliesThunk = new Lazy<IReadOnlyCollection<Assembly>>(DiscoverAssemblies);
		}

		/// <summary>
		/// Auto-discovers tasks in libraries that reference the Build system.
		/// </summary>
		/// <param name="host">The cake host.</param>
		/// <returns>The cake host.</returns>
		public static CakeHost UseDiscoveredTasks(this CakeHost host)
		{
			foreach (var assembly in _assembliesThunk.Value)
			{
				host.AddAssembly(assembly);
			}

			return host;
		}

		/// <summary>
		/// Auto-discovers hooks in libraries that reference the Build system.
		/// </summary>
		/// <param name="host">The cake host.</param>
		/// <returns>The cake host.</returns>
		public static CakeHost UseDiscoveredHooks(this CakeHost host)
		{
			bool IsHookType(Type type)
				=> type.IsAssignableTo(typeof(ITaskHook)) &&
					 !type.IsAbstract &&
					 type.IsClass &&
					 type.IsPublic;

			IEnumerable<Type> GetHooks(Assembly assembly)
			{
				foreach (var type in assembly.GetExportedTypes().Where(IsHookType))
				{
					yield return type;
				}
			}

			host.ConfigureServices(services =>
			{
				foreach (var assembly in _assembliesThunk.Value)
				{
					if (assembly is not null)
					{
						foreach (var hook in GetHooks(assembly))
						{
							services.AddTransient(typeof(ITaskHook), hook);
						}
					}
				}
			});

			return host;
		}

		static IReadOnlyCollection<Assembly> DiscoverAssemblies()
			=> new DependencyContextAssemblyProvider(DependencyContext.Default).Assemblies;
	}
}
