using MongoDB.Bson.Serialization.Attributes;

namespace TaskManagementApi.Model
{
    public class BaseEntity
    {
        [BsonElement("CreatedAt"), BsonRepresentation(MongoDB.Bson.BsonType.DateTime)]
        public DateTime CreatedAt { get; set; }
        [BsonElement("UpdatedAt"), BsonRepresentation(MongoDB.Bson.BsonType.DateTime)]
        public DateTime UpdatedAt { get; set; }
    }
}
