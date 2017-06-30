using MapTools.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Xml.Linq;

namespace MapTools.Map
{
    public class CMapData
    {
        public string name { get; set; }
        public string parent { get; set; }
        public uint flags { get; set; }
        public uint contentFlags { get; set; }
        public Vector3 streamingExtentsMin { get; set; }
        public Vector3 streamingExtentsMax { get; set; }
        public Vector3 entitiesExtentsMin { get; set; }
        public Vector3 entitiesExtentsMax { get; set; }
        public List<CEntityDef> entities { get; set; }
        public object containerLods { get; set; }
        public object boxOccluders { get; set; }
        public object occludeModels { get; set; }
        public HashSet<string> physicsDictionaries { get; set; }
        public instancedData instancedData;
        public object carGenerators { get; set; }
        public LODLightsSOA LODLightsSOA;
        public DistantLODLightsSOA DistantLODLightsSOA;
        public block block;

        public CMapData(string filename)
        {
            name = filename;
            entities = new List<CEntityDef>();
            UpdateBlock("GTADrifting","Neos7","GTADrifting");
        }

        public CMapData(XElement node)
        {
            entities = new List<CEntityDef>();
            physicsDictionaries = new HashSet<string>();
            name = node.Element("name").Value;
            parent = node.Element("parent").Value;
            flags = uint.Parse(node.Element("flags").Attribute("value").Value);
            streamingExtentsMin = new Vector3(
                float.Parse(node.Element("streamingExtentsMin").Attribute("x").Value),
                float.Parse(node.Element("streamingExtentsMin").Attribute("y").Value),
                float.Parse(node.Element("streamingExtentsMin").Attribute("z").Value)
                );
            streamingExtentsMax = new Vector3(
                float.Parse(node.Element("streamingExtentsMax").Attribute("x").Value),
                float.Parse(node.Element("streamingExtentsMax").Attribute("y").Value),
                float.Parse(node.Element("streamingExtentsMax").Attribute("z").Value)
                );
            entitiesExtentsMin = new Vector3(
                float.Parse(node.Element("entitiesExtentsMin").Attribute("x").Value),
                float.Parse(node.Element("entitiesExtentsMin").Attribute("y").Value),
                float.Parse(node.Element("entitiesExtentsMin").Attribute("z").Value)
                );
            entitiesExtentsMax = new Vector3(
                float.Parse(node.Element("entitiesExtentsMax").Attribute("x").Value),
                float.Parse(node.Element("entitiesExtentsMax").Attribute("y").Value),
                float.Parse(node.Element("entitiesExtentsMax").Attribute("z").Value)
                );

            if (node.Element("entities").Elements() != null && node.Element("entities").Elements().Count() > 0)
            {
                foreach (XElement ent in node.Element("entities").Elements())
                {
                    if (ent.Attribute("type").Value == "CEntityDef")
                        entities.Add(new CEntityDef(ent));
                    else
                        Console.WriteLine("Skipped unsupported entity: " + ent.Attribute("type").Value);
                }
            }

            if (node.Element("physicsDictionaries").Elements() != null && node.Element("physicsDictionaries").Elements().Count() > 0)
            {
                foreach (XElement phDict in node.Element("physicsDictionaries").Elements())
                {
                    if (phDict.Name == "Item")
                        physicsDictionaries.Add(phDict.Value);
                }
            }
            //MISSING CODE :DDDDDDDDDD

            block.name = node.Element("block").Element("name").Value;
            block.exportedBy = node.Element("block").Element("exportedBy").Value;
            block.owner = node.Element("block").Element("owner").Value;
            block.time = node.Element("block").Element("time").Value;
        }

