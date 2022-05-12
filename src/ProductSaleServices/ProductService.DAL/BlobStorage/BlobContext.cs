using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.DAL.BlobStorage
{
    public class BlobContext : IBlobContext
    {
        public BlobContext(IBlobStorageSettings settings)
        {
            BlobServiceClient = new BlobServiceClient(settings.ConnectionString);
            BlobContainerClient = CreateOrGetContainer(BlobServiceClient, settings.ContainerName);
        }

        private BlobServiceClient BlobServiceClient { get; }

        public BlobContainerClient BlobContainerClient { get; }

        private static BlobContainerClient CreateOrGetContainer(BlobServiceClient blobServiceClient, string containerName)
        {
            try
            {
                // Create the container or handle the exception if it already exists
                BlobContainerClient container = blobServiceClient.CreateBlobContainer(containerName, Azure.Storage.Blobs.Models.PublicAccessType.Blob);
                Console.WriteLine("Created container {0}", container.Name);
                return container;
            }
            catch (Azure.RequestFailedException e)
            {
                Console.WriteLine("HTTP error code {0}: {1}",
                                    e.Status, e.ErrorCode);
                Console.WriteLine(e.Message);
            }

            try
            {
                BlobContainerClient container = blobServiceClient.GetBlobContainerClient(containerName);
                Console.WriteLine("Read container {0}", container.Name);
                return container;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }
    }
}
