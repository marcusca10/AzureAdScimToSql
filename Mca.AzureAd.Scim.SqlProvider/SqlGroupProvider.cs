using Microsoft.SCIM;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Mca.AzureAd.Scim.SqlProvider
{
    public class SqlGroupProvider : ProviderBase
    {
        public override Task<Resource> CreateAsync(Resource resource, string correlationIdentifier)
        {
            throw new NotImplementedException();
        }

        public override Task DeleteAsync(IResourceIdentifier resourceIdentifier, string correlationIdentifier)
        {
            throw new NotImplementedException();
        }

        public override Task<Resource> RetrieveAsync(IResourceRetrievalParameters parameters, string correlationIdentifier)
        {
            throw new NotImplementedException();
        }

        public override Task UpdateAsync(IPatch patch, string correlationIdentifier)
        {
            throw new NotImplementedException();
        }

        // override missing
        public override Task<Resource[]> QueryAsync(IQueryParameters parameters, string correlationIdentifier)
        {
            throw new NotImplementedException();
        }

        // override missing
        public override Task<Resource> ReplaceAsync(Resource resource, string correlationIdentifier)
        {
            throw new NotImplementedException();
        }
    }
}