        public XElement WriteXML()
        {
            //CMapData
            XElement CMapDataNode = new XElement("CMapData");

            //name
            XElement nameNode = new XElement("name");
            nameNode.Value = name;
            CMapDataNode.Add(nameNode);

            //parent
            XElement parentNode = new XElement("parent");
            CMapDataNode.Add(parentNode);

            //flags
            XElement flagsNode = new XElement("flags", new XAttribute("value", flags.ToString()));
            CMapDataNode.Add(flagsNode);

            //contentFlags
            XElement contentFlagsNode = new XElement("contentFlags", new XAttribute("value", contentFlags.ToString()));
            CMapDataNode.Add(contentFlagsNode);

            //streamingExtentsMin
            XElement streamingExtentsMinNode = new XElement("streamingExtentsMin",
                new XAttribute("x", streamingExtentsMin.X.ToString()),
                new XAttribute("y", streamingExtentsMin.Y.ToString()),
                new XAttribute("z", streamingExtentsMin.Z.ToString())
                );
            CMapDataNode.Add(streamingExtentsMinNode);

            //streamingExtentsMax
            XElement streamingExtentsMaxNode = new XElement("streamingExtentsMax",
                new XAttribute("x", streamingExtentsMax.X.ToString()),
                new XAttribute("y", streamingExtentsMax.Y.ToString()),
                new XAttribute("z", streamingExtentsMax.Z.ToString())
                );
            CMapDataNode.Add(streamingExtentsMaxNode);

            //entitiesExtentsMin
            XElement entitiesExtentsMinNode = new XElement("entitiesExtentsMin",
                new XAttribute("x", entitiesExtentsMin.X.ToString()),
                new XAttribute("y", entitiesExtentsMin.Y.ToString()),
                new XAttribute("z", entitiesExtentsMin.Z.ToString())
                );
            CMapDataNode.Add(entitiesExtentsMinNode);

            //entitiesExtentsMax
            XElement entitiesExtentsMaxNode = new XElement("entitiesExtentsMax",
                new XAttribute("x", entitiesExtentsMax.X.ToString()),
                new XAttribute("y", entitiesExtentsMax.Y.ToString()),
                new XAttribute("z", entitiesExtentsMax.Z.ToString())
                );
            CMapDataNode.Add(entitiesExtentsMaxNode);

            //entities
            XElement entitiesNode = new XElement("entities");
            CMapDataNode.Add(entitiesNode);

            if (entities != null && entities.Count > 0)
            {
                foreach (CEntityDef entity in entities)
                    entitiesNode.Add(entity.WriteXML());
            }

            //containerLods
            XElement containerLodsNode = new XElement("containerLods");
            CMapDataNode.Add(containerLodsNode);

            //boxOccluders
            XElement boxOccludersNode = new XElement("boxOccluders");
            CMapDataNode.Add(boxOccludersNode);

            //occludeModels
            XElement occludeModelsNode = new XElement("occludeModels");
            CMapDataNode.Add(occludeModelsNode);

            //physicsDictionaries
            XElement physicsDictionariesNode = new XElement("physicsDictionaries");
            CMapDataNode.Add(physicsDictionariesNode);

            if (physicsDictionaries != null && physicsDictionaries.Count > 0)
            {
                foreach (string phDict in physicsDictionaries)
                    physicsDictionariesNode.Add(new XElement("Item",phDict));
            }

            //instancedData
            XElement instancedDataNode = new XElement("instancedData");
            CMapDataNode.Add(instancedDataNode);
            //ImapLink
            XElement ImapLinkNode = new XElement("ImapLink");
            instancedDataNode.Add(ImapLinkNode);
            //PropInstanceList
            XElement PropInstanceListNode = new XElement("PropInstanceList");
            instancedDataNode.Add(PropInstanceListNode);
            //GrassInstanceList
            XElement GrassInstanceListNode = new XElement("GrassInstanceList");
            instancedDataNode.Add(GrassInstanceListNode);

            //timeCycleModifiers
            XElement timeCycleModifiersNode = new XElement("timeCycleModifiers");
            CMapDataNode.Add(timeCycleModifiersNode);

            //carGenerators
            XElement carGeneratorsNode = new XElement("carGenerators");
            CMapDataNode.Add(carGeneratorsNode);

            //LODLightsSOA
            XElement LODLightsSOANode = new XElement("LODLightsSOA");
            CMapDataNode.Add(LODLightsSOANode);
            //direction
            XElement directionNode = new XElement("direction");
            LODLightsSOANode.Add(directionNode);
            //falloff
            XElement falloffNode = new XElement("falloff");
            LODLightsSOANode.Add(falloffNode);
            //falloffExponent
            XElement falloffExponentNode = new XElement("falloffExponent");
            LODLightsSOANode.Add(falloffExponentNode);
            //timeAndStateFlags
            XElement timeAndStateFlagsNode = new XElement("timeAndStateFlags");
            LODLightsSOANode.Add(timeAndStateFlagsNode);
            //hash
            XElement hashNode = new XElement("hash");
            LODLightsSOANode.Add(hashNode);
            //coneInnerAngle
            XElement coneInnerAngleNode = new XElement("coneInnerAngle");
            LODLightsSOANode.Add(coneInnerAngleNode);
            //coneOuterAngleOrCapExt
            XElement coneOuterAngleOrCapExtNode = new XElement("coneOuterAngleOrCapExt");
            LODLightsSOANode.Add(coneOuterAngleOrCapExtNode);
            //coronaIntensity
            XElement coronaIntensityNode = new XElement("coronaIntensity");
            LODLightsSOANode.Add(coronaIntensityNode);

            //DistantLODLightsSOA
            XElement DistantLODLightsSOANode = new XElement("DistantLODLightsSOA");
            CMapDataNode.Add(DistantLODLightsSOANode);
            //position
            XElement positionNode = new XElement("position");
            DistantLODLightsSOANode.Add(positionNode);
            //RGBI
            XElement RGBINode = new XElement("RGBI");
            DistantLODLightsSOANode.Add(RGBINode);
            //numStreetLights
            XElement numStreetLightsNode = new XElement("numStreetLights", new XAttribute("value", 0));
            DistantLODLightsSOANode.Add(numStreetLightsNode);
            //category
            XElement categoryNode = new XElement("category", new XAttribute("value", 0));
            DistantLODLightsSOANode.Add(categoryNode);

            //block
            XElement blockNode = new XElement("block");
            CMapDataNode.Add(blockNode);
            //version
            XElement versionNode = new XElement("version", new XAttribute("value", 0));
            blockNode.Add(versionNode);
            //flags
            XElement blockflagsNode = new XElement("flags", new XAttribute("value", 0));
            blockNode.Add(blockflagsNode);
            //name
            XElement blocknameNode = new XElement("name");
            blocknameNode.Value = block.name;
            blockNode.Add(blocknameNode);
            //exportedBy
            XElement exportedByNode = new XElement("exportedBy");
            exportedByNode.Value = block.exportedBy;
            blockNode.Add(exportedByNode);
            //owner
            XElement ownerNode = new XElement("owner");
            ownerNode.Value = block.owner;
            blockNode.Add(ownerNode);
            //time
            XElement timeNode = new XElement("time");
            timeNode.Value = block.time;
            blockNode.Add(timeNode);

            return CMapDataNode;
        }

