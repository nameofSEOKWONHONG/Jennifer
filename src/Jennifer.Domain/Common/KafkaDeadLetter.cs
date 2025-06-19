using eXtensionSharp.Mongo;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Jennifer.Domain.Common;

public class KafkaDeadLetter
{
    public long Id { get; set; }

    public string Topic { get; set; }

    public int Partition { get; set; }

    public long Offset { get; set; }

    public string Key { get; set; }

    public string Value { get; set; }

    public string ErrorMessage { get; set; }

    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public class KafkaDeadLetterConfiguration: IEntityTypeConfiguration<KafkaDeadLetter>
{
    public void Configure(EntityTypeBuilder<KafkaDeadLetter> builder)
    {
        builder.ToTable("KafkaDeadLetters", "audits");

        builder.HasKey(e => e.Id);

        builder.Property(e => e.Id)
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Topic)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(e => e.Partition);

        builder.Property(e => e.Offset);

        builder.Property(e => e.Key);

        builder.Property(e => e.Value);

        builder.Property(e => e.ErrorMessage);

        builder.Property(e => e.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")
            .ValueGeneratedOnAdd();
    }
}

public class DeadLetterDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("id")]
    public String Id { get; set; }

    [BsonElement("topic")] public string Topic { get; set; }

    [BsonElement("partition")] public int Partition { get; set; }

    [BsonElement("offset")] public long Offset { get; set; }

    [BsonElement("key")] public string Key { get; set; }

    [BsonElement("value")] public string Value { get; set; }

    [BsonElement("errorMessage")] public string ErrorMessage { get; set; }

    [BsonElement("createAt")] public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

public class DeadLetterDocumentConfiguration: IJMongoConfiguration<DeadLetterDocument>
{
    public void Configure(JMongoCollectionBuilder<DeadLetterDocument> builder)
    {
        builder.ToDocument("jennifer", "deadletters");
    }
}