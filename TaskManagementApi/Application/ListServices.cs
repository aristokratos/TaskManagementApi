namespace TaskManagementApi.Application
{
    using MongoDB.Driver;
    using Newtonsoft.Json;
    using StackExchange.Redis;
    using TaskManagementApi.Application.Interfaces;
    using TaskManagementApi.Context;
    using TaskManagementApi.Model;
    using Microsoft.Extensions.Logging;
    using MongoDB.Bson;

    public class ListServices : IListServices
    {
        #region Properties
        private readonly MongoDbService _context;
        private readonly IDatabase _redisDatabase;
        private readonly ILogger<ListServices> _logger;
        #endregion Properties

        #region Constructor

        public ListServices(MongoDbService context, ILogger<ListServices> logger, IDatabase redisDatabase)
        {
            _context = context;
            _logger = logger;
            _redisDatabase = redisDatabase;
        }
        #endregion Constructor

        #region Methods
        public async Task<ListModel> CreateListAsync(ListModel list)
        {
            try
            {
                _logger.LogInformation("Creating a new list with name: {Name}", list.Name);

                list.Id = ObjectId.GenerateNewId().ToString();
                list.Name = list.Name;
                list.TaskIds = list.TaskIds;
                list.OwnerUserId = list.OwnerUserId;
                list.UpdatedAt = DateTime.UtcNow;

                await _context.Lists.InsertOneAsync(list);
                _logger.LogInformation("List created successfully with ID: {Id}", list.Id);

                return list;
            }
            catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
            {
                _logger.LogError("Duplicate key error: A list with the same ID already exists.");
                throw new InvalidOperationException("A list with the same ID already exists.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the list.");
                throw;
            }
        }
        public async Task<ListModel> GetListByIdAsync(string id)
        {
            try
            {
                _logger.LogInformation("Fetching list with ID: {ListId}", id);
                return await _context.Lists.Find(list => list.Id.ToString() == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the list with ID: {ListId}", id);
                throw;
            }
        }
        public async Task<List<ListModel>> GetAllListsAsync()
        {
            const string cacheKey = "all_lists";

            try
            {
                _logger.LogInformation("Fetching all lists.");

                var cachedLists = await _redisDatabase.StringGetAsync(cacheKey);
                if (cachedLists.HasValue)
                {
                    var lists = JsonConvert.DeserializeObject<List<ListModel>>(cachedLists);
                    _logger.LogInformation("Retrieved lists from cache.");
                    return lists;
                }

                var listsFromDb = await _context.Lists.Find(list => true).ToListAsync();

                var serializedLists = JsonConvert.SerializeObject(listsFromDb);
                await _redisDatabase.StringSetAsync(cacheKey, serializedLists, TimeSpan.FromMinutes(10));

                _logger.LogInformation("Retrieved lists from database and cached.");
                return listsFromDb;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all lists.");
                throw;
            }
        }
        public async Task<bool> UpdateListAsync(ListModel updatedList)
        {
            try
            {
                _logger.LogInformation("Updating list with ID: {ListId}", updatedList.Id);

                var existingList = await _context.Lists.Find(l => l.Id == updatedList.Id).FirstOrDefaultAsync();

                if (existingList == null)
                {
                    _logger.LogWarning("List with ID: {ListId} not found.", updatedList.Id);
                    return false;
                }

                var update = Builders<ListModel>.Update
                    .Set(l => l.Name, updatedList.Name)
                    .Set(l => l.TaskIds, updatedList.TaskIds)
                    .Set(l => l.OwnerUserId, updatedList.OwnerUserId)
                    .Set(l => l.UpdatedAt, DateTime.UtcNow);

                var result = await _context.Lists.UpdateOneAsync(l => l.Id == updatedList.Id, update);
                var cacheKey = $"task:{updatedList.Id}";
                await _redisDatabase.KeyDeleteAsync(cacheKey);
                _logger.LogInformation("Cache invalidated for task ID: {TaskId}", updatedList.Id);

                if (result.IsAcknowledged && result.ModifiedCount > 0)
                {
                    _logger.LogInformation("List with ID: {ListId} updated successfully.", updatedList.Id);
                    return true;
                }
                else
                {
                    _logger.LogWarning("Failed to update list with ID: {ListId}.", updatedList.Id);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating list with ID: {ListId}.", updatedList.Id);
                return false;
            }
        }
        public async Task<bool> DeleteListAsync(string id)
        {
            try
            {
                _logger.LogInformation("Deleting list with ID: {ListId}", id);

                var result = await _context.Lists.DeleteOneAsync(list => list.Id.ToString() == id);

                if (result.IsAcknowledged && result.DeletedCount > 0)
                {
                    _logger.LogInformation("Successfully deleted list with ID: {ListId}", id);
                    return true;
                }
                else
                {
                    _logger.LogWarning("Failed to delete list with ID: {ListId}", id);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the list with ID: {ListId}", id);
                throw;
            }
        }
        #endregion Methods

        
    }
}
