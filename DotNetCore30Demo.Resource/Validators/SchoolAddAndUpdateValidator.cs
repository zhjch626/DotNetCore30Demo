using FluentValidation;

namespace DotNetCore30Demo.Resource.Validators
{
    public class SchoolAddAndUpdateValidator<T> : AbstractValidator<T> where T : SchoolResource
    {
        public SchoolAddAndUpdateValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithName("学校名称").WithMessage("{PropertyName}是必填项").MaximumLength(50)
                .WithMessage("{PropertyName}的长度不能超过{MaxLength}");
        }
    }
}