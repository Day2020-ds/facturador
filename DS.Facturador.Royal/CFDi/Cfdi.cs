using System;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Security.Cryptography;
using System.Xml.Xsl;

namespace CFDi
{
    public class Cfdi
    {
        private static string espacioNombre = @"http://www.sat.gob.mx/cfd/3";
        private static string ubicacionEsquema = @"http://www.sat.gob.mx/sitio_internet/cfd/3/cfdv32.xsd";
        //private static string rutaCadenaOriginal = @"http://www.sat.gob.mx/sitio_internet/cfd/3/cadenaoriginal_3_2/cadenaoriginal_3_2.xslt";


        protected XmlDocument xmlDocumento = null;
        //protected XmlDocument xmlDocumentoPago = null;
        protected Comprobante comprobante;
        protected ImpuestosLocales impuestosLocales;
        protected Donatarias donatarias;
        protected Pagos pagos;
        protected detallista detallista;
        protected requestForPayment requestForPayment;
        protected CartaPorte cartaPorte;

        public Cfdi()
        {
            comprobante = new Comprobante();
        }

        public Comprobante Comprobante { get { return this.comprobante; } }
        public ImpuestosLocales ImpuestosLocales { get { return this.impuestosLocales; } }
        public Donatarias Dontarias { get { return this.donatarias; } }
        public Pagos Pagos { get { return this.pagos; } }
        public detallista Detallista { get { return this.detallista; } }
        public requestForPayment RequestForPayment { get { return this.requestForPayment; } }
        public CartaPorte CartaPorte { get { return this.cartaPorte; } }

