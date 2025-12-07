using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BeFit.Models
{
    public class TrainingSession
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Data jest wymagana")]
        [Display(Name = "Data")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Czas rozpoczęcia jest wymagany")]
        [Display(Name = "Czas rozpoczęcia")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "Czas zakończenia jest wymagany")]
        [Display(Name = "Czas zakończenia")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        [Required]
        [Display(Name = "Użytkownik")]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public AppUser User { get; set; } = null!;

        public ICollection<ExerciseExecution> ExerciseExecutions { get; set; } = new List<ExerciseExecution>();

        [NotMapped]
        [Display(Name = "Czas trwania")]
        public TimeSpan Duration => EndTime > StartTime ? EndTime - StartTime : TimeSpan.Zero;
    }
}

