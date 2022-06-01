using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QuokkaDev.Cqrs.Abstractions;

namespace QuokkaDev.Cqrs.Decorators
{
    /// <summary>
    /// A decorator for logging Command request and response
    /// </summary>
    public class CommandLoggerDecorator : ICommandDispatcher
    {
        private readonly ICommandDispatcher dispatcher;
        private readonly ILogger<CommandLoggerDecorator> logger;
        private readonly LogCommandSettings settings;

        public CommandLoggerDecorator(ICommandDispatcher dispatcher, ILogger<CommandLoggerDecorator> logger, IOptions<LogCommandSettings> settings)
        {
            this.dispatcher = dispatcher;
            this.logger = logger;
            this.settings = settings.Value;
        }

        public Task<TCommandResult> Dispatch<TCommand, TCommandResult>(TCommand command, CancellationToken cancellation)
        {
            if(command == null)
            {
                throw new ArgumentException("Command is null");
            }
            return DispatchInternal<TCommand, TCommandResult>(command, cancellation);
        }

        public Task<TCommandResult> Dispatch<TCommand, TCommandResult>(TCommand command)
        {
            return Dispatch<TCommand, TCommandResult>(command, CancellationToken.None);
        }

        private async Task<TCommandResult> DispatchInternal<TCommand, TCommandResult>(TCommand command, CancellationToken cancellation)
        {
            logger.LogInformation("Handling {commandName}", typeof(TCommand).Name);
            if(settings.IsRequestLoggingEnabled && command != null)
            {
                logger.LogObject(command);
            }

            var response = await dispatcher.Dispatch<TCommand, TCommandResult>(command, cancellation);

            logger.LogInformation("Handled {commandResult}", typeof(TCommandResult).Name);
            if(settings.IsResponseLoggingEnabled && response != null)
            {
                logger.LogObject(response);
            }

            return response;
        }
    }
}
