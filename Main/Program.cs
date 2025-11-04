using KST.SharpDiffLib.Algorithms.MergeDiffs;
using KST.SharpDiffLib.Algorithms.ResolveConflicts;
using KST.SharpDiffLib.ConflictManagement;
using KST.SharpDiffLib.Definition;
using System;
using System.IO;
using System.Text.Json;
using System.Xml.Serialization;
using Bogus;

namespace TestKniznice
{
    public static class Program
    {
        private class Merger : MergerDefinition<Merger>
        {
            private Merger()
            {
                Define<Person>()
                    .ResolveAllSameRules(def => def
                        .Action(ResolveAction.UseRight)
                    );
            }
        }

        public static void Main()
        {
            var faker = new Faker("en");

            var generatedPerson = CreateFakePerson(faker);
            Console.WriteLine("Base person:");
            Console.WriteLine(generatedPerson);
            Console.WriteLine();

            ExportPerson(generatedPerson, "gp", exportJson: true, exportXml: true);
        }

        private static Person CreateFakePerson(Faker faker)
        {
            var personFaker = new Faker<Person>("en")
                .RuleFor(p => p.Title, f => f.Name.Prefix())
                .RuleFor(p => p.FirstName, f => f.Name.FirstName())
                .RuleFor(p => p.LastName, f => f.Name.LastName())
                .RuleFor(p => p.Email, f => f.Internet.Email())
                .RuleFor(p => p.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(p => p.Gender, f => f.PickRandom(new[] { "Male", "Female", "Other" }))
                .RuleFor(p => p.Age, f => f.Random.Int(18, 80))
                .RuleFor(p => p.Company, f => f.Company.CompanyName())
                .RuleFor(p => p.JobTitle, f => f.Name.JobTitle())
                .RuleFor(p => p.CreditCardNumber, f => f.Finance.CreditCardNumber())
                .RuleFor(p => p.Address, f => new Address
                {
                    Street = f.Address.StreetName(),
                    StreetNumber = f.Address.BuildingNumber(),
                    City = f.Address.City(),
                    County = f.Address.County(),
                    State = f.Address.State(),
                    ZipCode = f.Address.ZipCode(),
                    Country = f.Address.Country()
                });

            return personFaker.Generate();
        }

        private static void ExportPerson(Person person, string fileName = "person", bool exportJson = true, bool exportXml = true)
        {
            if (person == null)
            {
                Console.WriteLine("Person je null – export sa nevykoná.");
                return;
            }

            if (!exportJson && !exportXml)
            {
                Console.WriteLine("Ani JSON ani XML export nie je povolený – nič sa nevytvorí.");
                return;
            }

            try
            {
                // Relatívna cesta ku koreňu projektu
                string projectDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\"));
                string outputDir = Path.Combine(projectDir, "vytvoreneSubory", "Base");

                // Vytvorenie priečinku, ak neexistuje
                Directory.CreateDirectory(outputDir);

                Console.WriteLine($"Výstupné súbory budú uložené do: {outputDir}");

                if (exportJson)
                {
                    string jsonPath = Path.Combine(outputDir, $"{fileName}.json");
                    string json = JsonSerializer.Serialize(person, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(jsonPath, json);
                    Console.WriteLine($"JSON uložený do: {jsonPath}");
                }

                if (exportXml)
                {
                    string xmlPath = Path.Combine(outputDir, $"{fileName}.xml");
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(Person));
                    using (var writer = new StreamWriter(xmlPath))
                    {
                        xmlSerializer.Serialize(writer, person);
                    }
                    Console.WriteLine($"XML uložený do: {xmlPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba pri exporte: {ex.Message}");
            }
        }
    }
}
