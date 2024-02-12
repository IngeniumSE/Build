#if NETSTANDARD2_0 || NETSTANDARD2_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETCOREAPP3_0 || NETCOREAPP3_1 || NET45 || NET451 || NET452 || NET6 || NET461 || NET462 || NET47 || NET471 || NET472 || NET48
using System.ComponentModel;
#endif

/// <summary>
/// Provides utility methods for validating method arguments.
/// </summary>
[SkipCodeCoverage]
internal static class Ensure
{
	/// <summary>
	/// Validates the given parameter value is not null.
	/// </summary>
	/// <typeparam name="T">The value type.</typeparam>
	/// <param name="value">The value.</param>
	/// <param name="paramName">The parameter name.</param>
	/// <returns>The value</returns>
	public static T IsNotNull<T>([ValidatedNotNull] T? value, string paramName)
			where T : notnull
	{
		if (value is null)
		{
			throw new ArgumentNullException(paramName);
		}

		return value;
	}

	/// <summary>
	/// Validates the given string parameter value is not an empty string.
	/// </summary>
	/// <param name="value">The value.</param>
	/// <param name="paramName">The parameter name.</param>
	/// <returns>The value.</returns>
	public static string IsNotNullOrEmpty([ValidatedNotNull] string? value, string paramName)
	{
		if (value is not { Length: > 0 })
		{
			throw new ArgumentException($"The parameter '{paramName}' must be a non-empty value.");
		}

		return value;
	}
}

[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false, Inherited = false), SkipCodeCoverage]
internal sealed class ValidatedNotNullAttribute : Attribute
{ }

[AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false), SkipCodeCoverage]
internal sealed class SkipCodeCoverageAttribute : Attribute
{ }

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
#pragma warning disable CS9113 // Parameter is unread.
internal sealed class MemberNotNullWhen(bool value, string propertyName) : Attribute { }
#pragma warning restore CS9113 // Parameter is unread.

#if NETSTANDARD2_0 || NETSTANDARD2_1 || NETCOREAPP2_0 || NETCOREAPP2_1 || NETCOREAPP2_2 || NETCOREAPP3_0 || NETCOREAPP3_1 || NET45 || NET451 || NET452 || NET6 || NET461 || NET462 || NET47 || NET471 || NET472 || NET48
namespace System.Runtime.CompilerServices
{
	[AttributeUsage(AttributeTargets.All), EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class RequiredMemberAttribute : Attribute { }

	[AttributeUsage(AttributeTargets.All), EditorBrowsable(EditorBrowsableState.Never)]
	internal sealed class CompilerFeatureRequiredAttribute : Attribute
	{
		public CompilerFeatureRequiredAttribute(string name) { }
	}

	[EditorBrowsable(EditorBrowsableState.Never)]
	internal static class IsExternalInit { }
}
#endif