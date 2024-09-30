namespace TaskManagementApi.Application.Interfaces
{
    using TaskManagementApi.Model;
    public interface IGroupServices
    {
        Task<GroupModel> CreateGroupAsync(GroupModel group);
        Task<GroupModel> GetGroupByIdAsync(string id);
        Task<List<GroupModel>> GetAllGroupsAsync();
        Task<bool> UpdateGroupAsync(GroupModel updatedgroup);
        Task<bool> DeleteGroupAsync(string id);
    }
}
