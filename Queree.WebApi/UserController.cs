using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Queree.DynamicQuery;

namespace Queree.WebApi
{
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly AppDbContext _dbContext;

        public UserController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult Index(Query query)
        {
            return Ok(_dbContext.Users.ApplyQuery(query).ToList());
        }
    }
}