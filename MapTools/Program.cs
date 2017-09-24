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
                Console.WriteLine("GTA V MapTools by Neos7\n");
                Console.WriteLine("extents\nCalculates again the extents of all the .ymap.xml \n");
                Console.WriteLine("merge\nMerges all the .ytyp.xml and all the .ymap.xml \n");
                Console.WriteLine("move\nMoves all the entities of all the .ymap.xml by a given offset.\n");
                Console.WriteLine("editByName\nMoves and rotates entities of all the .ymap.xml matching the archetypeName.\n");
                Console.WriteLine("guid\nGenerates new guid for ymap entities.\n");
                Console.WriteLine("reset\nResets flags and lodDist.\n");
                Console.WriteLine("missingytd\nReturns the list of missing .ytd foreach ytyp.\n");
                Console.WriteLine("grid\nDivides all the .ymap.xml files into blocks of a given size.\n");
                Console.WriteLine("listsplit\nReads a list of names from a .txt and moves all the archetypes and entities in other files.\n");
                Console.WriteLine("overlapping\nRemoves all the overlapping entities in each .ymap.xml rounding their position by a given precision.\n");
                args = Console.ReadLine().Split();
            }
            if (args.Length != 0)
            {
                DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory());
                Ymap[] ymapfiles = CollectYmaps(dir);
                Ytyp[] ytypfiles = CollectYtps(dir);
                
                switch (args[0])
                {
                    case "merge":
                        Merge(ytypfiles,ymapfiles);
                        break;
                    case "extents":
                        Extents(ytypfiles,ymapfiles);
                        break;
                    case "reset":
                        Reset(ytypfiles,ymapfiles);
                        break;
                    case "move":
                        Move(ymapfiles);
                        break;
                    case "editByName":
                        MoveRotateByName(ymapfiles);
                        break;
                    case "guid":
                        RandomGuid(dir);
                        break;
                    case "missingytd":
                        MissingYtd(ytypfiles,dir);
                        break;
                    case "grid":
                        Grid(ymapfiles);
                        break;
                    case "overlapping":
                        DeleteOverlappingEntities(ymapfiles);
                        break;
                    case "listsplit":
                        RemoveFromList(ytypfiles,ymapfiles);
                        break;
                    case "test":
                        Dictionary<string, List<byte[]>> colours = CollectGrassInstanceColours(ymapfiles);
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

        public static void Grid(Ymap[] ymapfiles)
        {
            if (ymapfiles != null && ymapfiles.Length != 0)
            {
                Console.WriteLine("Insert the size of the blocks:");
                int blocksize = int.Parse(Console.ReadLine());
                for (int i = 0; i < ymapfiles.Length; i++)
                {
                    int k = 1;
                    List<CMapData> grid = ymapfiles[i].CMapData.GridSplitAll(blocksize);
                    foreach (CMapData block in grid)
                    {
                        Ymap tmp = ymapfiles[i];
                        tmp.CMapData = block;

                        XDocument ymapDoc = tmp.WriteXML();
                        ymapDoc.Save(ymapfiles[i].filename.Split('.')[0] + "_" + k.ToString("00") + ".ymap.xml");
                        k++;
                    }
                    Console.WriteLine(ymapfiles[i].filename + " splitted in " + (k - 1) + " blocks.");
                }
            }
        }

        public static void Move(Ymap[] ymapfiles)
        {
            if (ymapfiles != null && ymapfiles.Length != 0)
            {
                Console.WriteLine("Insert the position offset:");
                Vector3 offset = ReadVector3();
                for (int i = 0; i < ymapfiles.Length; i++)
                {
                    ymapfiles[i].CMapData.MoveEntities(offset);
                    XDocument ymapDoc = ymapfiles[i].WriteXML();
                    ymapDoc.Save(ymapfiles[i].filename);
                    Console.WriteLine("Moved all the entities of " + ymapfiles[i].filename);
                }
            }       
        }

        public static void MoveRotateByName(Ymap[] ymapfiles)
        {
            if (ymapfiles != null && ymapfiles.Length != 0)
            {
                Console.WriteLine("Insert archetypeName of the entities to edit:");
                string matchingName = Console.ReadLine();
                Console.WriteLine("Insert the position offset:");
                Vector3 positionOffset = ReadVector3();
                Console.WriteLine("Insert the rotation offset (in degrees):");
                Vector3 rotationOffset = ReadVector3();
                for (int i = 0; i < ymapfiles.Length; i++)
                {
                    int k = ymapfiles[i].CMapData.MoveAndRotateEntitiesByName(matchingName, positionOffset, rotationOffset);
                    XDocument ymapDoc = ymapfiles[i].WriteXML();
                    ymapDoc.Save(ymapfiles[i].filename);
                    Console.WriteLine("Edited " + k + " entities of " + ymapfiles[i].filename);
                }
            }
        }

        public static void Merge(Ytyp[] ytypfiles, Ymap[] ymapfiles)
        {
            if (ytypfiles != null && ytypfiles.Length != 0)
            {
                Ytyp merged_ytyp = Ytyp.Merge(ytypfiles);
                XDocument ytyp_new = merged_ytyp.WriteXML();
                ytyp_new.Save("merged.ytyp.xml");
                Console.WriteLine("Exported merged.ytyp.xml");
            }

            if (ymapfiles != null && ymapfiles.Length != 0)
            {
                Ymap merged_ymap = Ymap.Merge(ymapfiles);
                XDocument ymap_new = merged_ymap.WriteXML();
                ymap_new.Save("merged.ymap.xml");
                Console.WriteLine("Exported merged.ymap.xml");
            }      
        }

        public static void Extents(Ytyp[] ytypfiles, Ymap[] ymapfiles)
        {
            Ytyp merged = null;
            List<CBaseArchetypeDef> archetypeList = null;

            if (ytypfiles != null && ytypfiles.Length != 0)
            {
                merged = Ytyp.Merge(ytypfiles);
                archetypeList = merged.CMapTypes.archetypes;
            }

            if (ymapfiles != null && ymapfiles.Length != 0)
            {
                for(int i = 0; i < ymapfiles.Length; i++)
                {
                    HashSet<string> missing = ymapfiles[i].CMapData.UpdateExtents(archetypeList);
                    if (missing != null && missing.Count > 0)
                    {
                        foreach (string name in missing)
                            Console.WriteLine("Missing CBaseArchetypeDef: " + name);
                    }
                    XDocument ymapDoc = ymapfiles[i].WriteXML();
                    ymapDoc.Save(ymapfiles[i].filename);
                    Console.WriteLine("Updated extents for " + ymapfiles[i].filename);
                    Console.WriteLine("");
                }
            }
        }

        public static void Reset(Ytyp[] ytypfiles, Ymap[] ymapfiles)
        {
            List<CBaseArchetypeDef> archetypeList = null;

            if (ytypfiles != null && ytypfiles.Length != 0)
            {
                for (int i = 0; i < ytypfiles.Length; i++)
                {
                    ytypfiles[i].CMapTypes.UpdatelodDist();
                    foreach (CBaseArchetypeDef arc in ytypfiles[i].CMapTypes.archetypes)
                    {
                        arc.flags = 0;
                        arc.specialAttribute = 0;
                    }

                    XDocument ytypDoc = ytypfiles[i].WriteXML();
                    ytypDoc.Save(ytypfiles[i].filename);
                    Console.WriteLine("Resetted " + ytypfiles[i].filename);
                }
                Ytyp merged = Ytyp.Merge(ytypfiles);
                archetypeList = merged.CMapTypes.archetypes;
            }

            if (ymapfiles != null && ymapfiles.Length != 0)
            {
                for (int i = 0; i < ymapfiles.Length; i++)
                {
                    ymapfiles[i].CMapData.UpdatelodDist(archetypeList);
                    ymapfiles[i].CMapData.block = new block(0, 0, "GTADrifting", "Neos7", "GTADrifting");
                    ymapfiles[i].CMapData.flags = 0;
                    ymapfiles[i].CMapData.contentFlags = 1;
                    foreach (CEntityDef ent in ymapfiles[i].CMapData.entities)
                    {
                        ent.flags = 0;
                        ent.childLodDist = 0;
                    }

                    XDocument ymapDoc = ymapfiles[i].WriteXML();
                    ymapDoc.Save(ymapfiles[i].filename);
                    Console.WriteLine("Resetted " + (ymapfiles[i].filename));
                }
            }
        }

        public static void RandomGuid(DirectoryInfo dir)
        {
            FileInfo[] files = dir.GetFiles("*.ymap.xml");
            if (files.Length == 0)
                Console.WriteLine("No .ymap.xml file found.");
            else
            {
                Random rnd = new Random();
                HashSet<int> guidlist = new HashSet<int>();
                foreach (FileInfo file in files)
                {
                    XDocument doc = XDocument.Load(file.Name);
                    if (doc.Element("CMapData").Element("entities").Elements() != null)
                    {
                        foreach (XElement ent in doc.Element("CMapData").Element("entities").Elements())
                        {
                            int guid = rnd.Next();
                            if (!guidlist.Contains(guid))
                            {
                                guidlist.Add(guid);
                                ent.Element("guid").Attribute("value").Value = guid.ToString();
                            }
                        }
                    }
                    doc.Save(file.Name);
                    Console.WriteLine("Updated guids for " + file.Name);
                }
            }
        }

        public static Ymap[] CollectYmaps(DirectoryInfo dir)
        {
            Ymap[] ymaps = null;
            FileInfo[] files = dir.GetFiles("*.ymap.xml");
            if (files.Length == 0)
                Console.WriteLine("No .ymap.xml file found.");
            else
            {
                ymaps = new Ymap[files.Length];
                for (int i = 0; i < files.Length; i++)
                    ymaps[i] = new Ymap(XDocument.Load(files[i].Name), files[i].Name);
            }
            return ymaps;
        }

        public static Ytyp[] CollectYtps(DirectoryInfo dir)
        {
            Ytyp[] ytyps = null;
            FileInfo[] files = dir.GetFiles("*.ytyp.xml");
            if (files.Length == 0)
                Console.WriteLine("No .ytyp.xml file found.");
            else
            {
                ytyps = new Ytyp[files.Length];
                for (int i = 0; i < files.Length; i++)
                    ytyps[i] = new Ytyp(XDocument.Load(files[i].Name), files[i].Name);
            }
            return ytyps;
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

        public static void MissingYtd(Ytyp[] ytypfiles, DirectoryInfo dir)
        {
            FileInfo[] ytdfiles = dir.GetFiles(".ytd");
            List<string> ytdlist = new List<string>();
            foreach (FileInfo ytd in ytdfiles)
                ytdlist.Add(Path.GetFileNameWithoutExtension(ytd.Name));

            if (ytypfiles != null && ytypfiles.Length != 0)
            {
                for (int i = 0; i < ytypfiles.Length; i++)
                {
                    Console.WriteLine("Missing .ytd for " + ytypfiles[i].filename + ":");
                    List<string> missing = new List<string>();
                    foreach (string entry in ytypfiles[i].CMapTypes.textureDictionaryList())
                    {
                        if (!ytdlist.Contains(entry))
                            missing.Add(entry);
                    }
                    foreach (string missingytd in missing)
                        Console.WriteLine(missingytd + ".ytd");
                }
            }
        }

        public static Dictionary<string,List<byte[]>> CollectGrassInstanceColours(Ymap[] ymapfiles)
        {
            Dictionary<string, List<byte[]>> colours = new Dictionary<string, List<byte[]>>();
            if (ymapfiles != null && ymapfiles.Length != 0)
            {
                for (int i = 0; i < ymapfiles.Length; i++)
                {
                    foreach (GrassInstance batch in ymapfiles[i].CMapData.instancedData.GrassInstanceList)
                        foreach (Instance instance in batch.InstanceList)
                        {
                            if (colours.ContainsKey(batch.archetypeName))
                                colours[batch.archetypeName].Add(instance.Color);
                            else
                                colours.Add(batch.archetypeName, new List<byte[]>() { instance.Color });
                        }
                }
            }
            return colours;
        }

        public static void DeleteOverlappingEntities(Ymap[] ymapfiles)
        {
            if (ymapfiles != null && ymapfiles.Length != 0)
            {
                Console.WriteLine("Insert the number of decimals to round positions and detect duplicates:");
                int precision = int.Parse(Console.ReadLine());

                for (int i = 0; i < ymapfiles.Length; i++)
                {
                    int k = 0;
                    List<CEntityDef> entities_new = new List<CEntityDef>();
                    HashSet<KeyValuePair<string, Vector3>> newlist = new HashSet<KeyValuePair<string, Vector3>>();
                    foreach (CEntityDef entity in ymapfiles[i].CMapData.entities)
                    {
                        Vector3 position = entity.position;
                        position.X = (float)Math.Round(position.X, precision);
                        position.Y = (float)Math.Round(position.Y, precision);
                        position.Z = (float)Math.Round(position.Z, precision);
                        KeyValuePair<string, Vector3> tmp = new KeyValuePair<string, Vector3>(entity.archetypeName, position);
                        if (!newlist.Contains(tmp))
                        {
                            newlist.Add(tmp);
                            entities_new.Add(entity);
                        }
                        else k++;
                    }
                    ymapfiles[i].CMapData.entities = entities_new;
                    XDocument ymapDoc = ymapfiles[i].WriteXML();
                    ymapDoc.Save(ymapfiles[i].filename);
                    Console.WriteLine("Deleted " + k + " entities in " + (ymapfiles[i].filename));
                }
            }
        }

        public static void RemoveFromList(Ytyp[] ytypfiles,Ymap[] ymapfiles)
        {
            List<string> removelist = new List<string>();
            Console.WriteLine("Insert the file to load names from: (ex. list.txt)");
            string filename = Console.ReadLine();
            
            using (StreamReader reader = new StreamReader(filename))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    removelist.Add(line.Trim().ToLower());
                }
            }

            if (ytypfiles != null && ytypfiles.Length != 0)
            {
                for (int i = 0; i < ytypfiles.Length; i++)
                {
                    List<CBaseArchetypeDef> removed_archetypes = ytypfiles[i].CMapTypes.RemoveArchetypesByNames(removelist);
                    XDocument doc = ytypfiles[i].WriteXML();
                    doc.Save(ytypfiles[i].filename);
                    Console.WriteLine("Updated " + (ytypfiles[i].filename));

                    Ytyp removedytyp = ytypfiles[i];
                    removedytyp.CMapTypes.archetypes = removed_archetypes;
                    doc = removedytyp.WriteXML();
                    doc.Save(ytypfiles[i].filename.Split('.')[0] + "_removed.ytyp.xml");
                    Console.WriteLine("Exported " + ytypfiles[i].filename.Split('.')[0] + "_removed.ytyp.xml");

                }
            }

            if (ymapfiles != null && ymapfiles.Length != 0)
            {
                for (int i = 0; i < ymapfiles.Length; i++)
                {
                    List<CEntityDef> removed_entities = ymapfiles[i].CMapData.RemoveEntitiesByNames(removelist);
                    XDocument doc = ymapfiles[i].WriteXML();
                    doc.Save(ymapfiles[i].filename);
                    Console.WriteLine("Updated " + (ymapfiles[i].filename));

                    Ymap removedymap = ymapfiles[i];
                    removedymap.CMapData.entities = removed_entities;
                    doc = removedymap.WriteXML();
                    doc.Save(ymapfiles[i].filename.Split('.')[0] + "_removed.ymap.xml");
                    Console.WriteLine("Exported " + ymapfiles[i].filename.Split('.')[0] + "_removed.ymap.xml");
                }
            }
        }
    }
}
