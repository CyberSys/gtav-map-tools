using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MapTools.XML
{
    public class Ytyp
    {
        public string filename { get; set; }
        public CMapTypes CMapTypes { get; set; }

        public Ytyp(string name)
        {
            filename = name;
            CMapTypes = new CMapTypes(filename);
        }

        public XDocument WriteXML()
        {
            return new XDocument(new XDeclaration("1.0", "UTF-8", "no"), CMapTypes.WriteXML());
        }

        public Ytyp(XDocument document, string name)
        {
            filename = name;
            CMapTypes = new CMapTypes(document.Element("CMapTypes"));
        }

        public static Ytyp Merge(Ytyp[] list)
        {
            if(list == null || list.Length < 1)
                return null;
            Ytyp merged = new Ytyp("merged");
            foreach (Ytyp current in list)
            {
                if(current.CMapTypes.archetypes?.Any() ?? false)
                {
                    foreach (CBaseArchetypeDef archetype in current.CMapTypes.archetypes)
                    {
                        if (!merged.CMapTypes.archetypes.Contains(archetype))
                            merged.CMapTypes.archetypes.Add(archetype);
                        else
                            Console.WriteLine("Skipped duplicated CBaseArchetypeDef " + archetype.name);
                    }
                }
            }
            return merged;
        }

        public void UpdatelodDist()
        {
            foreach (CBaseArchetypeDef arc in CMapTypes.archetypes)
            {
                arc.lodDist = 100 + (1.5f * arc.bsRadius);
                arc.hdTextureDist = 0.75f * arc.lodDist;
            }
        }

        public List<CBaseArchetypeDef> RemoveArchetypesByNames(List<string> removelist)
        {
            List<CBaseArchetypeDef> removed = new List<CBaseArchetypeDef>();
            if (removelist == null || removelist.Count < 1)
                return removed;
            List<CBaseArchetypeDef> archetypes_new = new List<CBaseArchetypeDef>();

            if (CMapTypes.archetypes?.Any() ?? false)
            {
                foreach (CBaseArchetypeDef archetype in CMapTypes.archetypes)
                {
                    if (removelist.Contains(archetype.name))
                        removed.Add(archetype);
                    else
                        archetypes_new.Add(archetype);
                }
            }
            CMapTypes.archetypes = archetypes_new;
            return removed;
        }
    }

    public class CMapTypes
    {
        public object extensions { get; set; } //UNKNOWN
        public List<CBaseArchetypeDef> archetypes { get; set; }
        public string name { get; set; }
        public object dependencies { get; set; } //UNKNOWN
        public object compositeEntityTypes { get; set; } //UNKNOWN

        public IEnumerable<string> textureDictionaries() => archetypes.Select(a => a.textureDictionary).Distinct();

        public CMapTypes(string filename)
        {
            extensions = null;
            archetypes = new List<CBaseArchetypeDef>();
            name = filename;
            dependencies = null;
            compositeEntityTypes = null;
        }

        public CMapTypes(XElement node)
        {
            archetypes = new List<CBaseArchetypeDef>();
            if (node.Element("archetypes").Elements()?.Any() ?? false)
            {
                foreach (XElement arc in node.Element("archetypes").Elements())
                {
                    if (arc.Attribute("type").Value == "CBaseArchetypeDef")
                    {
                        CBaseArchetypeDef a = new CBaseArchetypeDef(arc);
                        archetypes.Add(a);
                    }
                    else if (arc.Attribute("type").Value == "CTimeArchetypeDef")
                    {
                        CTimeArchetypeDef a = new CTimeArchetypeDef(arc);
                        archetypes.Add(a);
                    }
                    else
                        Console.WriteLine("Skipped unsupported archetype: " + arc.Attribute("type").Value);
                }
            }
            name = node.Element("name").Value;
            dependencies = node.Element("dependencies");
            compositeEntityTypes = node.Element("compositeEntityTypes");
        }

        public XElement WriteXML()
        {
            //CMapTypes
            XElement CMapTypesNode = new XElement("CMapTypes");

            //extensions
            XElement extensionsNode = new XElement("extensions");
            CMapTypesNode.Add(extensionsNode);

            //archetypes
            XElement archetypesNode = new XElement("archetypes");
            CMapTypesNode.Add(archetypesNode);

            if (archetypes?.Any() ?? false)
            {
                foreach (var archetype in archetypes)
                {
                    if (archetype.GetType() == typeof(CTimeArchetypeDef))
                        archetypesNode.Add((archetype as CTimeArchetypeDef).WriteXML());
                    /*else if(archetype.GetType() == typeof(CMloArchetypeDef))
                        archetypesNode.Add((archetype as CMloArchetypeDef).WriteXML());*/
                    else
                        archetypesNode.Add(archetype.WriteXML());
                }
            }

            CMapTypesNode.Add(new XElement("name", name));

            //dependencies
            XElement dependenciesNode = new XElement("dependencies");
            CMapTypesNode.Add(dependenciesNode);

            //compositeEntityTypes
            XElement compositeEntityTypesNode = new XElement("compositeEntityTypes");
            CMapTypesNode.Add(compositeEntityTypesNode);

            return CMapTypesNode;
        }
    }
}
