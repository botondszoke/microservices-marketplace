using ProductService.DAL.DTOs;
using ProductService.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.BL
{
    public class ProductManager
    {
        private readonly IProductRepository _productRepository;

        public ProductManager(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IReadOnlyCollection<Product>> GetAllProducts()
            => await _productRepository.GetAllProduct();

        public async Task<Product> GetProductOrNull(string id)
            => await _productRepository.GetProductOrNull(id);

        public async Task<string> CreateProduct(Product product)
            => await _productRepository.CreateProduct(product);

        public async Task<bool> UpdateProduct(string id, Product product)
            => await _productRepository.UpdateProduct(id, product);

        public async Task<bool> DeleteProduct(string id)
            => await _productRepository.DeleteProduct(id);
    }
}
