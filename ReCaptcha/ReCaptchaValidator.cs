using System.Text.Json.Serialization;

namespace Ion;

public class ReCaptchaValidator
{
    public ReCaptchaValidator(ReCaptchaValidatorOptions options, IHttpClientFactory httpFactory)
    {
        _options = options;
        _httpFactory = httpFactory;
    }

    private readonly ReCaptchaValidatorOptions _options;
    private readonly IHttpClientFactory _httpFactory;

    public async Task<bool> ValidateAsync(string reCaptcha)
    {
        var client = _httpFactory.CreateClient();

        var uriBuilder = new UriBuilder(_options.RequestUri)
        {
            Query = $"secret={_options.Secret}&response={reCaptcha}"
        };

        using var message = new HttpRequestMessage(HttpMethod.Post, uriBuilder.Uri);
        using var response = await client.SendAsync(message);

        if (response.IsSuccessStatusCode)
        {
            var output = await response.Content.ReadFromJsonAsync<RequestOutput>();

            if (output != null)
            {
                return output.Success;
            }
        }

        return false;
    }

    private class RequestOutput
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }
    }
}
