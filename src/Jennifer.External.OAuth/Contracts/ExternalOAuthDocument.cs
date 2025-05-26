using eXtensionSharp.Mongo;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Jennifer.External.OAuth.Contracts;

[JMongoCollection("jennifer", "externalOAuth")]
public class ExternalOAuthDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("id")]
    public String Id { get; set; }

    [BsonElement("result")] public string Result { get; set; }
    
    [BsonElement("createdAt")] public DateTimeOffset CreatedAt { get; set; }
}

public class ExternalOAuthDocumentConfiguration: IJMongoConfiguration
{
    public void Configure(IJMongoFactory factory)
    {
        var builder = factory.Create<ExternalOAuthDocument>();
        var collection = builder.GetCollection();
        collection.Indexes.CreateOne(
            new CreateIndexModel<ExternalOAuthDocument>(
                Builders<ExternalOAuthDocument>.IndexKeys.Ascending(m => m.CreatedAt),
                new CreateIndexOptions { ExpireAfter = TimeSpan.FromDays(30) }
            )
        );
    }
}