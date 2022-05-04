using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.DAL.Repositories
{
    public interface IBlobRepository
    {
        public Task<DTOs.Blob> GetBlob(string name);
        public Task<IEnumerable<string>> ListBlobs();
        public Task<string> CreateBlob(string name, DTOs.Blob blob);
        public Task<bool> DeleteBlob(string name);
        public Task<string> UpdateBlob(string name, DTOs.Blob blob);
    }
}
