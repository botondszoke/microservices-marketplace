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

        public async Task<IReadOnlyCollection<ProductGroup>> GetProductGroupsByOwnerId(string ownerId)
            => await _productGroupRepository.GetProductGroupsByOwnerId(ownerId);

        public async Task<IReadOnlyCollection<ProductGroup>> GetUnavailableProductGroups()
            => await _productGroupRepository.GetUnavailableProductGroups();

        public async Task<ProductGroup> CreateProductGroup(ProductGroup productGroup)
        {

            if (productGroup.Quantity != 0)
            {
                productGroup.ID = string.Empty;
                return productGroup;
            }
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

        public async Task<bool> UpdateProductGroup(string id, ProductGroup oldProductGroup, ProductGroup newProductGroup)
        {
            if (oldProductGroup.OwnerID != newProductGroup.OwnerID || oldProductGroup.Quantity != newProductGroup.Quantity)
                return false;

            if (oldProductGroup.SampleProduct.ID == newProductGroup.SampleProduct.ID &&
                oldProductGroup.SampleProduct.GroupID == newProductGroup.SampleProduct.GroupID &&
                oldProductGroup.SampleProduct.OwnerID == newProductGroup.SampleProduct.OwnerID &&
                oldProductGroup.SampleProduct.Condition == newProductGroup.SampleProduct.Condition &&
                oldProductGroup.SampleProduct.Description == newProductGroup.SampleProduct.Description &&
                oldProductGroup.SampleProduct.Name == newProductGroup.SampleProduct.Name &&
                oldProductGroup.SampleProduct.PictureLinks.SequenceEqual(newProductGroup.SampleProduct.PictureLinks))
            {
                using (var tran = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead },
                TransactionScopeAsyncFlowOption.Enabled))
                {
                    IReadOnlyCollection<Product> products = await _productRepository.GetProductsByGroupId(id);

                    foreach (Product product in products)
                    {
                        Product newProduct = new()
                        {
                            ID = product.ID,
                            GroupID = product.GroupID,
                            OwnerID = product.OwnerID,
                            Condition = product.Condition,
                            Description = product.Description,  
                            Name = product.Name,
                            PictureLinks = product.PictureLinks,
                            IsAvailable = newProductGroup.SampleProduct.IsAvailable,
                        };
                        var res = await _productRepository.UpdateProduct(newProduct.ID, newProduct);
                        if (res == false)
                            return false;
                    }
                    var result = await _productGroupRepository.UpdateProductGroup(id, newProductGroup);
                    if (result == false)
                        return false;
                    tran.Complete();
                    return true;
                }
            }

            return false;
        }
    }
}
