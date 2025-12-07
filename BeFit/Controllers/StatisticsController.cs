using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BeFit.Data;
using BeFit.DTOs;
using System.Security.Claims;

namespace BeFit.Controllers
{
    [Authorize]
    public class StatisticsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Statistics
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var fourWeeksAgo = DateTime.Today.AddDays(-28);

            var statistics = await _context.ExerciseExecution
                .Include(e => e.ExerciseType)
                .Include(e => e.TrainingSession)
                .Where(e => e.TrainingSession.UserId == userId && 
                           e.TrainingSession.Date.Date >= fourWeeksAgo)
                .GroupBy(e => e.ExerciseType)
                .Select(g => new
                {
                    ExerciseType = g.Key,
                    TimesPerformed = g.Select(e => e.TrainingSessionId).Distinct().Count(),
                    TotalRepetitions = g.Sum(e => e.NumberOfSets * e.RepetitionsPerSet),
                    AverageWeight = g.Average(e => (double?)e.Weight) ?? 0,
                    MaxWeight = g.Max(e => (double?)e.Weight) ?? 0
                })
                .OrderBy(s => s.ExerciseType.Name)
                .ToListAsync();

            var statisticsDTOs = statistics.Select(s => new StatisticsDTO
            {
                ExerciseTypeName = s.ExerciseType.Name,
                TimesPerformed = s.TimesPerformed,
                TotalRepetitions = s.TotalRepetitions,
                AverageWeight = (decimal)s.AverageWeight,
                MaxWeight = (decimal)s.MaxWeight
            }).ToList();

            return View(statisticsDTOs);
        }
    }
}

