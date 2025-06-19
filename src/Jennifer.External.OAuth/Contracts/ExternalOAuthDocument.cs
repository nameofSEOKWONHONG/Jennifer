using eXtensionSharp.Mongo;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Jennifer.External.OAuth.Contracts;

public class ExternalOAuthDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("id")]
    public String Id { get; set; }

    [BsonElement("result")] public string Result { get; set; }
    
    [BsonElement("createdAt")] public DateTimeOffset CreatedAt { get; set; }
}

public class ExternalOAuthDocumentConfiguration: IJMongoConfiguration<ExternalOAuthDocument>
{
    public void Configure(JMongoCollectionBuilder<ExternalOAuthDocument> builder)
    {
        builder.ToDocument("jennifer", "external_oauth");
        builder.ToIndex(indexes =>
            indexes.CreateOne(
                new CreateIndexModel<ExternalOAuthDocument>(
                    Builders<ExternalOAuthDocument>.IndexKeys.Ascending(m => m.CreatedAt),
                    new CreateIndexOptions { ExpireAfter = TimeSpan.FromDays(30) }
                ))
        );
    }
}