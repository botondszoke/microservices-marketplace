using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.DAL.Repositories
{
    public interface IProductGroupRepository
    {
        public Task<IReadOnlyCollection<DTOs.ProductGroup>> GetAllProductGroups();
        public Task<DTOs.ProductGroup> GetProductGroupOrNull(string id);
        public Task<IReadOnlyCollection<DTOs.ProductGroup>> GetProductGroupsByOwnerId(string ownerId);
        public Task<IReadOnlyCollection<DTOs.ProductGroup>> GetUnavailableProductGroups();
        public Task<DTOs.ProductGroup> CreateProductGroup(DTOs.ProductGroup productGroup);
        public Task<bool> DeleteProductGroup(string id);
        public Task<bool> UpdateProductGroup(string id, DTOs.ProductGroup productGroup);
        public Task<bool> RemoveProductFromGroup(string id);
        public Task<bool> AddProductToGroup(string id);
    }
}
