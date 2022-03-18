using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleService.DAL.DTOs
{
    public class Sale
    {
        public string ID { get; set; }

        public string OwnerID { get; set; }

        public string ProductGroupID { get; set; }

        public double UnitPrice { get; set; }

        public Currency Currency { get; set; }
        
    }
}
