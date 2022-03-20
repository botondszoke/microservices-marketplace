using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.DAL
{
    public class ProductContext : IProductContext
    {
        public ProductContext(IProductDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            Products = database.GetCollection<Product>(settings.ProductCollectionName);
            ProductGroups = database.GetCollection<ProductGroup>(settings.ProductGroupCollectionName);
        }

        public IMongoCollection<Product> Products { get; }
        public IMongoCollection<ProductGroup> ProductGroups { get; }
    }
}
