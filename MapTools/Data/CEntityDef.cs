using System;
using System.Globalization;
using System.Numerics;
using System.Xml.Linq;

namespace MapTools.Data
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
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            //CEntityDef
            XElement CEntityDefField = new XElement("Item", new XAttribute("type", "CEntityDef"));

            //archetypeName
            XElement archetypeNameField = new XElement("archetypeName");
            archetypeNameField.Value = archetypeName;
            CEntityDefField.Add(archetypeNameField);

            //flags
            XElement flagsField = new XElement("flags", new XAttribute("value", flags.ToString()));
            CEntityDefField.Add(flagsField);

            //guid
            XElement guidField = new XElement("guid", new XAttribute("value", guid.ToString()));
            CEntityDefField.Add(guidField);

            //position
            XElement positionField = new XElement("position",
                new XAttribute("x", position.X.ToString(nfi)),
                new XAttribute("y", position.Y.ToString(nfi)),
                new XAttribute("z", position.Z.ToString(nfi))
                );
            CEntityDefField.Add(positionField);

            //rotation
            XElement rotationField = new XElement("rotation",
                new XAttribute("x", rotation.X.ToString(nfi)),
                new XAttribute("y", rotation.Y.ToString(nfi)),
                new XAttribute("z", rotation.Z.ToString(nfi)),
                new XAttribute("w", rotation.W.ToString(nfi))
                );
            CEntityDefField.Add(rotationField);

            //scaleXY
            XElement scaleXYField = new XElement("scaleXY", new XAttribute("value", scaleXY.ToString(nfi)));
            CEntityDefField.Add(scaleXYField);

            //scaleZ
            XElement scaleZField = new XElement("scaleZ", new XAttribute("value", scaleZ.ToString(nfi)));
            CEntityDefField.Add(scaleZField);

            //parentIndex
            XElement parentIndexField = new XElement("parentIndex", new XAttribute("value", parentIndex.ToString()));
            CEntityDefField.Add(parentIndexField);

            //lodDist
            XElement lodDistField = new XElement("lodDist", new XAttribute("value", lodDist.ToString(nfi)));
            CEntityDefField.Add(lodDistField);

            //childLodDist
            XElement childLodDistField = new XElement("childLodDist", new XAttribute("value", childLodDist.ToString(nfi)));
            CEntityDefField.Add(childLodDistField);

            //lodLevel
            XElement lodLevelField = new XElement("lodLevel");
            lodLevelField.Value = lodLevel;
            CEntityDefField.Add(lodLevelField);

            //numChildren
            XElement numChildrenField = new XElement("numChildren", new XAttribute("value", numChildren.ToString()));
            CEntityDefField.Add(numChildrenField);

            //priorityLevel
            XElement priorityLevelField = new XElement("priorityLevel");
            priorityLevelField.Value = priorityLevel;
            CEntityDefField.Add(priorityLevelField);

            //extensions
            XElement extensionsField = new XElement("extensions");
            CEntityDefField.Add(extensionsField);

            //ambientOcclusionMultiplier
            XElement ambientOcclusionMultiplierField = new XElement("ambientOcclusionMultiplier", new XAttribute("value", ambientOcclusionMultiplier.ToString()));
            CEntityDefField.Add(ambientOcclusionMultiplierField);

            //artificialAmbientOcclusion
            XElement artificialAmbientOcclusionField = new XElement("artificialAmbientOcclusion", new XAttribute("value", artificialAmbientOcclusion.ToString()));
            CEntityDefField.Add(artificialAmbientOcclusionField);

            //tintValue
            XElement tintValueField = new XElement("tintValue", new XAttribute("value", tintValue.ToString()));
            CEntityDefField.Add(tintValueField);

            return CEntityDefField;
        }

        public CEntityDef(XElement node)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            archetypeName = node.Element("archetypeName").Value;
            flags = uint.Parse(node.Element("flags").Attribute("value").Value);
            guid = uint.Parse(node.Element("guid").Attribute("value").Value);
            position = new Vector3(
                float.Parse(node.Element("position").Attribute("x").Value, nfi),
                float.Parse(node.Element("position").Attribute("y").Value, nfi),
                float.Parse(node.Element("position").Attribute("z").Value, nfi)
                );
            rotation = new Quaternion(
                float.Parse(node.Element("rotation").Attribute("x").Value, nfi),
                float.Parse(node.Element("rotation").Attribute("y").Value, nfi),
                float.Parse(node.Element("rotation").Attribute("z").Value, nfi),
                float.Parse(node.Element("rotation").Attribute("w").Value, nfi)
                );
            scaleXY = float.Parse(node.Element("scaleXY").Attribute("value").Value, nfi);
            scaleZ = float.Parse(node.Element("scaleZ").Attribute("value").Value, nfi);
            parentIndex = int.Parse(node.Element("parentIndex").Attribute("value").Value);
            lodDist = float.Parse(node.Element("lodDist").Attribute("value").Value, nfi);
            childLodDist = float.Parse(node.Element("childLodDist").Attribute("value").Value, nfi);
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
