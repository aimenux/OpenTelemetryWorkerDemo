using App.Helpers;

namespace App
{
    public class Proxy
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<Proxy> _logger;

        public Proxy(HttpClient httpClient, IConfiguration configuration, ILogger<Proxy> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task PrintAsync()
        {
            using var activity = OpenTelemetrySource.Instance.StartActivity();
            var url = _configuration.GetValue<string>("ProxyUrl");
            var response = await _httpClient.GetStringAsync(url);
            _logger.LogInformation("Response: {response}", response);
            activity!.SetTag("response", response);
        }
    }
}
