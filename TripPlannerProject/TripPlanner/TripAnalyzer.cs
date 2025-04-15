namespace TripPlanner;

using TripPlanner.Models;

public class TripAnalyzer
{
    private List<Trip> _trips = [];

    public decimal Budget { get; set; }


    public int TripCount()
    {
        return _trips.Count;
    }

    public void AddTrip(Trip trip)
    {
        _trips.Add(trip);
    }

    public void RemoveTrip(string name)
    {
        var trip = _trips.FirstOrDefault(t => string.Compare(t.Name, name, StringComparison.OrdinalIgnoreCase) == 0);

        if (trip != null)
            _trips.Remove(trip);
    }

    public List<AnalyzeData> Analyze()
    {
        List<AnalyzeData> data = [];

        foreach (var trip in _trips)
            data.Add(AnalyzeTrip(trip, Budget));

        return data;
    }

    public static AnalyzeData AnalyzeTrip(Trip trip, decimal budget)
    {
        AnalyzeData datem = new()
        {
            Budget = budget,
            Name = trip.Name,
            TotalCost = 0M
        };

        foreach (var item in trip.Items)
        {
            datem.TotalCost += item.Cost;

            datem.Items.Add(new AnalyzeDataItem 
            {
                Name = item.Name,
                Cost = item.Cost,
                BudgetPercentage = item.Cost / budget * 100M
            });
        }

        datem.Percentage = datem.TotalCost / datem.Budget * 100M;
        return datem;
    }

    public void Reset()
    {
        _trips.Clear();
    }
}