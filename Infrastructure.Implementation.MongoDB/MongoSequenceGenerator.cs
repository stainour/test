using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Infrastructure.Implementation.MongoDB
{
    public class MongoSequenceGenerator : ISequenceGenerator
    {
        private static readonly ObjectId SequenceDocId = new ObjectId("5aff52eb72d9993560b2d3cc");
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly IMongoCollection<Sequence> _mongoCollection;

        public MongoSequenceGenerator(IMongoDatabase database)
        {
            _database = database ?? throw new System.ArgumentNullException(nameof(database));
            _client = database.Client;
            _mongoCollection = _database.GetCollection<Sequence>(nameof(Sequence), new MongoCollectionSettings
            {
                WriteConcern = new WriteConcern(journal: true, w: 1)
            });

            _mongoCollection.UpdateOne(sequence => sequence.Id == SequenceDocId, new UpdateDefinitionBuilder<Sequence>().SetOnInsert(sequence => sequence.Value, 0), new UpdateOptions { IsUpsert = true });
        }

        public long NextValue()
        {
            return _mongoCollection.FindOneAndUpdate<Sequence>(
                sequence => sequence.Id == SequenceDocId,
                new UpdateDefinitionBuilder<Sequence>().Inc(sequence => sequence.Value, 1),
                new FindOneAndUpdateOptions<Sequence>
                {
                    ReturnDocument = ReturnDocument.After
                }).Value;
        }

        private class Sequence
        {
            [BsonId]
            public ObjectId Id { get; private set; }

            public long Value { get; private set; }
        }
    }
}