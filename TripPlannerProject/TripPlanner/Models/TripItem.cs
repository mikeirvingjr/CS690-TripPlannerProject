namespace TripPlanner.Models;

public class TripItem
{
    public int Id { get; set; }

    public int TripId { get; set; }

    public TripItemType ItemType { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Destination { get; set; } = string.Empty;

    public DateTime StartDate { get; set; }

    public decimal Duration { get; set; }

    public TripItemDurationType DurationType { get; set; }

    public Decimal Cost { get; set; } = 0M;
}