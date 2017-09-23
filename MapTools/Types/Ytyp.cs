using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MapTools.Types
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
            //document
            XDocument doc = new XDocument();
            //declaration
            XDeclaration declaration = new XDeclaration("1.0", "UTF-8", "no");
            doc.Declaration = declaration;
            //CMapTypes
            doc.Add(CMapTypes.WriteXML());
            return doc;
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
                    foreach (KeyValuePair<string, CBaseArchetypeDef> archetype in current.CMapTypes.archetypes)
                    {
                        if (!merged.CMapTypes.archetypes.ContainsKey(archetype.Key))
                            merged.CMapTypes.archetypes.Add(archetype.Key, archetype.Value);
                        else
                            Console.WriteLine("Skipped duplicated CBaseArchetypeDef " + archetype.Key);
                    }
                }
            }
            return merged;
        }
    }
}
