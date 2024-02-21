using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Reflection;

namespace GrwPinPadNet
{
    public class XmlKeyPairValue
    {
        public string IP { get; set; }
        public string CUIT { get; set; }
        public string CUITISV { get; set; }
        public string COMERCIO_ID { get; set; }
        public string TERMINAL_ID { get; set; }

        public XmlKeyPairValue GetValueElement(string _element)
        {
            XmlKeyPairValue value = new XmlKeyPairValue();
            //Create the XmlDocument.
            XmlDocument doc = new XmlDocument();
            string path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "UserConfig.xml");
            doc.Load(path);

            //Display all the book titles.
            XmlNodeList elemList = doc.GetElementsByTagName(_element);
            if (elemList != null)
            {
                Console.WriteLine(elemList[0].InnerText); //Will output '08:29:57'
            }
            /*
            for (int i = 0; i < elemList.Count; i++)
            {
                if(ConsoleWrite) Console.WriteLine(elemList[i].InnerXml);
            }
            */
            return value;
        }
    }
}

