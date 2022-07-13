namespace Client.Configurations
{
    public class PollyOptions
    {
        public static string PollyOptionSection => "PollyOptions";
        public int RetryCount { get; set; }
        public int RetrySleepInSeconds { get; set; }
        public int DurationOfBreakInSeconds { get; set; }
        public int ExceptionsAllowedBeforeBreaking { get; set; }
    }
}
