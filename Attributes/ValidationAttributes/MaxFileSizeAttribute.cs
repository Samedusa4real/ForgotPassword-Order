
using System.ComponentModel.DataAnnotations;

namespace PustokTemplate.Attributes.ValidationAttributes
{
    public class MaxFileSizeAttribute:ValidationAttribute
    {
        private readonly int _maxSize;

        public MaxFileSizeAttribute(int maxSize)
        {
            _maxSize = maxSize;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile)
            {
                IFormFile file = value as IFormFile;

                if (file.Length > _maxSize)
                {
                    return new ValidationResult("FileSize must be less or equal than" + _maxSize + " byte");
                }
            }
            else if (value is List<IFormFile>)
            {
                List<IFormFile> files = value as List<IFormFile>;
                foreach (var file in files)
                {
                    if (file.Length > _maxSize)
                    {
                        return new ValidationResult("FileSize must be less or equal than" + _maxSize + " byte");
                    }
                }
            }

            return ValidationResult.Success;
        }
    }
}
