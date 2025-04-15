using TripPlanner.Models;

namespace TripPlanner.Tests;


public class TripAnalyzerTests
{
    [Fact]
    public void Test_TripAnalyzer_AddTrip()
    {
        TripAnalyzer analyzer = new();
        Trip trip = new();

        analyzer.AddTrip(trip);

        Assert.Equal(1, analyzer.TripCount());
    }

    [Fact]
    public void Test_TripAnalyzer_Budget()
    {
        decimal budget = 5000M;
        TripAnalyzer analyzer = new();
        Trip trip = new();

        analyzer.AddTrip(trip);

        Assert.Equal(1, analyzer.TripCount());

        AnalyzeData data = TripAnalyzer.AnalyzeTrip(trip, budget);
        Assert.Equal(budget, data.Budget);
    }

    [Fact]
    public void Test_TripAnalyzer_TotalCost_RemoveTrip()
    {
        TripAnalyzer analyzer = new();
        Trip trip = new();

        analyzer.AddTrip(trip);

        Assert.Equal(1, analyzer.TripCount());

        analyzer.RemoveTrip(trip.Name);

        Assert.Equal(0, analyzer.TripCount());
    }

    [Fact]
    public void Test_TripAnalyzer_ResetTrips()
    {
        TripAnalyzer analyzer = new();
        Trip trip = new();

        analyzer.AddTrip(trip);

        Assert.Equal(1, analyzer.TripCount());

        analyzer.Reset();

        Assert.Equal(0, analyzer.TripCount());
    }

    [Fact]
    public void Test_TripAnalyzer_TotalCost_Percentage()
    {
        decimal budget = 1000M;
        Trip trip = new();

        TripItem item = new()
        {
            Name = "Flight",
            Cost = 800M
        };

        trip.Items.Add(item);

        AnalyzeData data = TripAnalyzer.AnalyzeTrip(trip, budget);

        Assert.Equal(budget, data.Budget);
        Assert.Equal(800M, data.TotalCost);
        Assert.Single(data.Items);

        if (data.Items.Count > 0)
        {
            Assert.Equal(800M, data.Items[0].Cost);
            Assert.Equal(80M, data.Items[0].BudgetPercentage);
        }
    }
}