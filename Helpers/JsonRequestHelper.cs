using System.Text.Json;

namespace MovieApp.ApiGateway.Helpers;

public static class JsonRequestHelper
{
    public static StringContent AddUserId(string jsonBody, string userId)
    {
        var json = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonBody);

        if (!string.IsNullOrEmpty(userId))
        {
            json["id"] = userId;
        }

        var updatedJson = JsonSerializer.Serialize(json);
        return new StringContent(updatedJson, System.Text.Encoding.UTF8, "application/json");
    }
}
