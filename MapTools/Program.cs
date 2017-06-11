using System;
using System.Collections.Generic;
using MapTools.Data;
using MapTools.Types;
using System.Xml.Linq;
using System.IO;

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
                Console.WriteLine("merge\nMerges all the .ytyp.xml and all the .ymap.xml \n");
                args = Console.ReadLine().Split();
            }
            if (args.Length != 0)
            {
                DirectoryInfo dir = null;
                FileInfo[] files = null;
                switch (args[0])
                {
                    case "merge":
                        if (args.Length > 1)
                            dir = new DirectoryInfo(args[1]);
                        else
                            dir = new DirectoryInfo(Directory.GetCurrentDirectory());
                        Console.WriteLine("YTYP MERGE");
                        HashSet<CBaseArchetypeDef> archetypeList = null;
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
                            Ymap merged = MergeYMAP(files, archetypeList);
                            XDocument ymap_new = merged.WriteXML();
                            ymap_new.Save("merged.ymap.xml");
                            Console.WriteLine("Exported merged.ymap.xml");
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

        public static Ymap MergeYMAP(FileInfo[] files, HashSet<CBaseArchetypeDef> archetypeList)
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
            List<string> missing = merged.CMapData.UpdateExtents(archetypeList);
            if (missing != null && missing.Count > 0)
            {
                Console.WriteLine("WARNING: Some CBaseArchetypeDef are missing, extents might not be accurate.");
                Console.WriteLine("Try copying their .ytyp in the current folder.");

                foreach (string name in missing)
                    Console.WriteLine("Missing CBaseArchetypeDef: " + name);
            }
            return merged;
        }
    }
}
