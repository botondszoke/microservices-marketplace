using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleService.DAL.SaleDatabase
{
    public class SaleContext : ISaleContext
    {
        public SaleContext(ISaleDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            Sales = database.GetCollection<Sale>(settings.CollectionName);
        }

        public IMongoCollection<Sale> Sales { get; }
    }
}
