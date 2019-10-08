using BlueCat.Contract.Test;
using FluentValidation;

namespace BlueCat.Api.Validators
{
    public class TestRequestValidator : AbstractValidator<TestRequest>
    {
        public TestRequestValidator()
        {
            RuleFor(m => m.RequestContent).Length(1, 10);
        }
    }
}
