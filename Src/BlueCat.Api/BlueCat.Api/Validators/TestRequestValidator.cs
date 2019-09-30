using BlueCat.Contract.Test;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlueCat.Api.Validators
{
    public class TestRequestValidator: AbstractValidator<TestRequest>
    {
        public TestRequestValidator()
        { }
    }
}
