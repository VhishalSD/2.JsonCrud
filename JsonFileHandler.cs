using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

public class JsonFileHandler
{
    public List<Person>? People { get; private set; }

    public List<Person> LoadFromJson(string filePath)
    {
        if (File.Exists(filePath))
        {
            string jsonString = File.ReadAllText(filePath);
            People = JsonSerializer.Deserialize<List<Person>>(jsonString) ?? new List<Person>();
            return People;
        }

        People = new List<Person>();
        return People;
    }

    public void SaveToJson(string filePath, List<Person> people)
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        string jsonString = JsonSerializer.Serialize(people, options);
        File.WriteAllText(filePath, jsonString);
    }
}