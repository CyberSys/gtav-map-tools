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
            physicsDictionaries = new HashSet<string>();
            instancedData = new instancedData();
            instancedData.GrassInstanceList = new List<GrassInstance>();
            block = new block(0,0,"GTADrifting","Neos7","GTADrifting");
        }

        public CMapData(XElement node)
        {
            name = node.Element("name").Value;
            parent = node.Element("parent").Value;
            flags = uint.Parse(node.Element("flags").Attribute("value").Value);
            contentFlags = uint.Parse(node.Element("contentFlags").Attribute("value").Value);
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

            entities = new List<CEntityDef>();
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

            //containerLods
            //boxOccluders
            //occludeModels

            physicsDictionaries = new HashSet<string>();
            if (node.Element("physicsDictionaries").Elements() != null && node.Element("physicsDictionaries").Elements().Count() > 0)
            {
                foreach (XElement phDict in node.Element("physicsDictionaries").Elements())
                {
                    if (phDict.Name == "Item")
                        physicsDictionaries.Add(phDict.Value);
                }
            }

            instancedData = new instancedData(node.Element("instancedData"));

            //carGenerators
            //LODLightsSOA
            //DistantLODLightsSOA

            block = new block(node.Element("block"));
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
            if(parent != null)
                parentNode.Value = parent;
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
            XElement instancedDataNode = instancedData.WriteXML();
            CMapDataNode.Add(instancedDataNode);

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
            XElement blockNode = block.WriteXML();
            CMapDataNode.Add(blockNode);

            return CMapDataNode;
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

            if(entities != null)
            {
                foreach (CEntityDef entity in entities)
                {
                    
                    CBaseArchetypeDef selected = null;
                    if (archetypes != null && archetypes.Count > 0)
                        archetypes.TryGetValue(entity.archetypeName, out selected);

                    float lodDist = entity.lodDist;

                    if (selected != null)
                    {
                        if (entity.lodDist <= 0.0f)
                            lodDist = selected.lodDist;

                        Vector3 aabbmax = Vector3.Transform(selected.bbMax, entity.rotation);
                        Vector3 aabbmin = Vector3.Transform(selected.bbMin, entity.rotation);
                        Vector3 centroid = Vector3.Transform(selected.bsCentre, entity.rotation);

                        entMax.X = Math.Max(entMax.X, entity.position.X + aabbmax.X + centroid.X);
                        entMax.Y = Math.Max(entMax.Y, entity.position.Y + aabbmax.Y + centroid.Y);
                        entMax.Z = Math.Max(entMax.Z, entity.position.Z + aabbmax.Z + centroid.Z);

                        entMin.X = Math.Min(entMin.X, entity.position.X + aabbmin.X + centroid.X);
                        entMin.Y = Math.Min(entMin.Y, entity.position.Y + aabbmin.Y + centroid.Y);
                        entMin.Z = Math.Min(entMin.Z, entity.position.Z + aabbmin.Z + centroid.Z);

                        strMax.X = Math.Max(strMax.X, entity.position.X + aabbmax.X + centroid.X + lodDist);
                        strMax.Y = Math.Max(strMax.Y, entity.position.Y + aabbmax.Y + centroid.Y + lodDist);
                        strMax.Z = Math.Max(strMax.Z, entity.position.Z + aabbmax.Z + centroid.Z + lodDist);

                        strMin.X = Math.Min(strMin.X, entity.position.X + aabbmin.X + centroid.X - lodDist);
                        strMin.Y = Math.Min(strMin.Y, entity.position.Y + aabbmin.Y + centroid.Y - lodDist);
                        strMin.Z = Math.Min(strMin.Z, entity.position.Z + aabbmin.Z + centroid.Z - lodDist);
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

                        strMax.X = Math.Max(strMax.X, entity.position.X + lodDist);
                        strMax.Y = Math.Max(strMax.Y, entity.position.Y + lodDist);
                        strMax.Z = Math.Max(strMax.Z, entity.position.Z + lodDist);

                        strMin.X = Math.Min(strMin.X, entity.position.X - lodDist);
                        strMin.Y = Math.Min(strMin.Y, entity.position.Y - lodDist);
                        strMin.Z = Math.Min(strMin.Z, entity.position.Z - lodDist);
                    }
                }
            }

            if (instancedData.GrassInstanceList != null)
            {
                foreach (GrassInstance item in instancedData.GrassInstanceList)
                {

                    entMax.X = Math.Max(entMax.X, item.BatchAABB.max.X);
                    entMax.Y = Math.Max(entMax.Y, item.BatchAABB.max.Y);
                    entMax.Z = Math.Max(entMax.Z, item.BatchAABB.max.Z);

                    entMin.X = Math.Min(entMin.X, item.BatchAABB.min.X);
                    entMin.Y = Math.Min(entMin.Y, item.BatchAABB.min.Y);
                    entMin.Z = Math.Min(entMin.Z, item.BatchAABB.min.Z);

                    strMax.X = Math.Max(strMax.X, item.BatchAABB.max.X + item.lodDist);
                    strMax.Y = Math.Max(strMax.Y, item.BatchAABB.max.Y + item.lodDist);
                    strMax.Z = Math.Max(strMax.Z, item.BatchAABB.max.Z + item.lodDist - 100); //IDK WHY

                    strMin.X = Math.Min(strMin.X, item.BatchAABB.min.X - item.lodDist);
                    strMin.Y = Math.Min(strMin.Y, item.BatchAABB.min.Y - item.lodDist);
                    strMin.Z = Math.Min(strMin.Z, item.BatchAABB.min.Z - item.lodDist + 100); //IDK WHY
                }
            }

            streamingExtentsMax = strMax;
            streamingExtentsMin = strMin;
            entitiesExtentsMax = entMax;
            entitiesExtentsMin = entMin;
            return missing;
        }

        /* NEW VERSION WIP
        public HashSet<string> UpdateExtents_NEW(Dictionary<string, CBaseArchetypeDef> archetypes)
        {
            HashSet<string> missing = new HashSet<string>();
            Vector3 entMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 entMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);
            Vector3 strMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 strMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            if (entities != null)
            {
                foreach (CEntityDef entity in entities)
                {

                    CBaseArchetypeDef selected = null;
                    if (archetypes != null && archetypes.Count > 0)
                        archetypes.TryGetValue(entity.archetypeName, out selected);

                    float lodDist = entity.lodDist;

                    if (selected != null)
                    {
                        if (entity.lodDist <= 0.0f)
                            lodDist = selected.lodDist;

                        Vector3 aabbmax = selected.bbMax;
                        Vector3 aabbmin = selected.bbMin;
                        Vector3 centroid = selected.bsCentre;

                        Vector3[] entBox = new Vector3[8];
                        entBox[0] = aabbmin;
                        entBox[1] = new Vector3(aabbmin.X * entity.scaleXY, aabbmin.Y * entity.scaleXY, aabbmax.Z * entity.scaleZ);
                        entBox[2] = new Vector3(aabbmin.X * entity.scaleXY, aabbmax.Y * entity.scaleXY, aabbmin.Z * entity.scaleZ);
                        entBox[3] = new Vector3(aabbmin.X * entity.scaleXY, aabbmax.Y * entity.scaleXY, aabbmax.Z * entity.scaleZ);
                        entBox[4] = new Vector3(aabbmax.X * entity.scaleXY, aabbmin.Y * entity.scaleXY, aabbmin.Z * entity.scaleZ);
                        entBox[5] = new Vector3(aabbmax.X * entity.scaleXY, aabbmin.Y * entity.scaleXY, aabbmax.Z * entity.scaleZ);
                        entBox[6] = new Vector3(aabbmax.X * entity.scaleXY, aabbmax.Y * entity.scaleXY, aabbmin.Z * entity.scaleZ);
                        entBox[7] = aabbmax;

                        Vector3 strBoxMax = aabbmax + (new Vector3(lodDist, lodDist, lodDist));
                        Vector3 strBoxMin = aabbmin - (new Vector3(lodDist, lodDist, lodDist));

                        Vector3[] strBox = new Vector3[8];
                        strBox[0] = strBoxMin;
                        strBox[1] = new Vector3(strBoxMin.X * entity.scaleXY, strBoxMin.Y * entity.scaleXY, strBoxMax.Z * entity.scaleZ);
                        strBox[2] = new Vector3(strBoxMin.X * entity.scaleXY, strBoxMax.Y * entity.scaleXY, strBoxMin.Z * entity.scaleZ);
                        strBox[3] = new Vector3(strBoxMin.X * entity.scaleXY, strBoxMax.Y * entity.scaleXY, strBoxMax.Z * entity.scaleZ);
                        strBox[4] = new Vector3(strBoxMax.X * entity.scaleXY, strBoxMin.Y * entity.scaleXY, strBoxMin.Z * entity.scaleZ);
                        strBox[5] = new Vector3(strBoxMax.X * entity.scaleXY, strBoxMin.Y * entity.scaleXY, strBoxMax.Z * entity.scaleZ);
                        strBox[6] = new Vector3(strBoxMax.X * entity.scaleXY, strBoxMax.Y * entity.scaleXY, strBoxMin.Z * entity.scaleZ);
                        strBox[7] = strBoxMax;

                        for (int i = 0; i < 8; i++)
                        {
                            entMax.X = Math.Max(entMax.X, Vector3.Transform(entBox[i], entity.rotation).X);
                            entMax.Y = Math.Max(entMax.Y, Vector3.Transform(entBox[i], entity.rotation).Y);
                            entMax.Z = Math.Max(entMax.Z, Vector3.Transform(entBox[i], entity.rotation).Z);

                            entMin.X = Math.Max(entMin.X, Vector3.Transform(entBox[i], entity.rotation).X);
                            entMin.Y = Math.Max(entMin.Y, Vector3.Transform(entBox[i], entity.rotation).Y);
                            entMin.Z = Math.Max(entMin.Z, Vector3.Transform(entBox[i], entity.rotation).Z);

                            strMax.X = Math.Max(strMax.X, Vector3.Transform(strBox[i], entity.rotation).X);
                            strMax.Y = Math.Max(strMax.Y, Vector3.Transform(strBox[i], entity.rotation).Y);
                            strMax.Z = Math.Max(strMax.Z, Vector3.Transform(strBox[i], entity.rotation).Z);

                            strMin.X = Math.Max(strMin.X, Vector3.Transform(strBox[i], entity.rotation).X);
                            strMin.Y = Math.Max(strMin.Y, Vector3.Transform(strBox[i], entity.rotation).Y);
                            strMin.Z = Math.Max(strMin.Z, Vector3.Transform(strBox[i], entity.rotation).Z);
                        }
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

                        strMax.X = Math.Max(strMax.X, entity.position.X + lodDist);
                        strMax.Y = Math.Max(strMax.Y, entity.position.Y + lodDist);
                        strMax.Z = Math.Max(strMax.Z, entity.position.Z + lodDist);

                        strMin.X = Math.Min(strMin.X, entity.position.X - lodDist);
                        strMin.Y = Math.Min(strMin.Y, entity.position.Y - lodDist);
                        strMin.Z = Math.Min(strMin.Z, entity.position.Z - lodDist);
                    }
                }
            }

            if (instancedData.GrassInstanceList != null)
            {
                foreach (GrassInstance item in instancedData.GrassInstanceList)
                {

                    entMax.X = Math.Max(entMax.X, item.BatchAABB.max.X);
                    entMax.Y = Math.Max(entMax.Y, item.BatchAABB.max.Y);
                    entMax.Z = Math.Max(entMax.Z, item.BatchAABB.max.Z);

                    entMin.X = Math.Min(entMin.X, item.BatchAABB.min.X);
                    entMin.Y = Math.Min(entMin.Y, item.BatchAABB.min.Y);
                    entMin.Z = Math.Min(entMin.Z, item.BatchAABB.min.Z);

                    strMax.X = Math.Max(strMax.X, item.BatchAABB.max.X + item.lodDist);
                    strMax.Y = Math.Max(strMax.Y, item.BatchAABB.max.Y + item.lodDist);
                    strMax.Z = Math.Max(strMax.Z, item.BatchAABB.max.Z + item.lodDist - 100); //IDK WHY

                    strMin.X = Math.Min(strMin.X, item.BatchAABB.min.X - item.lodDist);
                    strMin.Y = Math.Min(strMin.Y, item.BatchAABB.min.Y - item.lodDist);
                    strMin.Z = Math.Min(strMin.Z, item.BatchAABB.min.Z - item.lodDist + 100); //IDK WHY
                }
            }

            streamingExtentsMax = strMax;
            streamingExtentsMin = strMin;
            entitiesExtentsMax = entMax;
            entitiesExtentsMin = entMin;
            return missing;
        }*/

        //SPLITS THE ENTITIES IN A GRID ACCORDING TO THEIR POSITION
        public List<List<CEntityDef>> GridSplit(int block_size)
        {
            List<List<CEntityDef>> grid = new List<List<CEntityDef>>();
            int size = 8192;
            int numblocks = (size / block_size) + 1;

            for (int x = -numblocks; x <= numblocks; x++)
            {
                for (int y = -numblocks; y <= numblocks; y++)
                {
                    IEnumerable<CEntityDef> result = (from entity in entities
                                                  where entity.position.X < ((x + 1) * block_size)
                                                  where entity.position.X >= (x * block_size)
                                                  where entity.position.Y < ((y + 1) * block_size)
                                                  where entity.position.Y >= (y * block_size)
                                                  select entity);
                    if (result.Count() > 0)
                        grid.Add(result.ToList());
                }
            }
            return grid;
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

        public block(uint blockversion, uint blockflags, string blockname, string blockexportedby, string blockowner)
        {
            version = blockversion;
            flags = blockflags;
            name = blockname;
            exportedBy = blockexportedby;
            owner = blockowner;
            time = DateTime.UtcNow.ToString();
        }

        public block(XElement node)
        {
            version = uint.Parse(node.Element("version").Attribute("value").Value);
            flags = uint.Parse(node.Element("flags").Attribute("value").Value);
            name = node.Element("name").Value;
            exportedBy = node.Element("exportedBy").Value;
            owner = node.Element("owner").Value;
            time = node.Element("time").Value;
        }

        public XElement WriteXML()
        {
            //block
            XElement blockNode = new XElement("block");
            
            //version
            XElement versionNode = new XElement("version", new XAttribute("value", 0));
            blockNode.Add(versionNode);
            //flags
            XElement blockflagsNode = new XElement("flags", new XAttribute("value", 0));
            blockNode.Add(blockflagsNode);
            //name
            XElement blocknameNode = new XElement("name");
            blocknameNode.Value = name;
            blockNode.Add(blocknameNode);
            //exportedBy
            XElement exportedByNode = new XElement("exportedBy");
            exportedByNode.Value = exportedBy;
            blockNode.Add(exportedByNode);
            //owner
            XElement ownerNode = new XElement("owner");
            ownerNode.Value = owner;
            blockNode.Add(ownerNode);
            //time
            XElement timeNode = new XElement("time");
            timeNode.Value = time;
            blockNode.Add(timeNode);

            return blockNode;
        }
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
        public List<GrassInstance> GrassInstanceList { get; set; }

        public instancedData(XElement node)
        {
            ImapLink = null; //TEMP
            PropInstanceList = null; //TEMP
            GrassInstanceList = new List<GrassInstance>();
            foreach (XElement item in node.Element("GrassInstanceList").Elements())
                GrassInstanceList.Add(new GrassInstance(item));
        }

        public XElement WriteXML()
        {
            //instancedData
            XElement instancedDataNode = new XElement("instancedData");

            //ImapLink
            XElement ImapLinkNode = new XElement("ImapLink");
            instancedDataNode.Add(ImapLinkNode);
            //PropInstanceList
            XElement PropInstanceListNode = new XElement("PropInstanceList");
            instancedDataNode.Add(PropInstanceListNode);
            //GrassInstanceList
            XElement GrassInstanceListNode = new XElement("GrassInstanceList");
            instancedDataNode.Add(GrassInstanceListNode);

            if (GrassInstanceList != null && GrassInstanceList.Count > 0)
            {
                foreach (GrassInstance GrassInstanceItem in GrassInstanceList)
                    GrassInstanceListNode.Add(GrassInstanceItem.WriteXML());
            }

            return instancedDataNode;
        }
    }

    public class GrassInstance
    {
        public BatchAABB BatchAABB { get; set; }
        public Vector3 ScaleRange { get; set; }
        public string archetypeName { get; set; }
        public float lodDist { get; set; }
        public float LodFadeStartDist { get; set; }
        public float LodInstFadeRange { get; set; }
        public float OrientToTerrain { get; set; }
        public List<Instance> InstanceList { get; set; }

        public GrassInstance(XElement node)
        {
            BatchAABB = new BatchAABB(
                new Vector4(
                    float.Parse(node.Element("BatchAABB").Element("min").Attribute("x").Value),
                    float.Parse(node.Element("BatchAABB").Element("min").Attribute("y").Value),
                    float.Parse(node.Element("BatchAABB").Element("min").Attribute("z").Value),
                    float.Parse(node.Element("BatchAABB").Element("min").Attribute("w").Value)),
                new Vector4(
                    float.Parse(node.Element("BatchAABB").Element("max").Attribute("x").Value),
                    float.Parse(node.Element("BatchAABB").Element("max").Attribute("y").Value),
                    float.Parse(node.Element("BatchAABB").Element("max").Attribute("z").Value),
                    float.Parse(node.Element("BatchAABB").Element("max").Attribute("w").Value)));
            ScaleRange = new Vector3(
                float.Parse(node.Element("ScaleRange").Attribute("x").Value),
                float.Parse(node.Element("ScaleRange").Attribute("y").Value),
                float.Parse(node.Element("ScaleRange").Attribute("z").Value));
            archetypeName = node.Element("archetypeName").Value;
            lodDist = float.Parse(node.Element("lodDist").Attribute("value").Value);
            LodFadeStartDist = float.Parse(node.Element("LodFadeStartDist").Attribute("value").Value);
            LodInstFadeRange = float.Parse(node.Element("LodInstFadeRange").Attribute("value").Value);
            OrientToTerrain = float.Parse(node.Element("OrientToTerrain").Attribute("value").Value);
            InstanceList = new List<Instance>();
            foreach (XElement item in node.Element("InstanceList").Elements())
                InstanceList.Add(new Instance(item));
        }

        public XElement WriteXML()
        {
            //Item
            XElement ItemNode = new XElement("Item");

            //BatchAABB
            XElement BatchAABBNode = new XElement("BatchAABB");
            XElement minNode = new XElement("min",
                new XAttribute("x", BatchAABB.min.X.ToString()),
                new XAttribute("y", BatchAABB.min.Y.ToString()),
                new XAttribute("z", BatchAABB.min.Z.ToString()),
                new XAttribute("w", BatchAABB.min.W.ToString())
                );
            BatchAABBNode.Add(minNode);
            XElement maxNode = new XElement("max",
                new XAttribute("x", BatchAABB.max.X.ToString()),
                new XAttribute("y", BatchAABB.max.Y.ToString()),
                new XAttribute("z", BatchAABB.max.Z.ToString()),
                new XAttribute("w", BatchAABB.max.W.ToString())
                );
            BatchAABBNode.Add(maxNode);
            ItemNode.Add(BatchAABBNode);

            //ScaleRange
            XElement ScaleRangeNode = new XElement("ScaleRange",
                new XAttribute("x", ScaleRange.X.ToString()),
                new XAttribute("y", ScaleRange.Y.ToString()),
                new XAttribute("z", ScaleRange.Z.ToString())
                );
            ItemNode.Add(ScaleRangeNode);

            //archetypeName
            XElement archetypeNameNode = new XElement("archetypeName");
            archetypeNameNode.Value = archetypeName;
            ItemNode.Add(archetypeNameNode);

            //lodDist
            XElement lodDistNode = new XElement("lodDist", new XAttribute("value", lodDist));
            ItemNode.Add(lodDistNode);

            //LodFadeStartDist
            XElement LodFadeStartDistNode = new XElement("LodFadeStartDist", new XAttribute("value", LodFadeStartDist));
            ItemNode.Add(LodFadeStartDistNode);

            //LodInstFadeRange
            XElement LodInstFadeRangeNode = new XElement("LodInstFadeRange", new XAttribute("value", LodInstFadeRange));
            ItemNode.Add(LodInstFadeRangeNode);

            //OrientToTerrain
            XElement OrientToTerrainNode = new XElement("OrientToTerrain", new XAttribute("value", OrientToTerrain));
            ItemNode.Add(OrientToTerrainNode);

            //InstanceList
            XElement InstanceListNode = new XElement("InstanceList");
            ItemNode.Add(InstanceListNode);

            if(InstanceList != null && InstanceList.Count > 0)
            {
                foreach (Instance item in InstanceList)
                    InstanceListNode.Add(item.WriteXML());
            }
            
            return ItemNode;
        }
    }

    public struct Instance
    {
        public ushort[] Position { get; set; }
        public int NormalX { get; set; }
        public int NormalY { get; set; }
        public int[] Color { get; set; }
        public int Scale { get; set; }
        public int Ao { get; set; }
        public int[] Pad { get; set; }

        public Instance(XElement node)
        {
            Position = new ushort[3] {
                ushort.Parse(node.Element("Position").Value.Split('\n')[1]),
                ushort.Parse(node.Element("Position").Value.Split('\n')[2]),
                ushort.Parse(node.Element("Position").Value.Split('\n')[3])};
            NormalX = int.Parse(node.Element("NormalX").Attribute("value").Value);
            NormalY = int.Parse(node.Element("NormalY").Attribute("value").Value);
            Color = new int[3] {
                int.Parse(node.Element("Color").Value.Split('\n')[1]),
                int.Parse(node.Element("Color").Value.Split('\n')[2]),
                int.Parse(node.Element("Color").Value.Split('\n')[3])};
            Scale = int.Parse(node.Element("Scale").Attribute("value").Value); ;
            Ao = int.Parse(node.Element("Ao").Attribute("value").Value); ;
            Pad = new int[3] {
                int.Parse(node.Element("Pad").Value.Split('\n')[1]),
                int.Parse(node.Element("Pad").Value.Split('\n')[2]),
                int.Parse(node.Element("Pad").Value.Split('\n')[3])};
        }

        public XElement WriteXML()
        {
            //Item
            XElement ItemNode = new XElement("Item");

            //Position
            XElement PositionNode = new XElement("Position",new XAttribute("content","short_array"));
            PositionNode.Value = ("\n              " + Position[0]+ "\n              " + Position[1] + "\n              " + Position[2] + "\n            ");
            ItemNode.Add(PositionNode);

            //NormalX
            XElement NormalXNode = new XElement("NormalX", new XAttribute("value", NormalX));
            ItemNode.Add(NormalXNode);

            //NormalY
            XElement NormalYNode = new XElement("NormalY", new XAttribute("value", NormalY));
            ItemNode.Add(NormalYNode);

            //Color
            XElement ColorNode = new XElement("Color", new XAttribute("content", "char_array"));
            ColorNode.Value = ("\n              " + Color[0] + "\n              " + Color[1] + "\n              " + Color[2] + "\n            ");
            ItemNode.Add(ColorNode);

            //Scale
            XElement ScaleNode = new XElement("Scale", new XAttribute("value", Scale));
            ItemNode.Add(ScaleNode);

            //Ao
            XElement AoNode = new XElement("Ao", new XAttribute("value", Ao));
            ItemNode.Add(AoNode);

            //Pad
            XElement PadNode = new XElement("Pad", new XAttribute("content", "char_array"));
            PadNode.Value = ("\n              " + Pad[0] + "\n              " + Pad[1] + "\n              " + Pad[2] + "\n            ");
            ItemNode.Add(PadNode);

            return ItemNode;
        }
    }

    public struct BatchAABB
    {
        public Vector4 min { get; set; }
        public Vector4 max { get; set; }

        public BatchAABB(Vector4 batchmin,Vector4 batchmax)
        {
            min = batchmin;
            max = batchmax;
        }
    }
}
