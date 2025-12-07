using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BeFit.Data;
using BeFit.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BeFit.DTOs;

namespace BeFit.Controllers
{
    [Authorize]
    public class TrainingSessionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TrainingSessionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TrainingSessions
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var sessions = await _context.TrainingSession
                .Include(t => t.User)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
            return View(sessions.Select(s => new TrainingSessionDTO(s)));
        }

        // GET: TrainingSessions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var trainingSession = await _context.TrainingSession
                .Include(t => t.User)
                .Include(t => t.ExerciseExecutions)
                    .ThenInclude(e => e.ExerciseType)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (trainingSession == null)
            {
                return NotFound();
            }

            return View(new TrainingSessionDTO(trainingSession));
        }

        // GET: TrainingSessions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TrainingSessions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,StartTime,EndTime")] TrainingSessionDTO trainingSessionDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (trainingSessionDTO.EndTime <= trainingSessionDTO.StartTime)
            {
                ModelState.AddModelError("EndTime", "Czas zakończenia musi być późniejszy niż czas rozpoczęcia");
            }

            if (ModelState.IsValid)
            {
                var trainingSession = new TrainingSession
                {
                    Id = trainingSessionDTO.Id,
                    Date = trainingSessionDTO.Date,
                    StartTime = trainingSessionDTO.StartTime,
                    EndTime = trainingSessionDTO.EndTime,
                    UserId = userId
                };

                _context.Add(trainingSession);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(trainingSessionDTO);
        }

        // GET: TrainingSessions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var trainingSession = await _context.TrainingSession
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (trainingSession == null)
            {
                return NotFound();
            }
            return View(new TrainingSessionDTO(trainingSession));
        }

        // POST: TrainingSessions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,StartTime,EndTime")] TrainingSessionDTO trainingSessionDTO)
        {
            if (id != trainingSessionDTO.Id)
            {
                return NotFound();
            }

            if (trainingSessionDTO.EndTime <= trainingSessionDTO.StartTime)
            {
                ModelState.AddModelError("EndTime", "Czas zakończenia musi być późniejszy niż czas rozpoczęcia");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var existingSession = await _context.TrainingSession
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (existingSession == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    existingSession.Date = trainingSessionDTO.Date;
                    existingSession.StartTime = trainingSessionDTO.StartTime;
                    existingSession.EndTime = trainingSessionDTO.EndTime;
                    _context.Update(existingSession);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainingSessionExists(trainingSessionDTO.Id))
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
            return View(trainingSessionDTO);
        }

        // GET: TrainingSessions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var trainingSession = await _context.TrainingSession
                .Include(t => t.User)
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (trainingSession == null)
            {
                return NotFound();
            }

            return View(trainingSession);
        }

        // POST: TrainingSessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var trainingSession = await _context.TrainingSession
                .FirstOrDefaultAsync(m => m.Id == id && m.UserId == userId);

            if (trainingSession != null)
            {
                _context.TrainingSession.Remove(trainingSession);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrainingSessionExists(int id)
        {
            return _context.TrainingSession.Any(e => e.Id == id);
        }
    }
}

