using TripPlanner.Models;
using Spectre.Console;

namespace TripPlanner;

class Program
{
    private readonly static TripManager _tripManager = new(); 

    static void EnterBudget()
    {
        var budget = _tripManager.Budget;

        if (budget > 0M)
            IOManager.DisplayMessage($"Budget: ${budget}");

        var amount = IOManager.AskQuestion("Enter new budget: ", budget);
        _tripManager.Budget = budget;
    }

    static void AddTrip()
    {
        if (_tripManager.Budget == 0M)
        {
            IOManager.DisplayMessage("[red]Create a budget[/]");
            return;
        }

        Trip trip = new();
        PromptTripInfo(trip);

        if (!string.IsNullOrEmpty(trip.Name))
        {
            PromptTripItemMenu(trip);
            _tripManager.AddTrip(trip);
            _tripManager.SaveTrips();
        }
    }

    static void DisplayTrip(Trip trip, bool showTripItems = false)
    {
        var table = new Table();
        var cost = TripManager.CalculateTripCost(trip);
        string costFormat = $"$[green]{cost}[/]";
        var budget = _tripManager.Budget;

        if (cost > budget)
            costFormat = $"$[red]{cost}[/]";

        if (Math.Abs(budget - cost) <= 200M)
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
        trip.Name = IOManager.AskQuestion("Name:", trip.Name);
        trip.Destination = IOManager.AskQuestion("Location:", trip.Destination);
        trip.StartDate = IOManager.AskQuestion("Start Date [red]MM-DD-YYYY[/]:", trip.StartDate);
        trip.EndDate = IOManager.AskQuestion("End Date [red]MM-DD-YYYY[/]:", trip.EndDate);

        DisplayTrip(trip);
    }

    static void AddItem(Trip trip)
    {
        TripItem item = new();
        PrompTripItemInfo(item);

        if (!string.IsNullOrEmpty(item.Name))
        {
            trip.Items.Add(item);
            _tripManager.SaveTrips();
        }
    }

    static void EditItem(Trip trip)
    {
        if (trip.Items.Count > 0)
        {
            var tripItemList = trip.Items.Select(i => i.Name).ToList();
            tripItemList.Add("Cancel");

            var itemName = IOManager.DisplayChoices("Select trip item to edit:", tripItemList);

            if (string.Compare(itemName, "Cancel", StringComparison.OrdinalIgnoreCase) == 0)
                return;
            
            var item = trip.Items.FirstOrDefault(i => string.Compare(i.Name, itemName, StringComparison.OrdinalIgnoreCase) == 0);

            if (item != null)
            {
                PrompTripItemInfo(item);
                _tripManager.SaveTrips();
            }
            else
            {
                IOManager.DisplayMessage("Trip item [red]does not[/] exist");
            }
        }
        else
        {
            IOManager.DisplayMessage("This trip has [red]no items[/] to edit");
        }
    }

    static void RemoveItems(Trip trip)
    {
        if (trip.Items.Count > 0)
        {
            var tripItemList = trip.Items.Select(i => i.Name).ToList();
            tripItemList.Add("Cancel");

            var names = IOManager.DisplayMultiSelectChoices("Select trip item to edit:", tripItemList);

            if (names != null && names.Count > 0)
            {
                if (names.Any(n => string.Compare(n, "Cancel", StringComparison.OrdinalIgnoreCase) == 0))
                    return;

                foreach (var name in names)
                {
                    var item = trip.Items.FirstOrDefault(i => string.Compare(i.Name, name, StringComparison.OrdinalIgnoreCase) == 0);

                    if (item != null)
                    {
                        trip.Items.Remove(item);
                        IOManager.DisplayMessage($"Trip item {name} [green]removed[/]");
                    }
                }
            }
            else
            {
                IOManager.DisplayMessage("Trip items [red]were not[/] deleted");
            }
        }
        else
        {
            IOManager.DisplayMessage("This trip has [red]no items[/] to remove");
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
            choice = IOManager.DisplayChoices("Trip Menu", tripMenu.Select(t => t.Key));

            if (string.Compare(choice, "Done", StringComparison.OrdinalIgnoreCase) == 0)
                break;

            if (tripMenu.TryGetValue(choice, out Action? action))
                action?.Invoke();
        }
    }

    static void PrompTripItemInfo(TripItem item)
    {
        item.Name = IOManager.AskQuestion("Name:", item.Name);
        item.Destination = IOManager.AskQuestion("Location:", item.Destination);
        
        var type = IOManager.DisplayChoices("Type:", Enum.GetNames(typeof(TripItemType)));

        if (Enum.TryParse(type, out TripItemType itemType))
            item.ItemType = itemType;
        else
            item.ItemType = TripItemType.Excursion;
        
        item.StartDate = IOManager.AskQuestion("Start Date [red]MM-DD-YYYY[/]:", item.StartDate);
        item.Duration = IOManager.AskQuestion("Duration:", item.Duration);
        
        var durType = IOManager.DisplayChoices("Duration Type:", Enum.GetNames(typeof(TripItemDurationType)));

        if (Enum.TryParse(durType, out TripItemDurationType durationType))
            item.DurationType = durationType;
        else
            item.DurationType = TripItemDurationType.Minutes;
        
        item.Cost = IOManager.AskQuestion("Cost:", item.Cost);
        DisplayTripItems(item);
    }

