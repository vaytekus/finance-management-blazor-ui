using System.Text.Json;
using System.Text.Json.Serialization;

namespace FinanceManagement.Web.Services.Common;

public static class ApiJsonOptions
{
    public static JsonSerializerOptions Default { get; } = new(JsonSerializerDefaults.Web)
    {
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };
}
