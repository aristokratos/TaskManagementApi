namespace TaskManagementApi.Application
{
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Microsoft.IdentityModel.Tokens;
    using MongoDB.Driver;
    using StackExchange.Redis;
    using TaskManagementApi.Context;
    using TaskManagementApi.Model;
    using TaskManagementApi.Application.Interfaces;
    using Microsoft.Extensions.Options;
    using TaskManagementApi.Model.Dtos;

    public class UserAuthServices : IUserAuthServices
    {
        #region Properties
        private readonly MongoDbService _context;
        private readonly IDatabase _redisDatabase;
        private readonly ILogger<UserAuthServices> _logger;
        private readonly string _jwtSecret;
        private readonly IConfiguration _configuration;
        #endregion Properties

        #region Constructor
        public UserAuthServices(MongoDbService context, IDatabase redisDatabase, ILogger<UserAuthServices> logger, IOptions<JwtSettings> jwtSettings, IConfiguration configuration)
        {
            _context = context;
            _redisDatabase = redisDatabase;
            _logger = logger;
            _jwtSecret = jwtSettings.Value.Secret;
            _configuration = configuration;
        }
        #endregion Constructor
        #region Methods
        public async Task<bool> RegisterUserAsync(UserModel user)
        {
            try
            {
                var existingUser = await _context.Users
                    .Find(u => u.Username == user.Username || u.Email == user.Email)
                    .FirstOrDefaultAsync();

                if (existingUser != null)
                {
                    _logger.LogWarning("User registration failed: Username or Email already exists.");
                    return false;
                }

                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                user.Id = Guid.NewGuid().ToString();

                await _context.Users.InsertOneAsync(user);

                _logger.LogInformation("User registered successfully: {Username}", user.Username);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while registering user.");
                return false;
            }
        }

        public async Task<string> LoginUserAsync(string username, string password)
        {
            var user = await _context.Users
                    .Find(u => u.Username == username)
                    .FirstOrDefaultAsync();
            if (user != null)
            {
                return GenerateJwtToken(user);
            }

            throw new UnauthorizedAccessException("Invalid credentials");
        }

        #endregion Methods
        #region Private Methods
        private string GenerateJwtToken(UserModel user)
        {
            var secretKey = _configuration["JwtSettings:Secret"];

            if (string.IsNullOrEmpty(secretKey))
            {
                throw new ArgumentNullException("JwtSettings:Secret", "JWT Secret is missing from the configuration");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.Name, user.Username),
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
        #endregion Private Methods
    }

}






