using MapTools.Types;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Xml.Linq;

namespace MapTools.Data
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
        public HashSet<CEntityDef> entities { get; set; }
        public object containerLods { get; set; }
        public object boxOccluders { get; set; }
        public object occludeModels { get; set; }
        public object physicsDictionaries { get; set; }
        public instancedData instancedData;
        public object carGenerators { get; set; }
        public LODLightsSOA LODLightsSOA;
        public DistantLODLightsSOA DistantLODLightsSOA;
        public block block;

        public CMapData(string filename)
        {
            name = filename;
            entities = new HashSet<CEntityDef>();
            UpdateBlock();
        }

        public CMapData(XElement node)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            entities = new HashSet<CEntityDef>();
            name = node.Element("name").Value;
            parent = node.Element("parent").Value;
            flags = uint.Parse(node.Element("flags").Attribute("value").Value);
            streamingExtentsMin = new Vector3(
                float.Parse(node.Element("streamingExtentsMin").Attribute("x").Value, nfi),
                float.Parse(node.Element("streamingExtentsMin").Attribute("y").Value, nfi),
                float.Parse(node.Element("streamingExtentsMin").Attribute("z").Value, nfi)
                );
            streamingExtentsMax = new Vector3(
                float.Parse(node.Element("streamingExtentsMax").Attribute("x").Value, nfi),
                float.Parse(node.Element("streamingExtentsMax").Attribute("y").Value, nfi),
                float.Parse(node.Element("streamingExtentsMax").Attribute("z").Value, nfi)
                );
            entitiesExtentsMin = new Vector3(
                float.Parse(node.Element("entitiesExtentsMin").Attribute("x").Value, nfi),
                float.Parse(node.Element("entitiesExtentsMin").Attribute("y").Value, nfi),
                float.Parse(node.Element("entitiesExtentsMin").Attribute("z").Value, nfi)
                );
            entitiesExtentsMax = new Vector3(
                float.Parse(node.Element("entitiesExtentsMax").Attribute("x").Value, nfi),
                float.Parse(node.Element("entitiesExtentsMax").Attribute("y").Value, nfi),
                float.Parse(node.Element("entitiesExtentsMax").Attribute("z").Value, nfi)
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
            //MISSING CODE :DDDDDDDDDD

            block.name = node.Element("block").Element("name").Value;
            block.exportedBy = node.Element("block").Element("exportedBy").Value;
            block.owner = node.Element("block").Element("owner").Value;
            block.time = node.Element("block").Element("time").Value;
        }

        public XElement WriteXML()
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            //CMapData
            XElement CMapDataField = new XElement("CMapData");

            //name
            XElement nameField = new XElement("name");
            nameField.Value = name;
            CMapDataField.Add(nameField);

            //parent
            XElement parentField = new XElement("parent");
            CMapDataField.Add(parentField);

            //flags
            XElement flagsField = new XElement("flags", new XAttribute("value", flags.ToString()));
            CMapDataField.Add(flagsField);

            //contentFlags
            XElement contentFlagsField = new XElement("contentFlags", new XAttribute("value", contentFlags.ToString()));
            CMapDataField.Add(contentFlagsField);

            //streamingExtentsMin
            XElement streamingExtentsMinField = new XElement("streamingExtentsMin",
                new XAttribute("x", streamingExtentsMin.X.ToString(nfi)),
                new XAttribute("y", streamingExtentsMin.Y.ToString(nfi)),
                new XAttribute("z", streamingExtentsMin.Z.ToString(nfi))
                );
            CMapDataField.Add(streamingExtentsMinField);

            //streamingExtentsMax
            XElement streamingExtentsMaxField = new XElement("streamingExtentsMax",
                new XAttribute("x", streamingExtentsMax.X.ToString(nfi)),
                new XAttribute("y", streamingExtentsMax.Y.ToString(nfi)),
                new XAttribute("z", streamingExtentsMax.Z.ToString(nfi))
                );
            CMapDataField.Add(streamingExtentsMaxField);

            //entitiesExtentsMin
            XElement entitiesExtentsMinField = new XElement("entitiesExtentsMin",
                new XAttribute("x", entitiesExtentsMin.X.ToString(nfi)),
                new XAttribute("y", entitiesExtentsMin.Y.ToString(nfi)),
                new XAttribute("z", entitiesExtentsMin.Z.ToString(nfi))
                );
            CMapDataField.Add(entitiesExtentsMinField);

            //entitiesExtentsMax
            XElement entitiesExtentsMaxField = new XElement("entitiesExtentsMax",
                new XAttribute("x", entitiesExtentsMax.X.ToString(nfi)),
                new XAttribute("y", entitiesExtentsMax.Y.ToString(nfi)),
                new XAttribute("z", entitiesExtentsMax.Z.ToString(nfi))
                );
            CMapDataField.Add(entitiesExtentsMaxField);

            //entities
            XElement entitiesField = new XElement("entities");
            CMapDataField.Add(entitiesField);

            if (entities != null && entities.Count > 0)
            {
                foreach (CEntityDef entity in entities)
                    entitiesField.Add(entity.WriteXML());
            }

            //containerLods
            XElement containerLodsField = new XElement("containerLods");
            CMapDataField.Add(containerLodsField);

            //boxOccluders
            XElement boxOccludersField = new XElement("boxOccluders");
            CMapDataField.Add(boxOccludersField);

            //occludeModels
            XElement occludeModelsField = new XElement("occludeModels");
            CMapDataField.Add(occludeModelsField);

            //physicsDictionaries
            XElement physicsDictionariesField = new XElement("physicsDictionaries");
            CMapDataField.Add(physicsDictionariesField);

            //instancedData
            XElement instancedDataField = new XElement("instancedData");
            CMapDataField.Add(instancedDataField);
            //ImapLink
            XElement ImapLinkField = new XElement("ImapLink");
            instancedDataField.Add(ImapLinkField);
            //PropInstanceList
            XElement PropInstanceListField = new XElement("PropInstanceList");
            instancedDataField.Add(PropInstanceListField);
            //GrassInstanceList
            XElement GrassInstanceListField = new XElement("GrassInstanceList");
            instancedDataField.Add(GrassInstanceListField);

            //timeCycleModifiers
            XElement timeCycleModifiersField = new XElement("timeCycleModifiers");
            CMapDataField.Add(timeCycleModifiersField);

            //carGenerators
            XElement carGeneratorsField = new XElement("carGenerators");
            CMapDataField.Add(carGeneratorsField);

            //LODLightsSOA
            XElement LODLightsSOAField = new XElement("LODLightsSOA");
            CMapDataField.Add(LODLightsSOAField);
            //direction
            XElement directionField = new XElement("direction");
            LODLightsSOAField.Add(directionField);
            //falloff
            XElement falloffField = new XElement("falloff");
            LODLightsSOAField.Add(falloffField);
            //falloffExponent
            XElement falloffExponentField = new XElement("falloffExponent");
            LODLightsSOAField.Add(falloffExponentField);
            //timeAndStateFlags
            XElement timeAndStateFlagsField = new XElement("timeAndStateFlags");
            LODLightsSOAField.Add(timeAndStateFlagsField);
            //hash
            XElement hashField = new XElement("hash");
            LODLightsSOAField.Add(hashField);
            //coneInnerAngle
            XElement coneInnerAngleField = new XElement("coneInnerAngle");
            LODLightsSOAField.Add(coneInnerAngleField);
            //coneOuterAngleOrCapExt
            XElement coneOuterAngleOrCapExtField = new XElement("coneOuterAngleOrCapExt");
            LODLightsSOAField.Add(coneOuterAngleOrCapExtField);
            //coronaIntensity
            XElement coronaIntensityField = new XElement("coronaIntensity");
            LODLightsSOAField.Add(coronaIntensityField);

            //DistantLODLightsSOA
            XElement DistantLODLightsSOAField = new XElement("DistantLODLightsSOA");
            CMapDataField.Add(DistantLODLightsSOAField);
            //position
            XElement positionField = new XElement("position");
            DistantLODLightsSOAField.Add(positionField);
            //RGBI
            XElement RGBIField = new XElement("RGBI");
            DistantLODLightsSOAField.Add(RGBIField);
            //numStreetLights
            XElement numStreetLightsField = new XElement("numStreetLights", new XAttribute("value", 0));
            DistantLODLightsSOAField.Add(numStreetLightsField);
            //category
            XElement categoryField = new XElement("category", new XAttribute("value", 0));
            DistantLODLightsSOAField.Add(categoryField);

            //block
            XElement blockField = new XElement("block");
            CMapDataField.Add(blockField);
            //version
            XElement versionField = new XElement("version", new XAttribute("value", 0));
            blockField.Add(versionField);
            //flags
            XElement blockflagsField = new XElement("flags", new XAttribute("value", 0));
            blockField.Add(blockflagsField);
            //name
            XElement blocknameField = new XElement("name");
            blocknameField.Value = block.name;
            blockField.Add(blocknameField);
            //exportedBy
            XElement exportedByField = new XElement("exportedBy");
            exportedByField.Value = block.exportedBy;
            blockField.Add(exportedByField);
            //owner
            XElement ownerField = new XElement("owner");
            ownerField.Value = block.owner;
            blockField.Add(ownerField);
            //time
            XElement timeField = new XElement("time");
            timeField.Value = block.time;
            blockField.Add(timeField);

            return CMapDataField;
        }

        public void UpdateBlock()
        {
            block.name = "GTADrifting";
            block.exportedBy = "Neos7";
            block.owner = "GTADrifting";
            block.time = DateTime.UtcNow.ToString();
        }

        public void MoveEntities(Vector3 offset)
        {
            foreach(CEntityDef entity in entities)
                entity.position += offset;
        }

        //USES XYZ ROTATION IN DEGREES
        public void MoveAndRotateEntitiesByName(string entityname, Vector3 positionOffset, Vector3 rotationOffset)
        {
            Vector3 radians = rotationOffset * (float)Math.PI / 180;
            Quaternion quaternionOffset = Quaternion.CreateFromYawPitchRoll(radians.Y, radians.X, radians.Z);

            foreach (CEntityDef entity in entities)
            {
                if (entity.archetypeName == entityname)
                {
                    entity.position += positionOffset;
                    entity.rotation = Quaternion.Multiply(entity.rotation, quaternionOffset);
                }
            }
        }

        public void MoveAndRotateEntitiesByName(string entityname,Vector3 positionOffset, Quaternion rotationOffset)
        {
            foreach (CEntityDef entity in entities)
            {
                if(entity.archetypeName == entityname)
                {
                    entity.position += positionOffset;
                    entity.rotation = Quaternion.Multiply(entity.rotation, rotationOffset);
                }
            }
        }

        //UPDATES THE EXTENTS OF A CMAPDATA AND RETURNS NAMES OF THE MISSING ARCHETYPES TO WARN ABOUT INACCURATE CALCULATION
        public HashSet<string> UpdateExtents(HashSet<CBaseArchetypeDef> archetypes)
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
                {
                    IEnumerable<CBaseArchetypeDef> query = from arch in archetypes where arch.name == entity.archetypeName select arch;
                    int results = query.Count();
                    if (results > 0)
                    {
                        selected = query.Single();
                        if (results > 1)
                            Console.WriteLine("WARNING: Found duplicated CBaseArchetypeDef: "+selected.name);
                    }
                }

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
