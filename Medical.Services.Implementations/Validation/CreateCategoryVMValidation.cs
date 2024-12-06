using FluentValidation;
using Medical.Service.Interfaces.Dtos.Categories;

namespace Medical.Services.Implementations.Validation;

public class CreateCategoryVMValidation : AbstractValidator<CreateCategoryDto>
{
    public CreateCategoryVMValidation()
    {
        RuleFor(c => c.Name).NotEmpty().WithMessage("Please enter your category name.");
    }
}
