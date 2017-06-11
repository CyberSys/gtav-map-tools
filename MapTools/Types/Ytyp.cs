using System.Xml.Linq;

namespace MapTools.Types
{
    public class Ytyp
    {
        public CMapTypes CMapTypes { get; set; }

        public Ytyp(string filename)
        {
            CMapTypes = new CMapTypes(filename);
        }

        public XDocument WriteXML()
        {
            //document
            XDocument doc = new XDocument();
            //declaration
            XDeclaration declaration = new XDeclaration("1.0", "UTF-8", "no");
            doc.Declaration = declaration;
            //CMapTypes
            doc.Add(CMapTypes.WriteXML());
            return doc;
        }

        public Ytyp(XDocument document)
        {
            CMapTypes = new CMapTypes(document.Element("CMapTypes"));
        }
    }
}
