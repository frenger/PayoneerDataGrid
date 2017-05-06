using System.ComponentModel.DataAnnotations;

namespace PayoneerExercise.Models
{
    public class User
    {
        //public System.Guid AssetID { get; set; }

        [Required]
        [Display(Name = "User ID")]
        public int UserId { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Age")]
        public int Age { get; set; }

        //public string Comments { get; set; }

        //public bool Issued { get; set; }

    }
}
