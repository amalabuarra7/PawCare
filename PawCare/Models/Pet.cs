using System.ComponentModel.DataAnnotations;

namespace PawCare.Models
{
    public class Pet
    {
        [Key]
        public int PetId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Species { get; set; }

        public string Breed { get; set; }

        public int Age { get; set; }

        public string Description { get; set; }

        public string ImageUrl { get; set; }

        public bool IsAvailableForAdoption { get; set; } = true;

        public int UserId { get; set; }

        public User User { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}