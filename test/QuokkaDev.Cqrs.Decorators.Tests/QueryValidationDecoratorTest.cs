using FluentAssertions;
using QuokkaDev.Cqrs.Abstractions;
using QuokkaDev.Cqrs.Abstractions.Exceptions;
using QuokkaDev.Cqrs.Tests.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace QuokkaDev.Cqrs.Decorators.Tests
{
    public class QueryValidationDecoratorTest
    {
        private readonly IQueryDispatcher queryDispatcher;
        private readonly DependencyInjectionContext context;

        public QueryValidationDecoratorTest()
        {
            context = new DependencyInjectionContext();

            context.RegisterMockLogger<QueryValidationDecorator>();
            context.RegisterQueryHandler("My expected response");
            context.AddQueryValidationDecorator();

            context.BuildServiceProvider();
            queryDispatcher = context.GetService<IQueryDispatcher>();
        }

        [Fact]
        public async Task QueryValidationDecorator_Should_Be_Invoked()
        {
            // Arrange
            var request = new TestQuery() { Message = "" };

            // Act
            Func<Task> dispatch = async () => await queryDispatcher.Dispatch<TestQuery, TestQueryResult>(request);

            // Assert
            await dispatch.Should()
                .ThrowAsync<QueryValidationException>()
                .Where(e => e.Errors.Count == 1 && e.Errors.First().EndsWith("Test query is invalid"));
        }
    }
}
