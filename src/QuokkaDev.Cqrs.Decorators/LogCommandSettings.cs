namespace QuokkaDev.Cqrs.Decorators
{
    /// <summary>
    /// Settings for command logging
    /// </summary>
    public class LogCommandSettings
    {
        public bool IsRequestLoggingEnabled { get; set; }
        public bool IsResponseLoggingEnabled { get; set; }
    }
}
