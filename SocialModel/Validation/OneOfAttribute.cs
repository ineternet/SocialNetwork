using System.Reflection;

namespace SocialModel.Validation;

/// <summary>
/// Marks that at least one, or no more than one exclusive, of multiple optional parameters must be set.
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class OneOfAttribute : ValidationAttribute
{
    /// <summary>
    /// If true, and this parameter is set, all other <see cref="OneOfAttribute"/> parameters mustn't be set.
    /// </summary>
    public bool Exclusive { get; set; }

    /// <summary>
    /// Confirms a valid result if at least one parameter is set and all <see cref="Exclusive"/> requirements are satisfied.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="vc"></param>
    /// <returns></returns>
    protected override ValidationResult? IsValid(object? value, ValidationContext vc)
        => vc.ObjectType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.GetCustomAttribute<OneOfAttribute>() is not null)
            .Count(p => !string.IsNullOrEmpty(p.GetValue(vc.ObjectInstance)?.ToString()))
        switch
        {
            < 1 => new("At least one optional parameter must be present."),

            > 1 when this.Exclusive => new($"Cannot have other optional parameters when {vc.MemberName} is set."),

            >= 1 => ValidationResult.Success,
        };
}
