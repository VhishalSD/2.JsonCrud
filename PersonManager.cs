using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

// Klasse die verantwoordelijk is voor het beheren van personen
public class PersonManager
{
    private List<Person> people; // Lijst met personen
    private int nextId; // Volgend beschikbaar ID
    private JsonFileHandler jsonHandler; // Handler voor JSON-bestanden

    public PersonManager(JsonFileHandler jsonHandler)
    {
        this.jsonHandler = jsonHandler;
        people = jsonHandler.LoadFromJson("People.json");
        nextId = people.Any() ? people.Max(p => p.Id) + 1 : 1;
    }

    // Methode om een nieuwe persoon aan te maken
    public void CreatePerson()
    {
        try
        {
            // Voornaam
            Person person = new Person { Id = nextId++ };
            Console.WriteLine("\n=== Create New Person ===");
            Console.Write("First Name: ");
            person.FirstName = SanitizeInput(Console.ReadLine() ?? string.Empty);
            if (string.IsNullOrWhiteSpace(person.FirstName) || person.FirstName.Length > 50) return;

            // Achternaam
            Console.Write("Last Name: ");
            person.LastName = SanitizeInput(Console.ReadLine() ?? string.Empty);
            if (string.IsNullOrWhiteSpace(person.LastName) || person.LastName.Length > 50) return;

            // Geboortedatum
            DateTime birthDate;
            bool validDate = false;
            do
            {
                Console.Write($"Birth Date (dd-MM-yyyy, max age 120): ");
                string dateInput = (Console.ReadLine() ?? string.Empty).Trim();
                validDate = DateTime.TryParseExact(dateInput, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out birthDate);
                if (validDate && (birthDate > DateTime.Now || birthDate.Year < DateTime.Now.Year - 120)) validDate = false;
            } while (!validDate);
            person.BirthDate = birthDate;

            // Afdeling
            Console.Write("Department: ");
            person.Department = SanitizeInput(Console.ReadLine() ?? string.Empty);
            if (string.IsNullOrWhiteSpace(person.Department) || person.Department.Length > 50) return;

            // Bedrijf
            Console.Write("Company: ");
            person.Company = SanitizeInput(Console.ReadLine() ?? string.Empty);
            if (string.IsNullOrWhiteSpace(person.Company) || person.Company.Length > 50) return;

            people.Add(person);
            Console.WriteLine($"Person with ID {person.Id} created!");
        }
        catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); }
    }

    // Methode om personen te bekijken
    public void ReadPerson()
    {
        try
        {
            Console.WriteLine("\n=== Read Person Data ===");
            Console.WriteLine("1. View all people");
            Console.WriteLine("2. View specific person by ID");
            Console.Write("Select an option: ");
            string option = (Console.ReadLine() ?? string.Empty).Trim();

            if (option == "1")
            {
                if (!people.Any()) { Console.WriteLine("No people."); return; }
                Console.WriteLine("\nAll People:");
                Console.WriteLine("{0,-5} {1,-15} {2,-15} {3,-12} {4,-15} {5,-15}", "ID", "First Name", "Last Name", "Birth Date", "Department", "Company");
                foreach (var person in people)
                {
                    Console.WriteLine("{0,-5} {1,-15} {2,-15} {3,-12:dd-MM-yyyy} {4,-15} {5,-15}", person.Id, TruncateString(person.FirstName, 15), TruncateString(person.LastName, 15), person.BirthDate, TruncateString(person.Department, 15), TruncateString(person.Company, 15));
                }
            }
            else if (option == "2")
            {
                Console.Write("Enter person ID: ");
                if (int.TryParse(Console.ReadLine(), out int id))
                {
                    var person = people.FirstOrDefault(p => p.Id == id);
                    if (person != null) Console.WriteLine($"\nID: {person.Id}\nFirst Name: {person.FirstName}\nLast Name: {person.LastName}\nBirth Date: {person.BirthDate:dd-MM-yyyy}\nDepartment: {person.Department}\nCompany: {person.Company}");
                    else Console.WriteLine($"Person with ID {id} not found.");
                }
                else Console.WriteLine("Invalid ID.");
            }
            else Console.WriteLine("Invalid option.");
        }
        catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); }
    }

    // Methode om een persoon bij te werken
    public void UpdatePerson()
    {
        try
        {
            Console.WriteLine("\n=== Update Person ===");
            Console.Write("Enter person ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var person = people.FirstOrDefault(p => p.Id == id);
                if (person != null)
                {
                    // Velden optioneel bijwerken
                    Console.Write("New First Name (leave empty to skip): ");
                    string firstName = SanitizeInput(Console.ReadLine() ?? string.Empty);
                    if (!string.IsNullOrWhiteSpace(firstName) && firstName.Length <= 50) person.FirstName = firstName;

                    Console.Write("New Last Name (leave empty to skip): ");
                    string lastName = SanitizeInput(Console.ReadLine() ?? string.Empty);
                    if (!string.IsNullOrWhiteSpace(lastName) && lastName.Length <= 50) person.LastName = lastName;

                    Console.Write("New Birth Date (dd-MM-yyyy, leave empty to skip): ");
                    string dateInput = (Console.ReadLine() ?? string.Empty).Trim();
                    if (!string.IsNullOrWhiteSpace(dateInput) && DateTime.TryParseExact(dateInput, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthDate) && birthDate <= DateTime.Now && birthDate.Year >= DateTime.Now.Year - 120) person.BirthDate = birthDate;

                    Console.Write("New Department (leave empty to skip): ");
                    string department = SanitizeInput(Console.ReadLine() ?? string.Empty);
                    if (!string.IsNullOrWhiteSpace(department) && department.Length <= 50) person.Department = department;

                    Console.Write("New Company (leave empty to skip): ");
                    string company = SanitizeInput(Console.ReadLine() ?? string.Empty);
                    if (!string.IsNullOrWhiteSpace(company) && company.Length <= 50) person.Company = company;

                    Console.WriteLine($"Person with ID {id} updated!");
                }
                else Console.WriteLine($"Person with ID {id} not found.");
            }
            else Console.WriteLine("Invalid ID.");
        }
        catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); }
    }

    // Methode om een persoon te verwijderen
    public void DeletePerson()
    {
        try
        {
            Console.WriteLine("\n=== Delete Person ===");
            Console.Write("Enter person ID: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                var person = people.FirstOrDefault(p => p.Id == id);
                if (person != null)
                {
                    Console.Write($"Delete person with ID {id}? (yes/no): ");
                    if ((Console.ReadLine() ?? string.Empty).Trim().ToLower() == "yes") { people.Remove(person); Console.WriteLine($"Person with ID {id} deleted!"); }
                    else Console.WriteLine("Deletion canceled.");
                }
                else Console.WriteLine($"Person with ID {id} not found.");
            }
            else Console.WriteLine("Invalid ID.");
        }
        catch (Exception ex) { Console.WriteLine($"Error: {ex.Message}"); }
    }

    // Invoer schoonmaken van onveile karakters
    private string SanitizeInput(string input)
    {
        return string.IsNullOrWhiteSpace(input) ? input : input.Trim().Replace("\"", "").Replace("'", "").Replace(";", "").Replace("--", "");
    }

    // Te lange tekst inkorten
    private string TruncateString(string? value, int maxLength)
    {
        return string.IsNullOrEmpty(value) ? value ?? string.Empty : value.Length <= maxLength ? value : value.Substring(0, maxLength - 3) + "...";
    }

    // Property om buiten deze klasse de lijst op te vragen
    public List<Person> People { get { return people; } }

    public void SaveAndExit()
    {
        jsonHandler.SaveToJson("People.json", people, nextId);
    }
}

    