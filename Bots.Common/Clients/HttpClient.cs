namespace Bots.Common.Clients
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;

    public class HttpClient : IHttpClient, IDisposable
    {
        private System.Net.Http.HttpClient httpClient;

        private HttpClientHandler httpClientHandler;

        ~HttpClient()
        {
            this.Dispose(false);
        }

        public void Setup(Dictionary<string, string> headers, X509Certificate certificates, NetworkCredential creds, string accessToken)
        {
            this.httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                (httpRequestMessage, cert, cetChain, policyErrors) =>
                {
                    return true;
                }
            };

            this.httpClient = new System.Net.Http.HttpClient(this.httpClientHandler);

            if (certificates != null)
            {
                this.httpClientHandler.ClientCertificates.Add(certificates);
            }

            if (creds != null && this.httpClientHandler.Credentials == null)
            {
                this.httpClientHandler.Credentials = creds;
            }

            if (accessToken != null)
            {
                this.httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }

            if (headers != null && headers.Any())
            {
                foreach (var header in headers)
                {
                    this.httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
                }
            }
        }

        public async Task<HttpResponseMessage> Get(Uri url)
        {
            return await this.httpClient.GetAsync(url).ConfigureAwait(false);
        
        }

        public async Task<HttpResponseMessage> Post(Uri url, HttpContent content)
        {
            return await this.httpClient.PostAsync(url, content).ConfigureAwait(false);
        }

        public async Task<HttpResponseMessage> Put(Uri url, HttpContent content)
        {
            return await this.httpClient.PutAsync(url, content).ConfigureAwait(false);
        }

        public async Task<HttpResponseMessage> Patch(Uri url, HttpContent content)
        {
            return await this.httpClient.PatchAsync(url, content).ConfigureAwait(false);
        }

        public async Task<HttpResponseMessage> Delete(Uri url)
        {
            return await this.httpClient.DeleteAsync(url).ConfigureAwait(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.httpClient?.Dispose();
                this.httpClientHandler?.Dispose();
            }
        }
    }
}
