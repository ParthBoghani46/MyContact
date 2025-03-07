using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http; // Required for IFormFile

namespace MyContact.Models
{
    public class t_Contact
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int c_contactId { get; set; }

        public int c_userId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string c_contactName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string c_Email { get; set; }

        [RequiredCheckbox]
        public string c_Group { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters.")]
        public string? c_Address { get; set; }

        [StringLength(50, ErrorMessage = "Mobile number cannot exceed 50 characters.")]
        [RegularExpression(@"^\+?[0-9]{10}$", ErrorMessage = "Invalid mobile number.")]
        [Required(ErrorMessage = "Mobile No is required.")]
        public string? c_Mobile { get; set; }

        [StringLength(4000, ErrorMessage = "Image URL cannot exceed 4000 characters.")]
        public string? c_Image { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        public int c_Status { get; set; }

        [Required(ErrorMessage = "State is required.")]
        public int c_stateid { get; set; }

        [Required(ErrorMessage = "City is required.")]
        public int c_cityid { get; set; }

        [DataType(DataType.Upload)]
        [Required(ErrorMessage = "Contact Image is required.")]
        public IFormFile? ContactPicture { get; set; }
    }
}


