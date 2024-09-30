using MongoDB.Driver;
using TaskManagementApi.Model;

namespace TaskManagementApi.Context
{
    public class MongoDbService
    {
            private readonly IConfiguration _configuration;
            private readonly IMongoDatabase _database;

            public MongoDbService(IConfiguration configuration)
            {
            var connectionString = configuration.GetConnectionString("MongoDb");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString), "MongoDB connection string is missing.");
            }

            var mongoUrl = new MongoUrl(connectionString);
            var mongoClient = new MongoClient(mongoUrl);

            var databaseName = configuration["MongoDbDatabaseName"];
            if (string.IsNullOrEmpty(databaseName))
            {
                throw new ArgumentNullException(nameof(databaseName), "MongoDB database name is missing.");
            }

            _database = mongoClient.GetDatabase(databaseName);
        }

            public IMongoCollection<TaskModel> Tasks => _database.GetCollection<TaskModel>("Tasks");
            public IMongoCollection<UserModel> Users => _database.GetCollection<UserModel>("Users");
            public IMongoCollection<ListModel> Lists => _database.GetCollection<ListModel>("Lists");
            public IMongoCollection<GroupModel> Groups => _database.GetCollection<GroupModel>("Groups");
    }
}