using System.Collections;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using TripPlanner.Models;

namespace TripPlanner;

public class TripComparer(IEnumerable<Trip> trips, decimal budget)
{
    private decimal _budget = budget;

    private IEnumerable<Trip> _trips = trips;

    public Dictionary<string, AnalyzeData> AnalyzerData { get; } = [];

    public Dictionary<string, List<ComparerDataItem<decimal>>> ItemLengths { get; } = [];

    // public List<ComparerDataItem<TimeSpan>> TripLength { get; } = [];

    // public List<ComparerDataItem<int>> IteneraryCount { get; } = [];

    // public List<ComparerDataItem<decimal>> BudgetPercentage { get; } = [];

    // public List<ComparerDataItem<decimal>> TotalCost {get; } = []; 

    public string BestTrip { get; set; } = string.Empty;

    public void Compare()
    {
        AnalyzerData.Clear();
        ItemLengths.Clear();

        if (_trips.Any())
        {
            Collection<string> names = [];

            foreach (var trip in _trips)
            {
                ComparerDataItem<decimal>? newItemData = null;
                AnalyzerData[trip.Name] = TripAnalyzer.AnalyzeTrip(trip, _budget);

                foreach (var itemType in Enum.GetValues<TripItemType>())
                {
                    var items = trip.Items.Where(i => i.ItemType == itemType);
                    var duration = items.Sum(i => i.SumTime());
                    var itemTypeName = Enum.GetName<TripItemType>(itemType) ?? string.Empty;
                    names.Add(itemTypeName);
                    newItemData = new() { TripName = trip.Name, Value = duration };

                    if (ItemLengths.TryGetValue(itemTypeName, out List<ComparerDataItem<decimal>>? values))
                    {
                        values.Add(newItemData);
                        ItemLengths[itemTypeName] = values;
                    }
                    else
                    {
                        ItemLengths[itemTypeName] = [newItemData];
                    }
                }

                names.Add("Itenerary Count");
                newItemData = new() { TripName = trip.Name, Value = trip.Items.Count };                
                if (ItemLengths.TryGetValue(names.Last(), out List<ComparerDataItem<decimal>>? value))
                {
                    value.Add(newItemData);
                    ItemLengths[names.Last()] = value;
                }
                else
                {
                    ItemLengths[names.Last()] = [newItemData];
                }

                names.Add("Trip Length (in Days)");
                newItemData = new() { TripName = trip.Name, Value = (trip.EndDate - trip.StartDate).Days };                
                if (ItemLengths.TryGetValue(names.Last(), out List<ComparerDataItem<decimal>>? value2))
                {
                    value2.Add(newItemData);
                    ItemLengths[names.Last()] = value2;
                }
                else
                {
                    ItemLengths[names.Last()] = [newItemData];
                }

                names.Add("Budget Usage (%)");
                newItemData = new() { TripName = trip.Name, Value = Math.Round(AnalyzerData[trip.Name].Percentage, 2) };                
                if (ItemLengths.TryGetValue(names.Last(), out List<ComparerDataItem<decimal>>? value3))
                {
                    value3.Add(newItemData);
                    ItemLengths[names.Last()] = value3;
                }
                else
                {
                    ItemLengths[names.Last()] = [newItemData];
                }

                names.Add("Total Cost");
                newItemData = new() { TripName = trip.Name, Value = Math.Round(AnalyzerData[trip.Name].TotalCost, 2) };                
                if (ItemLengths.TryGetValue(names.Last(), out List<ComparerDataItem<decimal>>? value4))
                {
                    value4.Add(newItemData);
                    ItemLengths[names.Last()] = value4;
                }
                else
                {
                    ItemLengths[names.Last()] = [newItemData];
                }
            }

            Dictionary<string, int> counts = [];

            foreach (var key in ItemLengths.Keys)
            {
                var max = ItemLengths[key].Max(c => c.Value);
                var maxItem = ItemLengths[key].FirstOrDefault(c => c.Value == max);
                if (maxItem != null)
                {
                    maxItem.IsSelected = true;

                    if (counts.TryGetValue(maxItem.TripName, out int value))
                    {
                        counts[maxItem.TripName] = ++value;
                    }
                    else
                    {
                        counts[maxItem.TripName] = 1;
                    }
                }
            }
            
            BestTrip = counts.MaxBy(entry => entry.Value).Key;
        }
    }
}