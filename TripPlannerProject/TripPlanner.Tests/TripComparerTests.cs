using TripPlanner.Models;

namespace TripPlanner;

public class TripComparerTests
{
    [Fact]
     public void Test_TripComparer_CompareTrip()
    {        
        Trip trip = new() { Name = "Test Trip 1" };

        trip.Items.Add(new TripItem
        {
            Name = "Test Item 1",
            Cost = 800M
        });

        trip.Items.Add(new TripItem
        {
            Name = "Test Item 2",
            Cost = 300M
        });

        trip.Items.Add(new TripItem
        {
            Name = "Test Item 3",
            Cost = 500M
        });

        Trip trip2 = new() { Name = "Test Trip 2" };

        trip2.Items.Add(new TripItem
        {
            Name = "Test Item 1",
            Cost = 800M
        });

        trip2.Items.Add(new TripItem
        {
            Name = "Test Item 2",
            Cost = 300M
        });

        Trip trip3 = new() { Name = "Test Trip 3" };

        trip3.Items.Add(new TripItem
        {
            Name = "Test Item 1",
            Cost = 800M
        });

        TripComparer comparer = new([trip, trip2, trip3], 2000M);
        comparer.Compare();

        Assert.NotNull(comparer.AnalyzerData);
        Assert.NotEmpty(comparer.AnalyzerData);
        Assert.Equal(3, comparer.AnalyzerData.Count);
        Assert.Equal("Test Trip 1", comparer.BestTrip);        
    }
}