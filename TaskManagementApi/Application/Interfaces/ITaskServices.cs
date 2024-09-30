using TaskManagementApi.Model;

namespace TaskManagementApi.Application.Interfaces
{
    public interface ITaskServices
    {
        Task<TaskModel> CreateTaskAsync(TaskModel task, string listId);
        Task<TaskModel> GetTaskByIdAsync(string id);
        Task<List<TaskModel>> GetAllTasksAsync();
        Task<bool> UpdateTaskAsync(TaskModel updatedTask);
        Task<bool> DeleteTaskAsync(string id);
        Task<List<TaskModel>> GetTasksByListIdAsync(string listId);   
    }
}
