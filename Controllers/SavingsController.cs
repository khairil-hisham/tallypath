using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tallypath.Data;

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
    }
}