using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Tallypath.Data;
using Tallypath.Models;
using System.Data;
using FirebaseAdmin.Messaging;


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
                PaidBy = dto.PaidBy,
                IsMessage = dto.IsMessage,
                IsStatement = dto.IsStatement
            };


            var tokens = await _db.UserDevices
                .Where(t => _db.GroupMembers.Any(gm =>
                    gm.GroupId == dto.GroupId &&
                    gm.UserId == t.UserId &&
                    gm.UserId != userId
                ) && t.IsActive)
                .Select(t=> t.FcmToken)
                .ToListAsync();

            var message = new MulticastMessage
            {
                Tokens = tokens,
                Notification = new Notification
                {
                    Title = "Tallypath",
                    Body = $"New in {group.Name}"
                },
                Android = new AndroidConfig
                {
                    // This will update existing notification with same tag
                    Notification = new AndroidNotification
                    {
                        Tag = "unread_summary"
                    }
                },
                Data = new Dictionary<string, string>
                    {
                        { "type", "unread_summary" },
                        { "action", "open_group_screen" }
                    }
            };

            if (tokens.Count != 0)
            {

                await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message);

            }
            else
            {
                Console.WriteLine($"NO OTHER USER {message.ToString()}");
            }

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
                PaidBy = dto.PaidBy,
                IsMessage = dto.IsMessage,
                IsStatement = dto.IsStatement
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
                WITH credits AS (
                    SELECT
                        e."PaidBy" AS "UserId",
                        SUM(e."Amount") AS balance
                    FROM "Expenses" e
                    WHERE e."GroupId" = :groupId
                    GROUP BY e."PaidBy"
                ),
                debits AS (
                    SELECT
                        es."UserId",
                        -SUM(es."Share") AS balance
                    FROM "ExpenseSplit" es
                    JOIN "Expenses" e ON e."Id" = es."ExpenseId"
                    WHERE e."GroupId" = :groupId
                    GROUP BY es."UserId"
                )
                SELECT
                    "UserId",
                    SUM(balance) AS "NetBalance"
                FROM (
                    SELECT * FROM credits
                    UNION ALL
                    SELECT * FROM debits
                ) t
                GROUP BY "UserId";

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


            var results = new List<Debt>();

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

        [HttpPost("total/daily")]
        public async Task<ActionResult<List<DailyTotalDto>>> GetLast30DaysTotalRaw([FromBody] DailyExpenseRequest request)
        {
            var startUtc = DateTime.SpecifyKind(request.StartOfDayUtc, DateTimeKind.Utc);

            var results = await _db.Set<DailyTotalDto>()
                .FromSqlRaw("""
            WITH bounds AS (
                SELECT
                    (@startUtc::timestamptz + INTERVAL '1 day') AS start_utc,
                    (@startUtc::timestamptz - INTERVAL '29 days') AS end_utc
            )
            SELECT
                date_trunc('day', e."CreatedAt" AT TIME ZONE 'Asia/Kuala_Lumpur') AT TIME ZONE 'Asia/Kuala_Lumpur' AS "Date",
                SUM(es."Share") AS "Amount"
            FROM "ExpenseSplit" es
            JOIN "Expenses" e ON e."Id" = es."ExpenseId"
            CROSS JOIN bounds b
            WHERE
                e."CreatedAt" <= b.start_utc
                AND e."CreatedAt" > b.end_utc
                AND es."UserId" = @userId
                AND e."IsStatement" = false
            GROUP BY 1
            ORDER BY 1;
            """,
                    new NpgsqlParameter("startUtc", startUtc),
                    new NpgsqlParameter("userId", User.GetUserId())
                )
                .ToListAsync();
            return Ok(results);
        }



    }

}