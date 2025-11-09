using System.Text.Json;
using System.Text;

namespace MovieApp.ApiGateway.OcelotCustomMiddlewares;

public class AddUserIdHandler : DelegatingHandler
{
    private readonly ILogger<AddUserIdHandler> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AddUserIdHandler(ILogger<AddUserIdHandler> logger, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("AddUserIdHandler executing. Request: {method} {uri}", request.Method, request.RequestUri);

        // Só mexer em POST/PUT/PATCH com JSON
        if (request.Method == HttpMethod.Post || request.Method == HttpMethod.Patch)
        {
            if (request.Content != null && request.Content.Headers.ContentType?.MediaType == "application/json")
            {
                var json = await request.Content.ReadAsStringAsync(cancellationToken);

                _logger.LogDebug("Original body: {body}", json);

                try
                {
                    var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json) ?? new();
                    // pega id do contexto HTTP (claims) — o Ocelot mantém HttpContext
                    var userId = _httpContextAccessor.HttpContext?.User?.FindFirst("Id")?.Value;

                    if (!string.IsNullOrEmpty(userId))
                    {
                        dict["id"] = userId;
                        var newJson = JsonSerializer.Serialize(dict);

                        request.Content = new StringContent(newJson, Encoding.UTF8, "application/json");
                        // atualiza headers content-length (opcional)
                        request.Content.Headers.ContentLength = Encoding.UTF8.GetByteCount(newJson);

                        _logger.LogDebug("Modified body: {body}", newJson);
                    }
                    else
                    {
                        _logger.LogDebug("No Id claim found in HttpContext.User");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to modify body in AddUserIdHandler");
                }
            }
        }

        var response = await base.SendAsync(request, cancellationToken);
        _logger.LogInformation("AddUserIdHandler finished with status {status}", response.StatusCode);
        return response;
    }
}
