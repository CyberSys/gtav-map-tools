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
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            //CBaseArchetypeDef
            XElement CBaseArchetypeDefField = new XElement("Item", new XAttribute("type", "CBaseArchetypeDef"));

            //lodDist
            XElement lodDistField = new XElement("lodDist", new XAttribute("value", lodDist.ToString(nfi)));
            CBaseArchetypeDefField.Add(lodDistField);

            //flags
            XElement flagsField = new XElement("flags", new XAttribute("value", flags.ToString()));
            CBaseArchetypeDefField.Add(flagsField);

            //specialAttribute
            XElement specialAttributeField = new XElement("specialAttribute", new XAttribute("value", specialAttribute.ToString()));
            CBaseArchetypeDefField.Add(specialAttributeField);

            //bbMin
            XElement bbMinFiled = new XElement("bbMin",
                new XAttribute("x", bbMin.X.ToString(nfi)),
                new XAttribute("y", bbMin.Y.ToString(nfi)),
                new XAttribute("z", bbMin.Z.ToString(nfi))
                );
            CBaseArchetypeDefField.Add(bbMinFiled);

            //bbMax
            XElement bbMaxFiled = new XElement("bbMax",
                new XAttribute("x", bbMax.X.ToString(nfi)),
                new XAttribute("y", bbMax.Y.ToString(nfi)),
                new XAttribute("z", bbMax.Z.ToString(nfi))
                );
            CBaseArchetypeDefField.Add(bbMaxFiled);

            //bsCentre
            XElement bsCentreField = new XElement("bsCentre",
                new XAttribute("x", bsCentre.X.ToString(nfi)),
                new XAttribute("y", bsCentre.Y.ToString(nfi)),
                new XAttribute("z", bsCentre.Z.ToString(nfi))
                );
            CBaseArchetypeDefField.Add(bsCentreField);

            //bsRadius
            XElement bsRadiusField = new XElement("bsRadius", new XAttribute("value", bsRadius.ToString(nfi)));
            CBaseArchetypeDefField.Add(bsRadiusField);

            //hdTextureDist
            XElement hdTextureDistField = new XElement("hdTextureDist", new XAttribute("value", hdTextureDist.ToString(nfi)));
            CBaseArchetypeDefField.Add(hdTextureDistField);

            //name
            XElement nameField = new XElement("name");
            nameField.Value = name;
            CBaseArchetypeDefField.Add(nameField);

            //textureDictionary
            XElement textureDictionaryField = new XElement("textureDictionary");
            textureDictionaryField.Value = textureDictionary;
            CBaseArchetypeDefField.Add(textureDictionaryField);

            //clipDictionary
            XElement clipDictionaryField = new XElement("clipDictionary");
            if (clipDictionary != null)
                clipDictionaryField.Value = clipDictionary;
            CBaseArchetypeDefField.Add(clipDictionaryField);

            //drawableDictionary
            XElement drawableDictionaryField = new XElement("drawableDictionary");
            if (drawableDictionary != null)
                drawableDictionaryField.Value = drawableDictionary;
            CBaseArchetypeDefField.Add(drawableDictionaryField);

            //physicsDictionary
            XElement physicsDictionaryField = new XElement("physicsDictionary");
            if (physicsDictionary != null)
                physicsDictionaryField.Value = physicsDictionary;
            CBaseArchetypeDefField.Add(physicsDictionaryField);

            //assetType
            XElement assetTypeField = new XElement("assetType");
            assetTypeField.Value = assetType;
            CBaseArchetypeDefField.Add(assetTypeField);

            //assetName
            XElement assetNameField = new XElement("assetName");
            assetNameField.Value = assetName;
            CBaseArchetypeDefField.Add(assetNameField);

            //extensions
            XElement extensionsField = new XElement("extensions");
            CBaseArchetypeDefField.Add(extensionsField);

            return CBaseArchetypeDefField;
        }

        public CBaseArchetypeDef(XElement node)
        {
            NumberFormatInfo nfi = new NumberFormatInfo();
            nfi.NumberDecimalSeparator = ".";

            lodDist = float.Parse(node.Element("lodDist").Attribute("value").Value, nfi);
            flags = uint.Parse(node.Element("flags").Attribute("value").Value);
            specialAttribute = uint.Parse(node.Element("specialAttribute").Attribute("value").Value);
            bbMin = new Vector3(
                float.Parse(node.Element("bbMin").Attribute("x").Value, nfi),
                float.Parse(node.Element("bbMin").Attribute("y").Value, nfi),
                float.Parse(node.Element("bbMin").Attribute("z").Value, nfi)
                );
            bbMax = new Vector3(
                float.Parse(node.Element("bbMax").Attribute("x").Value, nfi),
                float.Parse(node.Element("bbMax").Attribute("y").Value, nfi),
                float.Parse(node.Element("bbMax").Attribute("z").Value, nfi)
                );
            bsCentre = new Vector3(
                float.Parse(node.Element("bsCentre").Attribute("x").Value, nfi),
                float.Parse(node.Element("bsCentre").Attribute("y").Value, nfi),
                float.Parse(node.Element("bsCentre").Attribute("z").Value, nfi)
                );
            bsRadius = float.Parse(node.Element("bsRadius").Attribute("value").Value, nfi);
            hdTextureDist = float.Parse(node.Element("hdTextureDist").Attribute("value").Value, nfi);
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
