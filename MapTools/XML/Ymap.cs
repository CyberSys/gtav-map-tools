using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Xml.Linq;

namespace MapTools.XML
{
    public class Ymap
    {
        public string filename { get; set; }
        public CMapData CMapData { get; set; }

        public Ymap(string name)
        {
            filename = name;
            CMapData = new CMapData(name);
        }

        public XDocument WriteXML()
        {
            return new XDocument(new XDeclaration("1.0", "UTF-8", "no"), CMapData.WriteXML());
        }

        public Ymap(XDocument document, string name)
        {
            filename = name;
            CMapData = new CMapData(document.Element("CMapData"));
        }

        public static Ymap Merge(Ymap[] list)
        {
            if (list == null || list.Length < 1)
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

                if (current.CMapData.physicsDictionaries != null && current.CMapData.physicsDictionaries.Count > 0)
                {
                    foreach (string physicsDictionary in current.CMapData.physicsDictionaries)
                    {
                        if (!merged.CMapData.physicsDictionaries.Contains(physicsDictionary))
                            merged.CMapData.physicsDictionaries.Add(physicsDictionary);
                        else
                            Console.WriteLine("Skipped duplicated physicsDictionary " + physicsDictionary);
                    }
                }

                if (current.CMapData.instancedData.GrassInstanceList != null && current.CMapData.instancedData.GrassInstanceList.Count > 0)
                {
                    foreach (GrassInstance instance in current.CMapData.instancedData.GrassInstanceList)
                    {
                        if (!merged.CMapData.instancedData.GrassInstanceList.Contains(instance))
                            merged.CMapData.instancedData.GrassInstanceList.Add(instance);
                        else
                            Console.WriteLine("Skipped duplicated GrassInstance Item " + instance.archetypeName);
                    }
                }
            }
            return merged;
        }
    }

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
        public List<carGenerator> carGenerators { get; set; }
        public LODLightsSOA LODLightsSOA;
        public DistantLODLightsSOA DistantLODLightsSOA;
        public block block;

        public CMapData(CMapData map)
        {
            this.name = map.name;
            this.parent = map.parent;
            this.flags = map.flags;
            this.contentFlags = map.contentFlags;
            this.streamingExtentsMin = map.streamingExtentsMin;
            this.streamingExtentsMax = map.streamingExtentsMax;
            this.entitiesExtentsMin = map.entitiesExtentsMin;
            this.entitiesExtentsMax = map.entitiesExtentsMax;
            this.entities = map.entities;
            this.containerLods = map.containerLods;
            this.boxOccluders = map.boxOccluders;
            this.occludeModels = map.occludeModels;
            this.physicsDictionaries = map.physicsDictionaries;
            this.instancedData = map.instancedData;
            this.carGenerators = map.carGenerators;
            this.LODLightsSOA = map.LODLightsSOA;
            this.DistantLODLightsSOA = map.DistantLODLightsSOA;
            this.block = map.block;
        }

        public CMapData(string filename)
        {
            name = filename;
            entities = new List<CEntityDef>();
            physicsDictionaries = new HashSet<string>();
            instancedData = new instancedData();
            instancedData.GrassInstanceList = new List<GrassInstance>();
            block = new block(0, 0, "GTADrifting", "Neos7's MapTools" , Environment.UserName);
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

            CMapDataNode.Add(new XElement("name", name));
            CMapDataNode.Add(new XElement("parent", parent ?? string.Empty));
            CMapDataNode.Add(new XElement("flags", new XAttribute("value", flags.ToString())));
            CMapDataNode.Add(new XElement("contentFlags", new XAttribute("value", contentFlags.ToString())));
            CMapDataNode.Add(new XElement("streamingExtentsMin",
                new XAttribute("x", streamingExtentsMin.X.ToString()),
                new XAttribute("y", streamingExtentsMin.Y.ToString()),
                new XAttribute("z", streamingExtentsMin.Z.ToString())
                ));
            CMapDataNode.Add(new XElement("streamingExtentsMax",
                new XAttribute("x", streamingExtentsMax.X.ToString()),
                new XAttribute("y", streamingExtentsMax.Y.ToString()),
                new XAttribute("z", streamingExtentsMax.Z.ToString())
                ));
            CMapDataNode.Add(new XElement("entitiesExtentsMin",
                new XAttribute("x", entitiesExtentsMin.X.ToString()),
                new XAttribute("y", entitiesExtentsMin.Y.ToString()),
                new XAttribute("z", entitiesExtentsMin.Z.ToString())
                ));
            CMapDataNode.Add(new XElement("entitiesExtentsMax",
                new XAttribute("x", entitiesExtentsMax.X.ToString()),
                new XAttribute("y", entitiesExtentsMax.Y.ToString()),
                new XAttribute("z", entitiesExtentsMax.Z.ToString())
                ));

            //entities
            XElement entitiesNode = new XElement("entities");
            CMapDataNode.Add(entitiesNode);

            if (entities != null && entities.Count > 0)
            {
                foreach (CEntityDef entity in entities)
                    entitiesNode.Add(entity.WriteXML());
            }

            CMapDataNode.Add(new XElement("containerLods"));
            CMapDataNode.Add(new XElement("boxOccluders"));
            CMapDataNode.Add(new XElement("occludeModels"));

            //physicsDictionaries
            XElement physicsDictionariesNode = new XElement("physicsDictionaries");
            CMapDataNode.Add(physicsDictionariesNode);

            if (physicsDictionaries != null && physicsDictionaries.Count > 0)
            {
                foreach (string phDict in physicsDictionaries)
                    physicsDictionariesNode.Add(new XElement("Item", phDict));
            }

            CMapDataNode.Add(instancedData.WriteXML());
            CMapDataNode.Add(new XElement("timeCycleModifiers"));
            CMapDataNode.Add(new XElement("carGenerators"));
            CMapDataNode.Add(LODLightsSOA.WriteXML());
            CMapDataNode.Add(DistantLODLightsSOA.WriteXML());
            CMapDataNode.Add(block.WriteXML());

            return CMapDataNode;
        }

        public void UpdatelodDist(List<CBaseArchetypeDef> archetypes)
        {
            if (archetypes == null || archetypes.Count < 0)
                return;

            foreach (CEntityDef ent in entities)
            {
                CBaseArchetypeDef arc = null;
                IEnumerable<CBaseArchetypeDef> query = (from archetype in archetypes
                                                        where (archetype.name == ent.archetypeName)
                                                        select archetype);
                if (query.Count() > 0)
                    arc = query.Single();
                if (arc != null)
                    ent.lodDist = 100 + (1.5f * arc.bsRadius);
            }
        }

        public void MoveEntities(Vector3 offset)
        {
            foreach (CEntityDef entity in entities)
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

        public int MoveAndRotateEntitiesByName(string entityname, Vector3 positionOffset, Quaternion rotationOffset)
        {
            int i = 0;
            foreach (CEntityDef entity in entities)
            {
                if (entity.archetypeName == entityname)
                {
                    entity.position += positionOffset;
                    entity.rotation = Quaternion.Multiply(entity.rotation, rotationOffset);
                    i++;
                }
            }
            return i;
        }

        //UPDATES THE EXTENTS OF A CMAPDATA AND RETURNS NAMES OF THE MISSING ARCHETYPES TO WARN ABOUT INACCURATE CALCULATION
        public HashSet<string> UpdateExtents(List<CBaseArchetypeDef> archetypes)
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
                    {
                        IEnumerable<CBaseArchetypeDef> query = (from archetype in archetypes
                                                                where (archetype.name == entity.archetypeName)
                                                                select archetype);
                        if (query.Count() > 0)
                            selected = query.Single();
                    }

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

        public List<CEntityDef> RemoveEntitiesByNames(List<string> removelist)
        {
            List<CEntityDef> removed = new List<CEntityDef>();
            if (removelist == null || removelist.Count < 1)
                return removed;
            List<CEntityDef> entities_new = new List<CEntityDef>();

            if (entities != null && entities.Count > 0)
            {
                foreach (CEntityDef entity in entities)
                {
                    if (removelist.Contains(entity.archetypeName))
                        removed.Add(entity);
                    else
                        entities_new.Add(entity);
                }
            }
            this.entities = entities_new;
            return removed;
        }

        public List<CMapData> GridSplitAll(int block_size)
        {
            List<CMapData> grid = new List<CMapData>();
            int size = 8192;
            int numblocks = (size / block_size) + 1;

            for (int x = -numblocks; x <= numblocks; x++)
            {
                for (int y = -numblocks; y <= numblocks; y++)
                {
                    CMapData current = new CMapData(this);

                    if (entities != null && entities.Count > 0)
                    {
                        IEnumerable<CEntityDef> entity_query = (from entity in entities
                                                                where entity.position.X < ((x + 1) * block_size)
                                                                where entity.position.X >= (x * block_size)
                                                                where entity.position.Y < ((y + 1) * block_size)
                                                                where entity.position.Y >= (y * block_size)
                                                                select entity);
                        if (entity_query.Count() > 0)
                            current.entities = entity_query.ToList();
                        else
                            current.entities = new List<CEntityDef>();
                    }
                    if (instancedData.GrassInstanceList != null && instancedData.GrassInstanceList.Count > 0)
                    {
                        IEnumerable<GrassInstance> grass_query = (from batch in instancedData.GrassInstanceList
                                                                  where (((batch.BatchAABB.max) + (batch.BatchAABB.min)) / 2).X < ((x + 1) * block_size)
                                                                  where (((batch.BatchAABB.max) + (batch.BatchAABB.min)) / 2).X >= (x * block_size)
                                                                  where (((batch.BatchAABB.max) + (batch.BatchAABB.min)) / 2).Y < ((y + 1) * block_size)
                                                                  where (((batch.BatchAABB.max) + (batch.BatchAABB.min)) / 2).Y >= (y * block_size)
                                                                  select batch);
                        if (grass_query.Count() > 0)
                            current.instancedData.GrassInstanceList = grass_query.ToList();
                        else
                            current.instancedData.GrassInstanceList = new List<GrassInstance>();
                    }
                    if (current.entities.Count > 0 || current.instancedData.GrassInstanceList.Count > 0)
                        grid.Add(current);
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
            XElement blockNode = new XElement("block");
            blockNode.Add(new XElement("version", new XAttribute("value", version)));
            blockNode.Add(new XElement("flags", new XAttribute("value", flags)));
            blockNode.Add(new XElement("name", name));
            blockNode.Add(new XElement("exportedBy", exportedBy));
            blockNode.Add(new XElement("owner", owner));
            blockNode.Add(new XElement("time", time));
            return blockNode;
        }
    }

    public struct DistantLODLightsSOA
    {
        public object position { get; set; } //Vector3??
        public uint[] RGBI { get; set; }
        public ushort numStreetLights { get; set; }
        public ushort category { get; set; }

        public XElement WriteXML()
        {
            //DistantLODLightsSOA
            XElement DistantLODLightsSOANode = new XElement("DistantLODLightsSOA");
            DistantLODLightsSOANode.Add(new XElement("position"));
            DistantLODLightsSOANode.Add(new XElement("RGBI"));
            DistantLODLightsSOANode.Add(new XElement("numStreetLights", new XAttribute("value", 0)));
            DistantLODLightsSOANode.Add(new XElement("category", new XAttribute("value", 0)));
            return DistantLODLightsSOANode;
        }
    }

    public struct LODLightsSOA
    {
        public object direction { get; set; } //Vector3???
        public float falloff { get; set; }
        public float falloffExponent { get; set; }
        public uint timeAndStateFlags { get; set; }
        public uint[] hash { get; set; }
        public float coneInnerAngle { get; set; }
        public byte[] coneOuterAngleOrCapExt { get; set; }
        public byte[] coronaIntensity { get; set; }

        public XElement WriteXML()
        {
            //LODLightsSOA
            XElement LODLightsSOANode = new XElement("LODLightsSOA");
            LODLightsSOANode.Add(new XElement("direction"));
            LODLightsSOANode.Add(new XElement("falloff"));
            LODLightsSOANode.Add(new XElement("falloffExponent"));
            LODLightsSOANode.Add(new XElement("timeAndStateFlags"));
            LODLightsSOANode.Add(new XElement("hash"));
            LODLightsSOANode.Add(new XElement("coneInnerAngle"));
            LODLightsSOANode.Add(new XElement("coneOuterAngleOrCapExt"));
            LODLightsSOANode.Add(new XElement("coronaIntensity"));

            return LODLightsSOANode;
        }
    }

    public struct instancedData
    {
        public object ImapLink { get; set; } //Is this even used by the game?
        public object PropInstanceList { get; set; } //Is this even used by the game?
        public List<GrassInstance> GrassInstanceList { get; set; }

        public instancedData(XElement node)
        {
            ImapLink = null;
            PropInstanceList = null;
            GrassInstanceList = new List<GrassInstance>();
            foreach (XElement item in node.Element("GrassInstanceList").Elements())
                GrassInstanceList.Add(new GrassInstance(item));
        }

        public XElement WriteXML()
        {
            //instancedData
            XElement instancedDataNode = new XElement("instancedData");
            instancedDataNode.Add(new XElement("ImapLink"));
            instancedDataNode.Add(new XElement("PropInstanceList"));

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

    public struct GrassInstance
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
            archetypeName = node.Element("archetypeName").Value.ToLower();
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

            ItemNode.Add(new XElement("BatchAABB",
                new XElement("min",
                    new XAttribute("x", BatchAABB.min.X.ToString()),
                    new XAttribute("y", BatchAABB.min.Y.ToString()),
                    new XAttribute("z", BatchAABB.min.Z.ToString()),
                    new XAttribute("w", BatchAABB.min.W.ToString())
                    ),
                new XElement("max",
                    new XAttribute("x", BatchAABB.max.X.ToString()),
                    new XAttribute("y", BatchAABB.max.Y.ToString()),
                    new XAttribute("z", BatchAABB.max.Z.ToString()),
                    new XAttribute("w", BatchAABB.max.W.ToString())
                    )
                 ));
            ItemNode.Add(new XElement("ScaleRange",
                new XAttribute("x", ScaleRange.X.ToString()),
                new XAttribute("y", ScaleRange.Y.ToString()),
                new XAttribute("z", ScaleRange.Z.ToString())
                ));
            ItemNode.Add(new XElement("archetypeName", archetypeName));
            ItemNode.Add(new XElement("lodDist", new XAttribute("value", lodDist)));
            ItemNode.Add(new XElement("LodFadeStartDist", new XAttribute("value", LodFadeStartDist)));
            ItemNode.Add(new XElement("LodInstFadeRange", new XAttribute("value", LodInstFadeRange)));
            ItemNode.Add(new XElement("OrientToTerrain", new XAttribute("value", OrientToTerrain)));

            //InstanceList
            XElement InstanceListNode = new XElement("InstanceList");
            ItemNode.Add(InstanceListNode);

            if (InstanceList != null && InstanceList.Count > 0)
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
        public byte NormalX { get; set; }
        public byte NormalY { get; set; }
        public byte[] Color { get; set; }
        public byte Scale { get; set; }
        public byte Ao { get; set; }
        public byte[] Pad { get; set; }

        public Instance(XElement node)
        {
            Position = new ushort[3] {
                ushort.Parse(node.Element("Position").Value.Split('\n')[1]),
                ushort.Parse(node.Element("Position").Value.Split('\n')[2]),
                ushort.Parse(node.Element("Position").Value.Split('\n')[3])};
            NormalX = byte.Parse(node.Element("NormalX").Attribute("value").Value);
            NormalY = byte.Parse(node.Element("NormalY").Attribute("value").Value);
            Color = new byte[3] {
                byte.Parse(node.Element("Color").Value.Split('\n')[1]),
                byte.Parse(node.Element("Color").Value.Split('\n')[2]),
                byte.Parse(node.Element("Color").Value.Split('\n')[3])};
            Scale = byte.Parse(node.Element("Scale").Attribute("value").Value); ;
            Ao = byte.Parse(node.Element("Ao").Attribute("value").Value); ;
            Pad = new byte[3] {
                byte.Parse(node.Element("Pad").Value.Split('\n')[1]),
                byte.Parse(node.Element("Pad").Value.Split('\n')[2]),
                byte.Parse(node.Element("Pad").Value.Split('\n')[3])};
        }

        public XElement WriteXML()
        {
            string PositionNode = (Environment.NewLine + (new string(' ', 14)) + Position[0] + Environment.NewLine + (new string(' ', 14)) + Position[1] + Environment.NewLine + (new string(' ', 14)) + Position[2] + Environment.NewLine + (new string(' ', 12)));
            string ColorNode = (Environment.NewLine + (new string(' ', 14)) + Color[0] + Environment.NewLine + (new string(' ', 14)) + Color[1] + Environment.NewLine + (new string(' ', 14)) + Color[2] + Environment.NewLine + (new string(' ', 12)));
            string PadNode = (Environment.NewLine + (new string(' ', 14)) + Pad[0] + Environment.NewLine + (new string(' ', 14)) + Pad[1] + Environment.NewLine + (new string(' ', 14)) + Pad[2] + Environment.NewLine + (new string(' ', 12)));
            //Item
            XElement ItemNode = new XElement("Item");
            ItemNode.Add(new XElement("Position", new XAttribute("content", "short_array"), PositionNode));
            ItemNode.Add(new XElement("NormalX", new XAttribute("value", NormalX)));
            ItemNode.Add(new XElement("NormalY", new XAttribute("value", NormalY)));
            ItemNode.Add(new XElement("Color", new XAttribute("content", "char_array"), ColorNode));
            ItemNode.Add(new XElement("Scale", new XAttribute("value", Scale)));
            ItemNode.Add(new XElement("Ao", new XAttribute("value", Ao)));
            ItemNode.Add(new XElement("Pad", new XAttribute("content", "char_array"), PadNode));
            return ItemNode;
        }
    }

    public struct BatchAABB
    {
        public Vector4 min { get; set; }
        public Vector4 max { get; set; }

        public BatchAABB(Vector4 batchmin, Vector4 batchmax)
        {
            min = batchmin;
            max = batchmax;
        }
    }

    public struct carGenerator
    {
        public Vector3 position { get; set; }
        public float orientX { get; set; }
        public float orientY { get; set; }
        public float perpendicularLength { get; set; }
        public string carModel { get; set; }
        public uint flags { get; set; }
        public int bodyColorRemap1 { get; set; }
        public int bodyColorRemap2 { get; set; }
        public int bodyColorRemap3 { get; set; }
        public int bodyColorRemap4 { get; set; }
        public object popGroup { get; set; }
        public sbyte livery { get; set; }
    }
}
