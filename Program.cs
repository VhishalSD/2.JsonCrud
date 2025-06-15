using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

// Hoofdprogramma voor het beheren van personen met CRUD-functionaliteit en JSON-opslag
class Program
{
    private static List<Person> people = new List<Person>(); // Lijst om personen op te slaan
    private static int nextId = 1; // Volgende beschikbare ID voor nieuwe personen
    private static readonly string filePath = "people.json"; // Bestandspad voor JSON

    // Klasse die een persoon vertegenwoordigt met eigenschappen voor opslag
    class Person
    {
        public int Id { get; set; } // Unieke identificatie van een persoon
        public string FirstName { get; set; } // Voornaam van de persoon
        public string LastName { get; set; } // Achternaam van de persoon
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime BirthDate { get; set; } // Geboortedatum in dd-MM-yyyy formaat
        public string Department { get; set; } // Afdeling waar de persoon werkt
        public string Company { get; set; } // Bedrijf van de persoon
    }

    // Aangepaste JsonConverter voor DateTime in dd-MM-yyyy formaat
    class CustomDateTimeConverter : JsonConverter<DateTime>
    {
        private const string DateFormat = "dd-MM-yyyy"; // Standaard formaat voor datums

        // Converteert JSON-string naar DateTime
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (DateTime.TryParseExact(reader.GetString(), DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                return result;
            }
            throw new JsonException($"Invalid date format. Expected format: {DateFormat}");
        }

