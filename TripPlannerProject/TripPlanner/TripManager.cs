using TripPlanner.Models;

namespace TripPlanner;

public class TripManager(string filepath = "trips.dat")
{
    private decimal _budget = 0M;

    public decimal Budget
    {
        get 
        {
            return _budget;
        }
        set
        {
             if (value > 0M && _budget != value)
            _budget = value;     
        }
    }
    
    private readonly List<Trip> _trips = [];

    public List<Trip> Trips
    {
        get { return _trips; }
    }
        
    private readonly string _tripFilePath = filepath;

    public void SaveTrips()
    {
        SaveData data = new() { Budget = _budget, Trips = _trips };
        FileSaver.Save(_tripFilePath, data);
    }

    public void AddTrip(Trip trip)
    {
        _trips.Add(trip);
    }

    public Trip? GetTrip(string name)
    {
        return _trips.FirstOrDefault(t => string.Compare(t.Name, name, StringComparison.OrdinalIgnoreCase) == 0);
    }

    public void RemoveTrip(Trip trip)
    {
        _trips.Remove(trip);
    }

    public void LoadTrips()
    {
        SaveData data = FileSaver.Load(_tripFilePath);

        if (data != null)
        {
            _budget = data.Budget;
            
            _trips.Clear();
            _trips.AddRange(data.Trips);
        }
    }

    public static decimal CalculateTripCost(Trip trip)
    {
        decimal cost = 0M;

        if (trip.Items.Count > 0)
        {
            foreach (var item in trip.Items)
                cost += item.Cost;
        }

        return cost;
    }
}