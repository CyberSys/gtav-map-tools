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
            //Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            if (args.Length == 0)
            {
                Console.WriteLine("------------------------\nGTA V MapTools by Neos7\n------------------------\n");
                //Console.WriteLine("If you don't specify a directory as args[1], the current one will be used.\n");
                Console.WriteLine("extents\nCalculates again the extents of all the .ymap.xml \n");
                Console.WriteLine("merge\nMerges all the .ytyp.xml and all the .ymap.xml \n");
                Console.WriteLine("move\nMoves all the entities of all the .ymap.xml by a given offset.\n");
                Console.WriteLine("editByName\nMoves and rotates entities of all the .ymap.xml matching the archetypeName.\n");
                Console.WriteLine("guid\nGenerates new guid for ymap entities.\n");
                args = Console.ReadLine().Split();
            }
            if (args.Length != 0)
            {
                DirectoryInfo dir = null;
                /*if (args.Length > 1)
                    dir = new DirectoryInfo(args[1]);
                else*/
                    dir = new DirectoryInfo(Directory.GetCurrentDirectory());
                FileInfo[] files = null;
                Dictionary<string,CBaseArchetypeDef> archetypeList = null;
                switch (args[0])
                {
                    case "merge":
                        Console.WriteLine("### MERGE YTYP ###");
                        files = dir.GetFiles("*.ytyp.xml");
                        if (files.Length == 0)
                            Console.WriteLine("No .ytyp.xml file found.");
                        else
                        {
                            Ytyp merged = MergeYTYP(files);
                            XDocument ytyp_new = merged.WriteXML();
                            ytyp_new.Save("merged.ytyp.xml");
                            Console.WriteLine("Exported merged.ytyp.xml");
                        }
                        Console.WriteLine("");
                        Console.WriteLine("### MERGE YMAP ###");
                        files = dir.GetFiles("*.ymap.xml");
                        if (files.Length == 0)
                            Console.WriteLine("No .ymap.xml file found.");
                        else
                        {
                            Ymap merged = MergeYMAP(files);
                            XDocument ymap_new = merged.WriteXML();
                            ymap_new.Save("merged.ymap.xml");
                            Console.WriteLine("Exported merged.ymap.xml");
                        }
                        Console.WriteLine("");

                        break;
                    case "extents":
                        Console.WriteLine("### CALCULATE EXTENTS ###");
                        Console.WriteLine("Collecting archetypes...");
                        files = dir.GetFiles("*.ytyp.xml");
                        if (files.Length == 0)
                            Console.WriteLine("No .ytyp.xml file found.");
                        else
                        {
                            Ytyp merged = MergeYTYP(files);
                            archetypeList = merged.CMapTypes.archetypes;
                        }
                        Console.WriteLine("");

                        Console.WriteLine("Collecting .ymap.xml to update...");
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
                                Console.WriteLine("");
                            }
                        }
                        break;
                    case "normalize":
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
                                current.Normalize(archetypeList);
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
                                Console.WriteLine("Moved all the entities of " + file.Name);
                            }
                        }
                        break;
                    case "editByName":
                        files = dir.GetFiles("*.ymap.xml");
                        if (files.Length == 0)
                            Console.WriteLine("No .ymap.xml file found.");
                        else
                        {
                            Console.WriteLine("Insert archetypeName of the entities to edit:");
                            string matchingName = Console.ReadLine();
                            Console.WriteLine("Insert the position offset:");
                            Vector3 positionOffset = ReadVector3();
                            Console.WriteLine("Insert the rotation offset (in degrees):");
                            Vector3 rotationOffset = ReadVector3();
                            foreach (FileInfo file in files)
                            {
                                Console.WriteLine("Found " + file.Name);
                                Ymap current = new Ymap(XDocument.Load(file.Name));
                                int i = current.CMapData.MoveAndRotateEntitiesByName(matchingName,positionOffset,rotationOffset);
                                XDocument ymapDoc = current.WriteXML();
                                ymapDoc.Save(file.Name);
                                Console.WriteLine("Edited "+ i + " entities of " + file.Name);
                            }
                        }
                        break;
                    case "guid":
                        files = dir.GetFiles("*.ymap.xml");
                        if (files.Length == 0)
                            Console.WriteLine("No .ymap.xml file found.");
                        else
                        {
                            foreach(FileInfo file in files)
                            {
                                XDocument doc = XDocument.Load(file.Name);
                                if(doc.Element("CMapData").Element("entities").Elements() != null)
                                {
                                    Random rnd = new Random();
                                    foreach (XElement ent in doc.Element("CMapData").Element("entities").Elements())
                                        ent.Element("guid").Attribute("value").Value = rnd.Next().ToString();
                                }
                                doc.Save(file.Name);
                                Console.WriteLine("Updated guids for " + file.Name);
                            }
                        }
                        break;
                    default:
                        Console.WriteLine(args[0] + " isn't a valid command.");
                        break;
                }
            }
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            Environment.Exit(0);
        }

        public static Ytyp MergeYTYP(FileInfo[] files)
        {
            List<Ytyp> list = new List<Ytyp>();
            foreach (FileInfo file in files)
            {
                Console.WriteLine("Found " + file.Name);
                list.Add(new Ytyp(XDocument.Load(file.Name)));
            }
            return Ytyp.Merge(list);
        }

        public static Ymap MergeYMAP(FileInfo[] files)
        {
            List<Ymap> list = new List<Ymap>();
            foreach (FileInfo file in files)
            {
                Console.WriteLine("Found " + file.Name);
                list.Add(new Ymap(XDocument.Load(file.Name)));
            }
            return Ymap.Merge(list);
        }

        public static Vector3 ReadVector3()
        {
            Vector3 offset = new Vector3();
            Console.WriteLine("For the decimal separator use the character '" + CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator + "'");
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
