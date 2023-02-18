using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;

namespace DaisyForum.ViewModels.Systems
{
    public class RoleViewModelValidator : AbstractValidator<RoleViewModel>
    {
        public RoleViewModelValidator()
        {
            RuleFor(x => x.RoleId).NotEmpty().WithMessage("Role Id is required")
                .MaximumLength(50).WithMessage("Role Id must be less than 50 characters")
                .MinimumLength(5).WithMessage("Role Id must be greater than 5 characters");

            RuleFor(x => x.RoleName).NotEmpty().WithMessage("Role Name is required");
        }
    }
}