namespace TaskManagementApi.Model
{
    using MongoDB.Bson.Serialization.Attributes;

    public class TaskModel : BaseEntity
    {
        [BsonId]
        [BsonElement("_Id"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public string Id { get; set; } = string.Empty;
        [BsonElement("Title"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public string? Title { get; set; }
        [BsonElement("Description"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public string? Description { get; set; }
        [BsonElement("Status"), BsonRepresentation(MongoDB.Bson.BsonType.Boolean)]
        public bool Status { get; set; } = true;
        [BsonElement("Priority"), BsonRepresentation(MongoDB.Bson.BsonType.Int32)]
        public int? Priority { get; set; }
        [BsonElement("ListId"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public string? ListId { get; set; }
        [BsonElement("GroupId"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public string? GroupId { get; set; }
        [BsonElement("StartHour"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public TimeSpan? StartHour { get; set; }
        [BsonElement("EndHour"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public TimeSpan? EndHour { get; set; }
        [BsonElement("AssignedUsers"), BsonRepresentation(MongoDB.Bson.BsonType.String)]
        public List<string>? AssignedUsers { get; set; }
        [BsonElement("IsActive"), BsonRepresentation(MongoDB.Bson.BsonType.Boolean)]
        public bool IsActive { get; set; }
        [BsonElement("HasExpired"), BsonRepresentation(MongoDB.Bson.BsonType.Boolean)]
        public bool HasExpired { get; set; }
    }

}
