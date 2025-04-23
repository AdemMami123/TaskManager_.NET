using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Task_Manager.Data;
using Task_Manager.Models;

namespace Task_Manager.Controllers
{
    [Authorize]
    [Route("api")]
    [ApiController] // allows authenticated users to fetch simplified lists of projects and tasks from the database
    public class ApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ApiController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet("projects")]
        public async Task<IActionResult> GetProjects()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            // Get all projects with minimal properties to avoid errors
            var projects = await _context.Projects
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    
                    DueDate = DateTime.Now.AddDays(30).ToString("yyyy-MM-dd") 
                })
                .ToListAsync();

            return Ok(projects);
        }

        [HttpGet("tasks")]
        public async Task<IActionResult> GetTasks()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Unauthorized();
            }

            // Get all tasks with minimal properties to avoid errors
            var tasks = await _context.Tasks
                .Select(t => new
                {
                    t.Id,
                    Title = "Task",
                    DueDate = DateTime.Now.AddDays(7).ToString("yyyy-MM-dd") // Placeholder date
                })
                .ToListAsync();

            return Ok(tasks);
        }
    }
}
