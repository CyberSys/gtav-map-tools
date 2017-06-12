using MapTools.Types;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace MapTools.Data
{
    public class Ymap
    {
        public CMapData CMapData { get; set; }

        public Ymap(string filename)
        {
            CMapData = new CMapData(filename);
        }

        public XDocument WriteXML()
        {
            //document
            XDocument doc = new XDocument();
            //declaration
            XDeclaration declaration = new XDeclaration("1.0", "UTF-8", "no");
            doc.Declaration = declaration;
            //CMapData
            doc.Add(CMapData.WriteXML());
            return doc;
        }

        public Ymap(XDocument document)
        {
            CMapData = new CMapData(document.Element("CMapData"));
        }

        public HashSet<string> UpdateExtents(HashSet<CBaseArchetypeDef> archetypeList)
        {
            HashSet<string> missing = CMapData.UpdateExtents(archetypeList);
            if (missing != null && missing.Count > 0)
            {
                Console.WriteLine("WARNING: Some CBaseArchetypeDef are missing, extents might not be accurate.");
                Console.WriteLine("Try copying their .ytyp.xml in the current folder.");
                foreach (string name in missing)
                    Console.WriteLine("Missing CBaseArchetypeDef: " + name);
            }
            return missing;
        }
    }
}
