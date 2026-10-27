using System;
using System.Collections.Generic;
using System.Text;
using Spectre.Console;
namespace RestaurangSystem.Classes
{
    public static class MenuHelper
    {
        public static string CreateMenu()
        {
            var accent = Color.Orange1;   // change this one line to retheme everything

            AnsiConsole.Clear();
            ShowLogo();
            AnsiConsole.Write(
                new Panel("Welcome! Pick an option below.")
                    .Header($"[{accent.ToMarkup()}]Restauraunt Manager[/]")
                    .Border(BoxBorder.Rounded)
                    .BorderColor(accent));

            return AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"[bold {accent.ToMarkup()}]What would you like to do?[/]")
                    .PageSize(10)
                    .WrapAround()
                    .HighlightStyle(new Style(foreground: accent, decoration: Decoration.Bold))
                    .AddChoices("Add Food", "Show Menu", "Create Order", "Show orders",
                                "Sort by Price", "Show Statistics", "Exit"));

        }

        public static void ShowLogo()
        {
            var accent = Color.Orange1;   // change this one line to retheme everything

            var logo = new Rows(
                new FigletText("Launch Squad").Centered().Color(accent),
                new FigletText("Restaurant").Centered().Color(Color.Red),
                new Rule("[grey]fine food since 2025[/]").RuleStyle("grey"));

            AnsiConsole.Write(
                new Panel(logo)
                    .Border(BoxBorder.Double)
                    .BorderColor(accent)
                    .Expand());
        }
    }
}
