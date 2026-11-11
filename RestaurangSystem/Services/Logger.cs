using System.Text.Json;
using System.Text.Json.Serialization;
namespace RestaurangSystem.Services
{
    // Nivåerna är en enum så att vi slipper skriva dem som text och stava fel
    public enum LogLevel { Info, Warning, Error }

    // Så här ser en rad i loggen ut: tid, nivå, meddelande och eventuellt felet
    public record LogEntry(DateTime Time, LogLevel Level, string Message, string? Exception);

    public static class Logger
    {
        // Loggfilen hamnar bredvid programmet (i bin-mappen)
        private static readonly string FilePath =
            Path.Combine(AppContext.BaseDirectory, "log.json");

        // Låset gör att två anrop inte kan skriva till filen samtidigt
        private static readonly object Lock = new();

        private static readonly JsonSerializerOptions Options = new()
        {
            WriteIndented = true,                           // snyggt formaterad JSON
            Converters = { new JsonStringEnumConverter() }  // skriver "Warning" istället för en siffra
        };

        public static void Log(LogLevel level, string message, Exception? ex = null)
        {
            try
            {
                lock (Lock)
                {
                    // Läs in gamla loggar, lägg till den nya och skriv hela listan igen
                    var entries = ReadAll();
                    entries.Add(new LogEntry(DateTime.Now, level, message, ex?.ToString()));
                    File.WriteAllText(FilePath, JsonSerializer.Serialize(entries, Options));
                }
            }
            catch (Exception ex2)
            {
                // Loggningen får aldrig krascha programmet. Vid fel hoppar vi över
                // raden och skriver bara en notering som syns när man felsöker.
                System.Diagnostics.Debug.WriteLine($"Logger misslyckades: {ex2.Message}");
            }
        }

        public static List<LogEntry> ReadAll()
        {
            try
            {
                // Finns ingen fil än börjar vi med en tom lista
                if (!File.Exists(FilePath)) return new List<LogEntry>();

                return JsonSerializer.Deserialize<List<LogEntry>>(File.ReadAllText(FilePath), Options)
                       ?? new List<LogEntry>();
            }
            catch (JsonException)
            {
                // Filen är trasig (t.ex. om programmet stängdes mitt i en skrivning),
                // så vi börjar om med en tom logg
                return new List<LogEntry>();
            }
        }

        // Genvägar så att man kan skriva Logger.Info("...") istället för Logger.Log(...)
        public static void Info(string message) => Log(LogLevel.Info, message);
        public static void Warning(string message) => Log(LogLevel.Warning, message);
        public static void Error(string message, Exception? ex = null) => Log(LogLevel.Error, message, ex);
    }
}