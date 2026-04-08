using Azure.AI.OpenAI;
using System.ClientModel.Primitives;

namespace AgentAIUtility.Utility
{
    /// <summary>
    /// Provides methods to monitor HTTP request and response
    /// messages of IChatClient 
    /// </summary>
    public static class HttpClientUtility
    {
        #region HttpClient

        public sealed class LoggingHandler : DelegatingHandler
        {
            public LoggingHandler(HttpMessageHandler innerHandler)
                : base(innerHandler) { }

            protected override async Task<HttpResponseMessage> SendAsync(
                HttpRequestMessage request,
                CancellationToken cancellationToken)
            {
                // Inspect the outgoing request
                Console.WriteLine($"Request URI: {request.RequestUri}");
                Console.WriteLine($"Method: {request.Method}");

                if (request.Content != null)
                {
                    var body = await request.Content.ReadAsStringAsync();
                    Console.WriteLine($"Request Body: {body}");
                }

                var response = await base.SendAsync(request, cancellationToken);

                // Inspect response if desired
                Console.WriteLine($"Response Status: {response.StatusCode}");

                return response;
            }
        }

        public static HttpClient CreateHttpClient(LoggingHandler? logHandler = null)
        {
            var innerHandler = new HttpClientHandler();
            var loggingHandler = logHandler ?? new LoggingHandler(innerHandler);

            HttpClient httpClient = new HttpClient(loggingHandler);

            return httpClient;
        }

        #endregion

        #region HttpClientPipelineTransport

        public class HttpClientPipelineTransportMonitor : HttpClientPipelineTransport
        {
            public HttpClientPipelineTransportMonitor() { }

            public HttpClientPipelineTransportMonitor(HttpClient httpClient)
                : base(httpClient) { }

            protected override void OnReceivedResponse(PipelineMessage message, HttpResponseMessage httpResponse)
            {
                base.OnReceivedResponse(message, httpResponse);
                ExploreResponse(message, httpResponse);
            }

            protected override void OnSendingRequest(PipelineMessage message, HttpRequestMessage httpRequest)
            {
                base.OnSendingRequest(message, httpRequest);
                ExploreRequest(message,httpRequest);
            }

            virtual protected void ExploreRequest(PipelineMessage message, HttpRequestMessage httpRequest)
            {
                string? body = httpRequest.Content?.ReadAsStringAsync().Result;

                PipelineRequest preq = message.Request;
                string id = preq.ClientRequestId;

                if (message.Response is PipelineResponse pres)
                {
                    bool err = pres.IsError;
                    string r = pres.ReasonPhrase;
                    int stat = pres.Status;
                }
            }

            virtual protected void ExploreResponse(PipelineMessage message, HttpResponseMessage httpResponse)
            {
                string? body = httpResponse.Content?.ReadAsStringAsync().Result;

                PipelineRequest preq = message.Request;
                string id = preq.ClientRequestId;

                if (message.Response is PipelineResponse pres)
                {
                    bool err = pres.IsError;
                    string r = pres.ReasonPhrase;
                    int stat = pres.Status;
                }
            }
        }

        public static HttpClientPipelineTransport CreateHttpClientTransport(HttpClient httpClient)
        {
            HttpClientPipelineTransport transport = new(httpClient);
            return transport;
        }

        public static HttpClientPipelineTransportMonitor CreateHttpClientTransportMonitor(HttpClient? httpClient = null)
        {
            HttpClientPipelineTransportMonitor transport;
            if (httpClient == null)
                transport = new(); // use shared HttpClient
            else transport = new(httpClient); // use custom HttpClient
            return transport;
        }

        public static AzureOpenAIClientOptions CreatezureOpenAIClientOptions(HttpClientPipelineTransport? transport = null)
        {
            var tsport = transport ?? CreateHttpClientTransportMonitor();
            var options = new AzureOpenAIClientOptions
            {
                Transport = tsport
            };
            return options;
        }

        #endregion
    }
}
