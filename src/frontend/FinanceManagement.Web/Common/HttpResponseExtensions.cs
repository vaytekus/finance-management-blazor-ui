using System.Net;
using System.Net.Http.Json;
using FinanceManagement.Web.Services.Common;

namespace FinanceManagement.Web.Common;

public static class HttpResponseExtensions
{
    public static async Task EnsureSuccessOrThrowApiErrorAsync(
        this HttpResponseMessage response,
        CancellationToken ct = default)
    {
        if(response.IsSuccessStatusCode)
        {
            return;
        }

        var problem = await response.Content.ReadFromJsonAsync<ApiError>(ct);

        throw new HttpRequestException(
            problem?.Detail ?? response.ReasonPhrase ?? "request failed",
            inner: null,
            statusCode: response.StatusCode);
    }

    public static async Task<T> ReadRequiredAsync<T>(
        this HttpResponseMessage response, CancellationToken ct = default)
    {
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(ApiJsonOptions.Default, ct)
            ?? throw new InvalidOperationException("Empty response from server");
    }

    public static async Task<T?> ReadOrNullAsync<T>(
        this HttpResponseMessage response, CancellationToken ct = default)
    {
        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return default;
        }

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(ApiJsonOptions.Default, ct);
    }

    private record ApiError(string? Detail);
}