    static void AnalyzeTrip()
    {
        if (_tripManager.Budget == 0M)
        {
            AnsiConsole.WriteLine("[red]Create a budget[/]");
            return;
        }

        if (_tripManager.Trips.Count > 0)
        {
            var tripList = _tripManager.Trips.Select(t => t.Name).ToList();
            tripList.Add("Cancel");

            var tripName = IOManager.DisplayChoices("Select Trip", tripList);

            if (string.Compare(tripName, "Cancel", StringComparison.OrdinalIgnoreCase) == 0)
                return;

            var trip = _tripManager.GetTrip(tripName); 

            if (trip != null)
            {
                AnalyzeTrip(trip);
            }
            else
            {
                IOManager.DisplayMessage("Trip [red]does not[/] exist");
            }
        }
        else
        {
            IOManager.DisplayMessage("You have [red]no trips[/] to analyze");
        }
    }

    static void AnalyzeTrip(Trip trip)
    {
        var data = TripAnalyzer.AnalyzeTrip(trip, _tripManager.Budget);

        AnsiConsole.Clear();
        AnsiConsole.Write(new Rows(new Text(data.Name).Centered()));

        string costFormat = $"$[green]{data.TotalCost}[/]";
        var budget = _tripManager.Budget;

        if (data.TotalCost > budget)
            costFormat = $"$[red]{data.TotalCost}[/]";

        if (Math.Abs(budget - data.TotalCost) <= 200M)
            costFormat = $"$[yellow]{data.TotalCost}[/]";

        costFormat = $"using {Math.Round(data.Percentage, 2)}% of {costFormat}";
        AnsiConsole.Write(new Rows(new Markup(costFormat).Centered()));

        var chart = new BarChart()
                        .Width(60)
                        .Label("[green bold underline]Itenerary[/]")
                        .CenterLabel();

        foreach (var item in data.Items)
        {
            Color color = item.BudgetPercentage > 70M ? Color.Red : item.BudgetPercentage > 50M ? Color.Yellow : Color.Green;
            chart.AddItem(item.Name, (double)Math.Round(item.BudgetPercentage, 2), color);
        }

        AnsiConsole.Write(chart);
        string choice = IOManager.DisplayChoices(string.Empty, ["Close"]);
    }

    static void EditTrip()
    {
        if (_tripManager.Budget == 0M)
        {
            AnsiConsole.WriteLine("[red]Create a budget[/]");
            return;
        }

        if (_tripManager.Trips.Count > 0)
        {
            var tripList = _tripManager.Trips.Select(t => t.Name).ToList();
            tripList.Add("Cancel");

            var tripName = IOManager.DisplayChoices("Select Trip", tripList);

            if (string.Compare(tripName, "Cancel", StringComparison.OrdinalIgnoreCase) == 0)
                return;

            var trip = _tripManager.GetTrip(tripName); 

            if (trip != null)
            {
                PromptTripInfo(trip);
                PromptTripItemMenu(trip);

                _tripManager.SaveTrips();
            }
            else
            {
                IOManager.DisplayMessage("Trip [red]does not[/] exist");
            }
        }
        else
        {
            IOManager.DisplayMessage("You have [red]no trips[/] to edit");
        }
    }

    static void Main(string[] args)
    {
        _tripManager.LoadTrips();
        string choice = string.Empty;
        
        Dictionary<string, Action> mainMenu = new() {
            {"Allocate Budget", () => EnterBudget()},
            {"Add Trip", () => AddTrip()},
            {"Edit Trips", () => EditTrip()}, 
            {"Analyze Trips", () => AnalyzeTrip()},
            {"Quit", () => {}}         
        };

        while (true)
        {
            var budget = $"Budget: $[green]{_tripManager.Budget}[/]";

            AnsiConsole.Clear();
            AnsiConsole.WriteLine("TripPlanner v1.1\n");
            AnsiConsole.Write(new Markup(budget));
            AnsiConsole.WriteLine();
            
            choice = IOManager.DisplayChoices("Main Menu", mainMenu.Select(m => m.Key));

            if (string.Compare(choice, "Quit", StringComparison.OrdinalIgnoreCase) == 0)
                break;

            if (mainMenu.TryGetValue(choice, out Action? action))
                action?.Invoke();
        }
    }
}
