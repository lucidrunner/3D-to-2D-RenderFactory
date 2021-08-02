using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

namespace Render3DTo2D.XML_Render_Info_Export
{
    public static class XmlMethods
    {
        //Possible TODO - Make sure this can take List attributes too
        public static void WriteStringList(XmlWriter aXMLWriter, string aElementListTag, string aElementsTag, List<string> aElements, List<(string AttributeTag, string AttributeContent)[]> aAttributes = null)
        {
            //write our animation lists
            aXMLWriter.WriteStartElement(aElementListTag);
            for (int _index = 0; _index < aElements.Count; _index++)
            {
                aXMLWriter.WriteStartElement(aElementsTag);
                
                //Write each attribute corresponding to the element
                if(aAttributes != null)
                {
                    WriteAttributes(aXMLWriter, aAttributes[_index]);
                }
                
                aXMLWriter.WriteString(aElements[_index]);
                aXMLWriter.WriteEndElement();
            }
            aXMLWriter.WriteEndElement();
        }

        public static void WriteStringElement(XmlWriter aXMLWriter, string aElementTag, string aElement, params (string AttributeTag, string AttributeContent)[] aAttributes)
        {
            //Write our name and tag
            aXMLWriter.WriteStartElement(aElementTag);
            
            //Write all the passed along attributes
            WriteAttributes(aXMLWriter, aAttributes);
            
            aXMLWriter.WriteString(aElement);
            aXMLWriter.WriteEndElement();
        }
        
        
        public static void WriteDocumentEnd(XmlWriter aXMLWriter)
        {
            //End the root node
            aXMLWriter.WriteEndElement();

            //End the document itself
            aXMLWriter.WriteEndDocument();
        }

        public static void WriteAttributes(XmlWriter aXMLWriter, (string AttributeTag, string AttributeContent)[] aAttributeArray)
        {
            foreach (var _attribute in aAttributeArray)
            {
                if (!string.IsNullOrEmpty(_attribute.AttributeTag) && !string.IsNullOrEmpty(_attribute.AttributeContent))
                {
                    aXMLWriter.WriteAttributeString(_attribute.AttributeTag, _attribute.AttributeContent);
                }
            }
        }

        public static void WriteVector3(XmlWriter aXMLWriter, string aVectorTag, Vector3 aVector3, int aPrecision = 10, params (string AttributeTag, string AttributeContent)[] aAttributes)
        {
            aXMLWriter.WriteStartElement(aVectorTag);
            aXMLWriter.WriteAttributeString(XmlTags.SOURCETYPE, "Vector3");
            WriteAttributes(aXMLWriter, aAttributes);
            WriteStringElement(aXMLWriter, "x", aVector3.x.ToString("N" + aPrecision));
            WriteStringElement(aXMLWriter, "y", aVector3.y.ToString("N" + aPrecision));
            WriteStringElement(aXMLWriter, "z", aVector3.z.ToString("N" + aPrecision));
            aXMLWriter.WriteEndElement();
        }
        
        public static void WriteQuaternion(XmlWriter aXMLWriter, string aVectorTag, Quaternion aQuaternion, int aPrecision = 10, params (string AttributeTag, string AttributeContent)[] aAttributes)
        {
            aXMLWriter.WriteStartElement(aVectorTag);
            aXMLWriter.WriteAttributeString(XmlTags.SOURCETYPE, "Quaternion");
            WriteAttributes(aXMLWriter, aAttributes);
            WriteStringElement(aXMLWriter, "x", aQuaternion.x.ToString("N" + aPrecision));
            WriteStringElement(aXMLWriter, "y", aQuaternion.y.ToString("N" + aPrecision));
            WriteStringElement(aXMLWriter, "z", aQuaternion.z.ToString("N" + aPrecision));
            WriteStringElement(aXMLWriter, "w", aQuaternion.w.ToString("N" + aPrecision));
            aXMLWriter.WriteEndElement();
        }

    }
}