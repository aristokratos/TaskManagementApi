namespace TaskManagementApi.Application
{
    using MongoDB.Bson;
    using MongoDB.Driver;
    using StackExchange.Redis;
    using Newtonsoft.Json;
    using TaskManagementApi.Application.Interfaces;
    using TaskManagementApi.Context;
    using TaskManagementApi.Model;
    using System.Text.RegularExpressions;

    public class GroupServices : IGroupServices
    {
        #region Properties
        private readonly MongoDbService _context;
        private readonly IDatabase _redisDatabase;
        private readonly ILogger<GroupServices> _logger;
        #endregion Properties

        #region Constructor
        public GroupServices(MongoDbService context, ILogger<GroupServices> logger, IConnectionMultiplexer redis)
        {
            _context = context;
            _logger = logger;
            _redisDatabase = redis.GetDatabase();
        }
        #endregion Constructor

        #region Methods
        public async Task<GroupModel> CreateGroupAsync(GroupModel group)
        {
            try
            {
                _logger.LogInformation("Creating a new group with name: {Name}", group.Name);

                // Ensure that the group has a unique ID
                group.Id = ObjectId.GenerateNewId().ToString();
                group.Name = group.Name;
                group.ListIds = group.ListIds;
                group.TaskIds = group.TaskIds;
                group.UserIds = group.UserIds;

                group.OwnerUserId = group.OwnerUserId;
                group.UpdatedAt = DateTime.UtcNow;

                await _context.Groups.InsertOneAsync(group);
                _logger.LogInformation("group created successfully with ID: {Id}", group.Id);

                return group;
            }
            catch (MongoWriteException ex) when (ex.WriteError?.Category == ServerErrorCategory.DuplicateKey)
            {
                _logger.LogError("Duplicate key error: A group with the same ID already exists.");
                throw new InvalidOperationException("A group with the same ID already exists.", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the group.");
                throw;
            }
        }

        public async Task<GroupModel> GetGroupByIdAsync(string id)
        {
            try
            {
                _logger.LogInformation("Fetching group with ID: {groupId}", id);
                return await _context.Groups.Find(group => group.Id.ToString() == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the group with ID: {groupId}", id);
                throw;
            }
        }
        
        public async Task<List<GroupModel>> GetAllGroupsAsync()
        {
            const string cacheKey = "all_groups";

            try
            {
                _logger.LogInformation("Fetching all groups.");

                // Check if the data is in the cache
                var cachedGroups = await _redisDatabase.StringGetAsync(cacheKey);
                if (cachedGroups.HasValue)
                {
                    // Deserialize from JSON and return
                    var groupsList = JsonConvert.DeserializeObject<List<GroupModel>>(cachedGroups);
                    _logger.LogInformation("Retrieved groups from cache.");
                    return groupsList;
                }

                // If not cached, fetch from database
                var groups = await _context.Groups.Find(group => true).ToListAsync();

                // Cache the data
                var serializedGroups = JsonConvert.SerializeObject(groups);
                await _redisDatabase.StringSetAsync(cacheKey, serializedGroups, TimeSpan.FromMinutes(30)); 

                _logger.LogInformation("Retrieved groups from database and cached.");
                return groups;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all groups.");
                throw;
            }
        }

        public async Task<bool> UpdateGroupAsync(GroupModel updatedgroup)
        {
            try
            {
                _logger.LogInformation("Updating group with ID: {groupId}", updatedgroup.Id);

                var existinggroup = await _context.Groups.Find(l => l.Id == updatedgroup.Id).FirstOrDefaultAsync();

                if (existinggroup == null)
                {
                    _logger.LogWarning("group with ID: {groupId} not found.", updatedgroup.Id);
                    return false;
                }

                var update = Builders<GroupModel>.Update
                    .Set(l => l.Name, updatedgroup.Name)
                    .Set(l => l.TaskIds, updatedgroup.TaskIds)
                    .Set(l => l.OwnerUserId, updatedgroup.OwnerUserId)
                    .Set(l => l.UpdatedAt, DateTime.UtcNow);

                var result = await _context.Groups.UpdateOneAsync(l => l.Id == updatedgroup.Id, update);
                var cacheKey = $"task:{updatedgroup.Id}";
                await _redisDatabase.KeyDeleteAsync(cacheKey);
                _logger.LogInformation("Cache invalidated for task ID: {TaskId}", updatedgroup.Id);

                if (result.IsAcknowledged && result.ModifiedCount > 0)
                {
                    _logger.LogInformation("group with ID: {groupId} updated successfully.", updatedgroup.Id);
                    return true;
                }
                else
                {
                    _logger.LogWarning("Failed to update group with ID: {groupId}.", updatedgroup.Id);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating group with ID: {groupId}.", updatedgroup.Id);
                return false;
            }
        }

        public async Task<bool> DeleteGroupAsync(string id)
        {
            try
            {
                _logger.LogInformation("Deleting group with ID: {groupId}", id);

                var result = await _context.Groups.DeleteOneAsync(group => group.Id.ToString() == id);

                if (result.IsAcknowledged && result.DeletedCount > 0)
                {
                    _logger.LogInformation("Successfully deleted group with ID: {groupId}", id);
                    return true;
                }
                else
                {
                    _logger.LogWarning("Failed to delete group with ID: {groupId}", id);
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the group with ID: {groupId}", id);
                throw;
            }
        }
        #endregion Methods
    }
}
