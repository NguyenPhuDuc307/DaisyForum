using FluentValidation;
namespace DaisyForum.ViewModels.Contents.Validators
{
    public class ReportCreateRequestValidator : AbstractValidator<ReportCreateRequest>
    {
        public ReportCreateRequestValidator()
        {
            RuleFor(x => x.Content).NotEmpty().WithMessage("Phải nhập nội dung");

            RuleFor(x => x.KnowledgeBaseId)
                .NotNull()
                .WithMessage("Chưa có mã bài đăng")
                .GreaterThan(0).WithMessage(string.Format(Messages.Required, "Mã bài đăng"));
        }
    }
}