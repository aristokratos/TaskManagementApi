namespace TaskManagementApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using TaskManagementApi.Application.Interfaces;
    using TaskManagementApi.Model;
    using MongoDB.Bson;
    [ApiController]
    [Route("api/[controller]")]
    public class ListController : ControllerBase
    {
        #region Properties
        private readonly IListServices _listServices;
        private readonly ILogger<ListController> _logger;
        #endregion Properties

        #region Constructor
        public ListController(IListServices listServices, ILogger<ListController> logger)
        {
            _listServices = listServices;
            _logger = logger;
        }
        #endregion Constructor

        #region Methods
        [HttpPost("create-list")]
        public async Task<IActionResult> CreateListAsync([FromBody] ListModel list)
        {
            if (list == null)
            {
                _logger.LogWarning("Received a null list object.");
                return BadRequest("List data is required.");
            }

            try
            {
                if (string.IsNullOrEmpty(list.Id))
                {
                    list.Id = ObjectId.GenerateNewId().ToString();
                }

                var result = await _listServices.CreateListAsync(list);

                return CreatedAtAction(nameof(GetListById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the list.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("get-list-by:{id}")]
        public async Task<IActionResult> GetListById(string id)
        {
            try
            {
                var list = await _listServices.GetListByIdAsync(id);
                if (list == null)
                {
                    _logger.LogWarning("List with ID: {ListId} not found.", id);
                    return NotFound();
                }
                return Ok(list);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the list with ID: {ListId}", id);
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("get-all-lists")]
        public async Task<IActionResult> GetAllListsAsync()
        {
            try
            {
                var lists = await _listServices.GetAllListsAsync();
                return Ok(lists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all lists.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPut("update-list-by-id:{id}")]
        public async Task<IActionResult> UpdateListAsync(string id, [FromBody] ListModel updatedList)
        {
            if (updatedList == null)
            {
                _logger.LogWarning("Received a null list object.");
                return BadRequest("List data is required.");
            }

            if (id != updatedList.Id.ToString())
            {
                _logger.LogWarning("ID mismatch: URL ID {Id} does not match body ID {BodyId}.", id, updatedList.Id);
                return BadRequest("ID mismatch.");
            }

            try
            {
                var success = await _listServices.UpdateListAsync(updatedList);
                if (!success)
                {
                    _logger.LogWarning("Failed to update list with ID: {ListId}.", id);
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the list with ID: {ListId}", id);
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpDelete("delete-list:{id}")]
        public async Task<IActionResult> DeleteListAsync(string id)
        {
            try
            {
                var success = await _listServices.DeleteListAsync(id);
                if (!success)
                {
                    _logger.LogWarning("Failed to delete list with ID: {ListId}.", id);
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the list with ID: {ListId}", id);
                return StatusCode(500, "Internal server error.");
            }
        }
        #endregion Methods
    }
}
