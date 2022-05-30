using FluentAssertions;
using QuokkaDev.Cqrs.Abstractions;
using QuokkaDev.Cqrs.Abstractions.Exceptions;
using QuokkaDev.Cqrs.Tests.Utilities;
using System;
using System.Threading.Tasks;
using Xunit;

namespace QuokkaDev.Cqrs.Decorators.Tests
{
    public class CommandValidationDecoratorWithExceptionsTest
    {
        private readonly ICommandDispatcher commandDispatcher;
        private readonly DependencyInjectionContext context;

        public CommandValidationDecoratorWithExceptionsTest()
        {
            context = new DependencyInjectionContext();

            context.RegisterMockLogger<CommandValidationDecorator>();
            context.RegisterCommandHandler("My expected response");
            context.AddCommandValidationDecorator(typeof(MyCustomException));

            context.BuildServiceProvider();
            commandDispatcher = context.GetService<ICommandDispatcher>();
        }

        [Fact]
        public async Task CommandValidationDecorator_Should_Be_Invoked_With_Custom_Exception()
        {
            // Arrange
            var request = new TestCommand() { Message = "" };

            // Act
            Func<Task> dispatch = async () => await commandDispatcher.Dispatch<TestCommand, TestCommandResult>(request);

            // Assert
            await dispatch.Should()
                .ThrowAsync<MyCustomException>()
                .WithInnerException(typeof(CommandValidationException));
        }
    }

    public class MyCustomException : Exception
    {

    }
}
