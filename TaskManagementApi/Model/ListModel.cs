using MongoDB.Bson.Serialization.Attributes;

namespace TaskManagementApi.Model
{
    public class ListModel : BaseEntity
    {
        [BsonId]
        [BsonElement("_Id"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public string Id { get; set; } = string.Empty;
        [BsonElement("Name"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public string? Name { get; set; }
        [BsonElement("TaskIds"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public List<string>? TaskIds { get; set; }
        [BsonElement("GroupId"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public string? OwnerUserId { get; set; }= null;
    }
}
