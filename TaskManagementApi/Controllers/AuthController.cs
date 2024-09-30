namespace TaskManagementApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using TaskManagementApi.Model;
    using TaskManagementApi.Model.Dtos;
    using TaskManagementApi.Application.Interfaces;
    public class AuthController : ControllerBase
    {
        #region Properties
        private readonly IUserAuthServices _authServices;
        #endregion Properties
        #region Constructor
        public AuthController(IUserAuthServices authServices)
        {
            _authServices = authServices;
        }
        #endregion Constructor
        #region Methods

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserModel user)
        {
            var result = await _authServices.RegisterUserAsync(user);
            if (result)
            {
                return Ok("User registered successfully.");
            }

            return BadRequest("User registration failed.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginRequest)
        {
            var token = await _authServices.LoginUserAsync(loginRequest.Username, loginRequest.Password);
            if (token != null)
            {
                return Ok(new { Token = token });
            }

            return Unauthorized("Invalid username or password.");
        }
        #endregion Methods
    }
}
