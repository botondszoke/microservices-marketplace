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
    public class ProductManager
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductGroupRepository _productGroupRepository;

        private bool CheckCompatibility (Product p1, Product p2)
        {
            if (p1.GroupID == p2.GroupID && p1.OwnerID == p2.OwnerID && p1.Condition == p2.Condition &&
                p1.Description == p2.Description && p1.Name == p2.Name && p1.PictureLinks.SequenceEqual(p2.PictureLinks) &&
                p1.IsAvailable == p2.IsAvailable)
            {
                return true;
            }
            return false;
        }

        public ProductManager(IProductRepository productRepository, IProductGroupRepository productGroupRepository)
        {
            _productRepository = productRepository;
            _productGroupRepository = productGroupRepository;
        }

        public async Task<IReadOnlyCollection<Product>> GetAllProducts()
            => await _productRepository.GetAllProduct();

        public async Task<Product> GetProductOrNull(string id)
            => await _productRepository.GetProductOrNull(id);

        public async Task<IReadOnlyCollection<Product>> GetProductsByOwnerId(string ownerId)
            => await _productRepository.GetProductsByOwnerId(ownerId);

        public async Task<string> CreateProduct(Product product)
        {
            if (product.GroupID != null)
                return string.Empty;
            return await _productRepository.CreateProduct(product);
        }

        public async Task<bool> UpdateProduct(string id, Product oldProduct, Product newProduct)
        {
            //Ha nem elérhető, csak az elérhetőség változhat
            if (!oldProduct.IsAvailable && (oldProduct.ID != newProduct.ID ||
                oldProduct.GroupID != newProduct.GroupID ||
                oldProduct.OwnerID != newProduct.OwnerID ||
                oldProduct.Condition != newProduct.Condition ||
                oldProduct.Description != newProduct.Description ||
                oldProduct.Name != newProduct.Name ||
                !oldProduct.PictureLinks.SequenceEqual(newProduct.PictureLinks)))
                return false;

            // Ha csoporthoz akarjuk hozzáadni, ellenőrizzük a megfelelőséget
            if (newProduct.GroupID != oldProduct.GroupID)
            {
                if (newProduct.GroupID == null)
                {
                    using (var tran = new TransactionScope(
                        TransactionScopeOption.Required,
                        new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead },
                        TransactionScopeAsyncFlowOption.Enabled))
                    {
                        var result = await _productGroupRepository.RemoveProductFromGroup(oldProduct.GroupID);
                        if (result == false)
                            return false;
                        result = await _productRepository.UpdateProduct(id, newProduct);
                        if (result == false)
                            return false;
                        tran.Complete();
                        return true;
                    }
                }

                else if (oldProduct.GroupID == null)
                {
                    using (var tran = new TransactionScope(
                        TransactionScopeOption.Required,
                        new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead },
                        TransactionScopeAsyncFlowOption.Enabled))
                    {
                        var newGroup = await _productGroupRepository.GetProductGroupOrNull(newProduct.GroupID);
                        if (newGroup == null)
                            return false;
                        if (!CheckCompatibility(newProduct, newGroup.SampleProduct))
                            return false;

                        var result = await _productRepository.UpdateProduct(id, newProduct);
                        if (result == false)
                            return false;

                        result = await _productGroupRepository.AddProductToGroup(newProduct.GroupID);
                        if (result == false)
                            return false;

                        tran.Complete();
                        return true;
                    }
                }

                else
                {
                    using (var tran = new TransactionScope(
                        TransactionScopeOption.Required,
                        new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead },
                        TransactionScopeAsyncFlowOption.Enabled))
                    {
                        var newGroup = await _productGroupRepository.GetProductGroupOrNull(newProduct.GroupID);
                        if (newGroup == null)
                            return false;
                        if (!CheckCompatibility(newProduct, newGroup.SampleProduct))
                            return false;

                        var result = await _productGroupRepository.RemoveProductFromGroup(oldProduct.GroupID);
                        if (result == false)
                            return false;

                        result = await _productRepository.UpdateProduct(id, newProduct);
                        if (result == false)
                            return false;

                        result = await _productGroupRepository.AddProductToGroup(newProduct.GroupID);
                        if (result == false)
                            return false;

                        tran.Complete();
                        return true;
                    }
                }
            }

            else
            {
                if (newProduct.GroupID != null)
                {
                    var newGroup = await _productGroupRepository.GetProductGroupOrNull(newProduct.GroupID);
                    if (newGroup == null)
                        return false;
                    if (!CheckCompatibility(newProduct, newGroup.SampleProduct))
                        return false;
                }

                return await _productRepository.UpdateProduct(id, newProduct);
            }

        }

        public async Task<bool> DeleteProduct(string id, Product product)
        {
            if (!product.IsAvailable)
                return false;

            if (product.GroupID != null)
            {
                using (var tran = new TransactionScope(
                    TransactionScopeOption.Required,
                    new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead },
                    TransactionScopeAsyncFlowOption.Enabled))
                {
                    var result = await _productGroupRepository.RemoveProductFromGroup(product.GroupID);
                    if (!result)
                        return false;
                    result = await _productRepository.DeleteProduct(id);
                    if (!result)
                        return false;
                    tran.Complete();
                    return true;
                }
            }
            return await _productRepository.DeleteProduct(id);
        }

        public async Task<bool> SoldProduct(Product product, string newOwnerId)
        {
            if (product.IsAvailable == true || product.GroupID == null)
                return false;
            Product newProduct = new()
            {
                ID = product.ID,
                OwnerID = newOwnerId,
                GroupID = null,
                Name = product.Name,
                Description = product.Description,
                Condition = product.Condition,
                IsAvailable = true,
                PictureLinks = product.PictureLinks
            };
            using (var tran = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead },
                TransactionScopeAsyncFlowOption.Enabled))
            {
                var result = await _productGroupRepository.RemoveProductFromGroup(product.GroupID);
                if (result == false)
                    return false;
                result = await _productRepository.UpdateProduct(product.ID, newProduct);
                if (result == false)
                    return false;
                tran.Complete();
                return true;
            }
        }

        public async Task<Product> SoldProductFromGroup(string groupId, string newOwnerId)
        {
            using (var tran = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead },
                TransactionScopeAsyncFlowOption.Enabled))
            {
                IReadOnlyCollection<Product> products = await _productRepository.GetProductsByGroupId(groupId);
                if (products.Count <= 0)
                    return new Product() { ID = string.Empty};

                var productId = products.First().ID;
                var result = await this.SoldProduct(products.First(), newOwnerId);
                if (result == false)
                    return null;

                return await _productRepository.GetProductOrNull(productId);
            }
        }
    }
}
