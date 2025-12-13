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

            // 1. Ensure group exists
            var group = await _db.Groups.FindAsync(dto.GroupId);
            if (group == null)
                return NotFound("Group not found.");

            // 2. Ensure user is a member of this group
            bool isMember = await _db.GroupMembers
                .AnyAsync(m => m.GroupId == dto.GroupId && m.UserId == userId);

            if (!isMember)
                return Forbid();


            var exp = new Expense
            {
                GroupId = dto.GroupId,
                Title = dto.Title,
                CreatorId = userId,
                Amount = dto.Amount,
                CreatedAt = DateTime.UtcNow,
                Splits = [.. dto.Splits.Select(s => new ExpenseSplit{
                    UserId = s.UserId,
                    Share = s.Share
                })],
                PaidBy = dto.PaidBy
            };

            _db.Expenses.Add(exp);

            group.Total += dto.Amount;

            await _db.SaveChangesAsync();

            // 6. Return with the new data
            return Ok(new ExpenseDto
            {
                Id = exp.Id,
                GroupId = exp.GroupId,
                Amount = exp.Amount,
                Title = exp.Title,
                CreatedAt = exp.CreatedAt,
                Splits = dto.Splits,
                PaidBy = dto.PaidBy
            });
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
                .OrderByDescending(m => m.CreatedAt);//latest [0] to oldest [n]

            if (before != null)
                query = (IOrderedQueryable<Expense>)query.Where(m => m.CreatedAt < before);

            var list = await query.Take(limit).ToListAsync();

            return Ok(list);
        }

        [HttpGet("after/group/{groupId}")]
        public async Task<IActionResult> GetExpensesAfter(Guid groupId, DateTime? after)
        {
            var userId = User.GetUserId();

            bool isMember = await _db.GroupMembers
                .AnyAsync(m => m.GroupId == groupId && m.UserId == userId);

            if (!isMember)
                return Forbid();

            var query = _db.Expenses
                .Where(m => m.GroupId == groupId)
                .OrderByDescending(m => m.CreatedAt);//latest [0] to oldest [n]

            if (after != null)
                query = (IOrderedQueryable<Expense>)query.Where(m => m.CreatedAt > after);

            var list = await query.ToListAsync();

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