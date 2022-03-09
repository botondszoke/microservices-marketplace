using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.DAL
{
    public class Product
    {
        [BsonId]
        public ObjectId ID { get; set; }
        public ObjectId OwnerID { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Condition { get; set; }
        public int StockQuantity { get; set; }
        public string[] PictureLinks { get; set; }
    }
}
