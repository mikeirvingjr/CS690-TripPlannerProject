using System.Xml.Serialization;
using Newtonsoft.Json;
using Spectre.Console;
using TripPlanner.Models;

namespace TripPlanner;

public static class FileSaver
{
    public static void Save(string path, SaveData data, bool append = false)
    {
        using TextWriter writer = new StreamWriter(path, append);
        
        try
        {
            var contents = JsonConvert.SerializeObject(data);
            writer.Write(contents);
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
        }
    }

    public static SaveData Load(string path)
    {
        SaveData data = new();

        if (File.Exists(path))
        {
            using TextReader reader = new StreamReader(path);

            try
            {
                var contents = reader.ReadToEnd(); 
                data = JsonConvert.DeserializeObject<SaveData>(contents)!;
            }
            catch (Exception ex)
            {
                AnsiConsole.WriteException(ex);
            }
        }

        return data;
    }
}