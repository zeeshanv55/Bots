namespace Bots.Common.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Bots.Common.Clients;
    using Bots.Common.Helpers;
    using Bots.Common.Models;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    public class HttpService : IHttpService
    {
        private readonly IHttpClient httpClient;

        private readonly int retryLimit;

        private readonly int retryWaitDuration;

        public HttpService(IHttpClient httpClient, IConfiguration configuration)
        {
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));

            if (configuration != null)
            {
                if (!int.TryParse(configuration["HttpRetryLimit"], out this.retryLimit))
                {
                    this.retryLimit = 0;
                }

                if (!int.TryParse(configuration["HttpRetryWaitDuration"], out this.retryWaitDuration))
                {
                    this.retryWaitDuration = 0;
                }
            }

            if (this.retryLimit <= 0)
            {
                this.retryLimit = 3;
            }

            if (this.retryWaitDuration <= 0)
            {
                this.retryWaitDuration = 5000;
            }
        }

        public async Task<RestApiResponse> Get(Uri url, Dictionary<string, string> headers, X509Certificate certificates = null, ILogger logger = null, NetworkCredential creds = null, string accessToken = null)
        {
            var currentRetryCount = 0;
            var currentWaitTime = 0;
            var returnResponse = new RestApiResponse();
            
            do
            {
                try
                {
                    Thread.Sleep(currentWaitTime);
                    this.httpClient.Setup(headers, certificates, creds, accessToken);

                    var response = await this.httpClient.Get(url).ConfigureAwait(false);
                    var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    returnResponse = new RestApiResponse
                    {
                        Content = result,
                        StatusCode = response.StatusCode,
                        IsSuccessStatusCode = response.IsSuccessStatusCode,
                    };
                }
                catch (Exception exception)
                {
                    logger?.LogError(exception, $"Discord.JonSnowBot.Common.Services.HttpService.Get: An exception occured during HTTP GET request. URL: {url}.");
                    returnResponse = new RestApiResponse
                    {
                        Content = ExceptionHelper.GetAllExceptionText(exception),
                        StatusCode = HttpStatusCode.InternalServerError,
                        IsSuccessStatusCode = false,
                    };
                }
                finally
                {
                    if (!returnResponse.IsSuccessStatusCode)
                    {
                        logger?.LogWarning($"Discord.JonSnowBot.Common.Services.HttpService.Get: An error occured during HTTP GET request. URL: {url}. currentRetryCount: {currentRetryCount}. currentWaitTime: {currentWaitTime}");
                        currentRetryCount++;
                        currentWaitTime += currentRetryCount * this.retryWaitDuration;
                    }
                }
            }
            while (!returnResponse.IsSuccessStatusCode && currentRetryCount < this.retryLimit);

            return returnResponse;
        }

        public async Task<RestApiResponse> Post(Uri url, string body, string mediaOrContentType, Dictionary<string, string> headers, X509Certificate certificates = null, ILogger logger = null, NetworkCredential creds = null, string accessToken = null)
        {
            var currentRetryCount = 0;
            var currentWaitTime = 0;
            var returnResponse = new RestApiResponse();

            do
            {
                try
                {
                    Thread.Sleep(currentWaitTime);
                    this.httpClient.Setup(headers, certificates, creds, accessToken);

                    var content = new StringContent(body, Encoding.UTF8, mediaOrContentType);
                    var response = await this.httpClient.Post(url, content).ConfigureAwait(false);
                    var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    content.Dispose();

                    returnResponse = new RestApiResponse
                    {
                        Content = result,
                        StatusCode = response.StatusCode,
                        IsSuccessStatusCode = response.IsSuccessStatusCode,
                    };
                }
                catch (Exception exception)
                {
                    logger?.LogError(exception, $"Discord.JonSnowBot.Common.Services.HttpService.Post: An exception occured during HTTP POST request. URL: {url}. Body: {body}.");
                    returnResponse = new RestApiResponse
                    {
                        Content = ExceptionHelper.GetAllExceptionText(exception),
                        StatusCode = HttpStatusCode.InternalServerError,
                        IsSuccessStatusCode = false,
                    };
                }
                finally
                {
                    if (!returnResponse.IsSuccessStatusCode)
                    {
                        logger?.LogWarning($"Discord.JonSnowBot.Common.Services.HttpService.Post: An error occured during HTTP POST request. URL: {url}. Body: {body}. currentRetryCount: {currentRetryCount}. currentWaitTime: {currentWaitTime}");
                        currentRetryCount++;
                        currentWaitTime += currentRetryCount * this.retryWaitDuration;
                    }
                }
            }
            while (!returnResponse.IsSuccessStatusCode && currentRetryCount < this.retryLimit);

            return returnResponse;
        }

        public async Task<RestApiResponse> Put(Uri url, string body, string mediaOrContentType, Dictionary<string, string> headers, X509Certificate certificates = null, ILogger logger = null, NetworkCredential creds = null, string accessToken = null)
        {
            var currentRetryCount = 0;
            var currentWaitTime = 0;
            var returnResponse = new RestApiResponse();

            do
            {
                try
                {
                    Thread.Sleep(currentWaitTime);
                    this.httpClient.Setup(headers, certificates, creds, accessToken);

                    var content = new StringContent(body, Encoding.UTF8, mediaOrContentType);
                    var response = await this.httpClient.Put(url, content).ConfigureAwait(false);
                    var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    content.Dispose();

                    returnResponse = new RestApiResponse
                    {
                        Content = result,
                        StatusCode = response.StatusCode,
                        IsSuccessStatusCode = response.IsSuccessStatusCode,
                    };
                }
                catch (Exception exception)
                {
                    logger?.LogError(exception, $"Discord.JonSnowBot.Common.Services.HttpService.Put: An exception occured during HTTP PUT request. URL: {url}. Body: {body}.");
                    returnResponse = new RestApiResponse
                    {
                        Content = ExceptionHelper.GetAllExceptionText(exception),
                        StatusCode = HttpStatusCode.InternalServerError,
                        IsSuccessStatusCode = false,
                    };
                }
                finally
                {
                    if (!returnResponse.IsSuccessStatusCode)
                    {
                        logger?.LogWarning($"Discord.JonSnowBot.Common.Services.HttpService.Put: An error occured during HTTP PUT request. URL: {url}. Body: {body}. currentRetryCount: {currentRetryCount}. currentWaitTime: {currentWaitTime}");
                        currentRetryCount++;
                        currentWaitTime += currentRetryCount * this.retryWaitDuration;
                    }
                }
            }
            while (!returnResponse.IsSuccessStatusCode && currentRetryCount < this.retryLimit);

            return returnResponse;
        }

        public async Task<RestApiResponse> Patch(Uri url, string body, string mediaOrContentType, Dictionary<string, string> headers, X509Certificate certificates, ILogger logger = null, NetworkCredential creds = null, string accessToken = null)
        {
            var currentRetryCount = 0;
            var currentWaitTime = 0;
            var returnResponse = new RestApiResponse();

            do
            {
                try
                {
                    Thread.Sleep(currentWaitTime);
                    this.httpClient.Setup(headers, certificates, creds, accessToken);

                    var content = new StringContent(body, Encoding.UTF8, mediaOrContentType);
                    var response = await this.httpClient.Patch(url, content).ConfigureAwait(false);
                    var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    content.Dispose();

                    returnResponse = new RestApiResponse
                    {
                        Content = result,
                        StatusCode = response.StatusCode,
                        IsSuccessStatusCode = response.IsSuccessStatusCode,
                    };
                }
                catch (Exception exception)
                {
                    logger?.LogError(exception, $"Discord.JonSnowBot.Common.Services.HttpService.Patch: An exception occured during HTTP PATCH request. URL: {url}. Body: {body}.");
                    returnResponse = new RestApiResponse
                    {
                        Content = ExceptionHelper.GetAllExceptionText(exception),
                        StatusCode = HttpStatusCode.InternalServerError,
                        IsSuccessStatusCode = false,
                    };
                }
                finally
                {
                    if (!returnResponse.IsSuccessStatusCode)
                    {
                        logger?.LogWarning($"Discord.JonSnowBot.Common.Services.HttpService.Patch: An error occured during HTTP PATCH request. URL: {url}. Body: {body}. currentRetryCount: {currentRetryCount}. currentWaitTime: {currentWaitTime}");
                        currentRetryCount++;
                        currentWaitTime += currentRetryCount * this.retryWaitDuration;
                    }
                }
            }
            while (!returnResponse.IsSuccessStatusCode && currentRetryCount < this.retryLimit);

            return returnResponse;
        }

        public async Task<RestApiResponse> Delete(Uri url, Dictionary<string, string> headers, X509Certificate certificates, ILogger logger = null, NetworkCredential creds = null, string accessToken = null)
        {
            var currentRetryCount = 0;
            var currentWaitTime = 0;
            var returnResponse = new RestApiResponse();

            do
            {
                try
                {
                    Thread.Sleep(currentWaitTime);
                    this.httpClient.Setup(headers, certificates, creds, accessToken);

                    var response = await this.httpClient.Delete(url).ConfigureAwait(false);
                    var result = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    returnResponse = new RestApiResponse
                    {
                        Content = result,
                        StatusCode = response.StatusCode,
                        IsSuccessStatusCode = response.IsSuccessStatusCode,
                    };
                }
                catch (Exception exception)
                {
                    logger?.LogError(exception, $"Discord.JonSnowBot.Common.Services.HttpService.Delete: An exception occured during HTTP DELETE request. URL: {url}.");
                    returnResponse = new RestApiResponse
                    {
                        Content = ExceptionHelper.GetAllExceptionText(exception),
                        StatusCode = HttpStatusCode.InternalServerError,
                        IsSuccessStatusCode = false,
                    };
                }
                finally
                {
                    if (!returnResponse.IsSuccessStatusCode)
                    {
                        logger?.LogWarning($"Discord.JonSnowBot.Common.Services.HttpService.Delete: An error occured during HTTP DELETE request. URL: {url}. currentRetryCount: {currentRetryCount}. currentWaitTime: {currentWaitTime}");
                        currentRetryCount++;
                        currentWaitTime += currentRetryCount * this.retryWaitDuration;
                    }
                }
            }
            while (!returnResponse.IsSuccessStatusCode && currentRetryCount < this.retryLimit);

            return returnResponse;
        }
    }
}
