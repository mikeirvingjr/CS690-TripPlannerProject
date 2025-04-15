using TripPlanner.Models;

namespace TripPlanner.Tests;

public class TripManagerTests
{
    readonly string filePath = "testmanager_trips.dat";

    [Fact]
    public void Test_TripManager_LoadTrips()
    {
        TripManager manager = new(filePath);
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

        manager.AddTrip(trip);
        manager.SaveTrips();
        manager.LoadTrips();

        var loadedTrip = manager.GetTrip("Test Trip 1");

        Assert.NotNull(loadedTrip);
        Assert.NotEmpty(loadedTrip.Items);
        Assert.Equal(trip.Items.Count, loadedTrip.Items.Count);

        for (int i=0; i<trip.Items.Count; i++)
        {
            Assert.Equal(trip.Items[i].Cost, loadedTrip.Items[i].Cost);
            Assert.Equal(trip.Items[i].Destination, loadedTrip.Items[i].Destination);
            Assert.Equal(trip.Items[i].Duration, loadedTrip.Items[i].Duration);
            Assert.Equal(trip.Items[i].DurationType, loadedTrip.Items[i].DurationType);
            Assert.Equal(trip.Items[i].ItemType, loadedTrip.Items[i].ItemType);
            Assert.Equal(trip.Items[i].Name, loadedTrip.Items[i].Name);
            Assert.Equal(trip.Items[i].StartDate, loadedTrip.Items[i].StartDate);
        }
    }

    [Fact]
    public void Test_TripManager_AddTrip()
    {
        Trip trip = new();
        TripManager manager = new(filePath);        

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

        manager.AddTrip(trip);
        Assert.NotEmpty(manager.Trips);
        Assert.Single(manager.Trips);
    }

    [Fact]
    public void Test_TripManager_GetTrips()
    {
        TripManager manager = new(filePath);
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

        manager.AddTrip(trip);
        var loadedTrip = manager.GetTrip("Test Trip 1");

        Assert.NotNull(loadedTrip);
        Assert.NotEmpty(loadedTrip.Items);
        Assert.Equal(trip.Items.Count, loadedTrip.Items.Count);

        for (int i=0; i<trip.Items.Count; i++)
        {
            Assert.Equal(trip.Items[i].Cost, loadedTrip.Items[i].Cost);
            Assert.Equal(trip.Items[i].Destination, loadedTrip.Items[i].Destination);
            Assert.Equal(trip.Items[i].Duration, loadedTrip.Items[i].Duration);
            Assert.Equal(trip.Items[i].DurationType, loadedTrip.Items[i].DurationType);
            Assert.Equal(trip.Items[i].ItemType, loadedTrip.Items[i].ItemType);
            Assert.Equal(trip.Items[i].Name, loadedTrip.Items[i].Name);
            Assert.Equal(trip.Items[i].StartDate, loadedTrip.Items[i].StartDate);
        }
    }

    [Fact]
    public void Test_TripManager_RemoveTrip()
    {
        Trip trip = new();
        TripManager manager = new(filePath);        

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

        manager.AddTrip(trip);
        Assert.NotEmpty(manager.Trips);
        Assert.Single(manager.Trips);

        manager.RemoveTrip(trip);
        Assert.Empty(manager.Trips);
    }

    [Fact]
    public void Test_TripManager_Calculation()
    {
        Trip trip = new();

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

        var totalCost = TripManager.CalculateTripCost(trip);
        Assert.Equal(1600M, totalCost);
    }
}