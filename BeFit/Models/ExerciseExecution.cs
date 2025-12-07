using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace BeFit.Models
{
    public class ExerciseExecution
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Typ ćwiczenia jest wymagany")]
        [Display(Name = "Typ ćwiczenia")]
        public int ExerciseTypeId { get; set; }

        [ForeignKey("ExerciseTypeId")]
        [ValidateNever]
        public ExerciseType ExerciseType { get; set; } = null!;

        [Range(1, int.MaxValue, ErrorMessage = "Sesja treningowa jest wymagana")]
        [Display(Name = "Sesja treningowa")]
        public int TrainingSessionId { get; set; }

        [ForeignKey("TrainingSessionId")]
        [ValidateNever]
        public TrainingSession TrainingSession { get; set; } = null!;

        [Required(ErrorMessage = "Obciążenie jest wymagane")]
        [Display(Name = "Obciążenie (kg)")]
        [Range(0, 1000, ErrorMessage = "Obciążenie musi być między 0 a 1000 kg")]
        [Column(TypeName = "decimal(6,2)")]
        public decimal Weight { get; set; }

        [Required(ErrorMessage = "Liczba serii jest wymagana")]
        [Display(Name = "Liczba serii")]
        [Range(1, 50, ErrorMessage = "Liczba serii musi być między 1 a 50")]
        public int NumberOfSets { get; set; }

        [Required(ErrorMessage = "Liczba powtórzeń jest wymagana")]
        [Display(Name = "Liczba powtórzeń w serii")]
        [Range(1, 200, ErrorMessage = "Liczba powtórzeń musi być między 1 a 200")]
        public int RepetitionsPerSet { get; set; }

        [NotMapped]
        [Display(Name = "Łączna liczba powtórzeń")]
        public int TotalRepetitions => NumberOfSets * RepetitionsPerSet;
    }
}

