using FluentValidation;

namespace DotNetCore30Demo.Resource.Validators
{
    public class SchoolUpdateValidator:SchoolAddAndUpdateValidator<SchoolResource>
    {
        public SchoolUpdateValidator()
        {
            RuleFor(v=>v.Id).NotEmpty().WithName("Id").WithMessage("{PropertyName}不能为空！");
        }
    }
}