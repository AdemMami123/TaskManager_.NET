using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Task_Manager.Data;
using Task_Manager.Models;

namespace Task_Manager.Controllers
{
    [Authorize(Roles = "Admin,Manager,Developer")]
    public class TaskItemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TaskItemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TaskItems (accessible to all authorized roles)
        public async Task<IActionResult> Index()
        {
            var tasks = _context.Tasks
                .Include(t => t.Developer)
                .Include(t => t.Project);
            return View(await tasks.ToListAsync());
        }

        // GET: TaskItems/Details/5 (accessible to all authorized roles)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskItem = await _context.Tasks
                .Include(t => t.Developer)
                .Include(t => t.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (taskItem == null)
            {
                return NotFound();
            }

            return View(taskItem);
        }

        // GET: TaskItems/Create (restricted to Admin and Manager)
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create()
        {
            // Use Developer and Project Name as display values
            ViewData["DeveloperId"] = new SelectList(_context.Developers, "Id", "Name");
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");
            return View();
        }

        // POST: TaskItems/Create (restricted to Admin and Manager)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([Bind("Id,Name,DeveloperId,ProjectId,Progress,Deadline,Advancement")] TaskItem taskItem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(taskItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DeveloperId"] = new SelectList(_context.Developers, "Id", "Name", taskItem.DeveloperId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", taskItem.ProjectId);
            return View(taskItem);
        }

        // GET: TaskItems/Edit/5 (restricted to Admin and Manager)
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskItem = await _context.Tasks.FindAsync(id);
            if (taskItem == null)
            {
                return NotFound();
            }
            ViewData["DeveloperId"] = new SelectList(_context.Developers, "Id", "Name", taskItem.DeveloperId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", taskItem.ProjectId);
            return View(taskItem);
        }

        // POST: TaskItems/Edit/5 (restricted to Admin and Manager)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,DeveloperId,ProjectId,Progress,Deadline,Advancement")] TaskItem taskItem)
        {
            if (id != taskItem.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taskItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskItemExists(taskItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DeveloperId"] = new SelectList(_context.Developers, "Id", "Name", taskItem.DeveloperId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", taskItem.ProjectId);
            return View(taskItem);
        }

        // Toggle task completion (modified to be accessible to developers for their own tasks)
        [HttpPost]
[Authorize(Roles = "Admin,Manager,Developer")]
public async Task<IActionResult> ToggleComplete(int id, bool isCompleted)
{
    var task = await _context.Tasks
        .Include(t => t.Developer)
        .FirstOrDefaultAsync(t => t.Id == id);

    if (task == null)
    {
        return NotFound();
    }

    // If the user is a Developer, restrict to only their own tasks
    if (User.IsInRole("Developer"))
    {
        var userEmail = User.Identity?.Name;

        
    }

    // Allow Admins and Managers to proceed freely

    task.IsCompleted = isCompleted;
    await _context.SaveChangesAsync();

    // Update developer progress
    await UpdateDeveloperProgress(task.DeveloperId);

    return RedirectToAction(nameof(Index));
}


        // GET: TaskItems/Delete/5 (restricted to Admin and Manager)
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskItem = await _context.Tasks
                .Include(t => t.Developer)
                .Include(t => t.Project)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (taskItem == null)
            {
                return NotFound();
            }

            return View(taskItem);
        }

        // POST: TaskItems/Delete/5 (restricted to Admin and Manager)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taskItem = await _context.Tasks.FindAsync(id);
            if (taskItem != null)
            {
                _context.Tasks.Remove(taskItem);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Helper method - no direct access
        private async Task UpdateDeveloperProgress(int developerId)
        {
            // Get the developer
            var developer = await _context.Developers.FindAsync(developerId);
            
            if (developer != null)
            {
                // Get all tasks for this developer
                var developerTasks = await _context.Tasks
                    .Where(t => t.DeveloperId == developerId)
                    .ToListAsync();
                
                int totalTasks = developerTasks.Count;
                
                // If developer has tasks, calculate progress percentage
                if (totalTasks > 0)
                {
                    int completedTasks = developerTasks.Count(t => t.IsCompleted);
                    // Calculate percentage and round to 2 decimal places
                    double progressPercentage = ((double)completedTasks / totalTasks) * 100;
                    
                    // If you have a Progress property in Developer model, update it here
                    // developer.Progress = progressPercentage;
                    // await _context.SaveChangesAsync();
                }
            }
        }

        private bool TaskItemExists(int id)
        {
            return _context.Tasks.Any(e => e.Id == id);
        }
    }
}
