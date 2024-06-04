using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WeatherApplication
{
    public sealed class HttpClientWrapper : IDisposable
    {
        private static readonly Lazy<HttpClientWrapper> m_instance =
            new Lazy<HttpClientWrapper>(() => new HttpClientWrapper());
        private readonly HttpClient m_httpClient;

        private HttpClientWrapper()
        {
            m_httpClient = new HttpClient();
            m_httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        public static HttpClientWrapper Instance => m_instance.Value;

        // Implement IDisposable pattern
        public void Dispose()
        {
            m_httpClient.Dispose();
        }

        // Expose the HttpClient instance if needed
        public HttpClient Client => m_httpClient;

        // Implement GetAsync method
        public async Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            try
            {
                // Send GET request asynchronously
                return await m_httpClient.GetAsync(requestUri);
            }
            catch (HttpRequestException ex)
            {
                // Handle HTTP request exceptions
                throw new HttpClientWrapperException("An error occurred while sending the HTTP request.", ex);
            }
            catch (TaskCanceledException ex)
            {
                throw new HttpClientWrapperException("The HTTP request timed out.", ex);
            }
        }
    }

    public class HttpClientWrapperException : Exception
    {
        public HttpClientWrapperException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
