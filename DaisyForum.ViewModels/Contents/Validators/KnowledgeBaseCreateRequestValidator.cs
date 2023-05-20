using FluentValidation;

namespace DaisyForum.ViewModels.Contents.Validators
{
    public class KnowledgeBaseCreateRequestValidator : AbstractValidator<KnowledgeBaseCreateRequest>
    {
        public KnowledgeBaseCreateRequestValidator()
        {
            RuleFor(x => x.CategoryId).GreaterThan(0)
                .WithMessage(string.Format(Messages.Required, "Danh mục"));

            RuleFor(x => x.Title).NotEmpty().WithMessage(string.Format(Messages.Required, "Tiêu đề"));

            RuleFor(x => x.Problem).NotEmpty().WithMessage(string.Format(Messages.Required, "Vấn đề"));
        }
    }
}