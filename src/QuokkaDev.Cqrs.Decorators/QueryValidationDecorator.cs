using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QuokkaDev.Cqrs.Abstractions;
using QuokkaDev.Cqrs.Abstractions.Exceptions;

namespace QuokkaDev.Cqrs.Decorators
{
    /// <summary>
    /// A decorator for running validation on query based on FluentValidation
    /// </summary>
    public class QueryValidationDecorator : IQueryDispatcher
    {
        private readonly IQueryDispatcher dispatcher;
        private readonly ILogger<QueryValidationDecorator> logger;
        private readonly ValidationRunner validationRunner;
        private readonly IServiceProvider serviceProvider;
        private readonly QueryValidationSettings settings;

        public QueryValidationDecorator(IQueryDispatcher dispatcher, ILogger<QueryValidationDecorator> logger, ValidationRunner validationRunner, IServiceProvider serviceProvider, IOptions<QueryValidationSettings> settings)
        {
            this.dispatcher = dispatcher;
            this.logger = logger;
            this.validationRunner = validationRunner;
            this.serviceProvider = serviceProvider;
            this.settings = settings.Value;
        }

        public async Task<TQueryResult> Dispatch<TQuery, TQueryResult>(TQuery query, CancellationToken cancellation)
        {
            try
            {
                await validationRunner.RunQueryValidation<TQuery>(query, logger, serviceProvider, cancellation);
                return await dispatcher.Dispatch<TQuery, TQueryResult>(query, cancellation);
            }
            catch(BaseCqrsException bex)
            {
                throw bex.WrapException(settings.CustomExceptionType);
            }
        }

        public Task<TQueryResult> Dispatch<TQuery, TQueryResult>(TQuery query)
        {
            return Dispatch<TQuery, TQueryResult>(query, CancellationToken.None);
        }
    }
}