        public void UpdateBlock(string name,string exportedby,string owner)
        {
            block.name = name;
            block.exportedBy = exportedby;
            block.owner = owner;
            block.time = DateTime.UtcNow.ToString();
        }

        public void UpdatelodDist(Dictionary<string, CBaseArchetypeDef> archetypes)
        {
            if (archetypes == null || archetypes.Count < 0)
                return;

            foreach(CEntityDef ent in entities)
            {
                CBaseArchetypeDef arc = null;
                archetypes.TryGetValue(ent.archetypeName, out arc);

                if(arc != null)
                    ent.lodDist = 100 + (1.5f * arc.bsRadius);
            }
        }

        public void MoveEntities(Vector3 offset)
        {
            foreach(CEntityDef entity in entities)
                entity.position += offset;
        }

        //USES XYZ ROTATION IN DEGREES
        public int MoveAndRotateEntitiesByName(string entityname, Vector3 positionOffset, Vector3 rotationOffset)
        {
            int i = 0;
            Vector3 radians = rotationOffset * (float)Math.PI / 180;
            Quaternion quaternionOffset = Quaternion.CreateFromYawPitchRoll(radians.Y, radians.X, radians.Z);

            foreach (CEntityDef entity in entities)
            {
                if (entity.archetypeName == entityname)
                {
                    entity.position += positionOffset;
                    entity.rotation = Quaternion.Multiply(entity.rotation, quaternionOffset);
                    i++;
                }
            }
            return i;
        }

        public int MoveAndRotateEntitiesByName(string entityname,Vector3 positionOffset, Quaternion rotationOffset)
        {
            int i = 0;
            foreach (CEntityDef entity in entities)
            {
                if(entity.archetypeName == entityname)
                {
                    entity.position += positionOffset;
                    entity.rotation = Quaternion.Multiply(entity.rotation, rotationOffset);
                    i++;
                }
            }
            return i;
        }

