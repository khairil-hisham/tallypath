using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
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
                .Include(e => e.Splits)
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
                .Include(e => e.Splits)
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

        [HttpGet("balance/{groupId}")]
        public async Task<IActionResult> GetBalance(Guid groupId)
        {
            string sqlString =
            """
                SELECT
                    es."UserId",
                    SUM(
                        CASE
                            WHEN e."PaidBy" = es."UserId" THEN e."Amount"
                            ELSE es."Share" * -1
                        END
                    ) AS "NetBalance"
                FROM "ExpenseSplit" es
                JOIN "Expenses" e ON e."Id" = es."ExpenseId"
                WHERE e."GroupId" = :groupId
                GROUP BY es."UserId";
            """;

            var balances = await _db.UserBalances.FromSqlRaw(sqlString, new NpgsqlParameter("groupId", groupId)).ToListAsync();
            var creditors = balances
                .Where(b => b.NetBalance > 0)
                .Select(b => new UserBalance
                {
                    UserId = b.UserId,
                    NetBalance = b.NetBalance
                })
                .OrderByDescending(b => b.NetBalance)
                .ToList();

            var debtors = balances
                .Where(b => b.NetBalance < 0)
                .Select(b => new UserBalance
                {
                    UserId = b.UserId,
                    NetBalance = b.NetBalance
                })
                .OrderBy(b => b.NetBalance)
                .ToList();


            Console.WriteLine("Creditors:");
            var results = new List<Debt>();
            foreach (var person in creditors)
            {
                Console.WriteLine(person.UserId);
                Console.WriteLine(person.NetBalance);
            }
            Console.WriteLine("Debtors:");
            foreach (var person in debtors)
            {
                Console.WriteLine(person.UserId);
                Console.WriteLine(person.NetBalance);
            }

            int i = 0, j = 0;
            while (i < debtors.Count && j < creditors.Count)
            {

                var debtor = debtors[i];
                var creditor = creditors[j];

                var amount = Math.Min(-debtor.NetBalance, creditor.NetBalance);

                results.Add(new Debt
                {
                    Debtor = debtor.UserId,
                    Creditor = creditor.UserId,
                    Amount = amount
                });

                debtor.NetBalance += amount;
                creditor.NetBalance -= amount;

                if (debtor.NetBalance == 0) i++;
                if (creditor.NetBalance == 0) j++;
            }


            return Ok(results);

        }

    }

}