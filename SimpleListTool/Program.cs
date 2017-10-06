using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;

namespace SimpleListTool
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("How to use the tool:");
                Console.WriteLine("list <(.ydr|.ytd|.ybn|.ydd|.ymap.xml|.ytyp.xml)> (example: list .ydr)");
                Console.WriteLine("missing <file.ytyp.xml> (example: missing myfile.ytyp.xml)");
                Console.WriteLine("useless <file.ytyp.xml> (example: useless myfile.ytyp.xml)");
            }

            DirectoryInfo dir = new DirectoryInfo(Directory.GetCurrentDirectory()); ;
            if (args.Length == 2)
            {
                switch (args[0])
                {
                    case "list":
                        if (args[1] == ".ydr")
                        {
                            using (StreamWriter writer = new StreamWriter("list_ydr.txt"))
                                foreach (FileInfo file in dir.GetFiles("*.ydr"))
                                    writer.WriteLine(Path.GetFileNameWithoutExtension(file.Name));
                        }
                        if (args[1] == ".ytd")
                        {
                            using (StreamWriter writer = new StreamWriter("list_ytd.txt"))
                                foreach (FileInfo file in dir.GetFiles("*.ytd"))
                                    writer.WriteLine(Path.GetFileNameWithoutExtension(file.Name));
                        }
                        if (args[1] == ".ybn")
                        {
                            using (StreamWriter writer = new StreamWriter("list_ybn.txt"))
                                foreach (FileInfo file in dir.GetFiles("*.ybn"))
                                    writer.WriteLine(Path.GetFileNameWithoutExtension(file.Name));
                        }
                        if (args[1] == ".ydd")
                        {
                            using (StreamWriter writer = new StreamWriter("list_ydd.txt"))
                                foreach (FileInfo file in dir.GetFiles("*.ydd"))
                                    writer.WriteLine(Path.GetFileNameWithoutExtension(file.Name));
                        }
                        if (args[1].EndsWith(".ymap.xml"))
                        {
                            HashSet<string> list = new HashSet<string>();
                            XDocument doc = XDocument.Load(args[1]);
                            foreach (XElement ent in doc.Element("CMapData").Element("entities").Elements())
                                list.Add(ent.Element("archetypeName").Value);

                            using (StreamWriter writer = new StreamWriter(args[1].Split('.')[0] + "_entities.txt"))
                                foreach (string file in list)
                                    writer.WriteLine(file);
                        }
                        if (args[1].EndsWith(".ytyp.xml"))
                        {
                            HashSet<string> list = new HashSet<string>();
                            XDocument doc = XDocument.Load(args[1]);
                            foreach (XElement arc in doc.Element("CMapTypes").Element("archetypes").Elements())
                                list.Add(arc.Element("name").Value);

                            using (StreamWriter writer = new StreamWriter(args[1].Split('.')[0] + "_archetypes.txt"))
                                foreach (string file in list)
                                    writer.WriteLine(file);
                        }
                        break;
                    case "missing":
                        if (args[1].EndsWith(".ytyp.xml"))
                        {
                            HashSet<string> list = new HashSet<string>();
                            XDocument doc = XDocument.Load(args[1]);
                            foreach (XElement arc in doc.Element("CMapTypes").Element("archetypes").Elements())
                            {
                                if (arc.Element("assetType").Value == "ASSET_TYPE_DRAWABLE")
                                    list.Add(arc.Element("name").Value.ToLower() + ".ydr");
                                else if (arc.Element("assetType").Value == "ASSET_TYPE_FRAGMENT")
                                    list.Add(arc.Element("name").Value.ToLower() + ".yft");
                                else if (arc.Element("assetType").Value == "ASSET_TYPE_DRAWABLEDICTIONARY")
                                    list.Add(arc.Element("name").Value.ToLower() + ".ydd");
                                else Console.WriteLine("Unsupported assetType" + arc.Element("assetType").Value);

                                list.Add(arc.Element("textureDictionary").Value.ToLower() + ".ytd");
                            }
                            foreach (FileInfo file in ((dir.GetFiles("*.ytd") ?? Enumerable.Empty<FileInfo>()).Concat(dir.GetFiles("*.ydr") ?? Enumerable.Empty<FileInfo>()).Concat(dir.GetFiles("*.yft") ?? Enumerable.Empty<FileInfo>()).Concat(dir.GetFiles("*.ydd") ?? Enumerable.Empty<FileInfo>())))
                                list.Remove(file.Name.ToLower());

                            using (StreamWriter writer = new StreamWriter(args[1].Split('.')[0] + "_missing.txt"))
                                foreach (string s in list)
                                    writer.WriteLine(s);
                        }
                        break;
                    case "useless":
                        if (args[1].EndsWith(".ytyp.xml"))
                        {
                            HashSet<string> list = new HashSet<string>();
                            XDocument doc = XDocument.Load(args[1]);
                            foreach (FileInfo file in ((dir.GetFiles("*.ytd") ?? Enumerable.Empty<FileInfo>()).Concat(dir.GetFiles("*.ydr") ?? Enumerable.Empty<FileInfo>()).Concat(dir.GetFiles("*.yft") ?? Enumerable.Empty<FileInfo>()).Concat(dir.GetFiles("*.ydd") ?? Enumerable.Empty<FileInfo>())))
                                list.Add(file.Name.ToLower());

                            foreach (XElement arc in doc.Element("CMapTypes").Element("archetypes").Elements())
                            {
                                if (arc.Element("assetType").Value == "ASSET_TYPE_DRAWABLE")
                                    list.Remove(arc.Element("name").Value.ToLower() + ".ydr");
                                else if (arc.Element("assetType").Value == "ASSET_TYPE_FRAGMENT")
                                    list.Remove(arc.Element("name").Value.ToLower() + ".yft");
                                else if (arc.Element("assetType").Value == "ASSET_TYPE_DRAWABLEDICTIONARY")
                                    list.Add(arc.Element("name").Value.ToLower() + ".ydd");
                                else Console.WriteLine("Unsupported assetType" + arc.Element("assetType").Value);

                                list.Remove(arc.Element("textureDictionary").Value.ToLower() + ".ytd");
                            }

                            using (StreamWriter writer = new StreamWriter(args[1].Split('.')[0] + "_useless.txt"))
                                foreach (string s in list)
                                    writer.WriteLine(s);
                        }
                        break;
                    default:
                        Console.WriteLine("Wrong input.");
                        break;
                }
            }
        }
    }
}
