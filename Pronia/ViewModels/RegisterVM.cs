using Pronia.Enumerations;
using System.ComponentModel.DataAnnotations;

namespace Pronia.ViewModels
{
    public class RegisterVM
    {
        [Required]
        [MinLength(4,ErrorMessage ="Username length can't be smaller than 4")]
        [MaxLength(25, ErrorMessage = "Username length can't exceed 25")]
        public string Username { get; set; }
        [Required]
        [MinLength(3, ErrorMessage = "Name length can't be smaller than 3")]
        [MaxLength(25, ErrorMessage = "Name length can't exceed 25")]
        public string Name { get; set; }
        public string Surname { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [MinLength(10, ErrorMessage = "Email length can't be smaller than 10")]
        [MaxLength(25, ErrorMessage = "Email length can't exceed 25")]
        [RegularExpression("^[a-zA-Z0-9]+(?:\\.[a-zA-Z0-9]+)*@[a-zA-Z]{2,}(?:\\.[a-zA-Z]{2,})+$",
        ErrorMessage = "Email is required and must be properly formatted.")]
        public string Email { get; set; }

        public Gender Gender { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; }
    }
}
