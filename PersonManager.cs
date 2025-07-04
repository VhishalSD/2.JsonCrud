using System;
using System.Collections.Generic;
using System.Linq;

public class PersonManager
{
    private readonly JsonFileHandler _jsonHandler;
    private List<Person> _people;

    public PersonManager(JsonFileHandler jsonHandler)
    {
        _jsonHandler = jsonHandler;
        _people = _jsonHandler.LoadFromJson("people.json");
    }

    public void CreatePerson()
    {
        Console.Write("Enter First Name: ");
        string firstName = Console.ReadLine() ?? "";

        Console.Write("Enter Last Name: ");
        string lastName = Console.ReadLine() ?? "";

        Console.Write("Enter Birthdate (yyyy-MM-dd): ");
        DateTime birthDate;
        while (!DateTime.TryParse(Console.ReadLine(), out birthDate))
        {
            Console.Write("Invalid date format. Enter Birthdate (yyyy-MM-dd): ");
        }

        Console.Write("Enter Department: ");
        string department = Console.ReadLine() ?? "";

        Console.Write("Enter Company: ");
        string company = Console.ReadLine() ?? "";

        int newId = _people.Any() ? _people.Max(p => p.Id) + 1 : 1;

        var newPerson = new Person
        {
            Id = newId,
            FirstName = firstName,
            LastName = lastName,
            BirthDate = birthDate,
            Department = department,
            Company = company
        };

        _people.Add(newPerson);
        Console.WriteLine("Person created successfully.");
    }

    public void ReadPerson()
    {
        Console.Write("Enter Person Id to read: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            var person = _people.FirstOrDefault(p => p.Id == id);
            if (person != null)
            {
                Console.WriteLine($"Id: {person.Id}");
                Console.WriteLine($"Name: {person.FirstName} {person.LastName}");
                Console.WriteLine($"Birthdate: {person.BirthDate:yyyy-MM-dd}");
                Console.WriteLine($"Department: {person.Department}");
                Console.WriteLine($"Company: {person.Company}");
            }
            else
            {
                Console.WriteLine("Person not found.");
            }
        }
        else
        {
            Console.WriteLine("Invalid Id input.");
        }
    }

    public void UpdatePerson()
    {
        Console.Write("Enter Person Id to update: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            var person = _people.FirstOrDefault(p => p.Id == id);
            if (person != null)
            {
                Console.Write($"Enter new First Name (current: {person.FirstName}): ");
                string firstName = Console.ReadLine();
                if (!string.IsNullOrEmpty(firstName))
                    person.FirstName = firstName;

                Console.Write($"Enter new Last Name (current: {person.LastName}): ");
                string lastName = Console.ReadLine();
                if (!string.IsNullOrEmpty(lastName))
                    person.LastName = lastName;

                Console.Write($"Enter new Birthdate (yyyy-MM-dd) (current: {person.BirthDate:yyyy-MM-dd}): ");
                string bdInput = Console.ReadLine();
                if (DateTime.TryParse(bdInput, out DateTime birthDate))
                    person.BirthDate = birthDate;

                Console.Write($"Enter new Department (current: {person.Department}): ");
                string department = Console.ReadLine();
                if (!string.IsNullOrEmpty(department))
                    person.Department = department;

                Console.Write($"Enter new Company (current: {person.Company}): ");
                string company = Console.ReadLine();
                if (!string.IsNullOrEmpty(company))
                    person.Company = company;

                Console.WriteLine("Person updated successfully.");
            }
            else
            {
                Console.WriteLine("Person not found.");
            }
        }
        else
        {
            Console.WriteLine("Invalid Id input.");
        }
    }

    public void DeletePerson()
    {
        Console.Write("Enter Person Id to delete: ");
        if (int.TryParse(Console.ReadLine(), out int id))
        {
            var person = _people.FirstOrDefault(p => p.Id == id);
            if (person != null)
            {
                _people.Remove(person);
                Console.WriteLine("Person deleted successfully.");
            }
            else
            {
                Console.WriteLine("Person not found.");
            }
        }
        else
        {
            Console.WriteLine("Invalid Id input.");
        }
    }

    public void SaveAndExit()
    {
        _jsonHandler.SaveToJson("people.json", _people);
        Console.WriteLine("Data saved. Exiting program.");
    }
}