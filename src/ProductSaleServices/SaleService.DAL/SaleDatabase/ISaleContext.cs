using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaleService.DAL.SaleDatabase
{
    public interface ISaleContext
    {
        public IMongoCollection<Sale> Sales { get; }
    }
}
