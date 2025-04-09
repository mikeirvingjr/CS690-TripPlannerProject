using TripPlanner.Models;
using Spectre.Console;

namespace TripPlanner;

class Program
{
    static readonly Budget _budget = new();
    
    static readonly List<Trip> _trips = [];
        
    static readonly List<TripItem> _items = [];

    static readonly string _tripFilePath = "trips.dat";

    static void SaveTrips()
    {
        SaveData data = new() { Budget = _budget.Amount, Trips = _trips };
        FileSaver.Save(_tripFilePath, data);
    }

    static void LoadTrips()
    {
        SaveData data = FileSaver.Load(_tripFilePath);

        if (data != null)
        {
            _budget.Amount = data.Budget;
            
            _trips.Clear();
            _trips.AddRange(data.Trips);
        }
    }

    static string DisplayChoices(string title, IEnumerable<string> choices, int pageSize = 10, string moreChoicesText = "Move up and down to reveal more")
    {
        var choice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(title)
                .PageSize(pageSize)
                .MoreChoicesText($"[grey]({moreChoicesText})[/]")
                .AddChoices(choices));

        return choice;
    }

    static List<string>? DisplayMultiSelectChoices(string title, IEnumerable<string> choices, int pageSize = 10, string moreChoicesText = "Move up and down to reveal more")
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

    static void EnterBudget()
    {
        if (_budget.Amount > 0M)
            AnsiConsole.WriteLine($"Budget: ${_budget.Amount}");

        var amount = AnsiConsole.Prompt(new TextPrompt<decimal>("Enter new budget: "));

        if (amount > 0M && _budget.Amount != amount)
        {
            _budget.Amount = amount;
        }
    }

    static void AddTrip()
    {
        if (_budget.Amount == 0M)
        {
            AnsiConsole.WriteLine("[red]Create a budget[/]");
            return;
        }

        Trip trip = new();
        PromptTripInfo(trip);

        if (!string.IsNullOrEmpty(trip.Name))
        {
            PromptTripItemMenu(trip);
            _trips.Add(trip);

            SaveTrips();
        }
    }

    static decimal CalculateTripCost(Trip trip)
    {
        decimal cost = 0M;

        if (trip.Items.Count > 0)
        {
            foreach (var item in trip.Items)
                cost += item.Cost;
        }

        return cost;
    }

    static void DisplayTrip(Trip trip, bool showTripItems = false)
    {
        var table = new Table();
        var cost = CalculateTripCost(trip);
        string costFormat = $"$[green]{cost}[/]";

        if (cost > _budget.Amount)
            costFormat = $"$[red]{cost}[/]";

        if (Math.Abs(_budget.Amount - cost) <= 200M)
            costFormat = $"$[yellow]{cost}[/]";

        if(showTripItems && trip.Items.Count > 0)
        {
            table.AddColumn(new TableColumn($"{trip.Name}").Centered());
            table.AddRow(costFormat);
    
            table.AddRow(trip.Destination);
            table.AddRow($"{trip.StartDate.ToShortDateString()} - {trip.EndDate.ToShortDateString()}");

            table.AddEmptyRow();
            table.AddRow(DisplayTripItems([.. trip.Items]));
        }
        else
        {
            table.AddColumn(new TableColumn(trip.Name).Centered());
            table.AddColumn(new TableColumn(new Markup(costFormat)).Centered());

            table.AddRow("Destination", trip.Destination);
            table.AddRow("Start Date", trip.StartDate.ToShortDateString());
            table.AddRow("End Date", trip.EndDate.ToShortDateString());
        }

        AnsiConsole.Write(table);
    }

    static Table DisplayTripItems(params TripItem[] items)
    {
        var table = new Table();

        table.Title("Itenerary");
        table.Border(TableBorder.None);

        if (items.Length > 0)
        {
            table.AddColumn(items[0].Name);
            table.AddColumn(items[0].Destination);
            table.AddColumn(items[0].ItemType.ToString());
            table.AddColumn(items[0].StartDate.ToShortDateString());            
            table.AddColumn($"{items[0].Duration} {items[0].DurationType.ToString().ToLowerInvariant()}");
            table.AddColumn($"${items[0].Cost}");

            foreach (var item in items.Skip(1))
                table.AddRow(item.Name, item.Destination, item.ItemType.ToString(), item.StartDate.ToShortDateString(), $"{item.Duration} {item.DurationType.ToString().ToLowerInvariant()}", $"${item.Cost}");
        }

        return table;
    }

    static void PromptTripInfo(Trip trip)
    {
        trip.Name = AnsiConsole.Prompt(new TextPrompt<string>("Name:").DefaultValue(trip.Name));
        trip.Destination = AnsiConsole.Prompt(new TextPrompt<string>("Location:").DefaultValue(trip.Destination));
        trip.StartDate = AnsiConsole.Prompt(new TextPrompt<DateTime>("Start Date [red]MM-DD-YYYY[/]:").DefaultValue(trip.StartDate));
        trip.EndDate = AnsiConsole.Prompt(new TextPrompt<DateTime>("End Date [red]MM-DD-YYYY[/]:").DefaultValue(trip.EndDate));

        DisplayTrip(trip);
    }

    static void AddItem(Trip trip)
    {
        TripItem item = new();
        PrompTripItemInfo(item);

        if (!string.IsNullOrEmpty(item.Name))
        {
            trip.Items.Add(item);
            SaveTrips();
        }
    }

    static void EditItem(Trip trip)
    {
        if (trip.Items.Count > 0)
        {
            var itemName = DisplayChoices("Select trip item to edit:", trip.Items.Select(i => i.Name));
            var item = trip.Items.FirstOrDefault(i => string.Compare(i.Name, itemName, StringComparison.OrdinalIgnoreCase) == 0);

            if (item != null)
            {
                PrompTripItemInfo(item);
                SaveTrips();
            }
            else
            {
                AnsiConsole.WriteLine("Trip item [red]does not[/] exist");
            }
        }
        else
        {
            AnsiConsole.WriteLine("This trip has [red]no items[/] to edit");
        }
    }

    static void RemoveItems(Trip trip)
    {
        if (trip.Items.Count > 0)
        {
            var names = DisplayMultiSelectChoices("Select trip item to edit:", trip.Items.Select(i => i.Name));            

            if (names != null && names.Count > 0)
            {


                foreach (var name in names)
                {
                    var item = trip.Items.FirstOrDefault(i => string.Compare(i.Name, name, StringComparison.OrdinalIgnoreCase) == 0);

                    if (item != null)
                    {
                        trip.Items.Remove(item);
                        AnsiConsole.WriteLine($"Trip item {name} [green]removed[/]");
                    }
                }
            }
            else
            {
                AnsiConsole.WriteLine("Trip items [red]were not[/] deleted");
            }
        }
        else
        {
            AnsiConsole.WriteLine("This trip has [red]no items[/] to remove");
        }
    }

    static void PromptTripItemMenu(Trip trip)
    {
        string choice = string.Empty;
        Dictionary<string, Action> tripMenu = new() {
            {"Add Item", () => AddItem(trip)},
            {"Edit Items", () => EditItem(trip)},
            {"Remove Items", () => RemoveItems(trip)},
            {"Done", () => {}}         
        };

        while (true)
        {
            Console.Clear();
            DisplayTrip(trip, true);
            choice = DisplayChoices("Trip Menu", tripMenu.Select(t => t.Key));

            if (string.Compare(choice, "Done", StringComparison.OrdinalIgnoreCase) == 0)
                break;

            if (tripMenu.TryGetValue(choice, out Action? action))
                action?.Invoke();
        }
    }

    static void PrompTripItemInfo(TripItem item)
    {
        item.Name = AnsiConsole.Prompt(new TextPrompt<string>("Name:").DefaultValue(item.Name));
        item.Destination = AnsiConsole.Prompt(new TextPrompt<string>("Location:").DefaultValue(item.Destination));
        
        var type = DisplayChoices("Type:", Enum.GetNames(typeof(TripItemType)));

        if (Enum.TryParse(type, out TripItemType itemType))
            item.ItemType = itemType;
        else
            item.ItemType = TripItemType.Excursion;
        
        item.StartDate = AnsiConsole.Prompt(new TextPrompt<DateTime>("Start Date [red]MM-DD-YYYY[/]:").DefaultValue(item.StartDate));
        item.Duration = AnsiConsole.Prompt(new TextPrompt<decimal>("Duration:").DefaultValue(item.Duration));
        
        var durType = DisplayChoices("Duration Type:", Enum.GetNames(typeof(TripItemDurationType)));

        if (Enum.TryParse(durType, out TripItemDurationType durationType))
            item.DurationType = durationType;
        else
            item.DurationType = TripItemDurationType.Minutes;
        
        item.Cost = AnsiConsole.Prompt(new TextPrompt<decimal>("Cost:").DefaultValue(item.Cost));
        DisplayTripItems(item);
    }

    static void EditTrip()
    {
        if (_budget.Amount == 0M)
        {
            AnsiConsole.WriteLine("[red]Create a budget[/]");
            return;
        }

        if (_trips.Count > 0)
        {
            var tripName = DisplayChoices("Select Trip", _trips.Select(t => t.Name));
            var trip = _trips.FirstOrDefault(t => string.Compare(t.Name, tripName, StringComparison.OrdinalIgnoreCase) == 0);

            if (trip != null)
            {
                PromptTripInfo(trip);
                PromptTripItemMenu(trip);

                SaveTrips();
            }
            else
            {
                AnsiConsole.WriteLine("Trip [red]does not[/] exist");
            }
        }
        else
        {
            AnsiConsole.WriteLine("You have [red]no trips[/] to edit");
        }
    }

    static void Main(string[] args)
    {
        LoadTrips();
        string choice = string.Empty;
        
        Dictionary<string, Action> mainMenu = new() {
            {"Allocate Budget", () => EnterBudget()},
            {"Add Trip", () => AddTrip()},
            {"Edit Trips", () => EditTrip()}, 
            {"Quit", () => {}}         
        };

        while (true)
        {
            var budget = $"Budget: $[green]{_budget.Amount}[/]";

            AnsiConsole.Clear();
            AnsiConsole.WriteLine("TripPlanner v1.0\n");
            AnsiConsole.Write(new Markup(budget));
            AnsiConsole.WriteLine();
            
            choice = DisplayChoices("Main Menu", mainMenu.Select(m => m.Key));

            if (string.Compare(choice, "Quit", StringComparison.OrdinalIgnoreCase) == 0)
                break;

            if (mainMenu.TryGetValue(choice, out Action? action))
                action?.Invoke();
        }
    }
}
