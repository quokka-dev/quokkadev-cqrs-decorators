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
    public class QueryLoggerDecoratorTest
    {
        private readonly IQueryDispatcher queryDispatcher;
        private readonly DependencyInjectionContext context;
        private readonly Mock<ILogger<QueryLoggerDecorator>> decoratorLogger;

        public QueryLoggerDecoratorTest()
        {
            context = new DependencyInjectionContext();

            context.RegisterMockLogger<QueryLoggerDecorator>();
            context.RegisterQueryHandler("My expected response");
            decoratorLogger = context.AddQueryLoggingDecorator();

            context.BuildServiceProvider();
            queryDispatcher = context.GetService<IQueryDispatcher>();
        }

        [Fact]
        public async Task QueryLoggerDecorator_Should_Be_Invoked()
        {
            // Arrange
            var request = new TestQuery() { Message = "My request" };

            // Act
            await queryDispatcher.Dispatch<TestQuery, TestQueryResult>(request);

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
