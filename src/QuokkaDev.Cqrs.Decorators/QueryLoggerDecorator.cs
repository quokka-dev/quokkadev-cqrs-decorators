using QuokkaDev.Cqrs.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace QuokkaDev.Cqrs.Decorators
{
    /// <summary>
    /// A decorator for logging Query request and response
    /// </summary>
    public class QueryLoggerDecorator : IQueryDispatcher
    {
        private readonly IQueryDispatcher dispatcher;
        private readonly ILogger<QueryLoggerDecorator> logger;
        private readonly LogQuerySettings settings;

        public QueryLoggerDecorator(IQueryDispatcher dispatcher, ILogger<QueryLoggerDecorator> logger, IOptions<LogQuerySettings> settings)
        {
            this.dispatcher = dispatcher;
            this.logger = logger;
            this.settings = settings.Value;
        }

        public Task<TQueryResult> Dispatch<TQuery, TQueryResult>(TQuery query, CancellationToken cancellation)
        {
            if(query == null) {
                throw new ArgumentException("Query is null");
            }
            return DispatchInternal<TQuery, TQueryResult>(query, cancellation);
        }

        public Task<TQueryResult> Dispatch<TQuery, TQueryResult>(TQuery query)
        {
            return Dispatch<TQuery, TQueryResult>(query, CancellationToken.None);
        }

        private async Task<TQueryResult> DispatchInternal<TQuery, TQueryResult>(TQuery query, CancellationToken cancellation)
        {
            logger.LogInformation("Handling {queryName}", typeof(TQuery).Name);
            if(settings.IsRequestLoggingEnabled && query != null) {
                logger.LogObject(query);
            }

            var response = await dispatcher.Dispatch<TQuery, TQueryResult>(query, cancellation);

            logger.LogInformation("Handled {queryResult}", typeof(TQueryResult).Name);
            if(settings.IsResponseLoggingEnabled && response != null) {
                logger.LogObject(response);
            }

            return response;
        }
    }
}