        // Schrijft DateTime naar JSON-string in dd-MM-yyyy formaat
        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(DateFormat, CultureInfo.InvariantCulture));
        }
    }

    // Hoofdmenu en programma-loop
    static void Main(string[] args)
    {
        LoadFromJson(); // Laad bestaande gegevens bij opstarten

        bool running = true;

        while (running)
        {
            try
            {
                Console.WriteLine("\n=== Person Management System ===");
                Console.WriteLine("1. Create new person");
                Console.WriteLine("2. Read person");
                Console.WriteLine("3. Update person");
                Console.WriteLine("4. Delete person");
                Console.WriteLine("5. Save and Exit");
                Console.Write("Select an option: ");

                string input = Console.ReadLine()?.Trim();

                switch (input)
                {
                    case "1":
                        CreatePerson();
                        SaveToJson(); // Sla op na wijziging
                        break;
                    case "2":
                        ReadPerson();
                        break;
                    case "3":
                        UpdatePerson();
                        SaveToJson(); // Sla op na wijziging
                        break;
                    case "4":
                        DeletePerson();
                        SaveToJson(); // Sla op na wijziging
                        break;
                    case "5":
                        SaveToJson(); // Sla op voor afsluiten
                        running = false;
                        break;
                    default:
                        Console.WriteLine("Invalid option, please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }
    }

    // Slaat de lijst met personen en de volgende ID op in een JSON-bestand
    static void SaveToJson()
    {
        try
        {
            var data = new
            {
                People = people,
                NextId = nextId
            };

            var options = new JsonSerializerOptions 
            { 
                WriteIndented = true, // Maakt JSON leesbaarder
                Converters = { new CustomDateTimeConverter() }
            };
            string jsonString = JsonSerializer.Serialize(data, options);
            File.WriteAllText(filePath, jsonString);
            Console.WriteLine("Data successfully saved to people.json.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving data to JSON: {ex.Message}");
        }
    }

    // Laadt de lijst met personen en de volgende ID uit een JSON-bestand
    static void LoadFromJson()
    {
        try
        {
            if (File.Exists(filePath))
            {
                string jsonString = File.ReadAllText(filePath);
                var options = new JsonSerializerOptions 
                { 
                    Converters = { new CustomDateTimeConverter() }
                };
                var data = JsonSerializer.Deserialize<JsonData>(jsonString, options);

                if (data != null)
                {
                    people = data.People ?? new List<Person>();
                    nextId = data.NextId;
                    Console.WriteLine("Data successfully loaded from people.json.");
                }
            }
            else
            {
                Console.WriteLine("No existing data file found. Starting with empty database.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading data from JSON: {ex.Message}");
        }
    }

    // Hulpklasse voor JSON-deserialisatie van personen en volgende ID
    private class JsonData
    {
        public List<Person> People { get; set; }
        public int NextId { get; set; }
    }

    // Maakt een nieuwe persoon aan en voegt deze toe aan de lijst
    static void CreatePerson()
    {
        try
        {
            Console.WriteLine("\n=== Create New Person ===");
            
            Person person = new Person();
            person.Id = nextId++;

            Console.Write("First Name: ");
            person.FirstName = SanitizeInput(Console.ReadLine());
            if (string.IsNullOrWhiteSpace(person.FirstName))
            {
                Console.WriteLine("First name cannot be empty.");
                return;
            }
            if (person.FirstName.Length > 50)
            {
                Console.WriteLine("First name cannot exceed 50 characters.");
                return;
            }

            Console.Write("Last Name: ");
            person.LastName = SanitizeInput(Console.ReadLine());
            if (string.IsNullOrWhiteSpace(person.LastName))
            {
                Console.WriteLine("Last name cannot be empty.");
                return;
            }
            if (person.LastName.Length > 50)
            {
                Console.WriteLine("Last name cannot exceed 50 characters.");
                return;
            }

            DateTime birthDate;
            bool validDate = false;
            int maxAge = 120;
            do
            {
                Console.Write($"Birth Date (dd-MM-yyyy, must be between {DateTime.Now.Year - maxAge} and {DateTime.Now.Year}): ");
                string dateInput = Console.ReadLine()?.Trim();
                validDate = DateTime.TryParseExact(dateInput, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out birthDate);
                
                if (validDate)
                {
                    if (birthDate > DateTime.Now)
                    {
                        Console.WriteLine("Birth date cannot be in the future.");
                        validDate = false;
                    }
                    else if (birthDate.Year < DateTime.Now.Year - maxAge)
                    {
                        Console.WriteLine($"Birth date cannot be before {DateTime.Now.Year - maxAge}.");
                        validDate = false;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid date format. Please use dd-MM-yyyy.");
                }
            } while (!validDate);
            person.BirthDate = birthDate;

            Console.Write("Department: ");
            person.Department = SanitizeInput(Console.ReadLine());
            if (string.IsNullOrWhiteSpace(person.Department))
            {
                Console.WriteLine("Department cannot be empty.");
                return;
            }
            if (person.Department.Length > 50)
            {
                Console.WriteLine("Department cannot exceed 50 characters.");
                return;
            }

            Console.Write("Company: ");
            person.Company = SanitizeInput(Console.ReadLine());
            if (string.IsNullOrWhiteSpace(person.Company))
            {
                Console.WriteLine("Company cannot be empty.");
                return;
            }
            if (person.Company.Length > 50)
            {
                Console.WriteLine("Company cannot exceed 50 characters.");
                return;
            }

            people.Add(person);
            Console.WriteLine($"Person with ID {person.Id} created successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating person: {ex.Message}");
        }
    }

    // Toont alle personen of een specifieke persoon op basis van ID
    static void ReadPerson()
    {
        try
        {
            Console.WriteLine("\n=== Read Person Data ===");
            Console.WriteLine("1. View all people");
            Console.WriteLine("2. View specific person by ID");
            Console.Write("Select an option: ");

            string option = Console.ReadLine()?.Trim();

            if (option == "1")
            {
                if (!people.Any())
                {
                    Console.WriteLine("No people in the database.");
                    return;
                }

                Console.WriteLine("\nAll People:");
                Console.WriteLine("{0,-5} {1,-15} {2,-15} {3,-12} {4,-15} {5,-15}", 
                    "ID", "First Name", "Last Name", "Birth Date", "Department", "Company");
                foreach (var person in people)
                {
                    Console.WriteLine("{0,-5} {1,-15} {2,-15} {3,-12:dd-MM-yyyy} {4,-15} {5,-15}", 
                        person.Id, TruncateString(person.FirstName, 15), TruncateString(person.LastName, 15), 
                        person.BirthDate, TruncateString(person.Department, 15), TruncateString(person.Company, 15));
                }
            }
            else if (option == "2")
            {
                Console.Write("Enter person ID: ");
                if (int.TryParse(Console.ReadLine(), out int id))
                {
                    Person person = people.FirstOrDefault(p => p.Id == id);
                    if (person != null)
                    {
                        Console.WriteLine("\nPerson Details:");
                        Console.WriteLine($"ID: {person.Id}");
                        Console.WriteLine($"First Name: {person.FirstName}");
                        Console.WriteLine($"Last Name: {person.LastName}");
                        Console.WriteLine($"Birth Date: {person.BirthDate:dd-MM-yyyy}");
                        Console.WriteLine($"Department: {person.Department}");
                        Console.WriteLine($"Company: {person.Company}");
                    }
                    else
                    {
                        Console.WriteLine($"Person with ID {id} not found.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid ID format.");
                }
            }
            else
            {
                Console.WriteLine("Invalid option, please select 1 or 2.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading person data: {ex.Message}");
        }
    }

    // Werkt een bestaande persoon bij op basis van ID
    static void UpdatePerson()
    {
        try
        {
            Console.WriteLine("\n=== Update Person ===");
            Console.Write("Enter person ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                Person person = people.FirstOrDefault(p => p.Id == id);
                if (person != null)
                {
                    Console.Write("New First Name (leave empty to keep unchanged): ");
                    string firstName = SanitizeInput(Console.ReadLine());
                    if (!string.IsNullOrWhiteSpace(firstName)) 
                    {
                        if (firstName.Length > 50)
                        {
                            Console.WriteLine("First name cannot exceed 50 characters.");
                        }
                        else
                        {
                            person.FirstName = firstName;
                        }
                    }

                    Console.Write("New Last Name (leave empty to keep unchanged): ");
                    string lastName = SanitizeInput(Console.ReadLine());
                    if (!string.IsNullOrWhiteSpace(lastName)) 
                    {
                        if (lastName.Length > 50)
                        {
                            Console.WriteLine("Last name cannot exceed 50 characters.");
                        }
                        else
                        {
                            person.LastName = lastName;
                        }
                    }

                    Console.Write("New Birth Date (dd-MM-yyyy, leave empty to keep unchanged): ");
                    string dateInput = Console.ReadLine()?.Trim();
                    if (!string.IsNullOrWhiteSpace(dateInput))
                    {
                        if (DateTime.TryParseExact(dateInput, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthDate))
                        {
                            if (birthDate > DateTime.Now)
                            {
                                Console.WriteLine("Birth date cannot be in the future. Birth date unchanged.");
                            }
                            else if (birthDate.Year < DateTime.Now.Year - 120)
                            {
                                Console.WriteLine($"Birth date cannot be before {DateTime.Now.Year - 120}. Birth date unchanged.");
                            }
                            else
                            {
                                person.BirthDate = birthDate;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid date format, birth date unchanged.");
                        }
                    }

                    Console.Write("New Department (leave empty to keep unchanged): ");
                    string department = SanitizeInput(Console.ReadLine());
                    if (!string.IsNullOrWhiteSpace(department)) 
                    {
                        if (department.Length > 50)
                        {
                            Console.WriteLine("Department cannot exceed 50 characters.");
                        }
                        else
                        {
                            person.Department = department;
                        }
                    }

                    Console.Write("New Company (leave empty to keep unchanged): ");
                    string company = SanitizeInput(Console.ReadLine());
                    if (!string.IsNullOrWhiteSpace(company)) 
                    {
                        if (company.Length > 50)
                        {
                            Console.WriteLine("Company cannot exceed 50 characters.");
                        }
                        else
                        {
                            person.Company = company;
                        }
                    }

                    Console.WriteLine($"Person with ID {id} updated successfully!");
                }
                else
                {
                    Console.WriteLine($"Person with ID {id} not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating person: {ex.Message}");
        }
    }

    // Verwijdert een persoon uit de lijst op basis van ID
    static void DeletePerson()
    {
        try
        {
            Console.WriteLine("\n=== Delete Person ===");
            Console.Write("Enter person ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                Person person = people.FirstOrDefault(p => p.Id == id);
                if (person != null)
                {
                    Console.Write($"Are you sure you want to delete person with ID {id}? (yes/no): ");
                    string confirmation = Console.ReadLine()?.Trim().ToLower();
                    if (confirmation == "yes" || confirmation == "y")
                    {
                        people.Remove(person);
                        Console.WriteLine($"Person with ID {id} deleted successfully!");
                    }
                    else
                    {
                        Console.WriteLine("Deletion canceled.");
                    }
                }
                else
                {
                    Console.WriteLine($"Person with ID {id} not found.");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID format.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting person: {ex.Message}");
        }
    }

    // Cleans(reinigt?) gebruikersinvoer om ongewenste tekens te verwijderen
    private static string SanitizeInput(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return input;

        return input.Trim()
                   .Replace("\"", "")
                   .Replace("'", "")
                   .Replace(";", "")
                   .Replace("--", "");
    }

    // Knipt lange strings af voor nette weergave in tabellen
    private static string TruncateString(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value.Substring(0, maxLength - 3) + "...";
    }
}