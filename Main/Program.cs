using Bogus;
using KST.SharpDiffLib.Algorithms.MergeDiffs;
using KST.SharpDiffLib.Algorithms.ResolveConflicts;
using KST.SharpDiffLib.ConflictManagement;
using KST.SharpDiffLib.Definition;
using System;
using System.IO;
using System.Text.Json;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace TestKniznice
{
    public static class Program
    {
        private class Merger : MergerDefinition<Merger>
        {
            private Merger()
            {                Define<Person>();
            }
        }

        private const bool HashSetCollections = false;
        private const bool HashListCollections = false;
        private const bool Person = true;

        private const int ITERATIONS = 1;

        private const string InputFolder = "input";

        public static void Main()
        {
            for (int iteration = 0; iteration < ITERATIONS; iteration++)
            {
                string BaseFile = $"base{iteration}.xml";
                string LeftFile = $"left{iteration}.xml";
                string RightFile = $"right{iteration}.xml";
                string ResultFile = $"result{iteration}.xml";

                string projectDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\"));
                var basePath = Path.Combine(projectDir, InputFolder, iteration.ToString(), BaseFile);
                var leftPath = Path.Combine(projectDir, InputFolder, iteration.ToString(), LeftFile);
                var rightPath = Path.Combine(projectDir, InputFolder, iteration.ToString(), RightFile);

                if (!File.Exists(basePath) || !File.Exists(leftPath) || !File.Exists(rightPath))
                {
                    Console.WriteLine($"Ended at iteration: {iteration}");
                    return;
                }

                var baseDoc = XDocument.Load(basePath);
                var leftDoc = XDocument.Load(leftPath);
                var rightDoc = XDocument.Load(rightPath);

                if (Person)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Person));
                    Person basePerson;
                    using (var reader = baseDoc.Root.CreateReader())
                    {
                        basePerson = (Person)serializer.Deserialize(reader);
                    }
                    Person leftPerson;
                    using (var reader = leftDoc.Root.CreateReader())
                    {
                        leftPerson = (Person)serializer.Deserialize(reader);
                    }
                    Person rightPerson;
                    using (var reader = rightDoc.Root.CreateReader())
                    {
                        rightPerson = (Person)serializer.Deserialize(reader);
                    }
                    
                    var merger = Merger.Instance;
                    Person resultPerson = merger.Merge(
                        basePerson,
                        leftPerson,
                        rightPerson
                    );
                    ExportPerson(resultPerson, $"merged_person_{iteration}");
                }
            }
        }

        private static void ExportPerson(Person person, string fileName = "person")
        {
            if (person == null)
            {
                Console.WriteLine("Person je null – export sa nevykoná.");
                return;
            }

            try
            {
                // Relatívna cesta ku koreňu projektu
                string projectDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\"));
                string outputDir = Path.Combine(projectDir, "output");

                // Vytvorenie priečinku, ak neexistuje
                Directory.CreateDirectory(outputDir);

                Console.WriteLine($"Výstupné súbory budú uložené do: {outputDir}");
                string xmlPath = Path.Combine(outputDir, $"{fileName}.xml");
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(Person));
                using (var writer = new StreamWriter(xmlPath))
                {
                    xmlSerializer.Serialize(writer, person);
                }
                Console.WriteLine($"XML uložený do: {xmlPath}");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba pri exporte: {ex.Message}");
            }
        }
    }
}
