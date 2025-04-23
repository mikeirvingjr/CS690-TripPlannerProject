using TripPlanner.Models;

namespace TripPlanner;

public class TripComparerTests
{
    [Fact]
    public void Test_TripComparer_CompareTrip()
    {        
        Trip trip = new() { Name = "Test Trip 1", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(10) };

        trip.Items.Add(new TripItem
        {
            Name = "Test Flight 1",
            ItemType = TripItemType.Excursion,
            Duration = 10,
            StartDate = trip.StartDate.AddDays(1),
            DurationType = TripItemDurationType.Hours,
            Cost = 800M
        });

        trip.Items.Add(new TripItem
        {
            Name = "Test Excursion 1",
            ItemType = TripItemType.Excursion,
            Duration = 10,
            DurationType = TripItemDurationType.Hours,
            StartDate = trip.StartDate.AddDays(2),
            Cost = 300M
        });

        trip.Items.Add(new TripItem
        {
            Name = "Test Accomodation 1",
            ItemType = TripItemType.Accomodation,
            Duration = 10,
            DurationType = TripItemDurationType.Days,
            StartDate = trip.StartDate,
            Cost = 500M
        });

        trip.Items.Add(new TripItem
        {
            Name = "Test Meal 1",
            ItemType = TripItemType.Meal,
            Duration = 10,
            DurationType = TripItemDurationType.Hours,
            StartDate = trip.StartDate.AddDays(3),
            Cost = 250M
        });

        Trip trip2 = new() { Name = "Test Trip 2", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(5) };

        trip2.Items.Add(new TripItem
        {
            Name = "Test Excursion 2",
            ItemType = TripItemType.Excursion,
            Duration = 10,
            DurationType = TripItemDurationType.Hours,
            StartDate = trip.StartDate.AddDays(2),
            Cost = 800M
        });

        trip2.Items.Add(new TripItem
        {
            Name = "Test Meal 2",
            ItemType = TripItemType.Meal,
            Duration = 1,
            DurationType = TripItemDurationType.Hours,
            StartDate = trip.StartDate.AddDays(1),
            Cost = 300M
        });

        Trip trip3 = new() { Name = "Test Trip 3", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(14) };

        trip3.Items.Add(new TripItem
        {
            Name = "Test Flight 2",
            ItemType = TripItemType.Flight,
            Duration = 10,
            DurationType = TripItemDurationType.Hours,
            StartDate = trip.StartDate.AddDays(-1),
            Cost = 800M
        });

        TripComparer comparer = new([trip, trip2, trip3], 2000M);
        comparer.Compare();

        Assert.NotNull(comparer.AnalyzerData);
        Assert.NotEmpty(comparer.AnalyzerData);
        Assert.Equal(3, comparer.AnalyzerData.Count);
        Assert.Equal("Test Trip 1", comparer.BestTrip);        
    }

    [Fact]
    public void Test_TripComparer_CompareTrip_Inconclusive()
    {        
        Trip trip = new() { Name = "Test Trip 1", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(10) };

        trip.Items.Add(new TripItem
        {
            Name = "Test Flight 1",
            ItemType = TripItemType.Excursion,
            Duration = 10,
            StartDate = trip.StartDate.AddDays(1),
            DurationType = TripItemDurationType.Hours,
            Cost = 800M
        });

        trip.Items.Add(new TripItem
        {
            Name = "Test Excursion 1",
            ItemType = TripItemType.Excursion,
            Duration = 10,
            DurationType = TripItemDurationType.Hours,
            StartDate = trip.StartDate.AddDays(2),
            Cost = 300M
        });

        trip.Items.Add(new TripItem
        {
            Name = "Test Accomodation 1",
            ItemType = TripItemType.Accomodation,
            Duration = 10,
            DurationType = TripItemDurationType.Days,
            StartDate = trip.StartDate,
            Cost = 500M
        });

        trip.Items.Add(new TripItem
        {
            Name = "Test Meal 1",
            ItemType = TripItemType.Meal,
            Duration = 10,
            DurationType = TripItemDurationType.Hours,
            StartDate = trip.StartDate.AddDays(3),
            Cost = 250M
        });

        Trip trip2 = new() { Name = "Test Trip 2", StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(10) };

        trip2.Items.Add(new TripItem
        {
            Name = "Test Flight 1",
            ItemType = TripItemType.Excursion,
            Duration = 10,
            StartDate = trip.StartDate.AddDays(1),
            DurationType = TripItemDurationType.Hours,
            Cost = 800M
        });

        trip2.Items.Add(new TripItem
        {
            Name = "Test Excursion 1",
            ItemType = TripItemType.Excursion,
            Duration = 10,
            DurationType = TripItemDurationType.Hours,
            StartDate = trip2.StartDate.AddDays(2),
            Cost = 300M
        });

        trip2.Items.Add(new TripItem
        {
            Name = "Test Accomodation 1",
            ItemType = TripItemType.Accomodation,
            Duration = 10,
            DurationType = TripItemDurationType.Days,
            StartDate = trip2.StartDate,
            Cost = 500M
        });

        trip2.Items.Add(new TripItem
        {
            Name = "Test Meal 1",
            ItemType = TripItemType.Meal,
            Duration = 10,
            DurationType = TripItemDurationType.Hours,
            StartDate = trip2.StartDate.AddDays(3),
            Cost = 250M
        });

        TripComparer comparer = new([trip, trip2], 2000M);
        comparer.Compare();

        Assert.NotNull(comparer.AnalyzerData);
        Assert.NotEmpty(comparer.AnalyzerData);
        Assert.Equal(2, comparer.AnalyzerData.Count);
        Assert.Equal("Inconclusive", comparer.BestTrip);        
    }
}