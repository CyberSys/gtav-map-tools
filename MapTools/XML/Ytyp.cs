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
                if(current.CMapTypes.archetypes != null && current.CMapTypes.archetypes.Count > 0)
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
    }

    public class CMapTypes
    {
        public object extensions { get; set; } //UNKNOWN
        public List<CBaseArchetypeDef> archetypes { get; set; }
        public string name { get; set; }
        public object dependencies { get; set; } //UNKNOWN
        public object compositeEntityTypes { get; set; } //UNKNOWN

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
            if (node.Element("archetypes").Elements() != null && node.Element("archetypes").Elements().Count() > 0)
            {
                foreach (XElement arc in node.Element("archetypes").Elements())
                {
                    if (arc.Attribute("type").Value == "CBaseArchetypeDef")
                    {
                        CBaseArchetypeDef a = new CBaseArchetypeDef(arc);
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

        public void UpdatelodDist()
        {
            foreach (CBaseArchetypeDef arc in archetypes)
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

            if (archetypes != null && archetypes.Count > 0)
            {
                foreach (CBaseArchetypeDef archetype in archetypes)
                {
                    if (removelist.Contains(archetype.name))
                        removed.Add(archetype);
                    else
                        archetypes_new.Add(archetype);
                }
            }
            this.archetypes = archetypes_new;
            return removed;
        }

        public List<string> textureDictionaryList()
        {
            List<string> txdList = new List<string>();
            foreach (CBaseArchetypeDef archetype in archetypes)
            {
                if (!txdList.Contains(archetype.textureDictionary))
                    txdList.Add(archetype.textureDictionary);
            }
            return txdList;
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

            if (archetypes != null && archetypes.Count > 0)
            {
                foreach (CBaseArchetypeDef archetype in archetypes)
                    archetypesNode.Add(archetype.WriteXML());
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
