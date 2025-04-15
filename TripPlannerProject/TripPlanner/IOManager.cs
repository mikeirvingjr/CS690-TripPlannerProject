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

    public static string DisplayChoices(string title, IEnumerable<string> choices, int pageSize = 10, string moreChoicesText = "Move up and down to reveal more")
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(title)
                .PageSize(pageSize)
                .MoreChoicesText($"[grey]({moreChoicesText})[/]")
                .AddChoices(choices));

        return choice;
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
}