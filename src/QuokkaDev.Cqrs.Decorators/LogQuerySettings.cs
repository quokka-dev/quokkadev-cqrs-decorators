namespace QuokkaDev.Cqrs.Decorators
{
    /// <summary>
    /// Settings for query logging
    /// </summary>
    public class LogQuerySettings
    {
        public bool IsRequestLoggingEnabled { get; set; }
        public bool IsResponseLoggingEnabled { get; set; }
    }
}
