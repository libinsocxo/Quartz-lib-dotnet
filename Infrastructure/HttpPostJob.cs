using LinkedinSchedulers.Models;
using Quartz;
using System.Security.Policy;
using System.Text.Json;

namespace LinkedinSchedulers.Infrastructure
{
    public class HttpPostJob : IJob
    {

        private readonly ILogger<HttpPostJob> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpPostJob(ILogger<HttpPostJob> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }
        public async Task Execute(IJobExecutionContext context)
        {
            //_logger.LogInformation($"function has called at {DateTime.Now}");



            var jsonData = context.JobDetail.JobDataMap.GetString("data");

            var request = JsonSerializer.Deserialize<ScheduleRequest>(jsonData);

            var url = request.Url;




            if (string.IsNullOrEmpty(url))
            {
                _logger.LogError("No URL provided for HttpPostJob.");

                return;
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Successfully called {url} at {DateTime.UtcNow}.");
                }
                else
                {
                    _logger.LogError($"Failed to call {url}. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception occurred while calling {url}.");
            }
        }

    }
}
