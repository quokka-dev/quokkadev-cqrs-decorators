using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using QuokkaDev.Cqrs.Abstractions;

namespace QuokkaDev.Cqrs.Decorators
{
    /// <summary>
    /// Extensions methods for managing decorators
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        #region Query logging

        public static QueryDispatcherDecoratorBuilder AddQueryLogging(this IServiceCollection services, Action<LogQuerySettings>? configureOptions = null)
        {
            services.Configure<LogQuerySettings>(configureOptions);
            services.Decorate<IQueryDispatcher, QueryLoggerDecorator>();

            return new QueryDispatcherDecoratorBuilder(services);
        }

        public static QueryDispatcherDecoratorBuilder AddQueryValidation(this IServiceCollection services, Action<QueryValidationSettings>? configureOptions = null)
        {
            services.AddSingleton<ValidationRunner>();
            services.Configure<QueryValidationSettings>(configureOptions);
            services.Decorate<IQueryDispatcher>((inner, provider) =>
                new QueryValidationDecorator(
                    inner,
                    provider.GetRequiredService<ILogger<QueryValidationDecorator>>(),
                    provider.GetRequiredService<ValidationRunner>(),
                    provider.GetRequiredService<IServiceProvider>(),
                    provider.GetRequiredService<IOptions<QueryValidationSettings>>()
                )
            );

            return new QueryDispatcherDecoratorBuilder(services);
        }

        public static QueryDispatcherDecoratorBuilder AndQueryLogging(this QueryDispatcherDecoratorBuilder builder, Action<LogQuerySettings>? configureOptions = null)
        {
            builder.Services.Configure<LogQuerySettings>(configureOptions);

            builder.Services.Decorate<IQueryDispatcher>((inner, provider) =>
                new QueryLoggerDecorator(
                    inner,
                    provider.GetRequiredService<ILogger<QueryLoggerDecorator>>(),
                    provider.GetRequiredService<IOptions<LogQuerySettings>>()
                )
            );

            return new QueryDispatcherDecoratorBuilder(builder.Services);
        }

        public static QueryDispatcherDecoratorBuilder AndQueryValidation(this QueryDispatcherDecoratorBuilder builder, Action<QueryValidationSettings>? configureOptions = null)
        {
            builder.Services.AddSingleton<ValidationRunner>();
            builder.Services.Configure<QueryValidationSettings>(configureOptions);
            builder.Services.Decorate<IQueryDispatcher>((inner, provider) =>
                new QueryValidationDecorator(
                    inner,
                    provider.GetRequiredService<ILogger<QueryValidationDecorator>>(),
                    provider.GetRequiredService<ValidationRunner>(),
                    provider.GetRequiredService<IServiceProvider>(),
                    provider.GetRequiredService<IOptions<QueryValidationSettings>>()
                )
            );

            return new QueryDispatcherDecoratorBuilder(builder.Services);
        }

        #endregion

        #region Command logging

        public static CommandDispatcherDecoratorBuilder AddCommandLogging(this IServiceCollection services, Action<LogCommandSettings>? configureOptions = null)
        {
            services.Configure<LogCommandSettings>(configureOptions);
            services.Decorate<ICommandDispatcher, CommandLoggerDecorator>();

            return new CommandDispatcherDecoratorBuilder(services);
        }

        public static CommandDispatcherDecoratorBuilder AndCommandLogging(this CommandDispatcherDecoratorBuilder builder, Action<LogCommandSettings>? configureOptions = null)
        {
            builder.Services.Configure<LogCommandSettings>(configureOptions);

            builder.Services.Decorate<ICommandDispatcher>((inner, provider) =>
                new CommandLoggerDecorator(
                    inner,
                    provider.GetRequiredService<ILogger<CommandLoggerDecorator>>(),
                    provider.GetRequiredService<IOptions<LogCommandSettings>>()
                )
            );

            return new CommandDispatcherDecoratorBuilder(builder.Services);
        }

        public static CommandDispatcherDecoratorBuilder AddCommandValidation(this IServiceCollection services, Action<CommandValidationSettings>? configureOptions = null)
        {
            services.AddSingleton<ValidationRunner>();
            services.Configure<CommandValidationSettings>(configureOptions);
            services.Decorate<ICommandDispatcher>((inner, provider) =>
                new CommandValidationDecorator(
                    inner,
                    provider.GetRequiredService<ILogger<CommandValidationDecorator>>(),
                    provider.GetRequiredService<ValidationRunner>(),
                    provider.GetRequiredService<IServiceProvider>(),
                    provider.GetRequiredService<IOptions<CommandValidationSettings>>()
                )
            );

            return new CommandDispatcherDecoratorBuilder(services);
        }

        public static CommandDispatcherDecoratorBuilder AndCommandValidation(this CommandDispatcherDecoratorBuilder builder, Action<CommandValidationSettings>? configureOptions = null)
        {
            builder.Services.AddSingleton<ValidationRunner>();
            builder.Services.Configure<CommandValidationSettings>(configureOptions);
            builder.Services.Decorate<ICommandDispatcher>((inner, provider) =>
                new CommandValidationDecorator(
                    inner,
                    provider.GetRequiredService<ILogger<CommandValidationDecorator>>(),
                    provider.GetRequiredService<ValidationRunner>(),
                    provider.GetRequiredService<IServiceProvider>(),
                    provider.GetRequiredService<IOptions<CommandValidationSettings>>()
                )
            );

            return new CommandDispatcherDecoratorBuilder(builder.Services);
        }

        #endregion
    }

    public class QueryDispatcherDecoratorBuilder
    {
        public QueryDispatcherDecoratorBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }

    public class CommandDispatcherDecoratorBuilder
    {
        public CommandDispatcherDecoratorBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
