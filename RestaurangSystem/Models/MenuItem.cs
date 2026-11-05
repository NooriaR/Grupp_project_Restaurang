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
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Content { get; set; }
        public bool IsVegetarian { get; set; }
        public Category Category { get; set; }

        //methods 


        public static MenuItem CreateMenuItem()
        {

            var name = AnsiConsole.Prompt(  // Prompt for the name of the menu item, fails validation if the name is empty or whitespace
                new TextPrompt<string>("Food [bold]name[/]:")
                    .Validate(n => !string.IsNullOrWhiteSpace(n)
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Name can't be empty[/]")));

            var price = AnsiConsole.Prompt( // Prompt for the price of the menu item, fails validation if the price is less than or equal to 0 or if it is not a valid number
                new TextPrompt<decimal>("[bold]Price[/]:")
                    .Validate(p => p > 0
                        ? ValidationResult.Success()
                        : ValidationResult.Error("[red]Price must be above 0[/]")));

            var content = AnsiConsole.Prompt( // Prompt for the content of the menu item, allows empty input
                new TextPrompt<string>("[bold]Content[/] (ingredients):")
                    .AllowEmpty());

            var isVegetarian = AnsiConsole.Confirm("Is it [bold]vegetarian[/]?", false); // Prompt for whether the menu item is vegetarian, defaults to false

            var category = AnsiConsole.Prompt( // Prompt for the category of the menu item, allows selection from the Category enum
                new SelectionPrompt<Category>()
                    .Title("Pick a [bold]category[/]:")
                    .AddChoices(Enum.GetValues<Category>()));

            return new MenuItem // Create a new MenuItem object with the provided values    
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
