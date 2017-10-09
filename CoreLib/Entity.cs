using System;
using System.Numerics;
using System.Xml.Linq;

namespace CoreLib
{
    public enum lodLevel : int
    {
        LODTYPES_DEPTH_HD = 0,
        LODTYPES_DEPTH_LOD = 1,
        LODTYPES_DEPTH_SLOD1 = 2,
        LODTYPES_DEPTH_SLOD2 = 3,
        LODTYPES_DEPTH_SLOD3 = 4,
        LODTYPES_DEPTH_ORPHANHD = 5,
        LODTYPES_DEPTH_SLOD4 = 6,
    };
    public enum priorityLevel : int
    {
        PRI_REQUIRED = 0,
        PRI_OPTIONAL_HIGH = 1,
        PRI_OPTIONAL_MEDIUM = 2,
        PRI_OPTIONAL_LOW = 3,
    };

    public class CEntityDef : IEquatable<CEntityDef>
    {
        public string archetypeName { get; set; }
        public uint flags { get; set; }
        public uint guid { get; set; }
        public Vector3 position { get; set; }
        public Quaternion rotation { get; set; } //Vector4
        public float scaleXY { get; set; }
        public float scaleZ { get; set; }
        public int parentIndex { get; set; }
        public float lodDist { get; set; }
        public float childLodDist { get; set; }
        public lodLevel lodLevel { get; set; }
        public uint numChildren { get; set; }
        public priorityLevel priorityLevel { get; set; }
        public object extensions { get; set; } //UNKNOWN
        public int ambientOcclusionMultiplier { get; set; }
        public int artificialAmbientOcclusion { get; set; }
        public uint tintValue { get; set; }

        public bool Equals(CEntityDef obj)
        {
            return this.guid.Equals(obj.guid);
        }

        public override int GetHashCode()
        {
            return guid.GetHashCode();
        }

        public XElement WriteXML()
        {
            //CEntityDef
            XElement CEntityDefNode = new XElement("Item", new XAttribute("type", "CEntityDef"));
            CEntityDefNode.Add(new XElement("archetypeName", archetypeName));
            CEntityDefNode.Add(new XElement("flags", new XAttribute("value", flags.ToString())));
            CEntityDefNode.Add(new XElement("guid", new XAttribute("value", guid.ToString())));
            CEntityDefNode.Add(new XElement("position",
                new XAttribute("x", position.X.ToString()),
                new XAttribute("y", position.Y.ToString()),
                new XAttribute("z", position.Z.ToString())
                ));
            CEntityDefNode.Add(new XElement("rotation",
                new XAttribute("x", rotation.X.ToString()),
                new XAttribute("y", rotation.Y.ToString()),
                new XAttribute("z", rotation.Z.ToString()),
                new XAttribute("w", rotation.W.ToString())
                ));
            CEntityDefNode.Add(new XElement("scaleXY", new XAttribute("value", scaleXY.ToString())));
            CEntityDefNode.Add(new XElement("scaleZ", new XAttribute("value", scaleZ.ToString())));
            CEntityDefNode.Add(new XElement("parentIndex", new XAttribute("value", parentIndex.ToString())));
            CEntityDefNode.Add(new XElement("lodDist", new XAttribute("value", lodDist.ToString())));
            CEntityDefNode.Add(new XElement("childLodDist", new XAttribute("value", childLodDist.ToString())));
            CEntityDefNode.Add(new XElement("lodLevel", lodLevel));
            CEntityDefNode.Add(new XElement("numChildren", new XAttribute("value", numChildren.ToString())));
            CEntityDefNode.Add(new XElement("priorityLevel", priorityLevel));
            CEntityDefNode.Add(new XElement("extensions"));
            CEntityDefNode.Add(new XElement("ambientOcclusionMultiplier", new XAttribute("value", ambientOcclusionMultiplier.ToString())));
            CEntityDefNode.Add(new XElement("artificialAmbientOcclusion", new XAttribute("value", artificialAmbientOcclusion.ToString())));
            CEntityDefNode.Add(new XElement("tintValue", new XAttribute("value", tintValue.ToString())));

            return CEntityDefNode;
        }

        public CEntityDef(XElement node)
        {
            archetypeName = node.Element("archetypeName").Value.ToLower();
            flags = uint.Parse(node.Element("flags").Attribute("value").Value);
            guid = uint.Parse(node.Element("guid").Attribute("value").Value);
            position = new Vector3(
                float.Parse(node.Element("position").Attribute("x").Value),
                float.Parse(node.Element("position").Attribute("y").Value),
                float.Parse(node.Element("position").Attribute("z").Value)
                );
            rotation = new Quaternion(
                float.Parse(node.Element("rotation").Attribute("x").Value),
                float.Parse(node.Element("rotation").Attribute("y").Value),
                float.Parse(node.Element("rotation").Attribute("z").Value),
                float.Parse(node.Element("rotation").Attribute("w").Value)
                );
            scaleXY = float.Parse(node.Element("scaleXY").Attribute("value").Value);
            scaleZ = float.Parse(node.Element("scaleZ").Attribute("value").Value);
            parentIndex = int.Parse(node.Element("parentIndex").Attribute("value").Value);
            lodDist = float.Parse(node.Element("lodDist").Attribute("value").Value);
            childLodDist = float.Parse(node.Element("childLodDist").Attribute("value").Value);
            lodLevel = (lodLevel)Enum.Parse(typeof(lodLevel),node.Element("lodLevel").Value);
            numChildren = uint.Parse(node.Element("numChildren").Attribute("value").Value);
            priorityLevel = (priorityLevel)Enum.Parse(typeof(priorityLevel),node.Element("priorityLevel").Value);
            extensions = node.Element("extensions").Value;
            ambientOcclusionMultiplier = int.Parse(node.Element("ambientOcclusionMultiplier").Attribute("value").Value);
            artificialAmbientOcclusion = int.Parse(node.Element("artificialAmbientOcclusion").Attribute("value").Value);
            tintValue = uint.Parse(node.Element("tintValue").Attribute("value").Value);
        }
    }
    public class CMloInstanceDef : CEntityDef
    {
        public uint groupId { get; set; }
        public uint floorId { get; set; }
        public object defaultEntitySets { get; set; } //Array_uint??
        public uint numExitPortals { get; set; }
        public uint MLOInstflags { get; set; }

        public CMloInstanceDef(XElement node) : base(node)
        {
            groupId = uint.Parse(node.Element("groupId").Attribute("value").Value);
            floorId = uint.Parse(node.Element("floorId").Attribute("value").Value);
            defaultEntitySets = null; //Temp
            numExitPortals = uint.Parse(node.Element("numExitPortals").Attribute("value").Value);
            MLOInstflags = uint.Parse(node.Element("MLOInstflags").Attribute("value").Value);
        }

        public new XElement WriteXML()
        {
            XElement CMloInstanceDefNode = base.WriteXML();
            CMloInstanceDefNode.Attribute("type").Value = "CMloInstanceDef";
            CMloInstanceDefNode.Add(new XElement("groupId", new XAttribute("value", groupId.ToString())));
            CMloInstanceDefNode.Add(new XElement("floorId", new XAttribute("value", floorId.ToString())));
            CMloInstanceDefNode.Add(new XElement("defaultEntitySets")); //Temp
            CMloInstanceDefNode.Add(new XElement("numExitPortals", new XAttribute("value", numExitPortals.ToString())));
            CMloInstanceDefNode.Add(new XElement("MLOInstflags", new XAttribute("value", MLOInstflags.ToString())));
            return CMloInstanceDefNode;
        }
    }
}
