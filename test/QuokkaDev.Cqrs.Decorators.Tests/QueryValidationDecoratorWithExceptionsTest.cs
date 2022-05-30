using FluentAssertions;
using QuokkaDev.Cqrs.Abstractions;
using QuokkaDev.Cqrs.Abstractions.Exceptions;
using QuokkaDev.Cqrs.Tests.Utilities;
using System;
using System.Threading.Tasks;
using Xunit;

namespace QuokkaDev.Cqrs.Decorators.Tests
{
    public class QueryValidationDecoratorWithExceptionsTest
    {
        private readonly IQueryDispatcher queryDispatcher;
        private readonly DependencyInjectionContext context;

        public QueryValidationDecoratorWithExceptionsTest()
        {
            context = new DependencyInjectionContext();

            context.RegisterMockLogger<QueryValidationDecorator>();
            context.RegisterQueryHandler("My expected response");
            context.AddQueryValidationDecorator(typeof(MyCustomException));

            context.BuildServiceProvider();
            queryDispatcher = context.GetService<IQueryDispatcher>();
        }

        [Fact]
        public async Task QueryValidationDecorator_Should_Be_Invoked_With_Custom_Exception()
        {
            // Arrange
            var request = new TestQuery() { Message = "" };

            // Act
            Func<Task> dispatch = async () => await queryDispatcher.Dispatch<TestQuery, TestQueryResult>(request);

            // Assert
            await dispatch.Should()
                .ThrowAsync<MyCustomException>()
                .WithInnerException(typeof(QueryValidationException));
        }
    }
}
