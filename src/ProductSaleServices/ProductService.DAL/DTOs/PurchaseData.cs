using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.DAL.DTOs
{
    public class PurchaseData
    {
        public string ProductName { get; set; }
        public string ProductCondition { get; set; }
        public string ProductDescription { get; set; }
        public string Quantity { get; set; }
        public string ToEmail { get; set; }

    }
}
