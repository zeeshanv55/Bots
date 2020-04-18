namespace Bots.Common.Clients
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;

    public interface IHttpClient
    {
        void Setup(Dictionary<string, string> headers, X509Certificate certificates, NetworkCredential creds, string accessToken);

        Task<HttpResponseMessage> Get(Uri url);

        Task<HttpResponseMessage> Post(Uri url, HttpContent content);

        Task<HttpResponseMessage> Put(Uri url, HttpContent content);

        Task<HttpResponseMessage> Patch(Uri url, HttpContent content);

        Task<HttpResponseMessage> Delete(Uri url);
    }
}
