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

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateExpense(CreateExpenseDto dto)
        {
            var userId = User.GetUserId();

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

        [HttpGet("group/{groupId}")]
        public async Task<IActionResult> GetExpenses(Guid groupId, DateTime? before, int limit = 50)
        {
            var userId = User.GetUserId();

            bool isMember = await _db.GroupMembers
                .AnyAsync(m => m.GroupId == groupId && m.UserId == userId);

            if (!isMember)
                return Forbid();

            var query = _db.Expenses
                .Where(m => m.GroupId == groupId)
                .OrderByDescending(m => m.CreatedAt);

            if (before != null)
                query = (IOrderedQueryable<Expense>)query.Where(m => m.CreatedAt < before);

            var list = await query.Take(limit).ToListAsync();

            return Ok(list);
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentExpenses()
        {
            var userId = User.GetUserId();

            var expenses = await _db.Expenses
                .Where(e => e.Group.Members.Any(gm => gm.UserId == userId))
                .OrderByDescending(e => e.CreatedAt)
                .Take(50)
                .ToListAsync();

            return Ok(expenses);
        }

    }

}