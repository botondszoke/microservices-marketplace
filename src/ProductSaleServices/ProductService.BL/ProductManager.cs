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
        private readonly IBlobRepository _blobRepository;
        private readonly IEmailSenderRepository _emailSenderRepository;

        private bool CheckCompatibility (Product p1, Product p2)
        {
            if (p1.GroupID == p2.GroupID && p1.OwnerID == p2.OwnerID && p1.Condition == p2.Condition &&
                p1.Description == p2.Description && p1.Name == p2.Name && //p1.PictureLinks.SequenceEqual(p2.PictureLinks) &&
                p1.IsAvailable == p2.IsAvailable)
            {
                return true;
            }
            return false;
        }

        public ProductManager(IProductRepository productRepository, IProductGroupRepository productGroupRepository, IBlobRepository blobRepository, IEmailSenderRepository emailSenderRepository)
        {
            _productRepository = productRepository;
            _productGroupRepository = productGroupRepository;
            _blobRepository = blobRepository;
            _emailSenderRepository = emailSenderRepository;
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

            if (product.PictureLinks.Length != product.EncodedPictures.Length || product.PictureLinks.Length > 5)
                return string.Empty;

            using (var tran = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead },
                TransactionScopeAsyncFlowOption.Enabled))
            {
                for (int i = 0; i < product.EncodedPictures.Length; i++)
                {
                    var blob = new Blob
                    {
                        Content = product.EncodedPictures[i],
                    };
                    var name = await _blobRepository.CreateBlob(product.PictureLinks[i], blob);
                    if (name == string.Empty)
                        return string.Empty;
                    product.PictureLinks[i] = name;

                }
                var id = await _productRepository.CreateProduct(product);
                if (id == string.Empty)
                    return id;
                tran.Complete();
                return id;
            }
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
            if (newProduct.GroupID != oldProduct.GroupID )//&& oldProduct.PictureLinks == newProduct.PictureLinks)
            {
                // Kivesszük csoportból
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
                        newProduct.IsAvailable = true;
                        result = await _productRepository.UpdateProduct(id, newProduct);
                        if (result == false)
                            return false;
                        tran.Complete();
                        return true;
                    }
                }

                // Hozzáadjuk csoporthoz

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

                // Egyik csoportból a másikba
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


            // A csoport nem változik
            else
            {
                if (newProduct.GroupID != null)
                {
                    var newGroup = await _productGroupRepository.GetProductGroupOrNull(newProduct.GroupID);
                    if (newGroup == null)
                        return false;
                    //newProduct.PictureLinks = newGroup.SampleProduct.PictureLinks;
                    if (!CheckCompatibility(newProduct, newGroup.SampleProduct))
                        return false;
                }

                using (var tran = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead },
                TransactionScopeAsyncFlowOption.Enabled))
                {
                    // Ha több kép lett
                    if (newProduct.PictureLinks.Length > oldProduct.PictureLinks.Length)
                    {
                        for (int i = 0; i < oldProduct.PictureLinks.Length; i++)
                        {
                            if (oldProduct.PictureLinks[i] != newProduct.PictureLinks[i])
                            {
                                var deleteResult = await _blobRepository.DeleteBlob(oldProduct.PictureLinks[i]);
                                if (deleteResult == false)
                                    return false;

                                var blob = new Blob
                                {
                                    Content = newProduct.EncodedPictures[i],
                                };
                                var name = await _blobRepository.CreateBlob(newProduct.PictureLinks[i], blob);
                                if (name == string.Empty)
                                    return false;
                                newProduct.PictureLinks[i] = name;
                            }
                        }
                        for (int i = oldProduct.PictureLinks.Length; i < newProduct.PictureLinks.Length; i++)
                        {
                            var blob = new Blob
                            {
                                Content = newProduct.EncodedPictures[i],
                            };
                            var name = await _blobRepository.CreateBlob(newProduct.PictureLinks[i], blob);
                            if (name == string.Empty)
                                return false;
                            newProduct.PictureLinks[i] = name;
                        }
                    }
                    // Ha kevesebb vagy ugyanannyi kép lett
                    else
                    {
                        for (int i = 0; i < newProduct.PictureLinks.Length; i++)
                        {
                            if (oldProduct.PictureLinks[i] != newProduct.PictureLinks[i])
                            {
                                var deleteResult = await _blobRepository.DeleteBlob(oldProduct.PictureLinks[i]);
                                if (deleteResult == false)
                                    return false;

                                var blob = new Blob
                                {
                                    Content = newProduct.EncodedPictures[i],
                                };
                                var name = await _blobRepository.CreateBlob(newProduct.PictureLinks[i], blob);
                                if (name == string.Empty)
                                    return false;
                                newProduct.PictureLinks[i] = name;
                            }
                        }
                        for (int i = newProduct.PictureLinks.Length; i < oldProduct.PictureLinks.Length; i++)
                        {
                            var deleteResult = await _blobRepository.DeleteBlob(oldProduct.PictureLinks[i]);
                            if (deleteResult == false)
                                return false;
                        }
                    }
                    var result = await _productRepository.UpdateProduct(id, newProduct);
                    if (result == false)
                        return false;
                    tran.Complete();
                    return true;
                }
            }

        }

        public async Task<bool> DeleteProduct(string id, Product product)
        {
            if (!product.IsAvailable)
                return false;

            using (var tran = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead },
                TransactionScopeAsyncFlowOption.Enabled))
            {
                if (product.GroupID != null)
                {
                    var groupDeleteResult = await _productGroupRepository.RemoveProductFromGroup(product.GroupID);
                    if (!groupDeleteResult)
                        return false;
                }

                for (int i = 0; i < product.PictureLinks.Length; i++)
                {
                    var deleteResult = await _blobRepository.DeleteBlob(product.PictureLinks[i]);
                    if (deleteResult == false)
                        return false;
                }
                var result = await _productRepository.DeleteProduct(id);
                if (!result)
                    return false;
                tran.Complete();
                return true;
            }
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
                PictureLinks = product.PictureLinks,
                EncodedPictures = product.EncodedPictures
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

        public async Task<List<Product>> SoldProductFromGroup(string groupId, string newOwnerId, int quantity)
        {
            using (var tran = new TransactionScope(
                TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = IsolationLevel.RepeatableRead },
                TransactionScopeAsyncFlowOption.Enabled))
            {
                IReadOnlyCollection<Product> products = await _productRepository.GetProductsByGroupId(groupId);
                List<Product> sold = new List<Product>();
                if (products.Count < quantity || products.Count <= 0 /*|| products.First().OwnerID == newOwnerId*/)
                {
                    sold.Add(new Product() { ID = string.Empty });
                    return sold;
                }

                for (int i = 0; i < quantity; i++)
                {
                    var productId = products.ElementAt(i).ID;
                    var result = await this.SoldProduct(products.ElementAt(i), newOwnerId);
                    if (result == false)
                        return new List<Product>();
                    sold.Add(await _productRepository.GetProductOrNull(productId));
                }

                return sold;
            }
        }

        public async Task<bool> SendPurchaseEmail(Product product, int quantity, string ownerID)
            => await _emailSenderRepository.SendPurchaseEmail(product, quantity, ownerID);
    }
}
