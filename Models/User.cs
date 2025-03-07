using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MyContact.Models
{
    public class t_User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int c_userId { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(100)]
        public string c_userName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(100)]
        public string c_Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
        [StringLength(500)]
        public string c_Password { get; set; }


        [StringLength(500)]
        [Required(ErrorMessage = "Confirm Password is required.")]
        [Compare("c_Password", ErrorMessage = "Passwords do not match.")]
        [NotMapped]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Addres is required.")]
        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters.")]
        public string? c_Address { get; set; }

        [Required(ErrorMessage = "Mobile number is required.")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile number must be exactly 10 digits.")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile number must be exactly 10 digits.")]
        public string? c_Mobile { get; set; }

        [StringLength(4000, ErrorMessage = "Image path cannot exceed 4000 characters.")]
        public string? c_Image { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        [StringLength(20, ErrorMessage = "Gender cannot exceed 20 characters.")]
        [RegularExpression(@"^(Male|Female|Other)$", ErrorMessage = "Gender must be 'Male', 'Female', or 'Other'.")]
        public string? c_Gender { get; set; }

        [Required(ErrorMessage = "ProfilePicture is required.")]
        public IFormFile? ProfilePicture { get; set; }

    }
}