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

        [BsonElement("ownerId")]
        public ObjectId OwnerID { get; set; }

        [BsonElement("groupId")]
        public ObjectId? GroupID { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("description")]
        public string? Description { get; set; }

        [BsonElement("condition")]
        public string? Condition { get; set; }

        [BsonElement("isAvailable")]
        public bool IsAvailable { get; set; }

        [BsonElement("pictureLinks")]
        public string[] PictureLinks { get; set; }
    }
}
