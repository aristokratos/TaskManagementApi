using MongoDB.Bson.Serialization.Attributes;

namespace TaskManagementApi.Model.Dtos
{
    public class LoginDto
    {
        [BsonElement("Username"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public string Username { get; set; }
        [BsonElement("Password"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public string Password { get; set; }
    }
}
