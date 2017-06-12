using System;
using System.Collections.Generic;
using MapTools.Data;
using MapTools.Types;
using System.Xml.Linq;
using System.IO;
using System.Numerics;
using System.Globalization;

namespace MapTools
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("------------------------\nGTA V MapTools by Neos7\n------------------------\n");
                Console.WriteLine("If you don't specify a directory as args[1], the current one will be used.\n");
                Console.WriteLine("extents\nCalculates again the extents of all the .ymap.xml \n");
                Console.WriteLine("merge\nMerges all the .ytyp.xml and all the .ymap.xml \n");
                Console.WriteLine("move\nMoves all the entities of all the .ymap.xml by a given offset\n");
                args = Console.ReadLine().Split();
            }
            if (args.Length != 0)
            {
                DirectoryInfo dir = null;
                if (args.Length > 1)
                    dir = new DirectoryInfo(args[1]);
                else
                    dir = new DirectoryInfo(Directory.GetCurrentDirectory());
                FileInfo[] files = null;
                HashSet<CBaseArchetypeDef> archetypeList = null;
                switch (args[0])
                {
                    case "merge":
                        Console.WriteLine("YTYP MERGE");
                        files = dir.GetFiles("*.ytyp.xml");
                        if (files.Length == 0)
                            Console.WriteLine("No .ytyp.xml file found.");
                        else
                        {
                            Ytyp merged = MergeYTYP(files);
                            XDocument ytyp_new = merged.WriteXML();
                            ytyp_new.Save("merged.ytyp.xml");
                            Console.WriteLine("Exported merged.ytyp.xml");
                            archetypeList = merged.CMapTypes.archetypes;
                        }

                        Console.WriteLine("YMAP MERGE");
                        files = dir.GetFiles("*.ymap.xml");
                        if (files.Length == 0)
                            Console.WriteLine("No .ymap.xml file found.");
                        else
                        {
                            Ymap merged = MergeYMAP(files);
                            merged.UpdateExtents(archetypeList);
                            XDocument ymap_new = merged.WriteXML();
                            ymap_new.Save("merged.ymap.xml");
                            Console.WriteLine("Exported merged.ymap.xml");
                        }
                        
                        break;
                    case "extents":
                        Console.WriteLine("Collecting archetypes...");
                        files = dir.GetFiles("*.ytyp.xml");
                        if (files.Length == 0)
                            Console.WriteLine("No .ytyp.xml file found.");
                        else
                        {
                            Ytyp merged = MergeYTYP(files);
                            archetypeList = merged.CMapTypes.archetypes;
                        }
                        Console.WriteLine("Scanning .ymap.xml to update...");
                        files = dir.GetFiles("*.ymap.xml");
                        if (files.Length == 0)
                            Console.WriteLine("No .ymap.xml file found.");
                        else
                        {
                            foreach (FileInfo file in files)
                            {
                                Console.WriteLine("Found " + file.Name);
                                Ymap current = new Ymap(XDocument.Load(file.Name));
                                current.UpdateExtents(archetypeList);
                                XDocument ymapDoc = current.WriteXML();
                                ymapDoc.Save(file.Name);
                                Console.WriteLine("Updated extents for " + file.Name);
                            }
                        }
                        break;
                    case "move":
                        files = dir.GetFiles("*.ymap.xml");
                        if (files.Length == 0)
                            Console.WriteLine("No .ymap.xml file found.");
                        else
                        {
                            Vector3 offset = ReadVector3();
                            foreach (FileInfo file in files)
                            {
                                Console.WriteLine("Found " + file.Name);
                                Ymap current = new Ymap(XDocument.Load(file.Name));
                                current.CMapData.MoveEntities(offset);
                                XDocument ymapDoc = current.WriteXML();
                                ymapDoc.Save(file.Name);
                                Console.WriteLine("Moved entities of " + file.Name);
                            }
                            Console.WriteLine("Remember to update their extents.");
                        }
                        break;
                    default:
                        Console.WriteLine(args[0] + " isn't a valid command.");
                        break;
                }
            }
            Console.ReadKey();
            Environment.Exit(0);
        }

        public static Ytyp MergeYTYP(FileInfo[] files)
        {
            Ytyp merged = new Ytyp("merged");
            foreach (FileInfo file in files)
            {
                Console.WriteLine("Found " + file.Name);
                Ytyp current = new Ytyp(XDocument.Load(file.Name));
                foreach (CBaseArchetypeDef archetype in current.CMapTypes.archetypes)
                {
                    if (!merged.CMapTypes.archetypes.Add(archetype))
                        Console.WriteLine("Skipped duplicated CBaseArchetypeDef: " + archetype.name);
                }
            }
            return merged;
        }

        public static Ymap MergeYMAP(FileInfo[] files)
        {
            Ymap merged = new Ymap("merged");
            foreach (FileInfo file in files)
            {
                Console.WriteLine("Found " + file.Name);
                Ymap current = new Ymap(XDocument.Load(file.Name));
                foreach (CEntityDef entity in current.CMapData.entities)
                {
                    if (!merged.CMapData.entities.Add(entity))
                        Console.WriteLine("Skipped duplicated CEntityDef: " + entity.guid);
                }
            }
            return merged;
        }

        public static Vector3 ReadVector3()
        {
            Vector3 offset = new Vector3();
            Console.WriteLine("For the decimal separator use the character '" + CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator + "'");
            Console.WriteLine("Insert the offset:");
            Console.WriteLine("X:");
            offset.X = float.Parse(Console.ReadLine());
            Console.WriteLine("Y:");
            offset.Y = float.Parse(Console.ReadLine());
            Console.WriteLine("Z:");
            offset.Z = float.Parse(Console.ReadLine());
            return offset;
        }
    }
}
