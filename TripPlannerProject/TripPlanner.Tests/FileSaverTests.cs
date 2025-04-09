using TripPlanner.Models;

namespace TripPlanner.Tests;


public class FileSaverTests
{
    readonly string filePath = "test_trips.dat";

    [Fact]
    public void Test_FileSaver_Saves()
    {
        SaveData data = new();

        Trip trip = new()
        {
            Destination = "New York",
            EndDate = new DateTime(2025, 08, 08),
            StartDate = new DateTime(2025, 08, 10),
            Name = "Trip to New York",            
        };

        trip.Items.Add(
                 new() 
                 { 
                    Cost = 10M, 
                    Destination = "SDF Airport", 
                    Duration = 2, 
                    DurationType = TripItemDurationType.Hours,
                    ItemType = TripItemType.Flight,
                    Name = "Flight to NY",
                    StartDate = new DateTime(2025, 08, 08),                    
                }
        );

        data.Budget = 5000M;
        data.Trips = [trip];

        FileSaver.Save(filePath, data);

        var contents = File.ReadAllText(filePath);

        Assert.NotEqual(contents, string.Empty);
    }

    [Fact]
    public void Test_FileSaver_Loads()
    {
        SaveData data = new();
        SaveData loadedData = new();

        Trip trip = new()
        {
            Destination = "New York",
            EndDate = new DateTime(2025, 08, 08),
            StartDate = new DateTime(2025, 08, 10),
            Name = "Trip to New York",            
        };

        trip.Items.Add(
                 new() 
                 { 
                    Cost = 10M, 
                    Destination = "SDF Airport", 
                    Duration = 2, 
                    DurationType = TripItemDurationType.Hours,
                    ItemType = TripItemType.Flight,
                    Name = "Flight to NY",
                    StartDate = new DateTime(2025, 08, 08),                    
                }
        );

        data.Budget = 5000M;
        data.Trips = [trip];

        FileSaver.Save(filePath, data);
        loadedData = FileSaver.Load(filePath);

        Assert.Equal(data.Budget, loadedData.Budget);
        Assert.Equal(data.Trips.Count, loadedData.Trips.Count);
        Assert.Equal(data.Trips[0].Items.Count, loadedData.Trips[0].Items.Count);
    }
}
