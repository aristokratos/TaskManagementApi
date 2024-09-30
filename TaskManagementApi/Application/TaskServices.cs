namespace TaskManagementApi.Application
{
    using MongoDB.Bson;
    using MongoDB.Driver;
    using Newtonsoft.Json;
    using StackExchange.Redis;
    using TaskManagementApi.Application.Interfaces;
    using TaskManagementApi.Context;
    using TaskManagementApi.Model;
    public class TaskServices : ITaskServices
    {
        #region Properties
        private readonly MongoDbService _context;
        private readonly IDatabase _redisDatabase;
        private readonly IMongoCollection<TaskModel> _tasksCollection;
        private readonly ILogger<TaskServices> _logger;
        #endregion Properties

        #region Constructor
        public TaskServices(MongoDbService context, ILogger<TaskServices> logger, IDatabase redisDatabase)
        {
            _context = context;
            _logger = logger;
            _redisDatabase = redisDatabase;
        }
        #endregion Constructor

        #region Methods
        public async Task<TaskModel> CreateTaskAsync(TaskModel task, string listId)
        {
            try
            {
                _logger.LogInformation("Creating a new task with title: {Title}", task.Title);


                task.IsActive = true;
                task.HasExpired = false;

                if (task.EndHour.HasValue && DateTime.UtcNow.TimeOfDay > task.EndHour.Value)
                {
                    task.HasExpired = true;
                    task.IsActive = false;
                }
                task.Id = ObjectId.GenerateNewId().ToString();
                await _context.Tasks.InsertOneAsync(task);

                if (_context == null)
                {
                    throw new InvalidOperationException("Database context is not initialized");
                }

                var filter = Builders<ListModel>.Filter.Eq(l => l.Id, listId);
                var update = Builders<ListModel>.Update.Push(l => l.TaskIds, task.Id);
                var updateResult = await _context.Lists.UpdateOneAsync(filter, update);

                if (updateResult.IsAcknowledged && updateResult.ModifiedCount > 0)
                {
                    _logger.LogInformation("Task successfully added to the list with ID: {ListId}", listId);
                }
                else
                {
                    _logger.LogWarning("Failed to update the list with ID: {ListId}", listId);
                }

                return task;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a task.");
                throw;
            }
        }


        public async Task<TaskModel> GetTaskByIdAsync(string id)
        {
            try
            {
                _logger.LogInformation("Fetching task with ID: {TaskId}", id);
                return await _context.Tasks.Find(task => task.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the task with ID: {TaskId}", id);
                throw;
            }
        }

        public async Task<List<TaskModel>> GetAllTasksAsync()
        {
            const string cacheKey = "all_tasks";

            try
            {
                _logger.LogInformation("Fetching all tasks.");

                var cachedTasks = await _redisDatabase.StringGetAsync(cacheKey);
                if (cachedTasks.HasValue)
                {
                    var tasksList = JsonConvert.DeserializeObject<List<TaskModel>>(cachedTasks);
                    _logger.LogInformation("Retrieved tasks from cache.");
                    return tasksList;
                }

                var tasks = await _context.Tasks.Find(task => true).ToListAsync();

                var serializedTasks = JsonConvert.SerializeObject(tasks);
                await _redisDatabase.StringSetAsync(cacheKey, serializedTasks, TimeSpan.FromMinutes(10));

                _logger.LogInformation("Retrieved tasks from database and cached.");
                return tasks;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all tasks.");
                throw;
            }
        }

        public async Task<bool> UpdateTaskAsync(TaskModel updatedTask)
        {
            try
            {
                _logger.LogInformation("Updating task with ID: {TaskId}.", updatedTask.Id);

                var existingTask = await _context.Tasks.Find(t => t.Id == updatedTask.Id).FirstOrDefaultAsync();

                if (existingTask == null)
                {
                    _logger.LogWarning("Task with ID: {TaskId} not found.", updatedTask.Id);
                    return false;
                }

                if (updatedTask.EndHour.HasValue && DateTime.UtcNow.TimeOfDay > updatedTask.EndHour.Value)
                {
                    updatedTask.HasExpired = true;
                    updatedTask.IsActive = false;
                }
                else
                {
                    updatedTask.HasExpired = existingTask.HasExpired;
                    updatedTask.IsActive = existingTask.IsActive;
                }

                var update = Builders<TaskModel>.Update
                    .Set(t => t.Title, updatedTask.Title)
                    .Set(t => t.Description, updatedTask.Description)
                    .Set(t => t.Priority, updatedTask.Priority)
                    .Set(t => t.ListId, updatedTask.ListId)
                    .Set(t => t.GroupId, updatedTask.GroupId)
                    .Set(t => t.StartHour, updatedTask.StartHour)
                    .Set(t => t.EndHour, updatedTask.EndHour)
                    .Set(t => t.AssignedUsers, updatedTask.AssignedUsers)
                    .Set(t => t.UpdatedAt, DateTime.UtcNow);

                var result = await _context.Tasks.UpdateOneAsync(t => t.Id == updatedTask.Id, update);
                var cacheKey = $"task:{updatedTask.Id}";
                await _redisDatabase.KeyDeleteAsync(cacheKey);
                _logger.LogInformation("Cache invalidated for task ID: {TaskId}", updatedTask.Id);


                if (result.IsAcknowledged && result.ModifiedCount > 0)
                {
                    _logger.LogInformation("Task with ID: {TaskId} updated successfully.", updatedTask.Id);
                    return true;
                }
                else
                {
                    _logger.LogWarning("Failed to update task with ID: {TaskId}.", updatedTask.Id);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating task with ID: {TaskId}.", updatedTask.Id);
                return false;
            }
        }



        public async Task<bool> DeleteTaskAsync(string id)
        {
            try
            {
                _logger.LogInformation("Deleting task with ID: {TaskId}", id);

                var result = await _context.Tasks.DeleteOneAsync(task => task.Id == id);

                if (result.IsAcknowledged && result.DeletedCount > 0)
                {
                    _logger.LogInformation("Successfully deleted task with ID: {TaskId}", id);
                    return true;
                }
                else
                {
                    _logger.LogWarning("Failed to delete task with ID: {TaskId}", id);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the task with ID: {TaskId}", id);
                throw;
            }
        }

        public async Task<List<TaskModel>> GetTasksByListIdAsync(string listId)
        {
            try
            {
                _logger.LogInformation("Fetching tasks for list with ID: {ListId}", listId);
                return await _context.Tasks.Find(task => task.ListId == listId).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching tasks for list with ID: {ListId}", listId);
                throw;
            }
        }
        #endregion Methods



    }
}
