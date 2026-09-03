using System.Text.RegularExpressions;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Domain.Enums;

namespace FinanceManagement.Application.Features.Operations.Common;

public static class OperationAmountResolver
{
    private const string _originalTagPrefix = "[Original:";

    private static readonly Regex _originalTagRegex = new(
        $@"^{Regex.Escape(_originalTagPrefix)}\s[^\]]+\]\s*",
        RegexOptions.Compiled);

    public static async Task<(decimal Amount, string? Note)> ResolveAsync(
        ICurrencyConverter converter,
        decimal amount,
        Currency? sourceCurrency,
        string? note,
        DateTime date,
        Currency walletCurrency,
        CancellationToken ct)
    {
        if (sourceCurrency is null || sourceCurrency == walletCurrency)
        {
            return (amount, note);
        }

        var converted = await converter.ConvertAsync(
            amount,
            sourceCurrency.Value,
            walletCurrency,
            DateOnly.FromDateTime(date),
            ct);

        var tag = FormattableString.Invariant($"{_originalTagPrefix} {amount} {sourceCurrency.Value}]");
        var cleanNote = note is null ? null : _originalTagRegex.Replace(note, string.Empty);
        var resultNote = string.IsNullOrWhiteSpace(cleanNote) ? tag : $"{tag} {cleanNote}";

        return (converted, resultNote);
    }
}
