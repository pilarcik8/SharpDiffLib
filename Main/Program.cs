using KST.SharpDiffLib.Definition;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace TestKniznice
{
    public static class Program
    {
        private class MergerPerson : MergerDefinition<MergerPerson>
        {
            private MergerPerson()
            {
                Define<Person>();
            }
        }

        private class MergerSet : MergerDefinition<MergerSet>
        {
            private MergerSet()
            {
                Define<HashSet<string>>();
            }
        }

        private class MergerList : MergerDefinition<MergerList>
        {
            private MergerList()
            {
                Define<List<string>>();
            }
        }

        static string DirWithFiles = "";
        static string SubfolderName = "sharpDiffLib";


        static List<int> ErroredFiles = new List<int>();


        public static void Main()
        {
            string userInput = UserInputTypeOfXML();
            DirWithFiles = UserInputDirToFiles();
            int iteration = 0;
            while (true) 
            {
                string BaseFile = $"base{iteration}.xml";
                string LeftFile = $"left{iteration}.xml";
                string RightFile = $"right{iteration}.xml";
                string ResultFile = $"expectedResult{iteration}.xml";

                var basePath = Path.Combine(DirWithFiles, iteration.ToString(), BaseFile);
                var leftPath = Path.Combine(DirWithFiles, iteration.ToString(), LeftFile);
                var rightPath = Path.Combine(DirWithFiles, iteration.ToString(), RightFile);

                if (!File.Exists(basePath) || !File.Exists(leftPath) || !File.Exists(rightPath))
                {
                    Console.WriteLine($"Ukončené na iterácii: {iteration}");
                    string errors = ErroredFiles.Count > 0 ? string.Join(", ", ErroredFiles) : "Žiadne";
                    Console.WriteLine($"Počet chybne spracovaných súborov: {ErroredFiles.Count}");
                    Console.WriteLine($"Konfilikty na iteráciach: {errors}");
                    Console.WriteLine(ErroredFiles.Count * 100.0 / iteration + "% konfliktov");
                    if (ErroredFiles.Count > 0)
                    {
                        File.WriteAllText(Path.Combine(DirWithFiles, SubfolderName, "sharpDiffErrors.txt"), $"{errors}");
                    }
                    break;
                }

                var baseDoc = XDocument.Load(basePath);
                var leftDoc = XDocument.Load(leftPath);
                var rightDoc = XDocument.Load(rightPath);

                if (userInput == "class")
                {
                    XmlSerializer serializer = new(typeof(Person));
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

                    try
                    {
                        var merger = MergerPerson.Instance;
                        Person resultPerson = merger.Merge(
                            basePerson,
                            leftPerson,
                            rightPerson
                        );
                        Export(resultPerson, null, null, $"mergedResult{iteration}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Chyba pri zlučovaní osoby v iterácii {iteration}: {ex.Message}");
                        ErroredFiles.Add(iteration);
                    }
                }

                if (userInput == "set")
                {
                    XmlSerializer serializer = new(typeof(HashSet<string>));
                    HashSet<string> baseSet;
                    using (var reader = baseDoc.Root.CreateReader())
                    {
                        baseSet = (HashSet<string>)serializer.Deserialize(reader);
                    }
                    HashSet<string> leftSet;
                    using (var reader = leftDoc.Root.CreateReader())
                    {
                        leftSet = (HashSet<string>)serializer.Deserialize(reader);
                    }
                    HashSet<string> rightSet;
                    using (var reader = rightDoc.Root.CreateReader())
                    {
                        rightSet = (HashSet<string>)serializer.Deserialize(reader);
                    }

                    try
                    {
                        var merger = MergerSet.Instance;
                        HashSet<string> resultSet = merger.Merge(
                            baseSet,
                            leftSet,
                            rightSet
                        );
                        Export(null, resultSet, null, $"mergedResult{iteration}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Chyba pri zlučovaní množiny v iterácii {iteration}: {ex.Message}");
                        ErroredFiles.Add(iteration);
                    }
                }

                if (userInput == "list")
                {
                    XmlSerializer serializer = new(typeof(List<string>));
                    List<string> baseList;
                    using (var reader = baseDoc.Root.CreateReader())
                    {
                        baseList = (List<string>)serializer.Deserialize(reader);
                    }
                    List<string> leftList;
                    using (var reader = leftDoc.Root.CreateReader())
                    {
                        leftList = (List<string>)serializer.Deserialize(reader);
                    }
                    List<string> rightList;
                    using (var reader = rightDoc.Root.CreateReader())
                    {
                        rightList = (List<string>)serializer.Deserialize(reader);
                    }

                    try
                    {
                        var merger = MergerList.Instance;
                        List<string> resultList = merger.Merge(
                            baseList,
                            leftList,
                            rightList
                        );
                        Export(null, null, resultList, $"mergedResult{iteration}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Chyba pri zlučovaní zoznamu v iterácii {iteration}: {ex.Message}");
                        ErroredFiles.Add(iteration);
                    }
                }
                iteration++;
            }
        }

        private static void Export(Person? person, HashSet<string>? set, List<string>? list, string fileName)
        {
            int notNullCount = 0;
            if (person != null) notNullCount++;
            if (set != null) notNullCount++;
            if (list != null) notNullCount++;

            if (notNullCount != 1)
                throw new ArgumentException("Iba jeden vstup nesmie byť null.");

            try
            {
                string outputDir = Path.Combine(DirWithFiles, SubfolderName);
                Directory.CreateDirectory(outputDir);

                string xmlPath = Path.Combine(outputDir, $"{fileName}.xml");
                if (person != null)
                {
                    XmlSerializer xmlSerializer = new(typeof(Person));
                    using var writer = new StreamWriter(xmlPath);
                    xmlSerializer.Serialize(writer, person);
                }
                else if (set != null)
                {
                    XmlSerializer xmlSerializer = new(typeof(HashSet<string>));
                    using var writer = new StreamWriter(xmlPath);
                    xmlSerializer.Serialize(writer, set);
                }
                else if (list != null)
                {
                    XmlSerializer xmlSerializer = new(typeof(List<string>));
                    using var writer = new StreamWriter(xmlPath);
                    xmlSerializer.Serialize(writer, list);
                }
                Console.WriteLine($"XML uložené do: {xmlPath}");

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Chyba pri exportovaní: {ex.Message}");
            }
        }

        public static string UserInputTypeOfXML()
        {
            string? input = "";
            while (input != "set" && input != "class" && input != "list")
            {
                Console.WriteLine("Vyber typ vstupu: 'set'/'class'/'list'");
                input = Console.ReadLine();
                if (input == null) continue;

                input = input.ToLower();
            }
            return input;
        }

        public static string UserInputDirToFiles()
        {
            Console.WriteLine("V priečinku majte očíslované podsložky od 0");
            Console.WriteLine("Prvý priečinok by mal obsahovať: '0/base0.xml', '0/left0.xml' a '0/right0.xml'");
            Console.WriteLine("Druhý priečinok by mal obsahovať: '1/base1.xml', '1/left1.xml' a '1/right1.xml'");
            Console.WriteLine("A tak ďalej... Ak sa priečinok alebo súbor z danej iterácie nenájde, program skončí.");
            Console.WriteLine("---------------------------------------------------------------------------------------------------------------");
            Console.WriteLine("Vložte absolútnu cestu k priečinku so súbormi");

            while (true)
            {
                string? input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Zadajte platnú cestu (nie prázdnu). Skúste znova:");
                    continue;
                }

                input = input.Trim().Trim('"');

                try
                {
                    string full = Path.GetFullPath(input);

                    if (!Directory.Exists(full))
                    {
                        Console.WriteLine($"Adresár neexistuje: {full}. Skontrolujte cestu a skúste znova:");
                        continue;
                    }

                    return full;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Neplatná cesta: {ex.Message}. Skúste znova:");
                }
            }
        }
    }
}
