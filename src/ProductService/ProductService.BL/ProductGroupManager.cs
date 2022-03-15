using ProductService.DAL.DTOs;
using ProductService.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace ProductService.BL
{
    public class ProductGroupManager
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductGroupRepository _productGroupRepository;

        public ProductGroupManager(IProductRepository productRepository, IProductGroupRepository productGroupRepository)
        {
            _productRepository = productRepository;
            _productGroupRepository = productGroupRepository;
        }

        public async Task<IReadOnlyCollection<ProductGroup>> GetAllProductGroups()
            => await _productGroupRepository.GetAllProductGroups();

        public async Task<ProductGroup> GetProductGroupOrNull(string id)
            => await _productGroupRepository.GetProductGroupOrNull(id);

        public async Task<string> CreateProductGroup(ProductGroup productGroup)
        {
            if (productGroup.Quantity != 0)
                return string.Empty;
            return await _productGroupRepository.CreateProductGroup(productGroup);
        }

        public async Task<bool> DeleteProductGroup(string id)
        {
            using (var tran = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead },
                TransactionScopeAsyncFlowOption.Enabled))
            {
                IReadOnlyCollection<Product> products = await _productRepository.GetProductsByGroupId(id);

                foreach (Product product in products)
                {
                    var res = await _productRepository.DeleteProduct(product.ID);
                    if (res == false)
                        return false;
                }
                var result = await _productGroupRepository.DeleteProductGroup(id);
                if (result == false)
                    return false;
                tran.Complete();
                return true;
            }
        }
    }
}
