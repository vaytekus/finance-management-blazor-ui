using System.ComponentModel.DataAnnotations;

namespace FinanceManagement.Application.Common;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public sealed class MinDecimalAttribute : ValidationAttribute
{
    public decimal Min { get; }
    
    public MinDecimalAttribute(double min)
    {
        Min = (decimal)min;
    }

    public override bool IsValid(object? value)
    {
        if (value is null)
        {
            return true;
        }

        return value is decimal d && d >= Min;
    }
    
    public override string FormatErrorMessage(string name) =>
        $"{name} must be greater than or equal to {Min}.";
}
