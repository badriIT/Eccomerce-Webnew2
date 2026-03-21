using Eccomerce_Web.Modules.Products.Dtos.Request;
using FluentValidation;

namespace Eccomerce_Web.Modules.Products.Validators
{
    public class ProductAddValidator : AbstractValidator<ProductDto>
    {
        public ProductAddValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("product name can't be blank")
                .MaximumLength(100).WithMessage("max lenght 100 characters");

            RuleFor(x => x.Size)
                .NotEmpty().WithMessage("size can't be blank")
                .Must(size => size != "0").WithMessage("size can not be 0!")
                .MaximumLength(100).WithMessage("max lenght 100 characters");

            RuleFor(x => x.Price).NotEmpty().WithMessage("price can't be blank")
                .Must(price => price >= 0.01).WithMessage("price must be greater than 0.01");


            RuleFor(x => x.Quantity).NotEmpty().WithMessage("quantity can't be blank")
                .Must(quantity => quantity >= 0.01).WithMessage("quantity must be greater than 0.01");

            RuleFor(x => x.Category).NotEmpty().WithMessage("Category can't be blank");

             
        }
    }
}
