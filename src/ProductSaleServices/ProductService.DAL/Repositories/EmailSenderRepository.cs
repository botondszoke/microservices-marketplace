using ProductService.DAL.DTOs;
using ProductService.DAL.EmailService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.DAL.Repositories
{
    public class EmailSenderRepository : IEmailSenderRepository
    {
        private readonly IEmailServiceContext _emailServiceContext;

        public EmailSenderRepository(IEmailServiceContext emailServiceContext)
        {
            _emailServiceContext = emailServiceContext;
        }
        public async Task<bool> SendPurchaseEmail(DTOs.Product product, int quantity, string ownerID)
        {
            PurchaseData data = new PurchaseData()
            {
                ProductName = product.Name,
                ProductCondition = product.Condition ?? "",
                ProductDescription = product.Description ?? "",
                Quantity = quantity.ToString(),
                ToEmail = ownerID
            };
            bool success = true;
            try
            {
                HttpResponseMessage response = await _emailServiceContext.HttpClient.PostAsJsonAsync(_emailServiceContext.purchaseEndpoint, data);
            }
            catch (Exception ex)
            {
                success = false;
                Console.WriteLine("Could not reach Email Service API. {0}", ex.Message);
            }
            return success;
        }
    }
}
