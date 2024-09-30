using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using TaskManagementApi.Application.Interfaces;
using TaskManagementApi.Model;

namespace TaskManagementApi.Controllers
{
    public class GroupController : Controller
    {
        #region Properties
        private readonly IGroupServices _groupServices;
        private readonly ILogger<GroupController> _logger;
        #endregion Properties

        #region Constructor
        public GroupController(IGroupServices groupServices, ILogger<GroupController> logger)
        {
            _groupServices = groupServices;
            _logger = logger;
        }
        #endregion Constructor

        #region Methods

        [HttpPost("create-group")]
        public async Task<IActionResult> CreateGroupAsync([FromBody] GroupModel Group)
        {
            if (Group == null)
            {
                _logger.LogWarning("Received a null group object.");
                return BadRequest("Group data is required.");
            }

            try
            {
                if (string.IsNullOrEmpty(Group.Id))
                {
                    Group.Id = ObjectId.GenerateNewId().ToString();
                }

                var result = await _groupServices.CreateGroupAsync(Group);

                return CreatedAtAction(nameof(GetGroupById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating the Group.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("get-group-id:{id}")]
        public async Task<IActionResult> GetGroupById(string id)
        {
            try
            {
                var Group = await _groupServices.GetGroupByIdAsync(id);
                if (Group == null)
                {
                    _logger.LogWarning("Group with ID: {GroupId} not found.", id);
                    return NotFound();
                }
                return Ok(Group);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the Group with ID: {GroupId}", id);
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("get-all-group")]
        public async Task<IActionResult> GetAllGroupsAsync()
        {
            try
            {
                var Groups = await _groupServices.GetAllGroupsAsync();
                return Ok(Groups);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching all Groups.");
                return StatusCode(500, "Internal server error.");
            }
        }


        [HttpPut("update- group:{id}")]
        public async Task<IActionResult> UpdateGroupAsync(string id, [FromBody] GroupModel updatedGroup)
        {
            if (updatedGroup == null)
            {
                _logger.LogWarning("Received a null Group object.");
                return BadRequest("Group data is required.");
            }

            if (id != updatedGroup.Id.ToString())
            {
                _logger.LogWarning("ID mismatch: URL ID {Id} does not match body ID {BodyId}.", id, updatedGroup.Id);
                return BadRequest("ID mismatch.");
            }

            try
            {
                var success = await _groupServices.UpdateGroupAsync(updatedGroup);
                if (!success)
                {
                    _logger.LogWarning("Failed to update Group with ID: {GroupId}.", id);
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the Group with ID: {GroupId}", id);
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpDelete("delete-group:{id}")]
        public async Task<IActionResult> DeleteGroupAsync(string id)
        {
            try
            {
                var success = await _groupServices.DeleteGroupAsync(id);
                if (!success)
                {
                    _logger.LogWarning("Failed to delete Group with ID: {GroupId}.", id);
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the Group with ID: {GroupId}", id);
                return StatusCode(500, "Internal server error.");
            }
        }

        #endregion Methods
    }
}
