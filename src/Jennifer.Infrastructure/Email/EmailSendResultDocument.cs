using eXtensionSharp.Mongo;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

namespace Jennifer.Infrastructure.Email;

[JMongoCollection("jennifer", "emailSendResult")]
public class EmailSendResultDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("id")]
    public String Id { get; set; }

    [BsonElement("toEmail")] public string ToEmail { get; set; }
    [BsonElement("src")] public string Src { get; set; }
    [BsonElement("result")] public string Result { get; set; }
    
    [BsonElement("createdAt")] public DateTimeOffset CreatedAt { get; set; }
}

public class EmailSendResultDocumentConfiguration: IJMongoConfiguration
{
    public void Configure(IJMongoFactory factory)
    {
        var builder = factory.Create<EmailSendResultDocument>();
        var collection = builder.GetCollection();
        collection.Indexes.CreateOne(
            new CreateIndexModel<EmailSendResultDocument>(
                Builders<EmailSendResultDocument>.IndexKeys.Ascending(m => m.ToEmail),
                new CreateIndexOptions { ExpireAfter = TimeSpan.FromDays(30) }
            )
        );
        collection.Indexes.CreateOne(
            new CreateIndexModel<EmailSendResultDocument>(
                Builders<EmailSendResultDocument>.IndexKeys.Ascending(m => m.CreatedAt),
                new CreateIndexOptions { ExpireAfter = TimeSpan.FromDays(30) }
            )
        );
    }
}