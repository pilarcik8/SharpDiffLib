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


        private const bool HASH_SET_ENABLE = true;
        private const bool LIST_ENABLE = false;
        private const bool CLASS_PERSON_ENABLE = false;

        private const int MAX_ITERATION = 100;

        private const string InputFolder = "input";

        public static void Main()
        {
            int enabledCount = 0;
            if (HASH_SET_ENABLE) enabledCount++;
            if (LIST_ENABLE) enabledCount++;
            if (CLASS_PERSON_ENABLE) enabledCount++;
            if (enabledCount != 1)
            {
                Console.WriteLine("Enable only one of HASH_SET_ENABLE, LIST_ENABLE, or CLASS_PERSON_ENABLE.");
                return;
            }

            for (int iteration = 0; iteration < MAX_ITERATION; iteration++)
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

                if (CLASS_PERSON_ENABLE)
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

                    var merger = MergerPerson.Instance;
                    Person resultPerson = merger.Merge(
                        basePerson,
                        leftPerson,
                        rightPerson
                    );
                    Export(resultPerson, null, null, $"merged_person_{iteration}");
                }

                if (HASH_SET_ENABLE)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(HashSet<string>));
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

                    var merger = MergerSet.Instance;
                    HashSet<string> resultSet = merger.Merge(
                        baseSet,
                        leftSet,
                        rightSet
                    );
                    Export(null, resultSet, null, $"merged_set_{iteration}");
                }

                if (LIST_ENABLE)
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<string>));
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
                    var merger = MergerList.Instance;
                    List<string> resultList = merger.Merge(
                        baseList,
                        leftList,
                        rightList
                    );
                    Export(null, null, resultList, $"merged_list_{iteration}");
                }
            }
        }

        private static void Export(Person person, HashSet<string> set, List<string> list, string fileName)
        {
            int notNullCount = 0;
            if (person != null) notNullCount++;
            if (set != null) notNullCount++;
            if (list != null) notNullCount++;

            if (notNullCount != 1)
                throw new ArgumentException("Only one of the parameters can be non-null.");

            try
            {
                // Relatívna cesta ku koreňu projektu
                string projectDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, @"..\..\..\"));
                string outputDir = Path.Combine(projectDir, "output");

                // Vytvorenie priečinku, ak neexistuje
                Directory.CreateDirectory(outputDir);

                Console.WriteLine($"Výstupné súbory budú uložené do: {outputDir}");
                string xmlPath = Path.Combine(outputDir, $"{fileName}.xml");
                if (person != null)
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(Person));
                    using (var writer = new StreamWriter(xmlPath))
                    {
                        xmlSerializer.Serialize(writer, person);
                    }
                }
                else if (set != null)
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(HashSet<string>));
                    using (var writer = new StreamWriter(xmlPath))
                    {
                        xmlSerializer.Serialize(writer, set);
                    }
                }
                else if (list != null)
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<string>));
                    using (var writer = new StreamWriter(xmlPath))
                    {
                        xmlSerializer.Serialize(writer, list);
                    }
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
