using MongoDB.Driver;
using TaskManagementApi.Model;

namespace TaskManagementApi.Context
{
    public class DataMigrationService
    {
        private readonly IMongoCollection<TaskModel> _tasksCollection;

        public DataMigrationService(MongoDbService context)
        {
            _tasksCollection = context.Tasks;
        }

        public async Task MigrateDataAsync()
        {
            // Example: Adding a new field to existing documents
            var filter = Builders<TaskModel>.Filter.Exists(t => t.Title == null);
            var update = Builders<TaskModel>.Update.Set(t => t.Title, "default value");
            var result = await _tasksCollection.UpdateManyAsync(filter, update);

            // Log the result or handle it as needed
        }
    }

}

