namespace TripPlanner.Models;

public class SaveData
{
    public decimal Budget { get; set; }

    public List<Trip> Trips { get; set; } = [];
}