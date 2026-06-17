using System.ComponentModel.DataAnnotations;

namespace PawCare.Models
{
    public class AdoptionRequest
    {
        [Key]
        public int RequestId { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int PetId { get; set; }
        public Pet Pet { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime RequestDate { get; set; } = DateTime.Now;

        public string Message { get; set; }
    }
}