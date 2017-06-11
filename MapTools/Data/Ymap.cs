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
    }
}
