using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using QuokkaDev.Cqrs.Abstractions.Exceptions;
using System.Reflection;

namespace QuokkaDev.Cqrs.Decorators
{
    /// <summary>
    /// Helper class to centralize validation logic
    /// </summary>
    public class ValidationRunner
    {
        public async Task RunQueryValidation<TRequest>(TRequest request, ILogger logger, IServiceProvider serviceProvider, CancellationToken cancellation)
        {
            await RunValidation<TRequest, QueryValidationException>(request, logger, serviceProvider, cancellation);
        }

        public async Task RunCommandValidation<TRequest>(TRequest request, ILogger logger, IServiceProvider serviceProvider, CancellationToken cancellation)
        {
            await RunValidation<TRequest, CommandValidationException>(request, logger, serviceProvider, cancellation);
        }

        private async Task RunValidation<TRequest, TException>(TRequest request, ILogger logger, IServiceProvider serviceProvider, CancellationToken cancellation)
        where TException : BaseCqrsException
        {
            var validators = serviceProvider.GetServices<IValidator<TRequest>>();

            if(validators.Any())
            {
                logger.LogTrace("Found {validators} validators for request {command}", validators.Count(), typeof(TRequest).Name);
                var context = new ValidationContext<TRequest>(request);
                var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellation)));
                string[] failures = validationResults
                    .SelectMany(r => r.Errors)
                    .Where(f => f != null)
                    .Select(failures => $"{failures.ErrorCode} - {failures.PropertyName} - {failures.ErrorMessage}")
                    .ToArray();

                if(failures.Length != 0 && Activator.CreateInstance(typeof(TException), failures) is TException exception)
                {
                    throw exception;
                }
            }
            else
            {
                logger.LogTrace("No validators found for requst {request}", typeof(TRequest).Name);
            }
        }

        public Exception ThrowException(BaseCqrsException innerException, Type? wrapperExceptionType)
        {
            if(wrapperExceptionType is null)
            {
                return innerException;
            }
            else
            {
                Exception? custom = Activator.CreateInstance(wrapperExceptionType) as Exception;
                Type exType = typeof(Exception);
                var messageField = exType.GetField("_message", BindingFlags.NonPublic | BindingFlags.Instance);
                messageField?.SetValue(custom, innerException.Message);
                var innerExField = exType.GetField("_innerException", BindingFlags.NonPublic | BindingFlags.Instance);
                innerExField?.SetValue(custom, innerException);
                return custom!;
            }
        }
    }
}
