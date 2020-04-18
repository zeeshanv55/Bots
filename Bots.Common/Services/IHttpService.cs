namespace Bots.Common.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using Bots.Common.Models;
    using Microsoft.Extensions.Logging;

    public interface IHttpService
    {
        Task<RestApiResponse> Get(Uri url, Dictionary<string, string> headers, X509Certificate certificates = null, ILogger logger = null, NetworkCredential creds = null, string accessToken = null);

        Task<RestApiResponse> Post(Uri url, string body, string mediaOrContentType, Dictionary<string, string> headers, X509Certificate certificates = null, ILogger logger = null, NetworkCredential creds = null, string accessToken = null);

        Task<RestApiResponse> Put(Uri url, string body, string mediaOrContentType, Dictionary<string, string> headers, X509Certificate certificates = null, ILogger logger = null, NetworkCredential creds = null, string accessToken = null);

        Task<RestApiResponse> Patch(Uri url, string body, string mediaOrContentType, Dictionary<string, string> headers, X509Certificate certificates = null, ILogger logger = null, NetworkCredential creds = null, string accessToken = null);

        Task<RestApiResponse> Delete(Uri url, Dictionary<string, string> headers, X509Certificate certificates = null, ILogger logger = null, NetworkCredential creds = null, string accessToken = null);
    }
}
