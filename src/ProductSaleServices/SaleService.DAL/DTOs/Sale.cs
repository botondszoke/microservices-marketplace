using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SaleService.DAL.DTOs
{
    public class Sale
    {
        public string ID { get; set; }

        public string OwnerID { get; set; }

        public string ProductGroupID { get; set; }

        public double UnitPrice { get; set; }

        [EnumDataType(typeof(Currency))]
        public Currency Currency { get; set; }
        
    }
}
