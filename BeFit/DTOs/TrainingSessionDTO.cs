using BeFit.Models;
using System.ComponentModel.DataAnnotations;

namespace BeFit.DTOs
{
    public class TrainingSessionDTO
    {
        public int Id { get; set; }

        [Display(Name = "Data")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Date { get; set; }

        [Display(Name = "Czas rozpoczęcia")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}")]
        public TimeSpan StartTime { get; set; }

        [Display(Name = "Czas zakończenia")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}")]
        public TimeSpan EndTime { get; set; }

        [Display(Name = "Czas trwania")]
        [DisplayFormat(DataFormatString = "{0:hh\\:mm}")]
        public TimeSpan Duration { get; set; }

        [Display(Name = "Użytkownik")]
        public string UserName { get; set; } = string.Empty;

        public ICollection<ExerciseExecutionDTO> ExerciseExecutions { get; set; } = new List<ExerciseExecutionDTO>();

        public TrainingSessionDTO() { }

        public TrainingSessionDTO(TrainingSession trainingSession)
        {
            Id = trainingSession.Id;
            Date = trainingSession.Date;
            StartTime = trainingSession.StartTime;
            EndTime = trainingSession.EndTime;
            Duration = trainingSession.EndTime > trainingSession.StartTime 
                ? trainingSession.EndTime - trainingSession.StartTime 
                : TimeSpan.Zero;
            UserName = trainingSession.User?.UserName ?? string.Empty;
            
            if (trainingSession.ExerciseExecutions != null)
            {
                ExerciseExecutions = trainingSession.ExerciseExecutions
                    .Select(e => new ExerciseExecutionDTO(e))
                    .ToList();
            }
        }
    }
}

