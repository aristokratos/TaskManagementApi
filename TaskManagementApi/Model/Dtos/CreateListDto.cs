using MongoDB.Bson.Serialization.Attributes;

namespace TaskManagementApi.Model.Dtos
{
    public class CreateListDto : BaseEntity
    {
        [BsonElement("Name"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public string? Name { get; set; }
        [BsonElement("TaskIds"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public List<string>? TaskIds { get; set; }
        [BsonElement("GroupId"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public string? OwnerUserId { get; set; } = null;
    }
}
