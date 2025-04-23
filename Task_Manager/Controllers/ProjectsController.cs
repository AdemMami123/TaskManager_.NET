using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Task_Manager.Data;
using Task_Manager.Models;
using Task_Manager.ViewModels;

namespace Task_Manager.Controllers
{
    [Authorize(Roles = "Admin,Manager,Developer")]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProjectsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Projects (accessible to all authorized roles)
        public async Task<IActionResult> Index()
        {
            // Load projects with Manager and Tasks
            var projects = await _context.Projects
                .Include(p => p.Manager)
                .Include(p => p.Tasks)
                .ToListAsync();

            var viewModel = projects.Select(p => new ProjectManagementViewModel
            {
                ProjectId = p.Id,
                ProjectName = p.Name,
                ManagerName = p.Manager != null ? p.Manager.Name : "N/A",
                TaskCount = p.Tasks.Count,
                // Calculate average progress from tasks, or 0 if none exist.
                ProgressRate = p.Tasks.Any() ? p.Tasks.Average(t => t.Progress) : 0
            }).ToList();

            return View(viewModel);
        }

        // GET: Projects/Details/5 (accessible to all authorized roles)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Load the project with its Manager and Tasks (including each Task's Developer)
            var project = await _context.Projects
                .Include(p => p.Manager)
                .Include(p => p.Tasks)
                    .ThenInclude(t => t.Developer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (project == null)
            {
                return NotFound();
            }

            // Build list of task details for this project.
            var taskDetails = project.Tasks.Select(t => new ProjectTaskDetailsViewModel
            {
                TaskName = t.Name,
                DeveloperName = t.Developer != null ? t.Developer.Name : "N/A",
                Progress = t.Progress,
                Deadline = t.Deadline
            }).ToList();

            // Pass the project name to the view using ViewBag.
            ViewBag.ProjectName = project.Name;

            return View(taskDetails);
        }

        // GET: Projects/Create (restricted to Admin and Manager)
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create()
        {
            ViewData["ManagerId"] = new SelectList(_context.Managers, "Id", "Name");
            ViewData["DeveloperIds"] = new SelectList(_context.Developers, "Id", "Name");
            return View();
        }

        // POST: Projects/Create (restricted to Admin and Manager)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create(ProjectCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Create the project
                var project = new Project
                {
                    Name = viewModel.Name,
                    ManagerId = viewModel.ManagerId,
                    Deadline = viewModel.Deadline
                };

                _context.Add(project);
                await _context.SaveChangesAsync();

                // Create tasks for the project
                if (viewModel.Tasks != null && viewModel.Tasks.Count > 0)
                {
                    foreach (var taskModel in viewModel.Tasks)
                    {
                        if (!string.IsNullOrEmpty(taskModel.Name))
                        {
                            var task = new TaskItem
                            {
                                Name = taskModel.Name,
                                DeveloperId = taskModel.DeveloperId,
                                ProjectId = project.Id,
                                Deadline = taskModel.Deadline ?? viewModel.Deadline, // Use project deadline if task deadline not specified
                                Progress = 0, // Initial progress is 0
                                IsCompleted = false // Initial completion status is false
                            };
                            
                            _context.Tasks.Add(task);
                        }
                    }
                    
                    await _context.SaveChangesAsync();
                }
                
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["ManagerId"] = new SelectList(_context.Managers, "Id", "Name", viewModel.ManagerId);
            ViewData["DeveloperIds"] = new SelectList(_context.Developers, "Id", "Name");
            return View(viewModel);
        }

        // GET: Projects/Edit/5 (restricted to Admin and Manager)
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }
            ViewData["ManagerId"] = new SelectList(_context.Managers, "Id", "Name", project.ManagerId);
            return View(project);
        }

        // POST: Projects/Edit/5 (restricted to Admin and Manager)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ManagerId,Deadline")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Projects.Any(e => e.Id == project.Id))
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
            ViewData["ManagerId"] = new SelectList(_context.Managers, "Id", "Name", project.ManagerId);
            return View(project);
        }

        // GET: Projects/Delete/5 (restricted to Admin and Manager)
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects
                .Include(p => p.Manager)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5 (restricted to Admin and Manager)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project != null)
            {
                _context.Projects.Remove(project);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
