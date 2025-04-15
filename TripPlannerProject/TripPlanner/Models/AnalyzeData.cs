using System.ComponentModel;

namespace TripPlanner.Models;

public class AnalyzeData
{
    public string Name { get; set; } = string.Empty;

    public decimal Budget { get; set; }

    public decimal TotalCost { get; set; }

    public decimal Percentage { get; set; }

    public List<AnalyzeDataItem> Items { get; } = [];
}