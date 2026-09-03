using FinanceManagement.Domain.Enums;

namespace FinanceManagement.Application.Interfaces.Services;

public interface ICurrencyConverter
{
    ValueTask<decimal> ConvertAsync(
        decimal amount, 
        Currency from, 
        Currency to, 
        DateOnly? onDate = null,
        CancellationToken ct = default);
}
