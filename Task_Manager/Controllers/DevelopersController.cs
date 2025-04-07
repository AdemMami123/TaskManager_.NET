using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task_Manager.Data;
using Task_Manager.Models;
using Task_Manager.ViewModels;

namespace Task_Manager.Controllers
{
    public class DevelopersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DevelopersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Developers
        public async Task<IActionResult> Index()
        {
            // Load developers including their Tasks and each Task's Project.
            var developers = await _context.Developers
                .Include(d => d.Tasks)
                    .ThenInclude(t => t.Project)
                .ToListAsync();

            var viewModel = developers.Select(d => new DeveloperManagementViewModel
            {
                DeveloperId = d.Id,
                DeveloperName = d.Name,
                // Calculate distinct projects from the developer's tasks.
                ProjectCount = d.Tasks.Select(t => t.ProjectId).Distinct().Count(),
                TotalTasks = d.Tasks.Count,
                CompletedTasks = d.Tasks.Count(t => t.IsCompleted),
                ProgressRate = d.Tasks.Count > 0 ?
                    ((double)d.Tasks.Count(t => t.IsCompleted) / d.Tasks.Count) * 100 : 0
            }).ToList();

            return View(viewModel);
        }

        // GET: Developers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Load the developer with its tasks and each task's project.
            var developer = await _context.Developers
                .Include(d => d.Tasks)
                    .ThenInclude(t => t.Project)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (developer == null)
            {
                return NotFound();
            }

            // Build a list of task details for the developer.
            var taskDetails = developer.Tasks.Select(t => new DeveloperDetailsViewModel
            {
                TaskName = t.Name,
                ProjectName = t.Project != null ? t.Project.Name : "N/A",
                ProgressRate = t.Progress // Assuming this represents progress (e.g., percentage)
            }).ToList();

            // Pass the developer's name to the view via ViewBag.
            ViewBag.DeveloperName = developer.Name;

            return View(taskDetails);
        }

        // GET: Developers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Developers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Position")] Developer developer)
        {
            if (ModelState.IsValid)
            {
                _context.Add(developer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(developer);
        }

        // GET: Developers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var developer = await _context.Developers.FindAsync(id);
            if (developer == null)
            {
                return NotFound();
            }
            return View(developer);
        }

        // POST: Developers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Position")] Developer developer)
        {
            if (id != developer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(developer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Developers.Any(e => e.Id == developer.Id))
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
            return View(developer);
        }

        // GET: Developers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var developer = await _context.Developers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (developer == null)
            {
                return NotFound();
            }

            return View(developer);
        }

        // POST: Developers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var developer = await _context.Developers.FindAsync(id);
            if (developer != null)
            {
                _context.Developers.Remove(developer);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
