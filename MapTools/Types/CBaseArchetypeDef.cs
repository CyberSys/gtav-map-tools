using System;
using System.Globalization;
using System.Numerics;
using System.Xml.Linq;

namespace MapTools.Types
{
    public class CBaseArchetypeDef : IEquatable<CBaseArchetypeDef>
    {
        public float lodDist { get; set; }
        public uint flags { get; set; }
        public uint specialAttribute { get; set; }
        public Vector3 bbMin { get; set; }
        public Vector3 bbMax { get; set; }
        public Vector3 bsCentre { get; set; }
        public float bsRadius { get; set; }
        public float hdTextureDist { get; set; }
        public string name { get; set; }
        public string textureDictionary { get; set; }
        public string clipDictionary { get; set; }
        public string drawableDictionary { get; set; }
        public string physicsDictionary { get; set; }
        public string assetType { get; set; }
        public string assetName { get; set; }
        public object extensions { get; set; } //UNKNOWN

        public bool Equals(CBaseArchetypeDef obj)
        {
            return this.name.Equals(obj.name);
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        public XElement WriteXML()
        {
            //CBaseArchetypeDef
            XElement CBaseArchetypeDefNode = new XElement("Item", new XAttribute("type", "CBaseArchetypeDef"));

            //lodDist
            XElement lodDistNode = new XElement("lodDist", new XAttribute("value", lodDist.ToString()));
            CBaseArchetypeDefNode.Add(lodDistNode);

            //flags
            XElement flagsNode = new XElement("flags", new XAttribute("value", flags.ToString()));
            CBaseArchetypeDefNode.Add(flagsNode);

            //specialAttribute
            XElement specialAttributeNode = new XElement("specialAttribute", new XAttribute("value", specialAttribute.ToString()));
            CBaseArchetypeDefNode.Add(specialAttributeNode);

            //bbMin
            XElement bbMinNode = new XElement("bbMin",
                new XAttribute("x", bbMin.X.ToString()),
                new XAttribute("y", bbMin.Y.ToString()),
                new XAttribute("z", bbMin.Z.ToString())
                );
            CBaseArchetypeDefNode.Add(bbMinNode);

            //bbMax
            XElement bbMaxFiled = new XElement("bbMax",
                new XAttribute("x", bbMax.X.ToString()),
                new XAttribute("y", bbMax.Y.ToString()),
                new XAttribute("z", bbMax.Z.ToString())
                );
            CBaseArchetypeDefNode.Add(bbMaxFiled);

            //bsCentre
            XElement bsCentreNode = new XElement("bsCentre",
                new XAttribute("x", bsCentre.X.ToString()),
                new XAttribute("y", bsCentre.Y.ToString()),
                new XAttribute("z", bsCentre.Z.ToString())
                );
            CBaseArchetypeDefNode.Add(bsCentreNode);

            //bsRadius
            XElement bsRadiusNode = new XElement("bsRadius", new XAttribute("value", bsRadius.ToString()));
            CBaseArchetypeDefNode.Add(bsRadiusNode);

            //hdTextureDist
            XElement hdTextureDistNode = new XElement("hdTextureDist", new XAttribute("value", hdTextureDist.ToString()));
            CBaseArchetypeDefNode.Add(hdTextureDistNode);

            //name
            XElement nameNode = new XElement("name");
            nameNode.Value = name;
            CBaseArchetypeDefNode.Add(nameNode);

            //textureDictionary
            XElement textureDictionaryNode = new XElement("textureDictionary");
            textureDictionaryNode.Value = textureDictionary;
            CBaseArchetypeDefNode.Add(textureDictionaryNode);

            //clipDictionary
            XElement clipDictionaryNode = new XElement("clipDictionary");
            if (clipDictionary != null)
                clipDictionaryNode.Value = clipDictionary;
            CBaseArchetypeDefNode.Add(clipDictionaryNode);

            //drawableDictionary
            XElement drawableDictionaryNode = new XElement("drawableDictionary");
            if (drawableDictionary != null)
                drawableDictionaryNode.Value = drawableDictionary;
            CBaseArchetypeDefNode.Add(drawableDictionaryNode);

            //physicsDictionary
            XElement physicsDictionaryNode = new XElement("physicsDictionary");
            if (physicsDictionary != null)
                physicsDictionaryNode.Value = physicsDictionary;
            CBaseArchetypeDefNode.Add(physicsDictionaryNode);

            //assetType
            XElement assetTypeNode = new XElement("assetType");
            assetTypeNode.Value = assetType;
            CBaseArchetypeDefNode.Add(assetTypeNode);

            //assetName
            XElement assetNameNode = new XElement("assetName");
            assetNameNode.Value = assetName;
            CBaseArchetypeDefNode.Add(assetNameNode);

            //extensions
            XElement extensionsNode = new XElement("extensions");
            CBaseArchetypeDefNode.Add(extensionsNode);

            return CBaseArchetypeDefNode;
        }

        public CBaseArchetypeDef(XElement node)
        {

            lodDist = float.Parse(node.Element("lodDist").Attribute("value").Value);
            flags = uint.Parse(node.Element("flags").Attribute("value").Value);
            specialAttribute = uint.Parse(node.Element("specialAttribute").Attribute("value").Value);
            bbMin = new Vector3(
                float.Parse(node.Element("bbMin").Attribute("x").Value),
                float.Parse(node.Element("bbMin").Attribute("y").Value),
                float.Parse(node.Element("bbMin").Attribute("z").Value)
                );
            bbMax = new Vector3(
                float.Parse(node.Element("bbMax").Attribute("x").Value),
                float.Parse(node.Element("bbMax").Attribute("y").Value),
                float.Parse(node.Element("bbMax").Attribute("z").Value)
                );
            bsCentre = new Vector3(
                float.Parse(node.Element("bsCentre").Attribute("x").Value),
                float.Parse(node.Element("bsCentre").Attribute("y").Value),
                float.Parse(node.Element("bsCentre").Attribute("z").Value)
                );
            bsRadius = float.Parse(node.Element("bsRadius").Attribute("value").Value);
            hdTextureDist = float.Parse(node.Element("hdTextureDist").Attribute("value").Value);
            name = node.Element("name").Value.ToLower(); //SOME YTYP CONTAINS ARCHETYPES WITH UPPERCASE -.-
            textureDictionary = node.Element("textureDictionary").Value;
            clipDictionary = node.Element("clipDictionary").Value;
            drawableDictionary = node.Element("drawableDictionary").Value;
            physicsDictionary = node.Element("physicsDictionary").Value;
            assetType = node.Element("assetType").Value;
            assetName = node.Element("assetName").Value.ToLower(); //SOME YTYP CONTAINS ARCHETYPES WITH UPPERCASE -.-
            extensions = node.Element("extensions");
        }
    }
}
