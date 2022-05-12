using Azure.Storage.Blobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.DAL.BlobStorage
{
    public interface IBlobContext
    {
        BlobContainerClient BlobContainerClient { get; }

    }
}
