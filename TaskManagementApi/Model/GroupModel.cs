using MongoDB.Bson.Serialization.Attributes;

namespace TaskManagementApi.Model
{
    public class GroupModel : BaseEntity
    {
        [BsonId]
        [BsonElement("_Id"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public string Id { get; set; } = string.Empty;
        [BsonElement("Name"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public string? Name { get; set; }
        [BsonElement("ListIds"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public List<string>? ListIds { get; set; }
        [BsonElement("TaskIds"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public List<string>? TaskIds { get; set; }
        [BsonElement("UserIds"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public List<string>? UserIds { get; set; }
        [BsonElement("OwnerUserId"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public string? OwnerUserId { get; set; }
    }
}
