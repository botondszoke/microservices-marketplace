using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.DAL
{
    public class ProductGroup
    {
        [BsonId]
        public ObjectId ID { get; set; }

        [BsonElement("ownerId")]
        public ObjectId OwnerID { get; set; }

        [BsonElement("sampleProduct")]
        public Product SampleProduct { get; set; }

        //[BsonElement("productIds")]
        //public ObjectId[] ProductIDs { get; set; }

        [BsonElement("quantity")]
        public int Quantity { get; set; }
    }
}
