using MongoDB.Driver;
using TaskManagementApi.Model;

namespace TaskManagementApi.Context
{
    public class DataSeeder
    {
        private readonly IMongoCollection<TaskModel> _tasksCollection;
        private readonly IMongoCollection<ListModel> _listsCollection;

        public DataSeeder(MongoDbService context)
        {
            _tasksCollection = context.Tasks;
            _listsCollection = context.Lists;
        }

        public async Task SeedDataAsync()
        {
            try
            {

                if (!await _tasksCollection.Find(task => true).AnyAsync())
                {
                    var initialTasks = new List<TaskModel>
                {
                    new TaskModel
                    {
                        Title = "Task table",
                        Description = "Meeting with the Jones.",
                        Status = true,
                        Priority = 1,
                        ListId = "example-list-id",
                        GroupId = "example-group-id",
                        StartHour = new TimeSpan(9, 0, 0),
                        EndHour = new TimeSpan(17, 0, 0),
                        AssignedUsers = new List<string> { "user1" },
                        IsActive = true,
                        HasExpired = false
                    }
                };

                    await _tasksCollection.InsertManyAsync(initialTasks);
                }

                if (!await _listsCollection.Find(list => true).AnyAsync())
                {
                    var initialLists = new List<ListModel>
                {
                    new ListModel
                    {
                        Name = "List table",
                        OwnerUserId = "user1",
                        TaskIds = new List<string>()
                    }
                };

                    await _listsCollection.InsertManyAsync(initialLists);
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }

}
