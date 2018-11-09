using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace DecryptSAMLAssertions
{
    class Program
    {
        public static XmlDocument LoadXmlDocument(string assertionFile)
        {
            using (var fs = File.OpenRead(assertionFile))
            {
                var document = new XmlDocument { PreserveWhitespace = true };
                document.Load(fs);
                fs.Close();
                return document;
            }
        }

        public static XmlDocument GetEncryptedAttributeDocument(string source)
        {
            var doc = LoadXmlDocument(source);
            XmlDocument doc2 = new XmlDocument();
            XmlNode copiedNode = doc2.ImportNode(doc.SelectSingleNode("//*[local-name() = 'EncryptedAssertion']"), true);
            doc2.AppendChild(copiedNode);
            return doc2;
        }

        static void Main(string[] args)
        {
            /*
             * You need to get a PKCS12 file containing your Public/Private keys for your SAML Service provider 
             * Set a password on it
             *  
             *  Then change the line below to load that file, set the second value to the PFX password if used
             */
            var cert = new X509Certificate2(@"certs\saml.pfx", "");

            /*
             * You need to save the SAML XML Response to file MAKE SURE YOU DONT PRETTYPRINT it,
             * as this will make the SIGNING of the XML invalid
             * 
             * Then change the line below to load the file
             * 
             */ 
            var doc = GetEncryptedAttributeDocument(@"saml.xml");
           
            //This line will load the encrypted assertion from the SAML Response
            var encryptedAssertion = new SAML2.Saml20EncryptedAssertion((RSA)cert.PrivateKey, doc);

            //This decrypts the response
            encryptedAssertion.Decrypt();

            //This extracts the decrypted response.
            var decryptedDocument = encryptedAssertion.Assertion;
            decryptedDocument.Save(@"decrypted.xml");



        }
    }
}
