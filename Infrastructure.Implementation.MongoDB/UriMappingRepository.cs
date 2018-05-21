using CoreDomain;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Implementation.MongoDB
{
    public class UriMappingRepository : IUriMappingRepository
    {
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<UriMapping> _mongoCollection;

        static UriMappingRepository()
        {
            BsonClassMap.RegisterClassMap<UriMapping>(map =>
            {
                map.AutoMap();
                map.MapIdProperty(mapping => mapping.Uri);
            });
        }

        public UriMappingRepository(IMongoDatabase database)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _mongoCollection = _database.GetCollection<UriMapping>("UriMapping");
            _mongoCollection.Indexes.CreateOne(Builders<UriMapping>.IndexKeys.Ascending(mapping => mapping.ShortenedKey), new CreateIndexOptions { Background = true, Unique = true });
        }

        public async Task<AddResult> AddIfNotExistsAsync(UriMapping uriMapping)
        {
            if (uriMapping == null) throw new ArgumentNullException(nameof(uriMapping));

            var result = await _mongoCollection.UpdateOneAsync(
                mapping => mapping.Uri == uriMapping.Uri,
                new UpdateDefinitionBuilder<UriMapping>().SetOnInsert(mapping => mapping.ShortenedKey, uriMapping.ShortenedKey).SetOnInsert(mapping => mapping.HitCount, uriMapping.HitCount),
                new UpdateOptions { IsUpsert = true });

            return result.MatchedCount == 1 ? AddResult.AlreadyExists : AddResult.OK;
        }

        public Task<IEnumerable<UriMapping>> AllAsync() => _mongoCollection.FindAsync(FilterDefinition<UriMapping>.Empty).ContinueWith(task => task.Result.ToEnumerable());

        public Task<UriMapping> FindByIdAsync(Uri id)
        {
            var stringId = GetStringId(id);
            return _mongoCollection.AsQueryable().FirstOrDefaultAsync(mapping => mapping.Uri == stringId);
        }

        public Task<UriMapping> FindByKeyAsync(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return _mongoCollection.AsQueryable().SingleOrDefaultAsync(mapping => mapping.ShortenedKey == key);
        }

        public Task IncrementHitCountAsync(Uri id)
        {
            var stringId = GetStringId(id);
            return _mongoCollection.UpdateOneAsync(mapping => mapping.Uri == stringId, new UpdateDefinitionBuilder<UriMapping>().Inc(mapping => mapping.HitCount, 1));
        }

        private static string GetStringId(Uri id)
        {
            if (id == null) throw new ArgumentNullException(nameof(id));
            return id.ToString();
        }
    }
}