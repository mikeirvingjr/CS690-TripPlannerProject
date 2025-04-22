using System.Reflection.Metadata.Ecma335;
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

    public Trip CopyTrip(Trip trip, bool copyDetails, string[] itemNames)
    {
        Trip newTrip = copyDetails 
        ? new()
        {
            Destination = trip.Destination,
            EndDate = trip.EndDate,
            Name = $"{trip.Name} Copy",
            StartDate = trip.StartDate
        } 
        : new() { Name = "New Trip" };

        if (itemNames.Length != 0)
        {
            foreach (var itemName in itemNames)
            {
                var item = trip.Items.FirstOrDefault(i => string.Compare(i.Name, itemName, StringComparison.OrdinalIgnoreCase) == 0);

                if (item != null)
                {
                    newTrip.Items.Add(new TripItem() 
                    {
                        Cost = item.Cost,
                        Destination = item.Destination,
                        Duration = item.Duration,
                        DurationType = item.DurationType,
                        ItemType = item.ItemType,
                        Name = item.Name,
                        StartDate = item.StartDate
                    });
                }
            }
        }

        return newTrip;
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