// Copyright (c) 2021 Ingenium Software Engineering. All rights reserved.
// This work is licensed under the terms of the MIT license.
// For a copy, see <https://opensource.org/licenses/MIT>.

namespace Build
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Linq;
	using System.Reflection;

	using Microsoft.Extensions.DependencyModel;

	/// <summary>
	/// Provides assemblies loaded using the application <see cref="DependencyContext"/>.
	/// </summary>
	public class DependencyContextAssemblyProvider
	{
		private readonly DependencyContext _dependencyContext;
		private readonly Lazy<ReadOnlyCollection<Assembly>> _assemblyThunk;

		/// <summary>
		/// Gets the set of assemblies the provider will consider when finding candidates.
		/// </summary>
		internal static HashSet<string> ReferencedAssemblies = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
				{
						"Build.Abstractions"
				};

		/// <summary>
		/// Initialises a new instance of <see cref="DependencyContextAssemblyProvider"/>.
		/// </summary>
		/// <param name="dependencyContext">The dependency context.</param>
		public DependencyContextAssemblyProvider(DependencyContext dependencyContext)
		{
			_dependencyContext = dependencyContext ?? throw new ArgumentNullException(nameof(dependencyContext));
			_assemblyThunk = new Lazy<ReadOnlyCollection<Assembly>>(() => FindAssemblies());
		}

		/// <inheritdoc />
		public IReadOnlyCollection<Assembly> Assemblies => _assemblyThunk.Value;

		private ReadOnlyCollection<Assembly> FindAssemblies()
		{
			var assemblies = GetCandidateLibraries()
					.SelectMany(l => l.GetDefaultAssemblyNames(_dependencyContext))
					.Select(Assembly.Load)
					.ToList();

			return new ReadOnlyCollection<Assembly>(assemblies);
		}

		private IEnumerable<RuntimeLibrary> GetCandidateLibraries()
		{
			if (ReferencedAssemblies is null)
			{
				return Enumerable.Empty<RuntimeLibrary>();
			}

			var candidateResolver = new CandidateResolver(_dependencyContext.RuntimeLibraries, ReferencedAssemblies);
			return candidateResolver.GetCandidates();
		}

		private class CandidateResolver
		{
			private readonly IDictionary<string, Dependency> _runtimeDependencies;

			public CandidateResolver(
					IReadOnlyList<RuntimeLibrary> runtimeDependencies,
					ISet<string> referenceAssemblies)
			{
				var dependenciesWithNoDuplicates = new Dictionary<string, Dependency>(StringComparer.OrdinalIgnoreCase);
				foreach (var dependency in runtimeDependencies)
				{
					if (!dependenciesWithNoDuplicates.ContainsKey(dependency.Name))
					{
						dependenciesWithNoDuplicates.Add(
								dependency.Name,
								CreateDependency(dependency, referenceAssemblies));
					}
				}

				_runtimeDependencies = dependenciesWithNoDuplicates;
			}

			private Dependency CreateDependency(RuntimeLibrary library, ISet<string> referenceAssemblies)
			{
				var classification = DependencyClassification.Unknown;
				if (referenceAssemblies.Contains(library.Name))
				{
					classification = DependencyClassification.BuildReference;
				}

				return new Dependency(library, classification);
			}

			private DependencyClassification ComputeClassification(string dependency)
			{
				if (!_runtimeDependencies.ContainsKey(dependency))
				{
					return DependencyClassification.DoesNotReferenceBuild;
				}

				var candidateEntry = _runtimeDependencies[dependency];
				if (candidateEntry.Classification != DependencyClassification.Unknown)
				{
					return candidateEntry.Classification;
				}

				var classification = DependencyClassification.DoesNotReferenceBuild;
				foreach (var candidateDependency in candidateEntry.Library.Dependencies)
				{
					var dependencyClassification = ComputeClassification(candidateDependency.Name);
					if (dependencyClassification == DependencyClassification.ReferencesBuild
							|| dependencyClassification == DependencyClassification.BuildReference)
					{
						classification = DependencyClassification.ReferencesBuild;
						break;
					}
				}

				candidateEntry.Classification = classification;

				return classification;
			}

			public IEnumerable<RuntimeLibrary> GetCandidates()
			{
				foreach (var dependency in _runtimeDependencies)
				{
					var classification = ComputeClassification(dependency.Key);
					if (classification == DependencyClassification.ReferencesBuild
							|| classification == DependencyClassification.BuildReference)
					{
						yield return dependency.Value.Library;
					}
				}
			}
		}

		private class Dependency
		{
			public Dependency(RuntimeLibrary library, DependencyClassification classification)
			{
				Library = library;
				Classification = classification;
			}

			public RuntimeLibrary Library { get; }
			public DependencyClassification Classification { get; set; }

			public override string ToString()
					=> $"Library: {Library.Name}, Classification: {Classification}";
		}

		private enum DependencyClassification
		{
			Unknown = 0,
			ReferencesBuild = 1,
			DoesNotReferenceBuild = 2,
			BuildReference = 3
		}
	}
}
