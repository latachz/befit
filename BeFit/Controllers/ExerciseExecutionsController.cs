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
    public class ExerciseExecutionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExerciseExecutionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ExerciseExecutions
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var executions = await _context.ExerciseExecution
                .Include(e => e.ExerciseType)
                .Include(e => e.TrainingSession)
                .Where(e => e.TrainingSession.UserId == userId)
                .OrderByDescending(e => e.TrainingSession.Date)
                .ToListAsync();
            return View(executions.Select(e => new ExerciseExecutionDTO(e)));
        }

        // GET: ExerciseExecutions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var exerciseExecution = await _context.ExerciseExecution
                .Include(e => e.ExerciseType)
                .Include(e => e.TrainingSession)
                .Where(e => e.Id == id && e.TrainingSession.UserId == userId)
                .FirstOrDefaultAsync();

            if (exerciseExecution == null)
            {
                return NotFound();
            }

            return View(new ExerciseExecutionDTO(exerciseExecution));
        }

        // GET: ExerciseExecutions/Create
        public async Task<IActionResult> Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseType, "Id", "Name");
            
            var sessions = await _context.TrainingSession
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Date)
                .Select(t => new { 
                    t.Id, 
                    Display = $"{t.Date:yyyy-MM-dd} {t.StartTime:hh\\:mm} - {t.EndTime:hh\\:mm}" 
                })
                .ToListAsync();
            
            ViewData["TrainingSessionId"] = new SelectList(sessions, "Id", "Display");
            return View();
        }

        // POST: ExerciseExecutions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ExerciseTypeId,TrainingSessionId,Weight,NumberOfSets,RepetitionsPerSet")] ExerciseExecutionDTO exerciseExecutionDTO)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (exerciseExecutionDTO.TrainingSessionId <= 0)
            {
                ModelState.AddModelError("TrainingSessionId", "Sesja treningowa jest wymagana");
            }
            else
            {
                var trainingSession = await _context.TrainingSession
                    .FirstOrDefaultAsync(t => t.Id == exerciseExecutionDTO.TrainingSessionId && t.UserId == userId);

                if (trainingSession == null)
                {
                    ModelState.AddModelError("TrainingSessionId", "Nie masz dostępu do tej sesji treningowej");
                }
            }
            
            if (exerciseExecutionDTO.ExerciseTypeId <= 0)
            {
                ModelState.AddModelError("ExerciseTypeId", "Typ ćwiczenia jest wymagany");
            }
            else
            {
                var exerciseType = await _context.ExerciseType
                    .FirstOrDefaultAsync(e => e.Id == exerciseExecutionDTO.ExerciseTypeId);

                if (exerciseType == null)
                {
                    ModelState.AddModelError("ExerciseTypeId", "Wybrany typ ćwiczenia nie istnieje");
                }
            }

            if (ModelState.IsValid)
            {
                var exerciseExecution = new ExerciseExecution
                {
                    Id = exerciseExecutionDTO.Id,
                    ExerciseTypeId = exerciseExecutionDTO.ExerciseTypeId,
                    TrainingSessionId = exerciseExecutionDTO.TrainingSessionId,
                    Weight = exerciseExecutionDTO.Weight,
                    NumberOfSets = exerciseExecutionDTO.NumberOfSets,
                    RepetitionsPerSet = exerciseExecutionDTO.RepetitionsPerSet
                };

                _context.Add(exerciseExecution);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseType, "Id", "Name", exerciseExecutionDTO.ExerciseTypeId);
            
            var sessions = await _context.TrainingSession
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Date)
                .Select(t => new { 
                    t.Id, 
                    Display = $"{t.Date:yyyy-MM-dd} {t.StartTime:hh\\:mm} - {t.EndTime:hh\\:mm}" 
                })
                .ToListAsync();
            
            ViewData["TrainingSessionId"] = new SelectList(sessions, "Id", "Display", exerciseExecutionDTO.TrainingSessionId);
            return View(exerciseExecutionDTO);
        }

        // GET: ExerciseExecutions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var exerciseExecution = await _context.ExerciseExecution
                .Include(e => e.TrainingSession)
                .Where(e => e.Id == id && e.TrainingSession.UserId == userId)
                .FirstOrDefaultAsync();

            if (exerciseExecution == null)
            {
                return NotFound();
            }

            var exerciseExecutionDTO = new ExerciseExecutionDTO(exerciseExecution);
            ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseType, "Id", "Name", exerciseExecutionDTO.ExerciseTypeId);
            
            var sessions = await _context.TrainingSession
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Date)
                .Select(t => new { 
                    t.Id, 
                    Display = $"{t.Date:yyyy-MM-dd} {t.StartTime:hh\\:mm} - {t.EndTime:hh\\:mm}" 
                })
                .ToListAsync();
            
            ViewData["TrainingSessionId"] = new SelectList(sessions, "Id", "Display", exerciseExecutionDTO.TrainingSessionId);
            return View(exerciseExecutionDTO);
        }

        // POST: ExerciseExecutions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ExerciseTypeId,TrainingSessionId,Weight,NumberOfSets,RepetitionsPerSet")] ExerciseExecutionDTO exerciseExecutionDTO)
        {
            if (id != exerciseExecutionDTO.Id)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (exerciseExecutionDTO.TrainingSessionId <= 0)
            {
                ModelState.AddModelError("TrainingSessionId", "Sesja treningowa jest wymagana");
            }
            else
            {
                var trainingSession = await _context.TrainingSession
                    .FirstOrDefaultAsync(t => t.Id == exerciseExecutionDTO.TrainingSessionId && t.UserId == userId);

                if (trainingSession == null)
                {
                    ModelState.AddModelError("TrainingSessionId", "Nie masz dostępu do tej sesji treningowej");
                }
            }

            if (exerciseExecutionDTO.ExerciseTypeId <= 0)
            {
                ModelState.AddModelError("ExerciseTypeId", "Typ ćwiczenia jest wymagany");
            }

            var existingExecution = await _context.ExerciseExecution
                .Include(e => e.TrainingSession)
                .FirstOrDefaultAsync(m => m.Id == id && m.TrainingSession.UserId == userId);

            if (existingExecution == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    existingExecution.ExerciseTypeId = exerciseExecutionDTO.ExerciseTypeId;
                    existingExecution.TrainingSessionId = exerciseExecutionDTO.TrainingSessionId;
                    existingExecution.Weight = exerciseExecutionDTO.Weight;
                    existingExecution.NumberOfSets = exerciseExecutionDTO.NumberOfSets;
                    existingExecution.RepetitionsPerSet = exerciseExecutionDTO.RepetitionsPerSet;
                    _context.Update(existingExecution);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExerciseExecutionExists(exerciseExecutionDTO.Id))
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

            ViewData["ExerciseTypeId"] = new SelectList(_context.ExerciseType, "Id", "Name", exerciseExecutionDTO.ExerciseTypeId);
            
            var sessions = await _context.TrainingSession
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.Date)
                .Select(t => new { 
                    t.Id, 
                    Display = $"{t.Date:yyyy-MM-dd} {t.StartTime:hh\\:mm} - {t.EndTime:hh\\:mm}" 
                })
                .ToListAsync();
            
            ViewData["TrainingSessionId"] = new SelectList(sessions, "Id", "Display", exerciseExecutionDTO.TrainingSessionId);
            return View(exerciseExecutionDTO);
        }

        // GET: ExerciseExecutions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var exerciseExecution = await _context.ExerciseExecution
                .Include(e => e.ExerciseType)
                .Include(e => e.TrainingSession)
                .Where(e => e.Id == id && e.TrainingSession.UserId == userId)
                .FirstOrDefaultAsync();

            if (exerciseExecution == null)
            {
                return NotFound();
            }

            return View(exerciseExecution);
        }

        // POST: ExerciseExecutions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var exerciseExecution = await _context.ExerciseExecution
                .Where(e => e.Id == id && e.TrainingSession.UserId == userId)
                .FirstOrDefaultAsync();

            if (exerciseExecution != null)
            {
                _context.ExerciseExecution.Remove(exerciseExecution);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExerciseExecutionExists(int id)
        {
            return _context.ExerciseExecution.Any(e => e.Id == id);
        }
    }
}

