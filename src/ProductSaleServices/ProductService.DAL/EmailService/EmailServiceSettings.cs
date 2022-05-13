using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.DAL.EmailService
{
    public class EmailServiceSettings : IEmailServiceSettings
    {
        public string EmailServiceAddress { get; set; }
        public string PurchaseEndpoint { get; set; }
    }
}