        //UPDATES THE EXTENTS OF A CMAPDATA AND RETURNS NAMES OF THE MISSING ARCHETYPES TO WARN ABOUT INACCURATE CALCULATION
        public HashSet<string> UpdateExtents(Dictionary<string,CBaseArchetypeDef> archetypes)
        {
            HashSet<string> missing = new HashSet<string>();
            Vector3 entMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 entMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            Vector3 strMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 strMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            foreach (CEntityDef entity in entities)
            {
                CBaseArchetypeDef selected = null;
                if(archetypes != null && archetypes.Count > 0)
                    archetypes.TryGetValue(entity.archetypeName,out selected);

                if (selected != null)
                {
                    Vector3 aabbmax = Vector3.Transform(selected.bbMax, entity.rotation);
                    Vector3 aabbmin = Vector3.Transform(selected.bbMin, entity.rotation);
                    Vector3 centroid = Vector3.Transform(selected.bsCentre, entity.rotation);

                    entMax.X = Math.Max(entMax.X, entity.position.X + aabbmax.X + centroid.X);
                    entMax.Y = Math.Max(entMax.Y, entity.position.Y + aabbmax.Y + centroid.Y);
                    entMax.Z = Math.Max(entMax.Z, entity.position.Z + aabbmax.Z + centroid.Z);

                    entMin.X = Math.Min(entMin.X, entity.position.X + aabbmin.X + centroid.X);
                    entMin.Y = Math.Min(entMin.Y, entity.position.Y + aabbmin.Y + centroid.Y);
                    entMin.Z = Math.Min(entMin.Z, entity.position.Z + aabbmin.Z + centroid.Z);

                    strMax.X = Math.Max(strMax.X, entity.position.X + aabbmax.X + centroid.X + entity.lodDist);
                    strMax.Y = Math.Max(strMax.Y, entity.position.Y + aabbmax.Y + centroid.Y + entity.lodDist);
                    strMax.Z = Math.Max(strMax.Z, entity.position.Z + aabbmax.Z + centroid.Z + entity.lodDist);

                    strMin.X = Math.Min(strMin.X, entity.position.X + aabbmin.X + centroid.X - entity.lodDist);
                    strMin.Y = Math.Min(strMin.Y, entity.position.Y + aabbmin.Y + centroid.Y - entity.lodDist);
                    strMin.Z = Math.Min(strMin.Z, entity.position.Z + aabbmin.Z + centroid.Z - entity.lodDist);
                }
                else
                {
                    missing.Add(entity.archetypeName);

                    entMax.X = Math.Max(entMax.X, entity.position.X);
                    entMax.Y = Math.Max(entMax.Y, entity.position.Y);
                    entMax.Z = Math.Max(entMax.Z, entity.position.Z);

                    entMin.X = Math.Min(entMin.X, entity.position.X);
                    entMin.Y = Math.Min(entMin.Y, entity.position.Y);
                    entMin.Z = Math.Min(entMin.Z, entity.position.Z);

                    strMax.X = Math.Max(strMax.X, entity.position.X + entity.lodDist);
                    strMax.Y = Math.Max(strMax.Y, entity.position.Y + entity.lodDist);
                    strMax.Z = Math.Max(strMax.Z, entity.position.Z + entity.lodDist);

                    strMin.X = Math.Min(strMin.X, entity.position.X - entity.lodDist);
                    strMin.Y = Math.Min(strMin.Y, entity.position.Y - entity.lodDist);
                    strMin.Z = Math.Min(strMin.Z, entity.position.Z - entity.lodDist);
                }
            }
            streamingExtentsMax = strMax;
            streamingExtentsMin = strMin;
            entitiesExtentsMax = entMax;
            entitiesExtentsMin = entMin;
            return missing;
        }
    }


    public struct block
    {
        public uint version { get; set; }
        public uint flags { get; set; }
        public string name { get; set; }
        public string exportedBy { get; set; }
        public string owner { get; set; }
        public string time { get; set; }
    }

    public struct DistantLODLightsSOA
    {
        public object position { get; set; }
        public object RGBI { get; set; }
        public object numStreetLights { get; set; }
        public object category { get; set; }
    }

    public struct LODLightsSOA
    {
        public object direction { get; set; }
        public object falloff { get; set; }
        public object falloffExponent { get; set; }
        public object timeAndStateFlags { get; set; }
        public object hash { get; set; }
        public object coneInnerAngle { get; set; }
        public object coneOuterAngleOrCapExt { get; set; }
        public object coronaIntensity { get; set; }
    }

    public struct instancedData
    {
        public object ImapLink { get; set; }
        public object PropInstanceList { get; set; }
        public object GrassInstanceList { get; set; }
    }
}
