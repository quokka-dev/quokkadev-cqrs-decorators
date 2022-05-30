using FluentValidation;

namespace QuokkaDev.Cqrs.Tests.Utilities
{
    public class TestCommand
    {
        public string Message { get; set; } = "";
    }

    public class TestCommandResult
    {
        public const string DEFAULT_RESPONSE = "Mock Response";
        public string Message { get; set; } = "";
    }

    public class TestCommandValidator : AbstractValidator<TestCommand>
    {
        public TestCommandValidator()
        {
            RuleFor(c => c.Message).NotEmpty().WithMessage("Test command is invalid");
        }
    }
}
