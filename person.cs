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


