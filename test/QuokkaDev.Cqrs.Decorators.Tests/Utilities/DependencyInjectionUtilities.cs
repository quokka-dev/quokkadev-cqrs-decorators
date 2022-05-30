using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace QuokkaDev.Cqrs.Tests.Utilities
{
    /// <summary>
    /// Utilities for configuring dependency injection
    /// </summary>
    internal static class DependencyInjectionUtilities
    {
        /// <summary>
        /// Register a Mock ILogger<typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The type of the logger</typeparam>
        /// <param name="services">The service collection where register the log</param>
        /// <returns>The original service collection. Used for chaining multiple calls</returns>
        public static IServiceCollection AddLogger<T>(this IServiceCollection services)
        {
            var loggerMock = new Mock<ILogger<T>>();
            services.AddSingleton(loggerMock.Object);
            return services;
        }
    }
}
