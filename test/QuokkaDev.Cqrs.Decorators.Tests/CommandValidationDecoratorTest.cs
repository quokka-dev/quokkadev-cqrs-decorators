using FluentAssertions;
using QuokkaDev.Cqrs.Abstractions;
using QuokkaDev.Cqrs.Abstractions.Exceptions;
using QuokkaDev.Cqrs.Decorators;
using QuokkaDev.Cqrs.Tests.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace QuokkaDev.CQRS.Tests
{
    public class CommandValidationDecoratorTest
    {
        private readonly ICommandDispatcher commandDispatcher;
        private readonly DependencyInjectionContext context;

        public CommandValidationDecoratorTest()
        {
            context = new DependencyInjectionContext();

            context.RegisterMockLogger<CommandValidationDecorator>();
            context.RegisterQueryHandler("My expected response");
            context.AddCommandValidationDecorator();

            context.BuildServiceProvider();
            commandDispatcher = context.GetService<ICommandDispatcher>();
        }

        [Fact]
        public async Task CommandValidationDecorator_Should_Be_Invoked()
        {
            // Arrange
            var request = new TestCommand() { Message = "" };

            // Act
            Func<Task> dispatch = async () => await commandDispatcher.Dispatch<TestCommand, TestCommandResult>(request);

            // Assert
            await dispatch.Should()
                .ThrowAsync<CommandValidationException>()
                .Where(e => e.Errors.Count == 1 && e.Errors.First().EndsWith("Test command is invalid"));
        }
    }
}
