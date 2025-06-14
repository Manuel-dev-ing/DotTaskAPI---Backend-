using System.ComponentModel.DataAnnotations;

namespace DotTaskAPI.DTOs
{
    public class PasswordDTO
    {
        [MinLength(8, ErrorMessage = "El password es muy corto, minimo 8 caracteres")]
        public string Password { get; set; }
        public string Password_Confirmation { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {

            if (Password != Password_Confirmation)
            {
                yield return new ValidationResult("Los passwords no son iguales", new[] { nameof(Password) });
            }


        }

    }
}
