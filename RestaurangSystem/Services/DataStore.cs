using RestaurangSystem.UI;
using Spectre.Console;
using System.Text.Json;
namespace RestaurangSystem.Services
{
    // DataStore är en generisk klass som kan hantera olika typer av objekt.
    // <T> innebär att klassen kan användas för exempelvis Staff, Order och MenuItem.
    //
    // where T : IIdentify innebär att typen T måste implementera
    // interfacet IIdentify. Det krävs eftersom alla objekt behöver ha ett Id. 
    public class DataStore<T> where T : IIdentify
    {
        // Skapar en lista som lagrar alla objekt av typen T.
        // Listan används för att lägga till, ta bort och söka efter objekt.
        public List<T> items = new List<T>();
        // Skapar en sökväg till JSON-filen baserat på vilken typ av objekt
        // som DataStore hanterar.
        // typeof(T).Name hämtar namnet på typen.
        // Exempelvis skapas Staff.json för Staff och MenuItem.json för MenuItem.
        // static innebär att variabeln tillhör klassen och readonly
        // innebär att värdet inte kan ändras efter att det har initierats.
        private static readonly string FilePath = $"{typeof(T).Name}.json";

        // METOD: AddItem
        // Lägger till ett nytt objekt i listan och tilldelar det ett unikt ID.
        public void AddItem(T item)
        {
            // Hämtar alla befintliga ID:n i listan.
            // Select(i => i.Id) väljer ID-egenskapen från varje objekt.
            // DefaultIfEmpty(0) innebär att värdet 0 används om listan är tom.
            // Max() hittar det högsta befintliga ID-numret.
            // + 1 gör att det nya objektet får nästa lediga ID-nummer.
            item.Id = items.Select(i => i.Id).DefaultIfEmpty(0).Max() + 1;
            // Lägger till det nya objektet i listan.
            // Objektet har nu fått ett eget ID.
            items.Add(item);

        }
        // METOD: RemoveItem
        // Tar bort ett specifikt objekt från listan.
        public void RemoveItem(T item)
        {
            items.Remove(item);
        }
        // METOD: FindItem
        // Söker efter ett objekt som uppfyller ett visst villkor.
        // Func<T, bool> är en funktion som tar emot ett objekt av typen T
        // och returnerar true eller false beroende på om villkoret uppfylls.
        //
        // T? innebär att metoden kan returnera null om inget objekt hittas.
        public T? FindItem(Func<T, bool> predicate)
        {
            try
            {                // FirstOrDefault söker igenom listan och returnerar
                             // det första objektet som uppfyller villkoret.
                             // Om inget objekt hittas returneras standardvärdet,
                             // vilket är null för referenstyper.

                return items.FirstOrDefault(predicate);
            }
            catch (Exception ex)
            {
                // Om ett fel uppstår under sökningen visas ett felmeddelande.
                // ex.Message innehåller information om vad felet beror på.
                // Returnerar standardvärdet om ett fel uppstår.
                MenuHelper.ShowError("Error while searching");
                Logger.Error($"Error while searching for item: {ex.Message}", ex);
                return default;
            }
        }
        // Skriver ut alla objekt i listan tillsammans med deras Id
        public void GetIds()
        {
            // foreach går igenom alla objekt som finns i listan
            foreach (var item in items)
            {
                // Skriver ut objektets typ och dess ID.
                // GetType().Name hämtar klassens namn.
                // item.Id hämtar objektets unika ID.
                // Färgerna hanteras med Spectre.Console.
                AnsiConsole.MarkupLine($"[green]Item: {item.GetType().Name} Id: {item.Id}[/]");
            }
        }
        // METOD: SaveToJson
        // Sparar alla objekt i listan till en JSON-fil.
        public void SaveToJson()
        {
            try
            {
                // Omvandlar listan med objekt till JSON-format.
                // Serialize används för att göra om C#-objekten till JSON-text.
                // WriteIndented = true gör JSON-filen mer lättläst
                // genom att lägga till indrag och radbrytningar.
                string json = JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });
                // Skriver JSON-informationen till filen.
                // Om filen redan finns skrivs dess innehåll över.
                // Om filen inte finns skapas en ny fil.
                File.WriteAllText(FilePath, json);
                // Visar ett meddelande som bekräftar att informationen sparats.
                AnsiConsole.MarkupLine($"[green]Data saved to {FilePath}[/]");
            }
            // Fångar ett fel som uppstår om programmet saknar behörighet
            // att skriva till filen.
            catch (UnauthorizedAccessException)
            {
                MenuHelper.ShowError("You do not have permission to access this file.");
                Logger.Error($"UnauthorizedAccessException while saving to {FilePath}");
            }
            // Fångar fel som uppstår vid filhantering,
            // exempelvis om filen inte kan skrivas.
            catch (IOException ex)
            {
                MenuHelper.ShowError("Could not save the file.");
                Logger.Error($"IOException while saving to {FilePath}", ex);
            }
            // Fångar andra oväntade fel som inte hanteras av
            // de tidigare catch-blocken.
            catch (Exception ex)
            {
                Logger.Error($"Unexpected error while saving to {FilePath}", ex);
                MenuHelper.ShowError("An unexpected error occurred while saving.");
            }
        }
        // METOD: LoadFromJson
        // Läser in information från JSON-filen och sparar den i listan.
        public void LoadFromJson()
        {
            try
            {
                // Läser hela innehållet från JSON-filen
                // och sparar texten i variabeln json. 
                string json = File.ReadAllText(FilePath);
                // Omvandlar JSON-texten till en lista med objekt av typen T.
                // Deserialize används för att omvandla JSON-text till C#-objekt.
                //
                // Om resultatet blir null skapas istället en tom lista.
                // Detta förhindrar att items får värdet null.
                items = JsonSerializer.Deserialize<List<T>>(json) ?? new List<T>();
                // Visar hur många objekt som har lästs in från JSON-filen.
                AnsiConsole.MarkupLine($"[green]Loaded {items.Count} objects from {FilePath}[/]");
            }
            // Hanterar situationen när JSON-filen inte finns.
            // Programmet visar ett meddelande om att ingen sparad data hittades.
            catch (FileNotFoundException)
            {
                Logger.Warning($"No saved data found for {FilePath}");
                MenuHelper.ShowError("No saved data found, starting with an empty list.");
            }
            // Hanterar fel om JSON-filen är skadad eller innehåller
            // information som inte kan omvandlas till rätt objekt.
            catch (JsonException)
            {
                // Skapar en ny tom lista eftersom informationen inte kunde läsas.
                Logger.Warning($"JsonException while loading from {FilePath}");
                MenuHelper.ShowError("The file is corrupted or has an invalid format. Starting with an empty list.");
                items = new List<T>();
            }
            // Hanterar andra filrelaterade problem,
            // exempelvis om programmet inte kan läsa filen.
            catch (IOException ex)
            {
                Logger.Error($"IOException while loading from {FilePath}", ex);
                MenuHelper.ShowError("Could not read the file.");
            }
            // Fångar övriga oväntade fel som kan uppstå vid inläsningen.
            catch (Exception ex)
            {
                Logger.Error($"Unexpected error while loading from {FilePath}", ex);
                MenuHelper.ShowError("An unexpected error occurred while loading.");
            }
        }
    }
}
