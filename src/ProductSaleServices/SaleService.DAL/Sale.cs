using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleService.DAL
{
    public class Sale
    {
        [BsonId]
        public ObjectId ID { get; set; }

        [BsonElement("ownerId")]
        public ObjectId OwnerID { get; set; }

        [BsonElement("productGroupId")]
        public ObjectId ProductGroupID { get; set; }

        [BsonElement("unitPrice")]
        public double UnitPrice { get; set; }

        [BsonElement("currency")]
        public string Currency { get; set; }
    }
}
