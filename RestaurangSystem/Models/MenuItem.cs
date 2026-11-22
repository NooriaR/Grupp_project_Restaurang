using RestaurangSystem.Services;
using Spectre.Console;
namespace RestaurangSystem.Models
{
    public enum Category
    {
        Appetizer,
        MainCourse,
        Dessert,
        Drink
    }

    public class MenuItem : IIdentify
    {
        //Attribut
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Content { get; set; }
        public bool IsVegetarian { get; set; }
        public Category Category { get; set; }

        //Metoder
        public static MenuItem CreateMenuItem()
        {
            //Frågar efter maträttens namn
            var name = AnsiConsole.Prompt(
                new TextPrompt<string>("Food [bold]name[/]:")
                    .Validate(n => !string.IsNullOrWhiteSpace(n)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Name can't be empty[/]")));
            // Frågar efter priset på maträtten
            var price = AnsiConsole.Prompt(
                new TextPrompt<decimal>("[bold]Price[/]:")
                    .Validate(p => p > 0
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Price must be above 0[/]")));

            var content = AnsiConsole.Prompt( // Frågar efter ingredienser eller innehåll
                new TextPrompt<string>("[bold]Content[/] (ingredients):")
                    .AllowEmpty());

            var isVegetarian = AnsiConsole.Confirm("Is it [bold]vegetarian[/]?", false); // Frågar om maträtten är vegetarisk

            var category = AnsiConsole.Prompt( // Låter användaren välja kategori
                new SelectionPrompt<Category>()
                    .Title("Pick a [bold]category[/]:")
                    .AddChoices(Enum.GetValues<Category>()));

            return new MenuItem // Skapar en ny maträtt    
            {
                Name = name,
                Price = price,
                Content = content,
                IsVegetarian = isVegetarian,
                Category = category
            };
        }
    }
}
