using System.Diagnostics.CodeAnalysis;
using FinanceManagement.Application.Exceptions;

namespace FinanceManagement.Application.Common;

public static class Guards
{
    public static void EnsureFound<T>([NotNull] this T? entity, object id) where T : class
    {
        if (entity is null)
        {
            throw new NotFoundException(typeof(T).Name, id);
        }
    }

    public static async Task<T> OrThrowAsync<T>(this Task<T?> task, object id) where T : class
    {
        var entity = await task;
        entity.EnsureFound(id);
        return entity;
    }

    public static async Task ThrowIfExistsAsync(this Task<bool> existCheck, string conflictMessage)
    {
        if (await existCheck)
        {
            throw new ConflictException(conflictMessage);
        }
    }
}
