namespace Bots.Common.Models
{
    using System.Net;

    public class RestApiResponse
    {
        public string Content { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public bool IsSuccessStatusCode { get; set; }
    }
}
