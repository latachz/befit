using System.ComponentModel.DataAnnotations;

namespace BeFit.DTOs
{
    public class StatisticsDTO
    {
        [Display(Name = "Typ ćwiczenia")]
        public string ExerciseTypeName { get; set; } = string.Empty;

        [Display(Name = "Liczba wykonanych sesji")]
        public int TimesPerformed { get; set; }

        [Display(Name = "Łączna liczba powtórzeń")]
        public int TotalRepetitions { get; set; }

        [Display(Name = "Średnie obciążenie (kg)")]
        [DisplayFormat(DataFormatString = "{0:F2}")]
        public decimal AverageWeight { get; set; }

        [Display(Name = "Maksymalne obciążenie (kg)")]
        [DisplayFormat(DataFormatString = "{0:F2}")]
        public decimal MaxWeight { get; set; }
    }
}

