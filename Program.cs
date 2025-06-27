using System;

class Program
{
    static void Main(string[] args)
    {
        // Initialiseer JSON-bestand-handler en personmanager
        var jsonHandler = new JsonFileHandler();
        var personManager = new PersonManager(jsonHandler);

        bool running = true;

        // Hoofdmenu-lus
        while (running)
        {
            try
            {
                // Toon menu
                Console.WriteLine("\n=== Person Management System ===");
                Console.WriteLine("1. Create new person");
                Console.WriteLine("2. Read person");
                Console.WriteLine("3. Update person");
                Console.WriteLine("4. Delete person");
                Console.WriteLine("5. Save and Exit");
                Console.Write("Select an option: ");

                // Lees invoer
                string input = (Console.ReadLine() ?? string.Empty).Trim();

                // Keuzemenu afhandelen
                switch (input)
                {
                    case "1":
                        personManager.CreatePerson();
                        break;

                    case "2":
                        personManager.ReadPerson();
                        break;

                    case "3":
                        personManager.UpdatePerson();
                        break;

                    case "4":
                        personManager.DeletePerson();
                        break;

                    case "5":
                        // Bepaal het juiste nextId voor opslag
                        int nextId = personManager.People.Any()
                            ? personManager.People.Max(p => p.Id) + 1
                            : 1;

                        // Sla data op en sluit af
                        jsonHandler.SaveToJson("people.json", personManager.People, nextId);
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

            // Pauze en scherm wissen
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();
            Console.Clear();
        }
    }
}