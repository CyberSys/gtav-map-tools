using System;
using System.Globalization;
using System.Numerics;
using System.Xml.Linq;

namespace MapTools.Map
{
    public class CEntityDef : IEquatable<CEntityDef>
    {
        public string archetypeName { get; set; }
        public uint flags { get; set; }
        public uint guid { get; set; }
        public Vector3 position { get; set; }
        public Quaternion rotation { get; set; }
        public float scaleXY { get; set; }
        public float scaleZ { get; set; }
        public int parentIndex { get; set; }
        public float lodDist { get; set; }
        public float childLodDist { get; set; }
        public string lodLevel { get; set; }
        public uint numChildren { get; set; }
        public string priorityLevel { get; set; }
        public object extensions { get; set; } //UNKNOWN
        public int ambientOcclusionMultiplier { get; set; } //CORRECT TYPE?
        public int artificialAmbientOcclusion { get; set; } //CORRECT TYPE?
        public uint tintValue { get; set; } //CORRECT TYPE?

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

            //archetypeName
            XElement archetypeNameNode = new XElement("archetypeName");
            archetypeNameNode.Value = archetypeName;
            CEntityDefNode.Add(archetypeNameNode);

            //flags
            XElement flagsNode = new XElement("flags", new XAttribute("value", flags.ToString()));
            CEntityDefNode.Add(flagsNode);

            //guid
            XElement guidNode = new XElement("guid", new XAttribute("value", guid.ToString()));
            CEntityDefNode.Add(guidNode);

            //position
            XElement positionNode = new XElement("position",
                new XAttribute("x", position.X.ToString()),
                new XAttribute("y", position.Y.ToString()),
                new XAttribute("z", position.Z.ToString())
                );
            CEntityDefNode.Add(positionNode);

            //rotation
            XElement rotationNode = new XElement("rotation",
                new XAttribute("x", rotation.X.ToString()),
                new XAttribute("y", rotation.Y.ToString()),
                new XAttribute("z", rotation.Z.ToString()),
                new XAttribute("w", rotation.W.ToString())
                );
            CEntityDefNode.Add(rotationNode);

            //scaleXY
            XElement scaleXYNode = new XElement("scaleXY", new XAttribute("value", scaleXY.ToString()));
            CEntityDefNode.Add(scaleXYNode);

            //scaleZ
            XElement scaleZNode = new XElement("scaleZ", new XAttribute("value", scaleZ.ToString()));
            CEntityDefNode.Add(scaleZNode);

            //parentIndex
            XElement parentIndexNode = new XElement("parentIndex", new XAttribute("value", parentIndex.ToString()));
            CEntityDefNode.Add(parentIndexNode);

            //lodDist
            XElement lodDistNode = new XElement("lodDist", new XAttribute("value", lodDist.ToString()));
            CEntityDefNode.Add(lodDistNode);

            //childLodDist
            XElement childLodDistNode = new XElement("childLodDist", new XAttribute("value", childLodDist.ToString()));
            CEntityDefNode.Add(childLodDistNode);

            //lodLevel
            XElement lodLevelNode = new XElement("lodLevel");
            lodLevelNode.Value = lodLevel;
            CEntityDefNode.Add(lodLevelNode);

            //numChildren
            XElement numChildrenNode = new XElement("numChildren", new XAttribute("value", numChildren.ToString()));
            CEntityDefNode.Add(numChildrenNode);

            //priorityLevel
            XElement priorityLevelNode = new XElement("priorityLevel");
            priorityLevelNode.Value = priorityLevel;
            CEntityDefNode.Add(priorityLevelNode);

            //extensions
            XElement extensionsNode = new XElement("extensions");
            CEntityDefNode.Add(extensionsNode);

            //ambientOcclusionMultiplier
            XElement ambientOcclusionMultiplierNode = new XElement("ambientOcclusionMultiplier", new XAttribute("value", ambientOcclusionMultiplier.ToString()));
            CEntityDefNode.Add(ambientOcclusionMultiplierNode);

            //artificialAmbientOcclusion
            XElement artificialAmbientOcclusionNode = new XElement("artificialAmbientOcclusion", new XAttribute("value", artificialAmbientOcclusion.ToString()));
            CEntityDefNode.Add(artificialAmbientOcclusionNode);

            //tintValue
            XElement tintValueNode = new XElement("tintValue", new XAttribute("value", tintValue.ToString()));
            CEntityDefNode.Add(tintValueNode);

            return CEntityDefNode;
        }

        public CEntityDef(XElement node)
        {
            archetypeName = node.Element("archetypeName").Value;
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
            lodLevel = node.Element("lodLevel").Value;
            numChildren = uint.Parse(node.Element("numChildren").Attribute("value").Value);
            priorityLevel = node.Element("priorityLevel").Value;
            extensions = node.Element("extensions").Value;
            ambientOcclusionMultiplier = int.Parse(node.Element("ambientOcclusionMultiplier").Attribute("value").Value);
            artificialAmbientOcclusion = int.Parse(node.Element("artificialAmbientOcclusion").Attribute("value").Value);
            tintValue = uint.Parse(node.Element("tintValue").Attribute("value").Value);
        }
    }
}
