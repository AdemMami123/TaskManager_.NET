using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Task_Manager.Data;
using Task_Manager.Models;
using Task_Manager.ViewModels;

namespace Task_Manager.Controllers
{
    [Authorize(Roles = "Admin,Manager,Developer")]
    public class ManagersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManagersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Managers (accessible to all authorized roles)
        public async Task<IActionResult> Index()
        {
            var managers = await _context.Managers
                .Include(m => m.Projects)
                .ThenInclude(p => p.Tasks)
                .ToListAsync();

            var viewModel = managers.Select(m => new ManagerViewModel
            {
                ManagerId = m.Id,
                ManagerName = m.Name,
                ProjectCount = m.Projects.Count,
                DeveloperCount = m.Projects
                    .SelectMany(p => p.Tasks)
                    .Select(t => t.DeveloperId)
                    .Distinct()
                    .Count()
            }).ToList();

            return View(viewModel);
        }

        // GET: Managers/Details/5 (accessible to all authorized roles)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var manager = await _context.Managers
                .Include(m => m.Projects)
                .ThenInclude(p => p.Tasks)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (manager == null) return NotFound();

            // Check for null Projects
            var detailsViewModel = new ManagerDetailsViewModel
            {
                ManagerName = manager.Name,
                Projects = manager.Projects?.Select(p => new ProjectSummaryViewModel
                {
                    ProjectName = p.Name,
                    TaskCount = p.Tasks?.Count() ?? 0, // Safely handle null Tasks
                    Deadline = p.Deadline
                }).ToList() ?? new List<ProjectSummaryViewModel>() // Return an empty list if Projects is null
            };

            return View(detailsViewModel);
        }

        // GET: Managers/Create (restricted to Admin and Manager)
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Managers/Create (restricted to Admin and Manager)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([Bind("Id,Name")] Manager manager)
        {
            if (ModelState.IsValid)
            {
                _context.Add(manager);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(manager);
        }

        // GET: Managers/Edit/5 (restricted to Admin and Manager)
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var manager = await _context.Managers.FindAsync(id);
            if (manager == null) return NotFound();

            return View(manager);
        }

        // POST: Managers/Edit/5 (restricted to Admin and Manager)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Manager manager)
        {
            if (id != manager.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(manager);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ManagerExists(manager.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(manager);
        }

        // GET: Managers/Delete/5 (restricted to Admin and Manager)
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var manager = await _context.Managers.FirstOrDefaultAsync(m => m.Id == id);
            if (manager == null) return NotFound();

            return View(manager);
        }

        // POST: Managers/Delete/5 (restricted to Admin and Manager)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var manager = await _context.Managers.FindAsync(id);
            if (manager != null)
            {
                _context.Managers.Remove(manager);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ManagerExists(int id)
        {
            return _context.Managers.Any(e => e.Id == id);
        }
    }
}
