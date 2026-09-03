using FinanceManagement.Contracts.Common;
using FinanceManagement.Application.Common.Pagination;
using Microsoft.EntityFrameworkCore;

namespace FinanceManagement.Infrastructure.Extensions;

public static class QueryableExtensions
{
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        int page,
        int pageSize,
        CancellationToken ct = default)
        where T : class
    {
        var totalCount = await query.CountAsync(ct);

        var items = await query
            .AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<T>
        {
            Items = items, TotalCount = totalCount, Page = page, PageSize = pageSize
        };
    }
}
