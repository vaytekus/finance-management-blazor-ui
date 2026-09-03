using FinanceManagement.Contracts.Common;

namespace FinanceManagement.Application.Common.Pagination;

public static class PagedResultExtensions
{
    public static PagedResult<TDest> Map<TSource, TDest>(
        this PagedResult<TSource> source,
        Func<TSource, TDest> map)
    {
        return new PagedResult<TDest>
        {
            Items = source.Items.Select(map).ToList(),
            TotalCount = source.TotalCount,
            Page = source.Page,
            PageSize = source.PageSize,
        };
    }
}
