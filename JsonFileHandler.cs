using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Collections.Generic;

// Klasse die verantwoordelijk is voor het lezen/schrijven van JSON-bestanden
public class JsonFileHandler
{
    // Interne opslag van ingelezen personen (optioneel extern opvraagbaar)
    public List<Person>? People { get; private set; }

    // Methode om data te laden vanuit een JSON-bestand
    public List<Person> LoadFromJson(string filePath)
    {
        if (File.Exists(filePath))
        {
            string jsonString = File.ReadAllText(filePath);

            // JsonOptions inclusief custom datumconverter
            var options = new JsonSerializerOptions
            {
                Converters = { new CustomDateTimeConverter() }
            };

            // Deserialize naar wrapperklasse met 'People' en 'NextId'
            var data = JsonSerializer.Deserialize<JsonData>(jsonString, options);

            People = data?.People ?? new List<Person>();
            return People;
        }

        // Als bestand niet bestaat: lege lijst teruggeven
        People = new List<Person>();
        return People;
    }

    // Methode om data op te slaan naar JSON-bestand
    public void SaveToJson(string filePath, List<Person> people, int nextId)
    {
        // Object met beide velden om te serializen
        var data = new
        {
            People = people,
            NextId = nextId
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new CustomDateTimeConverter() }
        };

        string jsonString = JsonSerializer.Serialize(data, options);
        File.WriteAllText(filePath, jsonString);
    }

    // Interne helperklasse die overeenkomt met de JSON-structuur
    private class JsonData
    {
        public List<Person>? People { get; set; }
        public int NextId { get; set; }
    }

    // Converter voor datum naar 'dd-MM-yyyy' formaat
    private class CustomDateTimeConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString() ?? "");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString("dd-MM-yyyy"));
        }
    }
}