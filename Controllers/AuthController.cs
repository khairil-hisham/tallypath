using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Tallypath.Data;
using Tallypath.Models;
using BCrypt.Net;

namespace LoginApi.Controllers
{
    [ApiController]
    [Route('/')]
    public class LandingController : ControllerBase
    {
        [HttpGet]
        public IActionResult Index()
        {
            // "index.html" must be in wwwroot
            return PhysicalFile("wwwroot/index.html", "text/html");
        }
    }


    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            if (_context.Users.Any(u => u.Username == dto.Username))
                return BadRequest("Username already exists");

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FullName = dto.FullName,
                Email = dto.Email,
                Mobile = dto.Mobile,
                Dob = dto.Dob
            };


            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User registered successfully");
        }

[HttpPost("login")]
public IActionResult Login([FromBody] LoginDto dto)
{
    // Find user by username OR email
    var user = _context.Users
        .FirstOrDefault(u => u.Username == dto.Identifier || u.Email == dto.Identifier);

    if (user == null)
        return Unauthorized("Invalid username/email or password");

    // Verify password
    bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
    if (!isPasswordValid)
        return Unauthorized("Invalid username/email or password");

    // Generate JWT token
    var tokenHandler = new JwtSecurityTokenHandler();
    var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

    var tokenDescriptor = new SecurityTokenDescriptor
    {
        Subject = new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim("email", user.Email),
            new Claim("fullname", user.FullName)
        }),
        Expires = DateTime.UtcNow.AddHours(2),
        Issuer = _config["Jwt:Issuer"],
        Audience = _config["Jwt:Audience"],
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
    };

    var token = tokenHandler.CreateToken(tokenDescriptor);
    return Ok(new
    {
        Token = tokenHandler.WriteToken(token),
        User = new { user.Id, user.Username, user.FullName, user.Email, user.Mobile, user.Dob}
    });
}
    }
}
