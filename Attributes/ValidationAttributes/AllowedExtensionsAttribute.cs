using System.ComponentModel.DataAnnotations;

namespace PustokTemplate.Attributes.ValidationAttributes
{
    public class AllowedExtensionsAttribute:ValidationAttribute
    {
        private readonly string[] _types;

        public AllowedExtensionsAttribute(params string[] types)
        {
            _types = types;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value is IFormFile)
            {
                IFormFile file = (IFormFile)value;
                if (!_types.Contains(file.ContentType))
                    return new ValidationResult("File type must be one of the types: " + string.Join(", ", _types));
            }
            else if (value is List<IFormFile>)
            {
                List<IFormFile> files = value as List<IFormFile>;
                foreach (var file in files)
                {
                    if (!_types.Contains(file.ContentType))
                        return new ValidationResult("File type must be one of the types: " + string.Join(", ", _types));
                }
            }

            return ValidationResult.Success;
        }
    }
}
