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
    public void Test_TripManager_CopyTrip()
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

        var newTrip = manager.CopyTrip(trip, true, ["Test Item 2", "Test Item 3"]);

        Assert.NotNull(newTrip);
        Assert.NotEmpty(newTrip.Items);
        Assert.Equal(2, newTrip.Items.Count);

        Assert.Equal($"{trip.Name} Copy", newTrip.Name);
        Assert.Equal(trip.Destination, newTrip.Destination);
        Assert.Equal(trip.StartDate, newTrip.StartDate);
        Assert.Equal(trip.EndDate, newTrip.EndDate);

        for (int i=0; i<newTrip.Items.Count; i++)
        {
            var item = trip.Items.Find(itm => string.Compare(itm.Name, newTrip.Items[i].Name, StringComparison.OrdinalIgnoreCase) == 0);
            Assert.NotNull(item);

            Assert.Equal(item.Cost, newTrip.Items[i].Cost);
            Assert.Equal(item.Destination, newTrip.Items[i].Destination);
            Assert.Equal(item.Duration, newTrip.Items[i].Duration);
            Assert.Equal(item.DurationType, newTrip.Items[i].DurationType);
            Assert.Equal(item.ItemType, newTrip.Items[i].ItemType);
            Assert.Equal(item.Name, newTrip.Items[i].Name);
            Assert.Equal(item.StartDate, newTrip.Items[i].StartDate);
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
    public void Test_TripManager_SetBudget()
    {
        TripManager manager = new(filePath)
        {
            Budget = 3000M
        };

        Assert.Equal(3000M, manager.Budget);

        manager.Budget = 2000M;

        Assert.Equal(2000M, manager.Budget);
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