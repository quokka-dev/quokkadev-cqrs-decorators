using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using QuokkaDev.Cqrs.Abstractions;
using QuokkaDev.Cqrs.Abstractions.Exceptions;
using QuokkaDev.Cqrs.Tests.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace QuokkaDev.Cqrs.Decorators.Tests
{
    public class ServiceCollectionUnitTest
    {
        public ServiceCollectionUnitTest()
        {
        }

        [Fact]
        public async Task Combining_Logging_And_Validation_For_Command_Should_Work()
        {
            // Arrange
            DependencyInjectionContext context = new DependencyInjectionContext();

            context.RegisterMockLogger<CommandValidationDecorator>();
            context.RegisterCommandHandler("My expected response");

            Mock<ILogger<CommandLoggerDecorator>> decoratorLogger = context.CombineCommandLoggingAndValidation();

            context.BuildServiceProvider();
            ICommandDispatcher commandDispatcher = context.GetService<ICommandDispatcher>();

            var request = new TestCommand() { Message = "" }; // Invalid command

            // Act
            Func<Task> dispatch = async () => await commandDispatcher.Dispatch<TestCommand, TestCommandResult>(request);

            // Assert
            await dispatch.Should()
                .ThrowAsync<CommandValidationException>()
                .Where(e => e.Errors.Count == 1 && e.Errors.First().EndsWith("Test command is invalid"));

            decoratorLogger.Verify(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Never()
            );
        }

        [Fact]
        public async Task Combining_Validation_And_Logging_For_Command_Should_Work()
        {
            // Arrange
            DependencyInjectionContext context = new DependencyInjectionContext();

            context.RegisterMockLogger<CommandValidationDecorator>();
            context.RegisterCommandHandler("My expected response");

            Mock<ILogger<CommandLoggerDecorator>> decoratorLogger = context.CombineCommandValidationAndLogging();

            context.BuildServiceProvider();
            ICommandDispatcher commandDispatcher = context.GetService<ICommandDispatcher>();

            var request = new TestCommand() { Message = "" }; // Invalid command

            // Act
            Func<Task> dispatch = async () => await commandDispatcher.Dispatch<TestCommand, TestCommandResult>(request);

            // Assert
            await dispatch.Should()
                .ThrowAsync<CommandValidationException>()
                .Where(e => e.Errors.Count == 1 && e.Errors.First().EndsWith("Test command is invalid"));

            decoratorLogger.Verify(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.AtLeast(2)
            );
        }

        [Fact]
        public async Task Combining_Logging_And_Validation_For_Query_Should_Work()
        {
            // Arrange
            DependencyInjectionContext context = new DependencyInjectionContext();

            context.RegisterMockLogger<QueryValidationDecorator>();
            context.RegisterQueryHandler("My expected response");

            Mock<ILogger<QueryLoggerDecorator>> decoratorLogger = context.CombineQueryLoggingAndValidation();

            context.BuildServiceProvider();
            IQueryDispatcher QueryDispatcher = context.GetService<IQueryDispatcher>();

            var request = new TestQuery() { Message = "" }; // Invalid Query

            // Act
            Func<Task> dispatch = async () => await QueryDispatcher.Dispatch<TestQuery, TestQueryResult>(request);

            // Assert
            await dispatch.Should()
                .ThrowAsync<QueryValidationException>()
                .Where(e => e.Errors.Count == 1 && e.Errors.First().EndsWith("Test query is invalid"));

            decoratorLogger.Verify(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.Never()
            );
        }

        [Fact]
        public async Task Combining_Validation_And_Logging_For_Query_Should_Work()
        {
            // Arrange
            DependencyInjectionContext context = new DependencyInjectionContext();

            context.RegisterMockLogger<QueryValidationDecorator>();
            context.RegisterQueryHandler("My expected response");

            Mock<ILogger<QueryLoggerDecorator>> decoratorLogger = context.CombineQueryValidationAndLogging();

            context.BuildServiceProvider();
            IQueryDispatcher QueryDispatcher = context.GetService<IQueryDispatcher>();

            var request = new TestQuery() { Message = "" }; // Invalid Query

            // Act
            Func<Task> dispatch = async () => await QueryDispatcher.Dispatch<TestQuery, TestQueryResult>(request);

            // Assert
            await dispatch.Should()
                .ThrowAsync<QueryValidationException>()
                .Where(e => e.Errors.Count == 1 && e.Errors.First().EndsWith("Test query is invalid"));

            decoratorLogger.Verify(x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception>(),
                (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()),
                Times.AtLeast(2)
            );
        }
    }
}
