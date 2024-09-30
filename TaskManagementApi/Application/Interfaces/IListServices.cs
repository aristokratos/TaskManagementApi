using TaskManagementApi.Model;

namespace TaskManagementApi.Application.Interfaces
{
    public interface IListServices
    {
        Task<ListModel> CreateListAsync(ListModel list);

        // Get a list by ID
        Task<ListModel> GetListByIdAsync(string id);

        // Get all lists
        Task<List<ListModel>> GetAllListsAsync();

        // Update an existing list
        Task<bool> UpdateListAsync(ListModel updatedList);

        // Delete a list by ID
        Task<bool> DeleteListAsync(string id);
    }
}
