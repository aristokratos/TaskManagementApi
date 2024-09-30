using MongoDB.Bson.Serialization.Attributes;

namespace TaskManagementApi.Model
{
    public class UserModel : BaseEntity
    {
        [BsonId]
        [BsonElement("_Id"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public string Id { get; set; } = string.Empty;
        [BsonElement("Username"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public string? Username { get; set; }
        [BsonElement("Password"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public string? Password { get; set; }
        [BsonElement("Email"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public string? Email { get; set; }
        [BsonElement("TaskIds"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public List<string>? TaskIds { get; set; }
        [BsonElement("ListIds"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public List<string>? ListIds { get; set; }
        [BsonElement("GroupIds"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public List<string>? GroupIds { get; set; }
    }

}
