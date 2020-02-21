using Microsoft.EntityFrameworkCore;
using Microsoft.SCIM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Mca.AzureAd.Scim.SqlProvider
{
    public class SqlUserProvider : ProviderBase
    {
        private readonly SqlProviderContext context;

        public SqlUserProvider()
        {
            this.context = new SqlProviderContext();
        }

        public override async Task<Resource> CreateAsync(Resource resource, string correlationIdentifier)
        {
            if (resource.Identifier != null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            Core2EnterpriseUser user = resource as Core2EnterpriseUser;
            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            if (await this.context.Users.AnyAsync(u => u.UserName == user.UserName))
            {
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }

            string resourceIdentifier = Guid.NewGuid().ToString();
            user.Identifier = resourceIdentifier;
            await this.context.Users.AddAsync(user);
            await this.context.SaveChangesAsync();
            return await Task.FromResult(user as Resource);
        }

        public override async Task DeleteAsync(IResourceIdentifier resourceIdentifier, string correlationIdentifier)
        {
            if (string.IsNullOrWhiteSpace(resourceIdentifier?.Identifier))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            string identifier = resourceIdentifier.Identifier;

            Core2EnterpriseUser user = await this.context.Users.Where(u => u.Identifier == identifier).FirstOrDefaultAsync();
            if (user == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            this.context.Users.Remove(user);
            await this.context.SaveChangesAsync();
        }

        public override async Task<Resource> RetrieveAsync(IResourceRetrievalParameters parameters, string correlationIdentifier)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (string.IsNullOrWhiteSpace(correlationIdentifier))
            {
                throw new ArgumentNullException(nameof(correlationIdentifier));
            }

            if (string.IsNullOrEmpty(parameters?.ResourceIdentifier?.Identifier))
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            Resource result = null;
            string identifier = parameters.ResourceIdentifier.Identifier;

            Core2EnterpriseUser user = await this.context.Users.Where(u => u.Identifier == identifier)
                .Include(u => u.Addresses)
                .Include(u => u.ElectronicMailAddresses)
                .Include(u => u.InstantMessagings)
                .Include(u => u.PhoneNumbers)
                .AsNoTracking().FirstOrDefaultAsync();
            if (user == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            result = user as Resource;
            return await Task.FromResult(result);
        }

        public override async Task UpdateAsync(IPatch patch, string correlationIdentifier)
        {
            if (null == patch)
            {
                throw new ArgumentNullException(nameof(patch));
            }

            if (null == patch.ResourceIdentifier)
            {
                throw new ArgumentException(SqlProviderResources.ExceptionInvalidPatch);
            }

            if (string.IsNullOrWhiteSpace(patch.ResourceIdentifier.Identifier))
            {
                throw new ArgumentException(SqlProviderResources.ExceptionInvalidPatch);
            }

            if (null == patch.PatchRequest)
            {
                throw new ArgumentException(SqlProviderResources.ExceptionInvalidPatch);
            }

            PatchRequest2 patchRequest = patch.PatchRequest as PatchRequest2;

            if (null == patchRequest)
            {
                string unsupportedPatchTypeName = patch.GetType().FullName;
                throw new NotSupportedException(unsupportedPatchTypeName);
            }

            Core2EnterpriseUser user = await this.context.Users.Where(u => u.Identifier == patch.ResourceIdentifier.Identifier)
                .Include(u => u.Addresses)
                .Include(u => u.ElectronicMailAddresses)
                .Include(u => u.InstantMessagings)
                .Include(u => u.PhoneNumbers)
                .FirstOrDefaultAsync();
            if (user == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            user.Apply(patchRequest);
            this.context.Users.Update(user);
            await this.context.SaveChangesAsync();
        }

        // override missing
        public override async Task<Resource[]> QueryAsync(IQueryParameters parameters, string correlationIdentifier)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            if (string.IsNullOrWhiteSpace(correlationIdentifier))
            {
                throw new ArgumentNullException(nameof(correlationIdentifier));
            }

            if (null == parameters.AlternateFilters)
            {
                throw new ArgumentException(SqlProviderResources.ExceptionInvalidParameters);
            }

            if (string.IsNullOrWhiteSpace(parameters.SchemaIdentifier))
            {
                throw new ArgumentException(SqlProviderResources.ExceptionInvalidParameters);
            }

            Resource[] results;
            IFilter queryFilter = parameters.AlternateFilters.SingleOrDefault();
            if (queryFilter == null)
            {
                IEnumerable<Core2EnterpriseUser> allUsers = await this.context.Users
                    .Include(u => u.Addresses)
                    .Include(u => u.ElectronicMailAddresses)
                    .Include(u => u.InstantMessagings)
                    .Include(u => u.PhoneNumbers)
                    .AsNoTracking().ToListAsync();
                results = allUsers.Select((Core2EnterpriseUser user) => user as Resource).ToArray();

                return await Task.FromResult(results);
            }

            if (string.IsNullOrWhiteSpace(queryFilter.AttributePath))
            {
                throw new ArgumentException(SqlProviderResources.ExceptionInvalidParameters);
            }

            if (string.IsNullOrWhiteSpace(queryFilter.ComparisonValue))
            {
                throw new ArgumentException(SqlProviderResources.ExceptionInvalidParameters);
            }

            if (queryFilter.FilterOperator != ComparisonOperator.Equals)
            {
                throw new NotSupportedException(SqlProviderResources.UnsupportedComparisonOperator);
            }

            if (queryFilter.AttributePath.Equals(AttributeNames.UserName))
            {
                IEnumerable<Core2EnterpriseUser> filteredUsers = await this.context.Users.Where(u => u.UserName == queryFilter.ComparisonValue)
                    .Include(u => u.Addresses)
                    .Include(u => u.ElectronicMailAddresses)
                    .Include(u => u.InstantMessagings)
                    .Include(u => u.PhoneNumbers)
                    .AsNoTracking().ToListAsync();
                results = filteredUsers.Select((Core2EnterpriseUser user) => user as Resource).ToArray();

                return await Task.FromResult(results);
            }

            if (queryFilter.AttributePath.Equals(AttributeNames.ExternalIdentifier))
            {
                IEnumerable<Core2EnterpriseUser> filteredUsers = await this.context.Users.Where(u => u.ExternalIdentifier == queryFilter.ComparisonValue)
                    .Include(u => u.Addresses)
                    .Include(u => u.ElectronicMailAddresses)
                    .Include(u => u.InstantMessagings)
                    .Include(u => u.PhoneNumbers)
                    .AsNoTracking().ToListAsync();
                results = filteredUsers.Select((Core2EnterpriseUser user) => user as Resource).ToArray();

                return await Task.FromResult(results);
            }

            throw new NotSupportedException(SqlProviderResources.UnsupportedFilterAttributeUser);
        }

        // override missing
        public override async Task<Resource> ReplaceAsync(Resource resource, string correlationIdentifier)
        {
            if (resource.Identifier == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            Core2EnterpriseUser update = resource as Core2EnterpriseUser;

            if (string.IsNullOrWhiteSpace(update.UserName))
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            if (await this.context.Users.AnyAsync(u => u.UserName == update.UserName && u.Identifier != update.Identifier))
            {
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }

            Core2EnterpriseUser user = await this.context.Users.Where(u => u.Identifier == update.Identifier)
                .Include(u => u.Addresses)
                .Include(u => u.ElectronicMailAddresses)
                .Include(u => u.InstantMessagings)
                .Include(u => u.PhoneNumbers)
                .FirstOrDefaultAsync();
            if (user == null)
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            // cascade updates to related tables
            // Addresses
            cascadeUpdates(update.Addresses, user.Addresses);
            // ElectronicMailAddresses
            cascadeUpdates(update.ElectronicMailAddresses, user.ElectronicMailAddresses);
            // InstantMessagings
            cascadeUpdates(update.InstantMessagings, user.InstantMessagings);
            // PhoneNumbers
            cascadeUpdates(update.PhoneNumbers, user.PhoneNumbers);

            // update object
            this.context.Entry(user).CurrentValues.SetValues(update);

            await this.context.SaveChangesAsync();
            return await Task.FromResult(user as Resource);
        }

        #region helper functions

        void cascadeUpdates<T>(IEnumerable<T> updates, IEnumerable<T> currents)
            where T : TypedItem
        {
            if (updates != null)
            {
                // remove
                string[] currentList = currents.Select(i => i.ItemType).ToArray();
                foreach (var current in currentList)
                {
                    if (!updates.Any(i => i.ItemType == current))
                    {
                        ((HashSet<T>)currents).Remove(currents.First(i => i.ItemType == current));
                    }
                }
                //Add or update
                foreach (var update in updates)
                {
                    var existing = currents.FirstOrDefault(i => i.ItemType == update.ItemType);
                    if (existing != null)
                    {
                        foreach (var prop in existing.GetType().GetProperties())
                        {
                            prop.SetValue(existing, prop.GetValue(update));
                        }
                    }
                    else
                    {
                        ((HashSet<T>)currents).Add(update);
                    }
                }
            }
            else
            {
                ((HashSet<T>)currents).Clear();
            }
        }

        #endregion

    }
}
