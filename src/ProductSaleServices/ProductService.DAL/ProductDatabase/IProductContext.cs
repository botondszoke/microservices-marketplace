using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.DAL.ProductDatabase
{
    public interface IProductContext
    {
        IMongoCollection<Product> Products { get; }
        IMongoCollection<ProductGroup> ProductGroups { get; }
    }
}
