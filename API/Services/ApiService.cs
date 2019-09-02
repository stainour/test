using CoreDomain;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    public class ApiService
    {
        private readonly IUriMappingRepository _repository;
        private readonly ISequenceGenerator _sequenceGenerator;

        public ApiService(ISequenceGenerator sequenceGenerator, IUriMappingRepository mappingRepository)
        {
            _sequenceGenerator = sequenceGenerator ?? throw new ArgumentNullException(nameof(sequenceGenerator));
            _repository = mappingRepository ?? throw new ArgumentNullException(nameof(mappingRepository));
        }

        public async Task<(bool createdWithoutConflict, UriMapping mapping)> CreateAsync(Uri uri)
        {
            if (uri == null) throw new ArgumentNullException(nameof(uri));

            if (await _repository.FindByIdAsync(uri) != null)
            {
                return (false, null);
            }

            var nextValue = _sequenceGenerator.NextValue();
            var mapping = new UriMapping(uri, nextValue);

            var result = await _repository.AddIfNotExistsAsync(mapping);

            return result == AddResult.OK ? (true, mapping) : (false, null);
        }

        public  Task<IEnumerable<UriMapping>> GetAllMappingsAsync()
        {
            return _repository.AllAsync();
        }

        public async Task<UriMapping> ResolveByKey(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            var uriMapping = await _repository.FindByKeyAsync(key);
            if (uriMapping == null)
            {
                return null;
            }

            _repository.IncrementHitCountAsync(new Uri(uriMapping.Uri));
            return uriMapping;
        }
    }
}