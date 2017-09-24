using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MapTools.Types
{
    public class CMapTypes
    {
        public object extensions { get; set; } //UNKNOWN
        public List<CBaseArchetypeDef> archetypes { get; set; }
        public string name { get; set; }
        public object dependencies { get; set; } //UNKNOWN
        public object compositeEntityTypes { get; set; } //UNKNOWN

        public CMapTypes(string filename)
        {
            archetypes = new List<CBaseArchetypeDef>();
            name = filename;
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
                arc.hdTextureDist = 100 + arc.bsRadius;
            }
        }

        public List<CBaseArchetypeDef> RemoveArchetypesByNames(List<string> removelist)
        {
            List<CBaseArchetypeDef> removed = new List<CBaseArchetypeDef>();
            if (removelist == null || removelist.Count > 0)
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

            //name
            XElement nameNode = new XElement("name");
            nameNode.Value = name;
            CMapTypesNode.Add(nameNode);

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
