
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
    public class ExpensesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ExpensesController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> CreateExpense(CreateExpenseDto dto)
        {
            var userId = Guid.Parse(User.FindFirst("id")!.Value);

            // Check membership
            bool isMember = await _db.GroupMembers
                .AnyAsync(m => m.GroupId == dto.GroupId && m.UserId == userId);

            if (!isMember)
                return Forbid();

            var exp = new Expense
            {
                GroupId = dto.GroupId,
                Content = dto.Content,
                CreatorId = userId
            };

            _db.Expenses.Add(exp);
            await _db.SaveChangesAsync();

            return Ok(exp);
        }
    }

}