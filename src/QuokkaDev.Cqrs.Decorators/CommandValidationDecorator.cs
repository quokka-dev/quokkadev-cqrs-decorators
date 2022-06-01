using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QuokkaDev.Cqrs.Abstractions;
using QuokkaDev.Cqrs.Abstractions.Exceptions;

namespace QuokkaDev.Cqrs.Decorators
{
    /// <summary>
    /// A decorator for running validation on command based on FluentValidation
    /// </summary>
    public class CommandValidationDecorator : ICommandDispatcher
    {
        private readonly ICommandDispatcher dispatcher;
        private readonly ILogger<CommandValidationDecorator> logger;
        private readonly ValidationRunner validationRunner;
        private readonly IServiceProvider serviceProvider;
        private readonly CommandValidationSettings settings;

        public CommandValidationDecorator(ICommandDispatcher dispatcher, ILogger<CommandValidationDecorator> logger, ValidationRunner validationRunner, IServiceProvider serviceProvider, IOptions<CommandValidationSettings> settings)
        {
            this.dispatcher = dispatcher;
            this.logger = logger;
            this.validationRunner = validationRunner;
            this.serviceProvider = serviceProvider;
            this.settings = settings.Value;
        }

        public async Task<TCommandResult> Dispatch<TCommand, TCommandResult>(TCommand command, CancellationToken cancellation)
        {
            try
            {
                await validationRunner.RunCommandValidation<TCommand>(command, logger, serviceProvider, cancellation);
                return await dispatcher.Dispatch<TCommand, TCommandResult>(command, cancellation);
            }
            catch(BaseCqrsException bex)
            {
                throw validationRunner.ThrowException(bex, settings.CustomExceptionType);
            }
        }

        public Task<TCommandResult> Dispatch<TCommand, TCommandResult>(TCommand command)
        {
            return Dispatch<TCommand, TCommandResult>(command, CancellationToken.None);
        }
    }
}
