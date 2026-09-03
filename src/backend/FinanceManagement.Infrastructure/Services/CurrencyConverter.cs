using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using FinanceManagement.Application.Exceptions;
using FinanceManagement.Application.Interfaces.Services;
using FinanceManagement.Domain.Enums;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace FinanceManagement.Infrastructure.Services;

public class CurrencyConverter : ICurrencyConverter
{
    internal const string _httpClientName = "NbuCurrencyConverter";
    private const string _cacheKeyPrefix = "NBU_RATES";
    private const string _nbuBasePath = "v1/statdirectory/exchange?json";
    private const string _baseCurrency = "UAH";
    private const int _moneyRoundDigits = 2;
    private static readonly TimeSpan _currentCacheTtl = TimeSpan.FromHours(1);
    private static readonly TimeSpan _historicalCacheTtl = TimeSpan.FromDays(7);

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMemoryCache _cache;
    private readonly ILogger<CurrencyConverter> _logger;
    private readonly ConcurrentDictionary<string, Lazy<Task<IReadOnlyDictionary<string, decimal>>>> _inFlight = new();

    public CurrencyConverter(
        IHttpClientFactory httpClientFactory,
        IMemoryCache cache,
        ILogger<CurrencyConverter> logger)
    {
        _httpClientFactory = httpClientFactory;
        _cache = cache;
        _logger = logger;
    }

    public async ValueTask<decimal> ConvertAsync(
        decimal amount,
        Currency from,
        Currency to,
        DateOnly? onDate = null,
        CancellationToken ct = default)
    {
        if (from == to)
        {
            return amount;
        }

        var effectiveDate = onDate ?? DateOnly.FromDateTime(DateTime.UtcNow);
        var rates = await GetRatesAsync(effectiveDate, ct);

        var fromRate = GetRateToUah(rates, from);
        var toRate = GetRateToUah(rates, to);

        var amountInUah = amount * fromRate;
        var result = amountInUah / toRate;

        return Math.Round(result, _moneyRoundDigits, MidpointRounding.ToEven);
    }

    private ValueTask<IReadOnlyDictionary<string, decimal>> GetRatesAsync(DateOnly date, CancellationToken ct)
    {
        var cacheKey = BuildCacheKey(date);
        if (_cache.TryGetValue(cacheKey, out IReadOnlyDictionary<string, decimal>? cached) && cached is not null)
        {
            return ValueTask.FromResult(cached);
        }

        return new ValueTask<IReadOnlyDictionary<string, decimal>>(GetOrStartFetchAsync(cacheKey, date).WaitAsync(ct));
    }

    private Task<IReadOnlyDictionary<string, decimal>> GetOrStartFetchAsync(string cacheKey, DateOnly date)
    {
        var lazy = _inFlight.GetOrAdd(cacheKey, k => new Lazy<Task<IReadOnlyDictionary<string, decimal>>>(
            () => FetchAndCacheAsync(k, date),
            LazyThreadSafetyMode.ExecutionAndPublication
        ));

        return lazy.Value;
    }

    private async Task<IReadOnlyDictionary<string, decimal>> FetchAndCacheAsync(string cacheKey, DateOnly date)
    {
        try
        {
            var rates = await FetchRatesAsync(date, CancellationToken.None);
            var ttl = date == DateOnly.FromDateTime(DateTime.UtcNow) ? _currentCacheTtl : _historicalCacheTtl;
            _cache.Set(cacheKey, rates, ttl);
            return rates;
        }
        finally
        {
            _inFlight.TryRemove(cacheKey, out _);
        }
    }

    private async Task<IReadOnlyDictionary<string, decimal>> FetchRatesAsync(DateOnly date, CancellationToken ct)
    {
        try
        {
            var client = _httpClientFactory.CreateClient(_httpClientName);
            var path = BuildPath(date);
            var response = await client.GetFromJsonAsync<List<NbuRate>>(path, ct)
                ?? throw new CurrencyConversionException("NBU returned empty response.");

            return response.ToDictionary(r => r.Cc, r => r.Rate, StringComparer.OrdinalIgnoreCase);
        }
        catch (Exception ex) when (ex is not CurrencyConversionException)
        {
            _logger.LogError(ex, "Failed to fetch NBU rates for {Date}.", date);
            throw new CurrencyConversionException($"Failed to fetch exchange rates from NBU for {date:yyyy-MM-dd}.", ex);
        }
    }

    private static decimal GetRateToUah(IReadOnlyDictionary<string, decimal> rates, Currency currency)
    {
        var code = currency.ToString();
        if (code == _baseCurrency)
        {
            return 1m;
        }

        if (!rates.TryGetValue(code, out var rate))
        {
            throw new CurrencyConversionException($"NBU does not publish rate for currency '{code}'.");
        }

        return rate;
    }

    private sealed record NbuRate(
        [property: JsonPropertyName("cc")] string Cc,
        [property: JsonPropertyName("rate")] decimal Rate);

    private static string BuildCacheKey(DateOnly date) => $"{_cacheKeyPrefix}:{date:yyyyMMdd}";

    private static string BuildPath(DateOnly date) => $"{_nbuBasePath}&date={date:yyyyMMdd}";
}
