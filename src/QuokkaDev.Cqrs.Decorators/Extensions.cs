using Microsoft.Extensions.Logging;
using QuokkaDev.Cqrs.Abstractions.Exceptions;
using System.Reflection;

namespace QuokkaDev.Cqrs.Decorators
{
    /// <summary>
    /// Generic extensions methods
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Log all the public properties of an object
        /// </summary>
        /// <param name="logger">The logger where the extension method apply</param>
        /// <param name="obj">The object to log</param>
        public static void LogObject(this ILogger logger, object obj)
        {
            if(obj != null)
            {
                var objType = obj.GetType();
                var props = new List<PropertyInfo>(objType.GetProperties());
                foreach(var prop in props)
                {
                    object propValue = prop.GetValue(obj, null) ?? "";
                    logger.LogInformation("{Property} : {@Value}", prop.Name, propValue);
                }
            }
        }

        internal static Exception WrapException(this BaseCqrsException innerException, Type? wrapperExceptionType)
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
