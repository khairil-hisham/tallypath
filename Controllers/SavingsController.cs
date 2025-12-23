using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tallypath.Data;
using Tallypath.Models;

namespace Tallypath.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SavingsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public SavingsController(AppDbContext db)
        {
            _db = db;
        }

        [Authorize]
        [HttpGet("user")]
        public async Task<IActionResult> GetUserPlans()
        {
            var plans = await _db.SavingPlans
                .Where(p => p.UserId == User.GetUserId())
                .OrderBy(p => p.CreatedAt)
                .ToListAsync();

            return Ok(plans);
        }
        

        [Authorize]
        [HttpPost("create")]
        public async Task<IActionResult> CreatePlan([FromBody] CreatePlanRequest request)
        {
            // 1. Validate plan title
            if (string.IsNullOrWhiteSpace(request.Title))
                return BadRequest("Plan title is required.");

            var userId = User.GetUserId();

            // 2. Create Plan 
            var plan = new Savings
            {
                UserId = userId,
                Title = request.Title,
                Target = request.Target,
                Due = request.Due,
                CreatedAt = DateTime.UtcNow,
            };

            // 5. Save to DB
            _db.SavingPlans.Add(plan);
            await _db.SaveChangesAsync();

            // 6. Return new group info
            return Ok("Plan created successfully");
        }
    }
}