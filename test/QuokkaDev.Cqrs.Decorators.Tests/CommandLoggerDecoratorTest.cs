using Microsoft.Extensions.Logging;
using Moq;
using QuokkaDev.Cqrs.Abstractions;
using QuokkaDev.Cqrs.Decorators;
using QuokkaDev.Cqrs.Tests.Utilities;
using System;
using System.Threading.Tasks;
using Xunit;

namespace QuokkaDev.CQRS.Tests
{
    public class CommandLoggerDecoratorTest
    {
        private readonly ICommandDispatcher commandDispatcher;
        private readonly DependencyInjectionContext context;
        private readonly Mock<ILogger<CommandLoggerDecorator>> decoratorLogger;

        public CommandLoggerDecoratorTest()
        {
            context = new DependencyInjectionContext();

            context.RegisterMockLogger<CommandLoggerDecorator>();
            context.RegisterCommandHandler("My expected response");
            decoratorLogger = context.AddCommandLoggingDecorator();

            context.BuildServiceProvider();
            commandDispatcher = context.GetService<ICommandDispatcher>();
        }

        [Fact]
        public async Task CommandLoggerDecorator_Should_Be_Invoked()
        {
            // Arrange
            var request = new TestCommand() { Message = "My request" };

            // Act
            await commandDispatcher.Dispatch<TestCommand, TestCommandResult>(request);

            // Assert
            decoratorLogger.Verify(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.AtLeast(4)
            );
        }
    }
}
