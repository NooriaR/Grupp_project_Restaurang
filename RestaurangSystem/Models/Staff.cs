using Spectre.Console;
using System.Text.Json.Serialization;

namespace RestaurangSystem.Models
{
    public class Staff : Users
    {
        // Egenskaper (Attributes)

        // Lagrar personalens lösenord.
        public string Password { get; set; }

        [JsonIgnore]
        public bool IsLoggedIn { get; set; }

        // Kopplar ett arbetsschema till den anställda.
        // Varje anställd får ett eget tomt arbetsschema.
        public WorkSchedule Schedule { get; set; } = new WorkSchedule();

        //Metoder
        public void WorkingSchedule()
        {
            // Kontrollerar om personalen är inloggad.
            if (!IsLoggedIn)
            {
                AnsiConsole.MarkupLine("[red]Du måste vara inloggad för att se arbetsschemat.[/]");
                return;
            }

            // Kontrollerar om det finns några arbetspass.
            if (Schedule.WorkDays.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]Inget arbetsschema finns tillgängligt.[/]");
                return;
            }

            // Skapar en tabell för arbetsschemat.
            var table = new Table();

            // Lägger till rubriker i tabellen.
            table.AddColumn("Dag");
            table.AddColumn("Arbetstid");

            // Går igenom alla arbetspass i schemat.
            foreach (string day in Schedule.WorkDays)
            {
                // Delar upp veckodag och arbetstid.
                string[] scheduleInfo = day.Split(' ', 2);

                // Lägger till arbetspasset i tabellen.
                table.AddRow(scheduleInfo[0], scheduleInfo[1]);
            }

            // Visar arbetsschemat i terminalen.
            AnsiConsole.Write(table);
        }
    }
}
