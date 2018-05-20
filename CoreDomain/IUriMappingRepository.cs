using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoreDomain
{
    public interface IUriMappingRepository
    {
        Task<AddResult> AddIfNotExistsAsync(UriMapping uriMapping);

        Task<IEnumerable<UriMapping>> AllAsync();

        Task<UriMapping> FindByIdAsync(Uri id);

        Task<UriMapping> FindByKeyAsync(string key);

        Task IncrementHitCountAsync(Uri id);
    }
}