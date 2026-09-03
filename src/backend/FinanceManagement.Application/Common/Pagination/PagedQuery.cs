namespace FinanceManagement.Application.Common.Pagination;

public abstract record PagedQuery
{
    public const int MinPage = 1;
    public const int DefaultPage = 1;
    public const int MinPageSize = 1;
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

    public int Page { get; set; } = DefaultPage;
    public int PageSize { get; set; } = DefaultPageSize;
    public string? SortBy { get; set; }
    public SortOrder SortOrder { get; set; } = SortOrder.Desc;
}