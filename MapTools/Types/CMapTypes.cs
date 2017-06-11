using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace MapTools.Types
{
    public class CMapTypes
    {
        public object extensions { get; set; } //UNKNOWN
        public HashSet<CBaseArchetypeDef> archetypes { get; set; }
        public string name { get; set; }
        public object dependencies { get; set; } //UNKNOWN
        public object compositeEntityTypes { get; set; } //UNKNOWN

        public CMapTypes(string filename)
        {
            archetypes = new HashSet<CBaseArchetypeDef>();
            name = filename;
        }

        public CMapTypes(XElement node)
        {
            archetypes = new HashSet<CBaseArchetypeDef>();
            if (node.Element("archetypes").Elements() != null && node.Element("archetypes").Elements().Count() > 0)
            {
                foreach (XElement arc in node.Element("archetypes").Elements())
                {
                    if (arc.Attribute("type").Value == "CBaseArchetypeDef")
                        archetypes.Add(new CBaseArchetypeDef(arc));
                }
            }
            name = node.Element("name").Value;
            dependencies = node.Element("dependencies");
            compositeEntityTypes = node.Element("compositeEntityTypes");
        }

        public XElement WriteXML()
        {
            //CMapTypes
            XElement CMapTypesField = new XElement("CMapTypes");

            //extensions
            XElement extensionsField = new XElement("extensions");
            CMapTypesField.Add(extensionsField);

            //archetypes
            XElement archetypesField = new XElement("archetypes");
            CMapTypesField.Add(archetypesField);

            if (archetypes != null && archetypes.Count > 0)
            {
                foreach (CBaseArchetypeDef archetype in archetypes)
                    archetypesField.Add(archetype.WriteXML());
            }

            //name
            XElement nameField = new XElement("name");
            nameField.Value = name;
            CMapTypesField.Add(nameField);

            //dependencies
            XElement dependenciesField = new XElement("dependencies");
            CMapTypesField.Add(dependenciesField);

            //compositeEntityTypes
            XElement compositeEntityTypesField = new XElement("compositeEntityTypes");
            CMapTypesField.Add(compositeEntityTypesField);

            return CMapTypesField;
        }
    }
}
