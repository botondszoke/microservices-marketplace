using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.DAL.BlobStorage
{
    public class BlobStorageSettings : IBlobStorageSettings
    {
        public string ConnectionString { get; set; }
        public string ContainerName { get; set; }
    }
}
