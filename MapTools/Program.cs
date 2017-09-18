using System;
using System.Collections.Generic;
using MapTools.Map;
using MapTools.Types;
using System.Xml.Linq;
using System.IO;
using System.Numerics;
using System.Globalization;
using System.Threading;

namespace MapTools
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            if (args.Length == 0)
            {
                Console.WriteLine("------------------------\nGTA V MapTools by Neos7\n------------------------\n");
                Console.WriteLine("extents\nCalculates again the extents of all the .ymap.xml \n");
                Console.WriteLine("merge\nMerges all the .ytyp.xml and all the .ymap.xml \n");
                Console.WriteLine("move\nMoves all the entities of all the .ymap.xml by a given offset.\n");
                Console.WriteLine("editByName\nMoves and rotates entities of all the .ymap.xml matching the archetypeName.\n");
                Console.WriteLine("guid\nGenerates new guid for ymap entities.\n");
                Console.WriteLine("reset\nResets flags and lodDist.\n");
                Console.WriteLine("ytdcheck\nReturns the list of missing .ytd foreach ytyp.\n");
                Console.WriteLine("grid\nDivides all the .ymap.xml files into blocks of a given size.\n");
                args = Console.ReadLine().Split();
            }
            if (args.Length != 0)
            {
                DirectoryInfo dir = null;
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
                                HashSet<string> missing = current.CMapData.UpdateExtents(archetypeList);
                                if (missing != null && missing.Count > 0)
                                {
                                    foreach (string name in missing)
                                        Console.WriteLine("Missing CBaseArchetypeDef: " + name);
                                }
                                XDocument ymapDoc = current.WriteXML();
                                ymapDoc.Save(file.Name);
                                Console.WriteLine("Updated extents for " + file.Name);
                                Console.WriteLine("");
                            }
                        }
                        break;
                    case "reset":
                        Console.WriteLine("Collecting archetypes...");
                        files = dir.GetFiles("*.ytyp.xml");
                        if (files.Length == 0)
                            Console.WriteLine("No .ytyp.xml file found.");
                        else
                        {
                            foreach (FileInfo file in files)
                            {
                                Ytyp current = new Ytyp(XDocument.Load(file.Name));
                                current.CMapTypes.UpdatelodDist();
                                foreach (CBaseArchetypeDef arc in current.CMapTypes.archetypes.Values)
                                {
                                    arc.flags = 0;
                                    arc.specialAttribute = 0;
                                }
                                XDocument ytypDoc = current.WriteXML();
                                ytypDoc.Save(file.Name);
                                Console.WriteLine("Resetted " + file.Name);
                            }
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

                                current.CMapData.UpdatelodDist(archetypeList);
                                current.CMapData.block = new block(0,0,"GTADrifting", "Neos7", "GTADrifting");
                                current.CMapData.flags = 0;
                                current.CMapData.contentFlags = 1;
                                foreach (CEntityDef ent in current.CMapData.entities)
                                {
                                    ent.flags = 0;
                                    ent.childLodDist = 0;
                                }

                                XDocument ymapDoc = current.WriteXML();
                                ymapDoc.Save(file.Name);
                                Console.WriteLine("Resetted " + file.Name);
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
                    case "ytdcheck":
                        files = dir.GetFiles("*.ytyp.xml");
                        if (files.Length == 0)
                            Console.WriteLine("No .ytyp.xml file found.");
                        else
                        {
                            List<string> list = new List<string>();
                            FileInfo[] ytdlist = dir.GetFiles("*.ytd");
                            foreach (FileInfo ytd in ytdlist)
                                list.Add(Path.GetFileNameWithoutExtension(ytd.Name));
                            CheckMissingYTD(files,list);
                        }
                        break;
                    case "grid":
                        files = dir.GetFiles("*.ymap.xml");
                        if (files.Length == 0)
                            Console.WriteLine("No .ymap.xml file found.");
                        else
                        {
                            Console.WriteLine("Insert the size of the blocks:");
                            int blocksize = int.Parse(Console.ReadLine());
                            foreach (FileInfo file in files)
                            {
                                int k = 1;
                                Console.WriteLine("Found " + file.Name);
                                Ymap current = new Ymap(XDocument.Load(file.Name));
                                List<List<CEntityDef>> grid = current.CMapData.GridSplit(blocksize);
                                foreach(List<CEntityDef> block in grid)
                                {
                                    Ymap tmp = current;
                                    tmp.CMapData.entities = block;

                                    XDocument ymapDoc = tmp.WriteXML();
                                    ymapDoc.Save(file.Name.Split('.')[0] + "_" + k.ToString("00") + ".ymap.xml");
                                    k++;
                                }
                                Console.WriteLine(file.Name + " splitted in " + (k-1) + " blocks.");
                            }
                        }
                        break;
                    case "test":
                        files = dir.GetFiles("*.ymap.xml");
                        if (files.Length == 0)
                            Console.WriteLine("No .ymap.xml file found.");
                        else
                        {
                            foreach (FileInfo file in files)
                            {
                                
                                Ymap current = new Ymap(XDocument.Load(file.Name));
                                XDocument test = current.WriteXML();
                                test.Save(file.Name);
                            }
                        }
                        break;
                    default:
                        Console.WriteLine(args[0] + " isn't a valid command.");
                        break;
                }
            }
            /*Console.WriteLine("Press any key to continue...");
            Console.ReadKey();*/
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

        public static Dictionary<string, CBaseArchetypeDef> CollectArchetypes(DirectoryInfo dir)
        {
            Dictionary<string, CBaseArchetypeDef> archetypes = new Dictionary<string, CBaseArchetypeDef>();
            FileInfo[] files = dir.GetFiles("*.ytyp.xml");
            if (files.Length != 0)
            { 
                Ytyp merged = MergeYTYP(files);
                archetypes = merged.CMapTypes.archetypes;
            }
            return archetypes;
        }

        public static void CheckMissingYTD(FileInfo[] ytypfiles,List<string> ytdlist)
        {
            foreach (FileInfo file in ytypfiles)
            {
                Console.WriteLine("CHECKING " + file.Name );
                Ytyp current = new Ytyp(XDocument.Load(file.Name));
                List<string> missing = new List<string>();
                foreach (string entry in current.CMapTypes.textureDictionaryList())
                {
                    if (!ytdlist.Contains(entry))
                        missing.Add(entry);
                }
                foreach (string missingytd in missing)
                    Console.WriteLine("Missing " + missingytd + ".ytd");
                Console.WriteLine("");
            }
        }
    }
}
