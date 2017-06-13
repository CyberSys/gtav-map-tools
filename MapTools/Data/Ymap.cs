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

        public HashSet<string> UpdateExtents(Dictionary<string,CBaseArchetypeDef> archetypeList)
        {
            HashSet<string> missing = CMapData.UpdateExtents(archetypeList);
            if (missing != null && missing.Count > 0)
            {
                //Console.WriteLine("WARNING: Some CBaseArchetypeDef are missing, extents might not be accurate.");
                foreach (string name in missing)
                    Console.WriteLine("Missing CBaseArchetypeDef: " + name);
            }
            return missing;
        }

        public void Normalize(Dictionary<string, CBaseArchetypeDef> archetypeList)
        {
            CMapData.UpdatelodDist(archetypeList);
            CMapData.UpdateBlock("GTADrifting","Neos7","GTADrifting");
            CMapData.flags = 0;
            CMapData.contentFlags = 1;
            foreach (CEntityDef ent in CMapData.entities)
            {
                ent.flags = 0;
            }
            CMapData.UpdateExtents(archetypeList);
        }

        public static Ymap Merge(List<Ymap> list)
        {
            if (list == null || list.Count < 1)
                return null;
            Ymap merged = new Ymap("merged");
            foreach (Ymap current in list)
            {
                if (current.CMapData.entities != null && current.CMapData.entities.Count > 0)
                {
                    foreach (CEntityDef entity in current.CMapData.entities)
                    {
                        if (!merged.CMapData.entities.Contains(entity))
                            merged.CMapData.entities.Add(entity);
                        else
                            Console.WriteLine("Skipped duplicated CEntityDef " + entity.guid);
                    }
                }
            }
            return merged;
        }
    }
}
