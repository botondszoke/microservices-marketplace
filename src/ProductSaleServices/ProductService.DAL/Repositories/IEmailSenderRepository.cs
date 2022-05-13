using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.DAL.Repositories
{
    public interface IEmailSenderRepository
    {
        public Task<bool> SendPurchaseEmail(DTOs.Product product, int quantity, string ownerID);
    }
}
