using System.Collections;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using TripPlanner.Models;

namespace TripPlanner;

public class TripComparer(IEnumerable<Trip> trips, decimal budget)
{
    private decimal _budget = budget;

    private readonly IEnumerable<Trip> _trips = trips;

    public Dictionary<string, AnalyzeData> AnalyzerData { get; } = [];

    public Dictionary<string, List<ComparerDataItem<decimal>>> ItemLengths { get; } = [];

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
                names.Clear();
                ComparerDataItem<decimal>? newItemData = null;
                AnalyzerData[trip.Name] = TripAnalyzer.AnalyzeTrip(trip, _budget);

                foreach (var itemType in Enum.GetValues<TripItemType>())
                {
                    var items = trip.Items.Where(i => i.ItemType == itemType);
                    var duration = items.Select(i => i.StartDate).Distinct().Count();
                    var count = items.Count();
                    var itemTypeName = Enum.GetName<TripItemType>(itemType) ?? string.Empty;
                    
                    names.Add($"{itemTypeName} count");
                    newItemData = new() { TripName = trip.Name, Value = count };

                    if (ItemLengths.TryGetValue(names.Last(), out List<ComparerDataItem<decimal>>? values))
                    {
                        values.Add(newItemData);
                        ItemLengths[names.Last()] = values;
                    }
                    else
                    {
                        ItemLengths[names.Last()] = [newItemData];
                    }

                    names.Add($"{itemTypeName} duration (in Days)");
                    newItemData = new() { TripName = trip.Name, Value = (decimal)duration };

                    if (ItemLengths.TryGetValue(names.Last(), out List<ComparerDataItem<decimal>>? valuesN))
                    {
                        valuesN.Add(newItemData);
                        ItemLengths[names.Last()] = valuesN;
                    }
                    else
                    {
                        ItemLengths[names.Last()] = [newItemData];
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

            Dictionary<string, int> counts = _trips.Select(t => t.Name).Distinct().ToDictionary(key => key, value => 0);

            foreach (var key in ItemLengths.Keys)
            {
                var minOrMax = string.Compare(key, "Total Cost", StringComparison.OrdinalIgnoreCase) == 0 
                  ? ItemLengths[key].Min(c => c.Value) : ItemLengths[key].Max(c => c.Value);
                
                var selectedItems = ItemLengths[key].Where(c => c.Value == minOrMax);
                if (selectedItems != null && selectedItems.Count() == 1)
                {
                    var selectedItem = selectedItems.First();                    
                    selectedItem.IsSelected = true;

                    if (counts.TryGetValue(selectedItem.TripName, out int value))
                    {
                        counts[selectedItem.TripName] = ++value;
                    }
                    else
                    {
                        counts[selectedItem.TripName] = 1;
                    }
                }
            }
            

            var maxItem = counts.Max(entry => entry.Value);
            var maxItems = counts.Where(entry => entry.Value == maxItem);
            BestTrip = maxItems.Count() == 1 ? maxItems.First().Key : "Inconclusive";
        }
    }
}