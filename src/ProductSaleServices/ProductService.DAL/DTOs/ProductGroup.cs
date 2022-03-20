using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.DAL.DTOs
{
    public class ProductGroup
    {
        public string ID { get; set; }

        public string OwnerID { get; set; }

        public Product SampleProduct { get; set; }

        //public string[] ProductIDs { get; set; }

        public int Quantity { get; set; }

    }
}
