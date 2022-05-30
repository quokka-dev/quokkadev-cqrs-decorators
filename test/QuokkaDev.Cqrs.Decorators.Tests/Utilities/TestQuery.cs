using FluentValidation;

namespace QuokkaDev.Cqrs.Tests.Utilities
{
    public class TestQuery
    {
        public string Message { get; set; } = "";
    }

    public class TestQueryResult
    {
        public const string DEFAULT_RESPONSE = "Mock Response";
        public string Message { get; set; } = "";
    }

    public class TestQueryValidator : AbstractValidator<TestQuery>
    {
        public TestQueryValidator()
        {
            RuleFor(c => c.Message).NotEmpty().WithMessage("Test query is invalid");
        }
    }
}
