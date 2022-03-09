using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.DAL.DTOs
{
    public class Product
    {
        public string ID { get; set; }
        public string OwnerID { get; set; }

        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Condition { get; set; }
        public int StockQuantity { get; set; }
        public string[] PictureLinks { get; set; }
    }
}