        private void ProcesarXml()
        {
            try
            {
                XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
                namespaces.Add("cfdi", "http://www.sat.gob.mx/cfd/3");
                namespaces.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");

                comprobante.schemaLocation = "http://www.sat.gob.mx/cfd/3 http://www.sat.gob.mx/sitio_internet/cfd/3/cfdv33.xsd";
                XmlSerializer serializer = new XmlSerializer(typeof(Comprobante));

                if (impuestosLocales != null)
                {
                    comprobante.schemaLocation += " http://www.sat.gob.mx/implocal http://www.sat.gob.mx/sitio_internet/cfd/implocal/implocal.xsd";
                    Comprobante.Complemento = new ComprobanteComplemento();
                    comprobante.Complemento.Any = new XmlElement[1];
                    comprobante.Complemento.Any[0] = ProcesarImpuestosLocales();
                }
                if (donatarias != null)
                {
                    comprobante.schemaLocation += " http://www.sat.gob.mx/donat  http://www.sat.gob.mx/sitio_internet/cfd/donat/donat11.xsd";
                    comprobante.Complemento = new ComprobanteComplemento();
                    comprobante.Complemento.Any = new XmlElement[1];
                    comprobante.Complemento.Any[0] = ProcesarDonatarias();
                }
                if (pagos != null)
                {
                    namespaces.Add("pago10", "http://www.sat.gob.mx/Pagos");
                    comprobante.schemaLocation += " http://www.sat.gob.mx/Pagos  http://www.sat.gob.mx/sitio_internet/cfd/Pagos/Pagos10.xsd";
                    comprobante.Complemento = new ComprobanteComplemento();
                    comprobante.Complemento.Any = new XmlElement[1];
                    comprobante.Complemento.Any[0] = ProcesarComplementoPagos();
                }
                if (detallista != null)
                {
                    namespaces.Add("detallista", "http://www.sat.gob.mx/detallista");
                    comprobante.schemaLocation += " http://www.sat.gob.mx/detallista http://www.sat.gob.mx/sitio_internet/cfd/detallista/detallista.xsd";
                    comprobante.Complemento = new ComprobanteComplemento();
                    comprobante.Complemento.Any = new XmlElement[1];
                    comprobante.Complemento.Any[0] = ProcesarDetallista();
                }
                if (cartaPorte != null) 
                {
                    //cambio version 2.0
                    namespaces.Add("cartaporte20", "http://www.sat.gob.mx/CartaPorte20");
                    comprobante.schemaLocation += " http://www.sat.gob.mx/CartaPorte20 http://www.sat.gob.mx/sitio_internet/cfd/CartaPorte/CartaPorte20.xsd";
                    comprobante.Complemento = new ComprobanteComplemento ();
                    comprobante.Complemento.Any = new XmlElement [1];
                    comprobante.Complemento.Any [0] = ProcesarCartaPorte ();
                }
                if (requestForPayment != null)
                {
                    comprobante.Addenda = new ComprobanteAddenda();
                    comprobante.Addenda.Any = new XmlElement[1];
                    comprobante.Addenda.Any[0] = ProcesarDetallista2();
                }

                StringWriter writer = new Utf8StringWriter();
                serializer.Serialize(writer, comprobante, namespaces);

                xmlDocumento = new XmlDocument();
                xmlDocumento.LoadXml(writer.ToString());

                //StringWriter writer2 = new StringWriter();
                //XmlSerializer serializer2 = new XmlSerializer(typeof(Pagos));
                //serializer2.Serialize(writer2, pagos, namespaces);

                //xmlDocumentoPago = new XmlDocument();
                //xmlDocumentoPago.LoadXml(writer2.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Donatarias AgregarDonatarias()
        {
            try
            {
                donatarias = new Donatarias();
                return this.Dontarias;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private XmlElement ProcesarDonatarias()
        {
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("donat", "http://www.sat.gob.mx/donat");
            XmlSerializer serializer = new XmlSerializer(typeof(Donatarias));

            StringWriter writer = new StringWriter();
            serializer.Serialize(writer, donatarias, namespaces);

            XmlDocument xmlDonatarias = new XmlDocument();
            xmlDonatarias.LoadXml(writer.ToString());

            return xmlDonatarias.DocumentElement;
        }
        
        public detallista AgregarDetallista()
        {
            try
            {
                detallista = new detallista();
                return this.detallista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public requestForPayment AgregarRequestForPayment()
        {
            try
            {
                requestForPayment = new requestForPayment();
                return this.requestForPayment;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private XmlElement ProcesarDetallista()
        {
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();

            namespaces.Add("detallista", "http://www.sat.gob.mx/detallista");
            XmlSerializer serializer = new XmlSerializer(typeof(detallista));

            StringWriter writer = new Utf8StringWriter();
            serializer.Serialize(writer, detallista, namespaces);

            XmlDocument xmlDetallista = new XmlDocument();
            xmlDetallista.LoadXml(writer.ToString());

            return xmlDetallista.DocumentElement;
        }

        private XmlElement ProcesarDetallista2()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(requestForPayment));

            StringWriter writer = new Utf8StringWriter();
            serializer.Serialize(writer, requestForPayment);

            XmlDocument xmlRequestForPayment = new XmlDocument();
            xmlRequestForPayment.LoadXml(writer.ToString());

            return xmlRequestForPayment.DocumentElement;
        }

        public CartaPorte AgregarCartaPorte ()
        {
            try {
                cartaPorte = new CartaPorte ();
                return this.cartaPorte;
            } catch (Exception ex) {
                throw ex;
            }
        }

        private XmlElement ProcesarCartaPorte ()
        {
            //cambio version 
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces ();
            namespaces.Add ("cartaporte20", "http://www.sat.gob.mx/CartaPorte20");

            XmlSerializer serializer = new XmlSerializer (typeof (CartaPorte));

            StringWriter writer = new Utf8StringWriter ();
            serializer.Serialize (writer, cartaPorte, namespaces);

            XmlDocument xmlCartaPorte = new XmlDocument ();
            xmlCartaPorte.LoadXml (writer.ToString ());

            var document = xmlCartaPorte.DocumentElement;
            document.RemoveAttribute ("xmlns:cartaporte20");
            return document;
        }

        public ImpuestosLocales AgregarImpuestosLocales()
        {
            try
            {
                impuestosLocales = new ImpuestosLocales();
                return this.ImpuestosLocales;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private XmlElement ProcesarImpuestosLocales()
        {
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("implocal", "http://www.sat.gob.mx/implocal");
            XmlSerializer serializer = new XmlSerializer(typeof(ImpuestosLocales));

            StringWriter writer = new Utf8StringWriter();
            serializer.Serialize(writer, impuestosLocales, namespaces);

            XmlDocument xmlImpuesoslocales = new XmlDocument();
            xmlImpuesoslocales.LoadXml(writer.ToString());

            return xmlImpuesoslocales.DocumentElement;
        }

        public Pagos AgregarComplementoPagos()
        {
            try
            {
                pagos = new Pagos();
                return this.Pagos;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private XmlElement ProcesarComplementoPagos()
        {
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("pago10", "http://www.sat.gob.mx/Pagos");


            XmlSerializer serializer = new XmlSerializer(typeof(Pagos));

            StringWriter writer = new StringWriter();
            serializer.Serialize(writer, pagos, namespaces);

            XmlDocument xmlPagos = new XmlDocument();
            xmlPagos.LoadXml(writer.ToString());

            return xmlPagos.DocumentElement;
        }

        private class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }
        }


        public string ObtenerXml()
        {
            ProcesarXml();
            return xmlDocumento.OuterXml;
        }

        public void GuardarXml(string rutaXml)
        {
            try
            {
                ProcesarXml();
                xmlDocumento.Save(rutaXml);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool validarXml()
        {
            try
            {
                ProcesarXml();
                XmlReaderSettings cfdSettings = new XmlReaderSettings();
                cfdSettings.Schemas.Add(espacioNombre, ubicacionEsquema);
                cfdSettings.ValidationType = ValidationType.Schema;
                cfdSettings.ValidationEventHandler += new ValidationEventHandler(cfdSettingsValidationEventHandler);

                XmlTextReader cfdTextReader = new XmlTextReader(new System.IO.StringReader(this.xmlDocumento.OuterXml));

                XmlReader cfd = XmlReader.Create(cfdTextReader, cfdSettings);

                while (cfd.Read()) { }

                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static void cfdSettingsValidationEventHandler(object sender, ValidationEventArgs e)
        {
            if (e.Severity == XmlSeverityType.Warning)
            {
                throw new Exception("Atención: " + e.Message);
            }
            else if (e.Severity == XmlSeverityType.Error)
            {
                throw new Exception("Error: " + e.Message);
            }
        }

        private string GenerarCadenaOriginal(XmlReader xmlReaderCFD, string RutaCadena)
        {
            try
            {
                XslCompiledTransform myXslTransform = new XslCompiledTransform();
                try
                {
                    myXslTransform.Load(RutaCadena);
                }
                catch(Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                System.IO.Stream stream = new System.IO.MemoryStream();
                myXslTransform.Transform(xmlReaderCFD, null, stream);
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                System.IO.StreamReader memoryReader = new System.IO.StreamReader(stream);
                return memoryReader.ReadLine();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al generar cadena original. " + ex.Message, ex);
            }
        }

        public string ObtenerCadenaOriginal(string rutaCadena)
        {
            try
            {
                ProcesarXml();
                return ObtenerCadenaOriginal(this.xmlDocumento, rutaCadena);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ObtenerCadenaOriginal(XmlDocument xmlDocumento, string rutaCadena)
        {
            try
            {
                ProcesarXml();
                return this.GenerarCadenaOriginal(XmlReader.Create(new StringReader(xmlDocumento.InnerXml)), rutaCadena);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ConsolidarTraslados()
        {
            try
            {
                var traslados = comprobante.Impuestos.Traslados.GroupBy(x => new { impuesto = x.Impuesto, tasa = x.TasaOCuota }).
                    Select(y => new ComprobanteImpuestosTraslado { Importe = y.Sum(x => x.Importe), TasaOCuota = y.Key.tasa, Impuesto = y.Key.impuesto });
                if (traslados != null)
                {
                    comprobante.Impuestos.Traslados = new ComprobanteImpuestosTraslado[traslados.Count()];
                    int x = 0;
                    foreach (ComprobanteImpuestosTraslado traslado in this.comprobante.Impuestos.Traslados)
                    {
                        comprobante.Impuestos.Traslados[x] = traslado;
                        x++;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public double ObtenerTotalTrasladados()
        {
            try
            {
                double totalTrasladados = 0;
                foreach (ComprobanteImpuestosTraslado traslado in this.comprobante.Impuestos.Traslados)
                    totalTrasladados += (double)traslado.Importe;
                return totalTrasladados;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void ConsolidarRetenciones()
        {
            try
            {
                var retenciones = comprobante.Impuestos.Retenciones.GroupBy(x => new { impuesto = x.Impuesto }).
                    Select(y => new ComprobanteImpuestosRetencion { Importe = y.Sum(x => x.Importe), Impuesto = y.Key.impuesto });
                if (retenciones != null)
                {
                    comprobante.Impuestos.Retenciones = new ComprobanteImpuestosRetencion[retenciones.Count()];
                    int x = 0;
                    foreach (ComprobanteImpuestosRetencion retencion in this.comprobante.Impuestos.Retenciones)
                    {
                        comprobante.Impuestos.Retenciones[x] = retencion;
                        x++;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public double ObtenerTotalRetenciones()
        {
            try
            {
                double totalRetenciones = 0;
                foreach (ComprobanteImpuestosRetencion retencion in this.comprobante.Impuestos.Retenciones)
                    totalRetenciones += (double)retencion.Importe;
                return totalRetenciones;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public string ObtenerSelloDigital(byte[] llavePrivada, string llavePassword, string rutaCadena)
        {
            try
            {
                return this.ObtenerSelloDigital(this.ObtenerCadenaOriginal(rutaCadena), llavePrivada, llavePassword);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ObtenerSelloDigital(string llavePrivada, string llavePassword, string rutaCadena, string vacio)
        {
            try
            {
                return this.ObtenerSelloDigital(this.ObtenerCadenaOriginal(rutaCadena), llavePrivada, llavePassword);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ObtenerSelloDigital(string cadenaOriginal, byte[] llavePrivadaBytes, string llavePassword)
        {
            try
            {
                System.Security.SecureString passwordSeguro = new System.Security.SecureString();
                passwordSeguro.Clear();
                foreach (char c in llavePassword.ToCharArray())
                    passwordSeguro.AppendChar(c);

                //byte[] llavePrivadaBytes = System.IO.File.ReadAllBytes(llavePrivada);
                RSACryptoServiceProvider rsaOpenSSLKey = JavaScience.opensslkey.DecodeEncryptedPrivateKeyInfo(llavePrivadaBytes, passwordSeguro);
                if (rsaOpenSSLKey == null)
                    throw new Exception("Datos de llave privada incorrectos.");
                //SHA1CryptoServiceProvider hasher = new SHA1CryptoServiceProvider();
                SHA256CryptoServiceProvider hasher = new SHA256CryptoServiceProvider();
                byte[] bytesFirmados = rsaOpenSSLKey.SignData(System.Text.Encoding.UTF8.GetBytes(cadenaOriginal), hasher);
                string strSello = Convert.ToBase64String(bytesFirmados);

                return strSello;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string ObtenerSelloDigital(string cadenaOriginal, string llavePrivada, string llavePassword)
        {
            try
            {
                System.Security.SecureString passwordSeguro = new System.Security.SecureString();
                passwordSeguro.Clear();
                foreach (char c in llavePassword.ToCharArray())
                    passwordSeguro.AppendChar(c);

                byte[] llavePrivadaBytes = System.IO.File.ReadAllBytes(llavePrivada);
                RSACryptoServiceProvider rsaOpenSSLKey = JavaScience.opensslkey.DecodeEncryptedPrivateKeyInfo(llavePrivadaBytes, passwordSeguro);
                if (rsaOpenSSLKey == null)
                    throw new Exception("Datos de llave privada incorrectos.");
                //SHA1CryptoServiceProvider hasher = new SHA1CryptoServiceProvider();
                SHA256CryptoServiceProvider hasher = new SHA256CryptoServiceProvider();
                byte[] bytesFirmados = rsaOpenSSLKey.SignData(System.Text.Encoding.UTF8.GetBytes(cadenaOriginal), hasher);
                string strSello = Convert.ToBase64String(bytesFirmados);

                return strSello;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private static string GetMD5(string cadenaOriginal)
        {
            byte[] hashValue;
            byte[] message = Encoding.UTF8.GetBytes(cadenaOriginal);

            MD5 hashString = new MD5CryptoServiceProvider();
            string hex = "";

            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;
        }

        private static string GetSHA1(string cadenaOriginal)
        {
            byte[] hashValue;
            byte[] message = Encoding.UTF8.GetBytes(cadenaOriginal);

            SHA1Managed hashString = new SHA1Managed();
            string hex = "";

            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;
        }

        public bool ValidaSelloDigital(string selloDigital, string cert, string cadena)
        {
            bool valido = false;
            try
            {
                byte[] sello_byte = Convert.FromBase64String(selloDigital);
                System.Security.Cryptography.X509Certificates.X509Certificate2 certificado = new System.Security.Cryptography.X509Certificates.X509Certificate2(cert);
                string spk = certificado.PublicKey.Key.ToXmlString(false);
                AsymmetricAlgorithm pk = AsymmetricAlgorithm.Create();
                pk.FromXmlString(spk);
                RSACryptoServiceProvider rsa = (RSACryptoServiceProvider)pk;
                rsa.PersistKeyInCsp = false;
                byte[] cadena_byte = System.Text.Encoding.UTF8.GetBytes(cadena);
                System.Security.Cryptography.SHA1CryptoServiceProvider x =
                new System.Security.Cryptography.SHA1CryptoServiceProvider();
                byte[] data = x.ComputeHash(cadena_byte);
                System.Security.Cryptography.MD5CryptoServiceProvider x2 =
                new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] data2 = x2.ComputeHash(cadena_byte);
                bool result = rsa.VerifyHash(data2, CryptoConfig.MapNameToOID("MD5"), sello_byte);
                bool result2 = rsa.VerifyHash(data, CryptoConfig.MapNameToOID("SHA1"), sello_byte);
                string ret = "Sello inválido.";
                if (result)
                {
                    ret = "Sello válido con digestion MD5 " + GetMD5(cadena);
                    valido = true;
                }
                else
                {
                    if (result2)
                    {
                        ret = "Sello válido con digestion SHA1 " + GetSHA1(cadena);
                        valido = true;
                    }
                }
                //return ret;
                return valido;
            }
            catch //(Exception ex)
            {
                //return "Error al desencriptar el sello digital " + ex.ToString();
                return valido;
            }
        }

        public string ObtenerNumeroCertificado(string rutaCertificado)
        {
            try
            {
                System.Security.Cryptography.X509Certificates.X509Certificate2 certificado = new System.Security.Cryptography.X509Certificates.X509Certificate2(rutaCertificado);
                string serie = certificado.GetSerialNumberString();
                StringBuilder sb = new StringBuilder(20);
                for (int i = 1; i < serie.Length; i += 2)
                    sb.Append(serie[i]);
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
