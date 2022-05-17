using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.DAL.EmailService
{
    public class EmailServiceContext : IEmailServiceContext
    {
        public HttpClient HttpClient { get; }
        public string purchaseEndpoint { get; }

        public EmailServiceContext(IEmailServiceSettings settings, HttpClient client)
        {
            HttpClient = client;
            HttpClient.BaseAddress = new Uri(settings.EmailServiceAddress);
            HttpClient.DefaultRequestHeaders.Accept.Clear();
            HttpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            purchaseEndpoint = settings.PurchaseEndpoint;
        }
    }
}
