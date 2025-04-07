using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Task_Manager.Models;
using Task_Manager.Data;
using Microsoft.EntityFrameworkCore;
using Task_Manager.ViewModels;

namespace Task_Manager.Controllers
{
    [Authorize(Roles = "Admin,Manager,Developer")] 
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Get counts
            int projectCount = await _context.Projects.CountAsync();
            int developerCount = await _context.Developers.CountAsync();
            int managerCount = await _context.Managers.CountAsync();
            
            // Get task statistics
            var allTasks = await _context.Tasks.ToListAsync();
            int totalTasks = allTasks.Count;
            int completedTasks = allTasks.Count(t => t.IsCompleted);
            int overdueTasks = allTasks.Count(t => t.Deadline < DateTime.Now && !t.IsCompleted);
            
            // Calculate completion percentage
            decimal completionRate = totalTasks > 0 ? (decimal)completedTasks / totalTasks * 100 : 0;
            decimal overdueRate = totalTasks > 0 ? (decimal)overdueTasks / totalTasks * 100 : 0;
            
            // Get projects with task completion rates
            var projects = await _context.Projects
                .Include(p => p.Tasks)
                .ToListAsync();
            
            var projectData = projects.Select(p => new
            {
                Name = p.Name,
                CompletionRate = p.Tasks.Any() 
                    ? (decimal)p.Tasks.Count(t => t.IsCompleted) / p.Tasks.Count * 100 
                    : 0
            }).ToList();

            // Create view model
            var dashboardVM = new DashboardViewModel
            {
                ProjectCount = projectCount,
                DeveloperCount = developerCount,
                ManagerCount = managerCount,
                TotalTasks = totalTasks,
                CompletedTasks = completedTasks,
                OverdueTasks = overdueTasks,
                CompletionRate = completionRate,
                OverdueRate = overdueRate,
                ProjectNames = projectData.Select(p => p.Name).ToList(),
                ProjectCompletionRates = projectData.Select(p => p.CompletionRate).ToList()
            };

            return View(dashboardVM);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
