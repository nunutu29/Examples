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
        // ask for HttpClient
        var client = _httpFactory.CreateClient();

        // create the request uri and assign secret and response parameters
        var uriBuilder = new UriBuilder(_options.RequestUri)
        {
            Query = $"secret={_options.Secret}&response={reCaptcha}"
        };

        // create the request message with POST method
        // https://developers.google.com/recaptcha/docs/verify
        using var message = new HttpRequestMessage(HttpMethod.Post, uriBuilder.Uri);
        
        // send the request
        using var response = await client.SendAsync(message);

        if (response.IsSuccessStatusCode)
        {
            // read the response and check if succeed
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
        
        // Other properties ignored
    }
}
