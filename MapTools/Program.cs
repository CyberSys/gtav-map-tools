using System;
using System.Collections.Generic;
using MapTools.XML;
using System.Xml.Linq;
using System.IO;
using System.Numerics;
using System.Globalization;
using System.Threading;
using System.Linq;

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
                Console.WriteLine("grasscolor\nReplaces the color of all the instances of all the batches of grass.\n");
                Console.WriteLine("particles\nGenerates batches of grass from particles exported in 3ds.\n");
                args = Console.ReadLine().Split();
            }
            if (args.Length != 0)
            {
                if(args[0] == "particles")
                {
                    BatchesFromParticles();
                }
                else
                {
                    DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory());
                    Ymap[] ymapfiles = CollectYmaps(dir);
                    Ytyp[] ytypfiles = CollectYtyps(dir);

                    switch (args[0])
                    {
                        case "merge":
                            Merge(ytypfiles, ymapfiles);
                            break;
                        case "extents":
                            Extents(ytypfiles, ymapfiles);
                            break;
                        case "reset":
                            Reset(ytypfiles, ymapfiles);
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
                            MissingYtd(ytypfiles, dir);
                            break;
                        case "grid":
                            Grid(ymapfiles);
                            break;
                        case "overlapping":
                            DeleteOverlappingEntities(ymapfiles);
                            break;
                        case "listsplit":
                            RemoveFromList(ytypfiles, ymapfiles);
                            break;
                        case "grasscolor":
                            ReplaceColorInstancedGrass(ymapfiles);
                            break;

                        //TEMP
                        case "mlo":
                            {
                                if (args.Length > 2)
                                    GenerateMLO(args[1], args[2]);
                                else Console.WriteLine("args[1] isn't a ytyp.xml or args[2] isn't a ymap.xml");
                            }
                            break;

                        default:
                            Console.WriteLine(args[0] + " isn't a valid command.");
                            break;
                    }
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
                    List<Ymap> splitted = ymapfiles[i].GridSplitAll(blocksize);
                    foreach (Ymap block in splitted)
                        block.WriteXML().Save(block.filename);
                    Console.WriteLine("{0} splitted in {1} blocks.", ymapfiles[i].filename, splitted.Count);
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
                    ymapfiles[i].MoveYmap(offset);
                    ymapfiles[i].WriteXML().Save(ymapfiles[i].filename);
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
                    int k = ymapfiles[i].MoveAndRotateEntitiesByName(matchingName, positionOffset, rotationOffset);
                    ymapfiles[i].WriteXML().Save(ymapfiles[i].filename);
                    Console.WriteLine("Edited {0} entities of {1}", k, ymapfiles[i].filename);
                }
            }
        }

        public static void Merge(Ytyp[] ytypfiles, Ymap[] ymapfiles)
        {
            if (ytypfiles != null && ytypfiles.Length != 0)
            {
                Ytyp.Merge(ytypfiles).WriteXML().Save("merged.ytyp.xml");
                Console.WriteLine("Exported merged.ytyp.xml");
            }

            if (ymapfiles != null && ymapfiles.Length != 0)
            {
                Ymap.Merge(ymapfiles).WriteXML().Save("merged.ymap.xml");
                Console.WriteLine("Exported merged.ymap.xml");
            }
        }

        public static void Extents(Ytyp[] ytypfiles, Ymap[] ymapfiles)
        {
            List<CBaseArchetypeDef> archetypeList = null;
            if (ytypfiles != null && ytypfiles.Length != 0)
                archetypeList = Ytyp.Merge(ytypfiles).CMapTypes.archetypes;

            if (ymapfiles != null && ymapfiles.Length != 0)
            {
                for (int i = 0; i < ymapfiles.Length; i++)
                {
                    HashSet<string> missing = ymapfiles[i].UpdateExtents(archetypeList);
                    if (missing?.Any() ?? false)
                    {
                        foreach (string name in missing)
                            Console.WriteLine("Missing CBaseArchetypeDef: " + name);
                    }
                    ymapfiles[i].WriteXML().Save(ymapfiles[i].filename);
                    Console.WriteLine("Updated extents for " + ymapfiles[i].filename);
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
                    ytypfiles[i].UpdatelodDist();
                    foreach (CBaseArchetypeDef arc in ytypfiles[i].CMapTypes.archetypes)
                    {
                        arc.flags = 0;
                        arc.specialAttribute = 0;
                    }
                    ytypfiles[i].WriteXML().Save(ytypfiles[i].filename);
                    Console.WriteLine("Resetted " + ytypfiles[i].filename);
                }
                archetypeList = Ytyp.Merge(ytypfiles).CMapTypes.archetypes;
            }

            if (ymapfiles != null && ymapfiles.Length != 0)
            {
                for (int i = 0; i < ymapfiles.Length; i++)
                {
                    ymapfiles[i].UpdatelodDist(archetypeList);
                    ymapfiles[i].CMapData.block = new CBlockDesc(0, 0, "GTADrifting", "Neos7's MapTools", Environment.UserName);
                    ymapfiles[i].CMapData.flags = 0;
                    ymapfiles[i].CMapData.contentFlags = 1;
                    foreach (CEntityDef ent in ymapfiles[i].CMapData.entities)
                    {
                        ent.flags = 0;
                        ent.childLodDist = 0;
                    }
                    ymapfiles[i].WriteXML().Save(ymapfiles[i].filename);
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
                            int guid;
                            do guid = rnd.Next();
                            while (guidlist.Contains(guid));

                            guidlist.Add(guid);
                            ent.Element("guid").Attribute("value").Value = guid.ToString();
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

        public static Ytyp[] CollectYtyps(DirectoryInfo dir)
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
            Console.WriteLine("For the decimal separator use the character '{0}'", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            Console.WriteLine("X:");
            offset.X = float.Parse(Console.ReadLine());
            Console.WriteLine("Y:");
            offset.Y = float.Parse(Console.ReadLine());
            Console.WriteLine("Z:");
            offset.Z = float.Parse(Console.ReadLine());
            return offset;
        }

        public static byte[] ReadRGB()
        {
            byte[] RGB = new byte[3];
            Console.WriteLine("Insert the color you want to use for the instanced grass in RGB format (0-255)");
            Console.WriteLine("R:");
            RGB[0] = byte.Parse(Console.ReadLine());
            Console.WriteLine("G:");
            RGB[1] = byte.Parse(Console.ReadLine());
            Console.WriteLine("B:");
            RGB[2] = byte.Parse(Console.ReadLine());
            return RGB;
        }

        public static void MissingYtd(Ytyp[] ytypfiles, DirectoryInfo dir)
        {
            IEnumerable<string> ytdlist = dir.GetFiles(".ytd").Select(a => Path.GetFileNameWithoutExtension(a.Name));
            if (ytypfiles != null && ytypfiles.Length != 0)
            {
                for (int i = 0; i < ytypfiles.Length; i++)
                {
                    IEnumerable<string> missing = ytypfiles[i].CMapTypes.textureDictionaries().Except(ytdlist.AsEnumerable());
                    Console.WriteLine("Missing .ytd for {0} : {1}", ytypfiles[i].filename, missing.Count().ToString());
                    foreach (string missingytd in missing)
                        Console.WriteLine(missingytd + ".ytd");
                }
            }
        }

        public static Dictionary<string, List<byte[]>> CollectGrassInstanceColors(Ymap[] ymapfiles)
        {
            Dictionary<string, List<byte[]>> colors = new Dictionary<string, List<byte[]>>();
            if (ymapfiles != null && ymapfiles.Length != 0)
            {
                for (int i = 0; i < ymapfiles.Length; i++)
                {
                    foreach (GrassInstance batch in ymapfiles[i].CMapData.instancedData.GrassInstanceList)
                        foreach (Instance instance in batch.InstanceList)
                        {
                            if (colors.ContainsKey(batch.archetypeName))
                                colors[batch.archetypeName].Add(instance.Color);
                            else
                                colors.Add(batch.archetypeName, new List<byte[]>() { instance.Color });
                        }
                }
            }
            return colors;
        }

        public static void ReplaceColorInstancedGrass(Ymap[] ymapfiles)
        {
            if (ymapfiles != null && ymapfiles.Length != 0)
            {
                Console.WriteLine("Insert the minimum RGB values to randomize from");
                byte[] min = ReadRGB();
                Console.WriteLine("Insert the maximum RGB values to randomize from");
                byte[] max = ReadRGB();
                for (int i = 0; i < ymapfiles.Length; i++)
                {
                    ymapfiles[i].EditInstancedGrassColor(min, max);
                    ymapfiles[i].WriteXML().Save(ymapfiles[i].filename);
                    Console.WriteLine("Updated instanced grass color in {0}", ymapfiles[i].filename);
                }
            }
        }

        public static void DeleteOverlappingEntities(Ymap[] ymapfiles)
        {
            if (ymapfiles != null && ymapfiles.Length != 0)
            {
                Console.WriteLine("Insert the minimum distance allowed among entities with same names:");
                Console.WriteLine("For the decimal separator use the character '{0}'", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
                float limit = float.Parse(Console.ReadLine());

                for (int i = 0; i < ymapfiles.Length; i++)
                {
                    int k = 0;
                    List<CEntityDef> allentities = new List<CEntityDef>(ymapfiles[i].CMapData.entities);
                    foreach (CEntityDef entity in allentities)
                    {
                        List<CEntityDef> samenamelist = allentities.Where(a => a.archetypeName == entity.archetypeName).Except(new List<CEntityDef>() { entity }).ToList();

                        foreach (CEntityDef samename in samenamelist)
                        {
                            if (Vector3.Distance(entity.position, samename.position) <= limit)
                            {
                                if (ymapfiles[i].CMapData.entities.Remove(samename))
                                    k++;
                            }
                        }

                    }
                    if (k > 0)
                    {
                        ymapfiles[i].WriteXML().Save(ymapfiles[i].filename);
                        Console.WriteLine("Deleted {0} entities in {1}", k, ymapfiles[i].filename);
                    }
                }
            }
        }

        public static void RemoveFromList(Ytyp[] ytypfiles, Ymap[] ymapfiles)
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
                    List<CBaseArchetypeDef> removed_archetypes = ytypfiles[i].RemoveArchetypesByNames(removelist);
                    ytypfiles[i].WriteXML().Save(ytypfiles[i].filename);
                    Console.WriteLine("Updated " + (ytypfiles[i].filename));

                    Ytyp removedytyp = ytypfiles[i];
                    removedytyp.CMapTypes.archetypes = removed_archetypes;

                    removedytyp.WriteXML().Save(ytypfiles[i].filename.Split('.')[0] + "_removed.ytyp.xml");
                    Console.WriteLine("Exported {0}_removed.ytyp.xml", ytypfiles[i].filename.Split('.')[0]);

                }
            }

            if (ymapfiles != null && ymapfiles.Length != 0)
            {
                for (int i = 0; i < ymapfiles.Length; i++)
                {
                    List<CEntityDef> removed_entities = ymapfiles[i].RemoveEntitiesByNames(removelist);
                    ymapfiles[i].WriteXML().Save(ymapfiles[i].filename);
                    Console.WriteLine("Updated " + (ymapfiles[i].filename));

                    Ymap removedymap = ymapfiles[i];
                    removedymap.CMapData.entities = removed_entities;

                    removedymap.WriteXML().Save(ymapfiles[i].filename.Split('.')[0] + "_removed.ymap.xml");
                    Console.WriteLine("Exported {0}_removed.ymap.xml", ymapfiles[i].filename.Split('.')[0]);
                }
            }
        }

        //TEMP
        public static void GenerateMLO(string ytyppath, string ymappath)
        {
            Console.WriteLine("Insert the name for the MLO:");
            string mloname = Console.ReadLine();

            Ytyp theytyp = new Ytyp(XDocument.Load(ytyppath), mloname);
            Ymap theymap = new Ymap(XDocument.Load(ymappath), mloname);

            if (theytyp != null && theymap != null)
            {

                CMloInstanceDef mloent = new CMloInstanceDef(mloname);
                CMloArchetypeDef mloarc = new CMloArchetypeDef(mloname);

                //COPY VALUES
                mloarc.entities = theymap.CMapData.entities;
                mloarc.lodDist = theytyp.CMapTypes.archetypes.Max(arc => arc.lodDist);
                mloarc.hdTextureDist = theytyp.CMapTypes.archetypes.Max(arc => arc.hdTextureDist);
                mloent.lodDist = theymap.CMapData.entities.Max(ent => ent.lodDist);

                //GET CENTROID OF ENTITIES AND USE IT AS MLO POSITION
                foreach (CEntityDef ent in theymap.CMapData.entities)
                    mloent.position += ent.position;
                mloent.position = mloent.position / theymap.CMapData.entities.Count;

                //CHANGE COORDSYSTEM TO PARENT'S ONE
                foreach (CEntityDef ent in mloarc.entities)
                    ent.position = ent.position - mloent.position;

                //WEIRD WAY OF SAVING BECAUSE I'M ACTUALLY TOO BORED TO FIX ALL THE CODE TO SUPPORT MLO :DDDDDDDDDDD
                theytyp.CMapTypes.name = mloname;
                XDocument doc = theytyp.WriteXML();
                doc.Element("CMapTypes").Element("archetypes").Add(mloarc.WriteXML());
                doc.Save(mloname + ".ytyp.xml");
                Console.WriteLine(mloname + ".ytyp.xml");

                theymap.CMapData.name = mloname;
                theymap.CMapData.entities = new List<CEntityDef>();
                doc = theymap.WriteXML();
                doc.Element("CMapData").Element("entities").Add(mloent.WriteXML());
                doc.Save(mloname + ".ymap.xml");
                Console.WriteLine(mloname + ".ymap.xml");
            }
        }

        public static void BatchesFromParticles()
        {
            List<float[]> particlesInfo = new List<float[]>();
            Console.WriteLine("Insert the name of the file to load");
            string filepath = Console.ReadLine();

            float posXmax = float.MinValue;
            float posXmin = float.MaxValue;
            float posYmax = float.MinValue;
            float posYmin = float.MaxValue;
            float posZmax = float.MinValue;
            float posZmin = float.MaxValue;

            BinaryReader reader = new BinaryReader(File.OpenRead(filepath));
            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {

                float posx = reader.ReadSingle();
                float posy = reader.ReadSingle();
                float posz = reader.ReadSingle();
                float dirx = reader.ReadSingle();
                float diry = reader.ReadSingle();

                float[] pInfo = new float[5] { posx, posy, posz, dirx, diry };

                //Console.WriteLine("POS: {0},{1},{2} DIR:{3},{4}", posx, posy, posz, dirx, diry);
                particlesInfo.Add(pInfo);

                posXmax = Math.Max(posXmax, posx);
                posXmin = Math.Min(posXmin, posx);
                posYmax = Math.Max(posYmax, posy);
                posYmin = Math.Min(posYmin, posy);
                posZmax = Math.Max(posZmax, posz);
                posZmin = Math.Min(posZmin, posz);
            }
            reader.Close();

            if (particlesInfo == null || !particlesInfo.Any())
                return;
            Random rnd = new Random();

            List<string> archetypes = new List<string>() { "proc_brittlebush_01", "proc_desert_sage_01", "proc_drygrasses01", "proc_drygrasses01b", "proc_drygrassfronds01", "proc_dryplantsgrass_01", "proc_dryplantsgrass_02", "proc_dry_plants_01", "proc_forest_grass01", "proc_forest_ivy_01", "proc_grassdandelion01", "proc_grasses01", "proc_grasses01b", "proc_grassfronds01", "proc_grassplantmix_01", "proc_grassplantmix_02", "proc_indian_pbrush_01", "proc_leafybush_01", "proc_leafyplant_01", "proc_lizardtail_01", "proc_lupins_01", "proc_meadowmix_01", "proc_meadowpoppy_01", "proc_sage_01", "proc_scrub_bush01", "proc_sml_reeds_01", "proc_sml_reeds_01b", "proc_sml_reeds_01c", "proc_stones_01", "proc_stones_02", "proc_stones_03", "proc_stones_04", "proc_stones_05", "proc_stones_06", "proc_wildquinine", "prop_dandy_b", "prop_dryweed_001_a", "prop_dryweed_002_a", "prop_fernba", "prop_fernbb", "prop_flowerweed_005_a", "prop_grass_001_a", "prop_grass_ca", "prop_grass_da", "prop_log_aa", "prop_log_ab", "prop_log_ac", "prop_log_ad", "prop_log_ae", "prop_log_af", "prop_saplin_001_b", "prop_saplin_001_c", "prop_saplin_002_b", "prop_saplin_002_c", "prop_small_bushyba", "prop_tall_drygrass_aa", "prop_tall_grass_ba", "prop_thindesertfiller_aa", "prop_weed_001_aa", "prop_weed_002_ba", "urbandryfrnds_01", "urbandrygrass_01", "urbangrnfrnds_01", "urbangrngrass_01", "urbanweeds01", "urbanweeds01_l1", "urbanweeds02", "urbanweeds02_l1" };
            string archetype = archetypes[rnd.Next(0, archetypes.Count()-1)];

            Vector3 batchSize = new Vector3(100, 100, 75);
            Vector3 bbmax = new Vector3(posXmax, posYmax, posZmax);
            Vector3 bbmin = new Vector3(posXmin, posYmin, posZmin);

            Vector3 numblocks = (bbmax - bbmin) / batchSize;
            //Console.WriteLine("{0},{1},{2}",numblocks.X,numblocks.Y,numblocks.Z);
            int blockX = (int)(numblocks.X + 1);
            int blockY = (int)(numblocks.Y + 1);
            int blockZ = (int)(numblocks.Z + 1);

            Ymap map = new Ymap("instancedData");
            map.CMapData.contentFlags = 1088;
            map.CMapData.physicsDictionaries.Add("v_proc1");

            for (int x = -blockX; x <= blockX; x++)
            {
                for (int y = -blockY; y <= blockY; y++)
                {
                    for (int z = -blockZ; z <= blockZ; z++)
                    {
                        IEnumerable<float[]> currentBatch = Enumerable.Empty<float[]>();

                        float maxX = (x + 1) * batchSize.X;
                        float minX = x * batchSize.X;
                        float maxY = (y + 1) * batchSize.Y;
                        float minY = y * batchSize.Y;
                        float maxZ = (z + 1) * batchSize.Z;
                        float minZ = z * batchSize.Z;

                        ///Console.WriteLine("maxX:{0},minX{1},maxY{2},minY{3},maxZ{4},minZ{5}",maxX,minX,maxY,minY,maxZ,minZ);

                        currentBatch = particlesInfo.Where(a => a[0] < maxX && a[0] >= minX && a[1] < maxY && a[1] >= minY && a[2] < maxZ && a[2] >= minZ).ToList();
                       // Console.WriteLine(currentBatch.Count());

                        if (currentBatch?.Any() ?? false)
                        {
                            BatchAABB aabb = new BatchAABB(new Vector4(minX,minY,minZ,0),new Vector4(maxX,maxY,maxZ,0));
                            GrassInstance grassBatch = new GrassInstance(archetype);
                            grassBatch.BatchAABB = aabb;

                            foreach(float[] inst in currentBatch)
                            {
                                Vector4 worldPos = new Vector4(inst[0],inst[1],inst[2],0);
                                Vector4 batchPos = (worldPos - aabb.min) / (aabb.max - aabb.min) * 65535;
                                byte NormalX = (byte)((inst[3] + 1) * 0.5 * 255);
                                byte NormalY = (byte)((inst[4] + 1) * 0.5 * 255);
                                byte[] color = new byte[3] { 150, 150, 150 };
                                byte scale = (byte)rnd.Next(0,255); ;

                                Instance i = new Instance(new ushort[] { (ushort)batchPos.X, (ushort)batchPos.Y, (ushort)batchPos.Z}, NormalX,NormalY, color, scale);
                                grassBatch.InstanceList.Add(i);
                            }
                            map.CMapData.instancedData.GrassInstanceList.Add(grassBatch);
                        }
                    }
                }
            }
            Console.WriteLine("Total batches: {0}", map.CMapData.instancedData.GrassInstanceList.Count);
            Console.WriteLine("Total instances: {0}", particlesInfo.Count);
            map.WriteXML().Save("grass.ymap.xml");
            Console.WriteLine("Exported grass.ymap.xml");
        }
    }
}
