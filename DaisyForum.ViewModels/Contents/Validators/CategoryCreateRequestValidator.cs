using FluentValidation;

namespace DaisyForum.ViewModels.Contents.Validators
{
    public class CategoryCreateRequestValidator : AbstractValidator<CategoryCreateRequest>
    {
        public CategoryCreateRequestValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(string.Format(Messages.Required, "Tên"));

            RuleFor(x => x.SeoAlias).NotEmpty().WithMessage(string.Format(Messages.Required, "Seo alias"));
        }
    }
}