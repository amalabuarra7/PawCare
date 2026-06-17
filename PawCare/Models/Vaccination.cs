using System.ComponentModel.DataAnnotations;

namespace PawCare.Models
{
    public class Vaccination
    {
        [Key]
        public int VaccinationId { get; set; }

        public int PetId { get; set; }
        public Pet Pet { get; set; }

        [Required]
        public string VaccineName { get; set; }

        public DateTime DateGiven { get; set; }

        public DateTime NextDueDate { get; set; }

        public string Notes { get; set; }
    }
}