namespace TaskManagementApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using TaskManagementApi.Application.Interfaces;
    using TaskManagementApi.Model;

    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        #region Properties
        private readonly ITaskServices _taskServices;
        #endregion Properties

        #region Constructor
        public TaskController(ITaskServices taskServices)
        {
            _taskServices = taskServices;
        }
        #endregion Constructor

        #region Methods

        [HttpPost("create-task")]
        public async Task<IActionResult> CreateTask([FromBody] TaskModel task, [FromQuery] string listId)
        {
            if (task == null || string.IsNullOrWhiteSpace(listId))
            {
                return BadRequest("Invalid task data or list ID.");
            }

            try
            {
                var createdTask = await _taskServices.CreateTaskAsync(task, listId);
                return CreatedAtAction(nameof(GetTaskById), new { id = createdTask.Id }, createdTask);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("get-task-by-{id}")]
        public async Task<IActionResult> GetTaskById(string id)
        {
            try
            {
                var  task = await _taskServices.GetTaskByIdAsync(id);
                if (task == null)
                {
                    return NotFound();
                }

                return Ok(task);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("get-all-tasks")]
        public async Task<IActionResult> GetAllTasks()
        {
            try
            {
                var tasks = await _taskServices.GetAllTasksAsync();
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("update-task-{id}")]
        public async Task<IActionResult> UpdateTask(string id, [FromBody] TaskModel updatedTask)
        {
            if (updatedTask == null || id != updatedTask.Id)
            {
                return BadRequest("Task ID mismatch or invalid task data.");
            }

            try
            {
                var success = await _taskServices.UpdateTaskAsync(updatedTask);
                if (!success)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("delete-task-{id}")]
        public async Task<IActionResult> DeleteTask(string id)
        {
            try
            {
                var success = await _taskServices.DeleteTaskAsync(id);
                if (!success)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("lget-task-by-listid-{listId}")]
        public async Task<IActionResult> GetTasksByListId(string listId)
        {
            try
            {
                var tasks = await _taskServices.GetTasksByListIdAsync(listId);
                if (tasks == null || tasks.Count == 0)
                {
                    return NotFound();
                }

                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        #endregion Methods
    }
}
