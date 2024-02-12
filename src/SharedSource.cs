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