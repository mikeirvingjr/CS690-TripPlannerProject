namespace TripPlanner.Models;

public class ComparerDataItem<T>
{
    public string TripName { get; set; } = string.Empty;

    public T? Value { get; set; }

    public bool IsSelected { get; set; }
}