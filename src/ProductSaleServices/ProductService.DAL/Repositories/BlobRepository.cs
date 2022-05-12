using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ProductService.DAL.BlobStorage;

namespace ProductService.DAL.Repositories
{
    public class BlobRepository : IBlobRepository
    {
        private readonly IBlobContext _blobContext;

        public BlobRepository(IBlobContext blobContext)
        {
            _blobContext = blobContext;
        }

        public async Task<DTOs.Blob> GetBlob(string name)
        {
            var blobClient = _blobContext.BlobContainerClient.GetBlobClient(name);
            var blobDownloadInfo = await blobClient.DownloadContentAsync();
            return new DTOs.Blob{
                Content = Convert.ToBase64String(blobDownloadInfo.Value.Content)
            };
        }

        public async Task<IEnumerable<string>> ListBlobs()
        {
            var items = new List<string>();
            await foreach(var blob in _blobContext.BlobContainerClient.GetBlobsAsync())
            {
                items.Add(blob.Name);
            }
            return items;
        }

        public async Task<string> CreateBlob(string name, DTOs.Blob blob)
        {
            string[] parts = name.Split('.');
            name = Guid.NewGuid().ToString() + '.' + parts[parts.Length-1];

            try
            {
                var blobClient = _blobContext.BlobContainerClient.GetBlobClient(name);

                await blobClient.UploadAsync(BinaryData.FromBytes(Convert.FromBase64String(blob.Content)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }

            return name;
        }

        public async Task<bool> DeleteBlob(string name)
        {
            try
            {
                var blobClient = _blobContext.BlobContainerClient.GetBlobClient(name);
                await blobClient.DeleteIfExistsAsync();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public async Task<string> UpdateBlob(string name, DTOs.Blob blob)
        {

            try
            {
                var blobClient = _blobContext.BlobContainerClient.GetBlobClient(name);
                var result = await blobClient.DeleteIfExistsAsync();
                if (!result)
                    return string.Empty;

                string[] parts = name.Split('.');
                name = Guid.NewGuid().ToString() + parts[parts.Length - 1];
                blobClient = _blobContext.BlobContainerClient.GetBlobClient(name);
                await blobClient.UploadAsync(BinaryData.FromBytes(Convert.FromBase64String(blob.Content)));
            }
            catch (Exception ex)
            {
                return string.Empty;
            }

            return name;
        }
    }
}
