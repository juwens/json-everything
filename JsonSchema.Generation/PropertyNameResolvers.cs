using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using Humanizer;

namespace Json.Schema.Generation;

/// <summary>
/// Declares a property name resolution which is used to provide a property name.
/// </summary>
/// <param name="input">The property.</param>
/// <returns>The property name</returns>
public delegate string PropertyNameResolver(MemberInfo input);

/// <summary>
/// Declares a property name resolution which is used to provide a property name or null (if a name cannot be resolved).
/// Use this to combine combine (null-coalescing operator) two or more logics into one <see cref="PropertyNameResolver"/>
/// </summary>
/// <param name="input">The property.</param>
/// <returns>The property name or null, if no name was resolved.</returns>
internal delegate string? PartialPropertyNameResolver(MemberInfo input);

/// <summary>
/// Defines a set of predefined property name resolution methods.
/// </summary>
public static class PropertyNameResolvers
{
	/// <summary>
	/// Makes no changes. Properties are generated with the name of the property in code.
	/// </summary>
	public static readonly PropertyNameResolver AsDeclared = x => x.Name;
	/// <summary>
	/// Property names to camel case (e.g. `camelCase`).
	/// </summary>
	public static readonly PropertyNameResolver CamelCase = x => x.Name.Camelize();
	/// <summary>
	/// Property names to pascal case (e.g. `PascalCase`).
	/// </summary>
	public static readonly PropertyNameResolver PascalCase = x => x.Name.Pascalize();
	/// <summary>
	/// Property names to snake case (e.g. `Snake_Case`).
	/// </summary>
	public static readonly PropertyNameResolver SnakeCase = x => x.Name.Underscore();
	/// <summary>
	/// Property names to lower snake case (e.g. `lower_snake_case`).
	/// </summary>
	public static readonly PropertyNameResolver LowerSnakeCase = x => x.Name.Underscore().ToLowerInvariant();
	/// <summary>
	/// Property names to upper snake case (e.g. `UPPER_SNAKE_CASE`).
	/// </summary>
	public static readonly PropertyNameResolver UpperSnakeCase = x => x.Name.Underscore().ToUpperInvariant();
	/// <summary>
	/// Property names to kebab case (e.g. `Kebab-Case`).
	/// </summary>
	public static readonly PropertyNameResolver KebabCase = x => x.Name.Kebaberize();
	/// <summary>
	/// Property names to upper kebab case (e.g. `UPPER-KEBAB-CASE`).
	/// </summary>
	public static readonly PropertyNameResolver UpperKebabCase = x => x.Name.Kebaberize().ToUpperInvariant();
	/// <summary>
	/// Property name is read from <see cref="JsonPropertyNameAttribute"/>, falls back to <see cref="AsDeclared"/> if attribute is absent.
	/// </summary>
	public static readonly PropertyNameResolver ByJsonPropertyName = x => PartialPropertyNameResolvers.ByJsonPropertyName(x) ?? AsDeclared(x);
}

/// <summary>
/// Defines a set of predefined property name resolution methods.
/// </summary>
internal static class PartialPropertyNameResolvers
{
	/// <summary>
	/// Property name is read from <see cref="JsonPropertyNameAttribute"/>, returns null if attribute is missing.
	/// </summary>
	internal static readonly PartialPropertyNameResolver ByJsonPropertyName = x => x.GetCustomAttributes<JsonPropertyNameAttribute>().FirstOrDefault()?.Name;
}