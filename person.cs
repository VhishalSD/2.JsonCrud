using System;
using System.Text.Json;
using System.Text.Json.Serialization;

// Klasse die een persoon vorstelt
public class Person
{
    // Uniek ID voor de persoon
    public int Id { get; set; }

    // Voornaam van de persoon (mag null zijn)
    public string? FirstName { get; set; }

    // Achternaam van de peroon (mag null zijn)
    public string? LastName { get; set; }


    // Geboortedatum van de persoon, met aangepaste JSON-conversie
    [JsonConverter(typeof(CustomDateTimeConverter))]
    public DateTime BirthDate { get; set; }

    // Afdeling waar de persoon werkt
    public string? Department { get; set; }

    // Bedrijf waar de persoon werkt
    public string? Company { get; set; }
}

// Eigen converter voor het lezen en schrijven van datums in JSON
public class CustomDateTimeConverter : JsonConverter<DateTime>
{
    // Methode om een datum te lezen vanuit JSON (tekst -> DateTime)
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        // Haal de tekst uit de JSON en zet deze om naar een DateTime object
        return DateTime.Parse(reader.GetString() ?? "");
    }
    // Methode om een datum uit te schrijven naar JSON (DateTime -> tekst)
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        // Schrijf de datum als string in het formaat: dag-maand-jaar
        writer.WriteStringValue(value.ToString("dd-MM-yyyy"));
    }
}