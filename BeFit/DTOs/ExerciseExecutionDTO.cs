using BeFit.Models;
using System.ComponentModel.DataAnnotations;

namespace BeFit.DTOs
{
    public class ExerciseExecutionDTO
    {
        public int Id { get; set; }

        [Display(Name = "Typ ćwiczenia")]
        public string ExerciseTypeName { get; set; } = string.Empty;

        public int ExerciseTypeId { get; set; }

        [Display(Name = "Sesja treningowa")]
        public string TrainingSessionDisplay { get; set; } = string.Empty;

        public int TrainingSessionId { get; set; }

        [Display(Name = "Obciążenie (kg)")]
        [DisplayFormat(DataFormatString = "{0:F2}")]
        public decimal Weight { get; set; }

        [Display(Name = "Liczba serii")]
        public int NumberOfSets { get; set; }

        [Display(Name = "Liczba powtórzeń w serii")]
        public int RepetitionsPerSet { get; set; }

        [Display(Name = "Łączna liczba powtórzeń")]
        public int TotalRepetitions { get; set; }

        public ExerciseExecutionDTO() { }

        public ExerciseExecutionDTO(ExerciseExecution exerciseExecution)
        {
            Id = exerciseExecution.Id;
            ExerciseTypeId = exerciseExecution.ExerciseTypeId;
            ExerciseTypeName = exerciseExecution.ExerciseType?.Name ?? string.Empty;
            TrainingSessionId = exerciseExecution.TrainingSessionId;
            
            if (exerciseExecution.TrainingSession != null)
            {
                TrainingSessionDisplay = $"{exerciseExecution.TrainingSession.Date:yyyy-MM-dd} " +
                    $"{exerciseExecution.TrainingSession.StartTime:hh\\:mm} - " +
                    $"{exerciseExecution.TrainingSession.EndTime:hh\\:mm}";
            }
            
            Weight = exerciseExecution.Weight;
            NumberOfSets = exerciseExecution.NumberOfSets;
            RepetitionsPerSet = exerciseExecution.RepetitionsPerSet;
            TotalRepetitions = exerciseExecution.TotalRepetitions;
        }
    }
}

