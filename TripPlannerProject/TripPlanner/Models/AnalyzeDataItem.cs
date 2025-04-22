namespace TripPlanner.Models;

public class AnalyzeDataItem
{
    public string Name { get; set; } = string.Empty;

    public decimal Cost { get; set; }

    public string ItemType { get; set; } = string.Empty;

    public decimal BudgetPercentage { get; set; }
}