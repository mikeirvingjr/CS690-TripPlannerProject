using System.IO.Compression;
using Spectre.Console;

namespace TripPlanner;

public static class IOManager
{
    public static T AskQuestion<T>(string message, T defaultValue)
    {
        if (!EqualityComparer<T>.Default.Equals(defaultValue, default(T)) && !string.IsNullOrEmpty(defaultValue?.ToString()))
            return AnsiConsole.Prompt(new TextPrompt<T>(message).DefaultValue(defaultValue));

        return AnsiConsole.Prompt(new TextPrompt<T>(message));
    }

    public static void DisplayMessage(string message)
    {
        AnsiConsole.Write(message);
    }

    public static string DisplayChoices<T>(string title, IEnumerable<string> choices, T defaultValue, int pageSize = 10, string moreChoicesText = "Move up and down to reveal more")
    {
        var prompt = new SelectionPrompt<string>()
                .Title(title)
                .PageSize(pageSize)
                .MoreChoicesText($"[grey]({moreChoicesText})[/]")
                .AddChoices(choices);

        if (!EqualityComparer<T>.Default.Equals(defaultValue, default(T)) && !string.IsNullOrEmpty(defaultValue?.ToString()))
        {   
            List<string> orderedChoices = [.. choices];

            orderedChoices.Remove(defaultValue.ToString() ?? string.Empty);
            orderedChoices.Insert(0, defaultValue.ToString() ?? string.Empty);

            return AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title(title)
                .PageSize(pageSize)
                .MoreChoicesText($"[grey]({moreChoicesText})[/]")
                .AddChoices(orderedChoices)
            );
        }

        return AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title(title)
                .PageSize(pageSize)
                .MoreChoicesText($"[grey]({moreChoicesText})[/]")
                .AddChoices(choices)
        );
    }

    public static List<string>? DisplayMultiSelectChoices(string title, IEnumerable<string> choices, int pageSize = 10, string moreChoicesText = "Move up and down to reveal more")
    {
        var selected = AnsiConsole.Prompt(
                new MultiSelectionPrompt<string>()
                    .Title(title)
                    .PageSize(pageSize)
                    .MoreChoicesText($"[grey]{moreChoicesText}[/]")
                    .InstructionsText(
                        "[grey](Press [blue]<space>[/] to toggle a choice, " + 
                        "[green]<enter>[/] to accept)[/]")
                    .AddChoices(choices));

        return selected;
    }

    public static void WaitForInput(string message)
    {
        string choice = IOManager.DisplayChoices(string.Empty, [message], string.Empty);
    }
}