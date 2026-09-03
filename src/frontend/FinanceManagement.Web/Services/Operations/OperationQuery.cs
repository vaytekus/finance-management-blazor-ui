using FinanceManagement.Contracts.Enums;

namespace FinanceManagement.Web.Services.Operations;

public class OperationQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; } = "date";
    public bool SortDescending { get; set; } = true;

    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public Guid? WalletId { get; set; }
    public Guid? TypeId { get; set; }
    public decimal? AmountMin { get; set; }
    public decimal? AmountMax { get; set; }
    public string? Search { get; set; }
    public Currency? Currency { get; set; }
}
