using System.ComponentModel.DataAnnotations;

namespace Shopping_Tutorial.Repository.Validation
{
    public class FileExtentionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName);
                string[] extensions = { "jpg", "jpeg", "png", "gif" };

                bool result = extensions.Any(x => extension.EndsWith(x));
                if (!result)
                {
                    return new ValidationResult("File không hợp lệ, chỉ chấp nhận các định dạng: jpg, jpeg, png, gif");
                }
            }
            return ValidationResult.Success;
        }
    }    
}
