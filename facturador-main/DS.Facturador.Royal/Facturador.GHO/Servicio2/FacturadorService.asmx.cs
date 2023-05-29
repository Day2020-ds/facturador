using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Microsoft.AspNet.Identity;
using AspNet.Identity.MySQL;
using Microsoft.AspNet.Identity.Owin;
using Facturador.GHO.Models;
using System.IO;
using BLToolkit.Data;
using BLToolkit.Data.Linq;
using Facturador.GHO.Controllers;

namespace Facturador.GHO.Servicio
{
    /// <summary>
    /// Soap Header for the Secured Web Service.
    /// Username and Password are required for AuthenticateUser(),
    ///   and AuthenticatedToken is required for everything else.
    /// </summary>
    public class SecuredWebServiceHeader : System.Web.Services.Protocols.SoapHeader
    {
        public string Username;
        public string Password;
        public string AuthenticatedToken;
    }
    /// <summary>
    /// Descripción breve de FacturadorService
    /// </summary>
    [WebService(Namespace = "http://daysoft.com.mx/webservices/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // Para permitir que se llame a este servicio web desde un script, usando ASP.NET AJAX, quite la marca de comentario de la línea siguiente. 
    // [System.Web.Script.Services.ScriptService]
    public class FacturadorService : System.Web.Services.WebService
    {
        private string idUsuario;
        public SecuredWebServiceHeader SoapHeader;

        [WebMethod]
        [System.Web.Services.Protocols.SoapHeader("SoapHeader")]
        public string AuthenticateUser()
        {
            if (SoapHeader == null)
                return "Error: Por favor proporcione su usuario y contraseña";
            if (string.IsNullOrEmpty(SoapHeader.Username) || string.IsNullOrEmpty(SoapHeader.Password))
                return "Error: Por favor proporcione su usuario y contraseña";

            // Are the credentials valid?
            if (!IsUserValid(SoapHeader.Username, SoapHeader.Password))
                return "Error: El usuario o contraseña no es valido.";

            // Create and store the AuthenticatedToken before returning it
            string token = Guid.NewGuid().ToString();
            HttpRuntime.Cache.Add(
                token,
                idUsuario,
                null,
                System.Web.Caching.Cache.NoAbsoluteExpiration,
                TimeSpan.FromMinutes(60),
                System.Web.Caching.CacheItemPriority.NotRemovable,
                null);

            return token;
        }

        private bool IsUserValid(string Username, string Password)
        {
            var manager = new UserManager();
            IdentityUser user = manager.Find(Username, Password);
            if (user != null)
            {
                idUsuario = user.Id;
                return true;
            }
            return false;
        }

        private bool IsUserValid(SecuredWebServiceHeader SoapHeader)
        {
            if (SoapHeader == null)
                return false;

            // Does the token exists in our Cache?
            if (!string.IsNullOrEmpty(SoapHeader.AuthenticatedToken))
                return (HttpRuntime.Cache[SoapHeader.AuthenticatedToken] != null);

            return false;
        }

        [WebMethod]
        [System.Web.Services.Protocols.SoapHeader("SoapHeader")]
        public bool IsLogged()
        {
            return IsUserValid(SoapHeader);
        }

        [WebMethod]
        [System.Web.Services.Protocols.SoapHeader("SoapHeader")]
        public string Role()
        {
            if (!IsUserValid(SoapHeader))
                return "Error: No se a autentificado como usuario.";
            var manager = new UserManager();
            IdentityUser user = manager.FindById(HttpRuntime.Cache[SoapHeader.AuthenticatedToken].ToString());
            if (user != null)
            {
                return manager.IsInRole(user.Id, "Admin") ? "Admin" : (manager.IsInRole(user.Id, "Usuario") ? "Usuario" : "");
            }
            return "Error: No se encontro el usuario";
        }

        [WebMethod]
        [System.Web.Services.Protocols.SoapHeader("SoapHeader")]
        public string UploadTicket(byte[] file, string numeroTicket)
        {
            try
            {
                if (!IsUserValid(SoapHeader))
                    return "Error: No se a autentificado como usuario.";
                StreamReader ms = new StreamReader(new MemoryStream(file));
                this.GuardarDB(ms);

                return "Exito: " + numeroTicket;
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        protected void GuardarDB(StreamReader archivo)
        {
            Seguridad seg = new Seguridad();
            Ticket ticket = new Ticket(archivo);
            DataModel.ticket cabecera = ticket.ObtenerCabecera();
            List<DataModel.ticket_detalle> detalles = ticket.ObtenerDetalles();

            using (var db = new DataModel.OstarDB())
            {
                db.BeginTransaction();

                var emisor = (from e in db.emisor
                              where e.rfc == seg.Encriptar(ticket.ObtenerRFC())
                              select e).FirstOrDefault();

                if (emisor != null)
                {
                    var existeTicket = db.ticket
                        .Where(t => t.emisor == emisor.idemisor && t.serie == cabecera.serie && t.folio == cabecera.folio && t.total == cabecera.total).Count();

                    if (existeTicket <= 0)
                    {
                        cabecera.emisor = emisor.idemisor;
                        var idTicket = db.InsertWithIdentity(cabecera);

                        foreach (DataModel.ticket_detalle detalle in detalles)
                        {
                            detalle.ticket = Convert.ToInt32(idTicket);
                            var query = db.InsertWithIdentity(detalle);
                        }
                    }
                    else
                        throw new Exception("Ya existe el ticket " + cabecera.serie + cabecera.folio);
                }
                else
                    throw new Exception("no se encontro el RFC " + ticket.ObtenerRFC());

                db.CommitTransaction();
            }
        }

        [WebMethod]
        [System.Web.Services.Protocols.SoapHeader("SoapHeader")]
        public List<Emisor> ObtenerEmpresas()
        {
            try
            {
                if (!IsUserValid(SoapHeader))
                    throw new Exception("Error: No se a autentificado como usuario.");
                List<Emisor> emisores = ObtenerEmisores();
                return emisores;
            }
            catch// (Exception ex)
            {
                return null;
            }
        }

        private List<Emisor> ObtenerEmisores()
        {
            using (var db = new DataModel.OstarDB())
            {
                try
                {
                    Seguridad seg = new Seguridad();
                    //List<Emisor> emisores = (from emisor in db.emisor
                    //                         select new Emisor
                    //                             {
                    //                                 Id = emisor.idemisor,
                    //                                 RazonSocial = emisor.razon_social,
                    //                                 RFC = emisor.rfc,
                    //                                 RegimenFiscal = emisor.regimen_fiscal
                    //                             }).ToList();
                    //return emisores;

                    List<int> emisoresActuales = new List<int>();
                    if (this.Role() == "Admin")
                    {
                        emisoresActuales = (from em in db.emisor
                                            select em.idemisor).ToList();
                    }
                    else
                    {
                        emisoresActuales = (from em in db.emisor
                                            join ue in db.useremisor on em.idemisor equals ue.EmisorId
                                            where ue.UserId == HttpRuntime.Cache[SoapHeader.AuthenticatedToken].ToString()
                                            select em.idemisor).ToList();
                    }
                    List<Emisor> empresas = (from h in db.emisor
                                             where emisoresActuales.Contains(h.idemisor)
                                             select new Emisor
                             {
                                 Id = h.idemisor,
                                 RazonSocial = string.IsNullOrEmpty(h.razon_social) ? "" : seg.Desencriptar(h.razon_social),
                                 RFC = string.IsNullOrEmpty(h.rfc) ? "" : seg.Desencriptar(h.rfc),
                                 RegimenFiscal = string.IsNullOrEmpty(h.regimen_fiscal) ? "" : seg.Desencriptar(h.regimen_fiscal)
                             }).Take(this.MaximoEmpresas()).ToList();
                    return empresas;
                }
                catch//(Exception ex)
                {
                    return null;
                }
            }
        }

        private int MaximoEmpresas()
        {
            License lic;
            string rutaLicencia = Server.MapPath("~/Facturacion/Licencia/");
            if (File.Exists(rutaLicencia + "licencia.vali"))
                lic = new License(rutaLicencia + "licencia.vali");
            else
                lic = new License();
            return lic.ObtenerEmpresas();
        }

        [WebMethod]
        [System.Web.Services.Protocols.SoapHeader("SoapHeader")]
        public Emisor ObtenerEmisor(int Id)
        {
            using (var db = new DataModel.OstarDB())
            {
                try
                {
                    Seguridad seg = new Seguridad();
                    if (!IsUserValid(SoapHeader))
                        throw new Exception("Error: No se a autentificado como usuario.");
                    var query = (from e in db.emisor
                                 where e.idemisor == Id
                                 select new
                                 {
                                     Id = e.idemisor,
                                     RazonSocial = string.IsNullOrEmpty(e.razon_social) ? "" : seg.Desencriptar(e.razon_social),
                                     RFC = string.IsNullOrEmpty(e.rfc) ? "" : seg.Desencriptar(e.rfc),
                                     RegimenFiscal = string.IsNullOrEmpty(e.regimen_fiscal) ? "" : seg.Desencriptar(e.regimen_fiscal),
                                     logo = e.logotipo,
                                     idDomicilio = e.domicilio,
                                     idCertificado = e.cetificado
                                 }).FirstOrDefault();

                    Emisor emisor = new Emisor();
                    emisor.Id = query.Id;
                    emisor.RazonSocial = query.RazonSocial;
                    emisor.RFC = query.RFC;
                    emisor.RegimenFiscal = query.RegimenFiscal;

                    emisor.domicilio = (from d in db.domicilio
                                        where d.iddomicilio == query.idDomicilio
                                        select new Domicilio
                                  {
                                      Id = d.iddomicilio,
                                      Calle = string.IsNullOrEmpty(d.calle) ? "" : seg.Desencriptar(d.calle),
                                      NoExterior = string.IsNullOrEmpty(d.no_exterior) ? "" : seg.Desencriptar(d.no_exterior),
                                      NoInterior = string.IsNullOrEmpty(d.no_interior) ? "" : seg.Desencriptar(d.no_interior),
                                      Colonia = string.IsNullOrEmpty(d.colonia) ? "" : seg.Desencriptar(d.colonia),
                                      Municipio = string.IsNullOrEmpty(d.municipio) ? "" : seg.Desencriptar(d.municipio),
                                      Localidad = string.IsNullOrEmpty(d.localidad) ? "" : seg.Desencriptar(d.localidad),
                                      Referencia = string.IsNullOrEmpty(d.referencia) ? "" : seg.Desencriptar(d.referencia),
                                      Estado = string.IsNullOrEmpty(d.estado) ? "" : seg.Desencriptar(d.estado),
                                      Pais = string.IsNullOrEmpty(d.pais) ? "" : seg.Desencriptar(d.pais),
                                      CodigoPostal = string.IsNullOrEmpty(d.codigo_postal) ? "" : seg.Desencriptar(d.codigo_postal)
                                  }).FirstOrDefault();

                    emisor.certificado = (from c in db.certificado
                                          where c.idcertificado == query.idCertificado
                                          select new Certificado
                                {
                                    Id = c.idcertificado,
                                    nombreLlave = c.llave_privada,
                                    nombreCSD = c.csd,
                                    Contrasenia = string.IsNullOrEmpty(c.contrasenia) ? "" : seg.Desencriptar(c.contrasenia)
                                }).FirstOrDefault();
                    return emisor;
                }
                catch //(Exception ex)
                {
                    return null;
                }
            }
        }

        [WebMethod]
        [System.Web.Services.Protocols.SoapHeader("SoapHeader")]
        public string AgregarEmisor(Emisor emisor)
        {
            try
            {
                Seguridad seg = new Seguridad();
                if (!IsUserValid(SoapHeader))
                    return "Error: No se a autentificado como usuario.";
                string rutaCertificado = Server.MapPath("~" + System.Configuration.ConfigurationManager.AppSettings["rutaCertificado"]);
                using (var db = new DataModel.OstarDB())
                {
                    var numEmp = db.emisor.Count();
                    if (numEmp >= this.MaximoEmpresas())
                        return "Solo puedes registrar " + this.MaximoEmpresas() + " empresas";
                    string llavePriv64 = emisor.certificado.LlavePrivada;
                    string cert64 = emisor.certificado.CSD;
                    string logo64 = emisor.Logo;

                    TimbradoDS timbre = new TimbradoDS();
                    timbre.RFC = emisor.RFC;
                    timbre.certificado = cert64;
                    timbre.llave = llavePriv64;
                    timbre.passwordLlave = emisor.certificado.Contrasenia;

                    if (emisor.RFC == "AAA010101AAA")
                        timbre.TipoTimbrado = Servicio.wsDaySoft.TypeCFD.Test;
                    else
                    {
                        timbre.TipoTimbrado = Servicio.wsDaySoft.TypeCFD.Production;

                        var serv = db.servisim.FirstOrDefault();
                        if (serv != null)
                        {
                            timbre.Usuario = serv.usuario;
                            timbre.Password = string.IsNullOrWhiteSpace(serv.contrasenia) ? "" : seg.Desencriptar(serv.contrasenia);
                            timbre.Token = string.IsNullOrWhiteSpace(serv.token) ? "" : seg.Desencriptar(serv.token);
                        }
                    }

                    timbre.RegistrarCSD();

                    var idCertificado = db.certificado.InsertWithIdentity(() => new DataModel.certificado
                    {
                        contrasenia = string.IsNullOrEmpty(emisor.certificado.Contrasenia) ? "" : seg.Encriptar(emisor.certificado.Contrasenia),
                        llave_privada = emisor.certificado.nombreLlave,
                        csd = emisor.certificado.nombreCSD
                    });

                    var idDomicilio = db.domicilio.InsertWithIdentity(() => new DataModel.domicilio
                    {
                        calle = string.IsNullOrEmpty(emisor.domicilio.Calle) ? "" : seg.Encriptar(emisor.domicilio.Calle),
                        colonia = string.IsNullOrEmpty(emisor.domicilio.Colonia) ? "" : seg.Encriptar(emisor.domicilio.Colonia),
                        codigo_postal = string.IsNullOrEmpty(emisor.domicilio.CodigoPostal) ? "" : seg.Encriptar(emisor.domicilio.CodigoPostal),
                        estado = string.IsNullOrEmpty(emisor.domicilio.Estado) ? "" : seg.Encriptar(emisor.domicilio.Estado),
                        localidad = string.IsNullOrEmpty(emisor.domicilio.Localidad) ? "" : seg.Encriptar(emisor.domicilio.Localidad),
                        municipio = string.IsNullOrEmpty(emisor.domicilio.Municipio) ? "" : seg.Encriptar(emisor.domicilio.Municipio),
                        no_exterior = string.IsNullOrEmpty(emisor.domicilio.NoExterior) ? "" : seg.Encriptar(emisor.domicilio.NoExterior),
                        no_interior = string.IsNullOrEmpty(emisor.domicilio.NoInterior) ? "" : seg.Encriptar(emisor.domicilio.NoInterior),
                        pais = string.IsNullOrEmpty(emisor.domicilio.Pais) ? "" : seg.Encriptar(emisor.domicilio.Pais),
                        referencia = string.IsNullOrEmpty(emisor.domicilio.Referencia) ? "" : seg.Encriptar(emisor.domicilio.Referencia)
                    });

                    var value = db.emisor.InsertWithIdentity(() => new DataModel.emisor
                    {
                        rfc = string.IsNullOrEmpty(emisor.RFC) ? "" : seg.Encriptar(emisor.RFC),
                        razon_social = string.IsNullOrEmpty(emisor.RazonSocial) ? "" : seg.Encriptar(emisor.RazonSocial),
                        regimen_fiscal = string.IsNullOrEmpty(emisor.RegimenFiscal) ? "" : seg.Encriptar(emisor.RegimenFiscal),
                        logotipo = emisor.NombreLogo,
                        domicilio = Convert.ToInt32(idDomicilio),
                        cetificado = Convert.ToInt32(idCertificado)
                    });
                    if (!Directory.Exists(rutaCertificado + value))
                    {
                        Directory.CreateDirectory(rutaCertificado + value);
                    }
                    File.WriteAllBytes(rutaCertificado + value + "/" + emisor.certificado.nombreLlave, Convert.FromBase64String(llavePriv64));
                    File.WriteAllBytes(rutaCertificado + value + "/" + emisor.certificado.nombreCSD, Convert.FromBase64String(cert64));
                    if(!string.IsNullOrEmpty(logo64))
                    {
                        File.WriteAllBytes(rutaCertificado + value + "/" + emisor.NombreLogo, Convert.FromBase64String(logo64));
                    }
                    db.CommitTransaction();

                    return "Registro exitoso";
                }
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        [WebMethod]
        [System.Web.Services.Protocols.SoapHeader("SoapHeader")]
        public string EditarEmisor(Emisor emisor)
        {
            try
            {
                using (var db = new DataModel.OstarDB())
                {
                    db.BeginTransaction();
                Seguridad seg = new Seguridad();
                if (!IsUserValid(SoapHeader))
                    return "Error: No se a autentificado como usuario.";
                string rutaCertificado = Server.MapPath("~" + System.Configuration.ConfigurationManager.AppSettings["rutaCertificado"]);

                string llavePriv64 = emisor.certificado.LlavePrivada;
                string cert64 = emisor.certificado.CSD;
                string logo64 = emisor.Logo;

                if (!string.IsNullOrEmpty(llavePriv64) || !string.IsNullOrEmpty(cert64))
                {
                    TimbradoDS timbre = new TimbradoDS();
                    timbre.RFC = emisor.RFC;
                    if (!string.IsNullOrEmpty(cert64))
                        timbre.certificado = cert64;
                    else
                        timbre.certificado = EncodeBase64(rutaCertificado + emisor.Id + "/" + emisor.certificado.nombreCSD);
                    if (!string.IsNullOrEmpty(llavePriv64))
                        timbre.llave = llavePriv64;
                    else
                        timbre.llave = EncodeBase64(rutaCertificado + emisor.Id + "/" + emisor.certificado.nombreLlave);

                    timbre.passwordLlave = emisor.certificado.Contrasenia;

                    if (emisor.RFC == "AAA010101AAA")
                        timbre.TipoTimbrado = Servicio.wsDaySoft.TypeCFD.Test;
                    else
                    {
                        timbre.TipoTimbrado = Servicio.wsDaySoft.TypeCFD.Production;

                        var serv = db.servisim.FirstOrDefault();
                        if (serv != null)
                        {
                            timbre.Usuario = serv.usuario;
                            timbre.Password = string.IsNullOrWhiteSpace(serv.contrasenia) ? "" : seg.Desencriptar(serv.contrasenia);
                            timbre.Token = string.IsNullOrWhiteSpace(serv.token) ? "" : seg.Desencriptar(serv.token);
                        }
                    }
                    timbre.RegistrarCSD();
                }

                    var idEmisor = db.emisor
                        .Where(e => e.idemisor == emisor.Id)
                        .Update(e => new DataModel.emisor
                        {
                            razon_social = string.IsNullOrEmpty(emisor.RazonSocial) ? "" : seg.Encriptar(emisor.RazonSocial),
                            rfc = string.IsNullOrEmpty(emisor.RFC) ? "" : seg.Encriptar(emisor.RFC),
                            regimen_fiscal = string.IsNullOrEmpty(emisor.RegimenFiscal) ? "" : seg.Encriptar(emisor.RegimenFiscal)
                        });

                    var idDomicilio = db.domicilio
                        .Where(e => e.iddomicilio == emisor.domicilio.Id)
                        .Update(e => new DataModel.domicilio
                        {
                            calle = string.IsNullOrEmpty(emisor.domicilio.Calle) ? "" : seg.Encriptar(emisor.domicilio.Calle),
                            no_exterior = string.IsNullOrEmpty(emisor.domicilio.NoExterior) ? "" : seg.Encriptar(emisor.domicilio.NoExterior),
                            no_interior = string.IsNullOrEmpty(emisor.domicilio.NoInterior) ? "" : seg.Encriptar(emisor.domicilio.NoInterior),
                            colonia = string.IsNullOrEmpty(emisor.domicilio.Colonia) ? "" : seg.Encriptar(emisor.domicilio.Colonia),
                            referencia = string.IsNullOrEmpty(emisor.domicilio.Referencia) ? "" : seg.Encriptar(emisor.domicilio.Referencia),
                            localidad = string.IsNullOrEmpty(emisor.domicilio.Localidad) ? "" : seg.Encriptar(emisor.domicilio.Localidad),
                            municipio = string.IsNullOrEmpty(emisor.domicilio.Municipio) ? "" : seg.Encriptar(emisor.domicilio.Municipio),
                            estado = string.IsNullOrEmpty(emisor.domicilio.Estado) ? "" : seg.Encriptar(emisor.domicilio.Estado),
                            pais = string.IsNullOrEmpty(emisor.domicilio.Estado) ? "" : seg.Encriptar(emisor.domicilio.Pais),
                            codigo_postal = string.IsNullOrEmpty(emisor.domicilio.CodigoPostal) ? "" : seg.Encriptar(emisor.domicilio.CodigoPostal)
                        });

                    var idCertificado = db.certificado
                        .Where(e => e.idcertificado == emisor.certificado.Id)
                        .Update(e => new DataModel.certificado
                        {
                            contrasenia = string.IsNullOrEmpty(emisor.certificado.Contrasenia) ? "" : seg.Encriptar(emisor.certificado.Contrasenia)
                        });

                    if (!string.IsNullOrEmpty(llavePriv64))
                    {

                        if (!Directory.Exists(rutaCertificado + emisor.Id))
                        {
                            Directory.CreateDirectory(rutaCertificado + emisor.Id);
                        }
                        var llave = db.certificado
                            .Where(e => e.idcertificado == emisor.certificado.Id)
                            .Update(e => new DataModel.certificado
                            {
                                llave_privada = emisor.certificado.nombreLlave
                            });
                        File.WriteAllBytes(rutaCertificado + emisor.Id + "/" + emisor.certificado.nombreLlave, Convert.FromBase64String(llavePriv64));
                    }
                    if (!string.IsNullOrEmpty(cert64))
                    {
                        var cert = db.certificado
                            .Where(e => e.idcertificado == emisor.certificado.Id)
                            .Update(e => new DataModel.certificado
                            {
                                csd = emisor.certificado.nombreCSD
                            });
                        File.WriteAllBytes(rutaCertificado + emisor.Id + "/" + emisor.certificado.nombreCSD, Convert.FromBase64String(cert64));
                    }
                    if (!string.IsNullOrEmpty(logo64))
                    {
                        var logo = db.emisor
                           .Where(e => e.idemisor == emisor.Id)
                           .Update(e => new DataModel.emisor
                           {
                               logotipo = emisor.NombreLogo
                           });
                        File.WriteAllBytes(rutaCertificado + emisor.Id + "/" + emisor.NombreLogo, Convert.FromBase64String(logo64));
                    }
                    db.CommitTransaction();
                }
                return "Se ha modificado correctamente";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        [WebMethod]
        [System.Web.Services.Protocols.SoapHeader("SoapHeader")]
        public string EliminarEmisor(int id)
        {
            if (!IsUserValid(SoapHeader))
                return "Error: No se a autentificado como usuario.";
            try
            {
                using (var db = new DataModel.OstarDB())
                {
                    db.emisor
                        .Where(e => e.idemisor == id)
                        .Delete();
                }
                return "Eliminado correctamente";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        [WebMethod]
        [System.Web.Services.Protocols.SoapHeader("SoapHeader")]
        public string SaveFiles(string nameXML, byte[]fileXML, string namePDF, byte[]filePDF, string empresa, DateTime month)
        {
            try
            {
                string Server = HttpContext.Current.Server.MapPath("~/"  + System.Configuration.ConfigurationManager.AppSettings["rutaComprobantes"]);
                File.WriteAllBytes(Server + "/" + empresa + "/" + month.ToString("yyyy-MM") + "/" + nameXML + ".xml", fileXML);
                File.WriteAllBytes(Server + "/" + empresa + "/" + month.ToString("yyyy-MM") + "/" + namePDF + ".pdf", filePDF);
                return "Guardados correctamente";
            }
            catch (Exception ex)
            {
                return "Error: " + ex.Message;
            }
        }

        [WebMethod]
        [System.Web.Services.Protocols.SoapHeader("SoapHeader")]
        public List<Facturas> ConsultarFacturas(ParametrosFacturas parametros)
        {
            Seguridad seg = new Seguridad();
            List<Facturas> facturas = null;
            try
            {
                using (var db = new DataModel.OstarDB())
                {
                    if (!IsUserValid(SoapHeader))
                        throw new Exception("No se a autentificado como usuario.");
                    //facturas =
                    //    (from f in db.factura
                    //     join t in db.ticket on f.ticket equals t.idticket
                    //     join e in db.emisor on t.emisor equals e.idemisor
                    //     join r in db.receptor on f.receptor equals r.idreceptor
                    //     where f.fecha_timbrado >= parametros.FechaInicial
                    //        && f.fecha_timbrado < parametros.FechaFinal
                    //        && f.uuid.Contains(parametros.FolioFiscal) && r.rfc.Contains(parametros.ReceptorRFC) && e.rfc.Contains(parametros.EmisorRFC)
                    //     orderby f.fecha_timbrado
                    //     select new Facturas
                    //     {
                    //         FolioFiscal = f.uuid,
                    //         FechaTimbrado = f.fecha_timbrado,
                    //         Ticket = t.no_ticket,
                    //         Subtotal = t.subtotal,
                    //         Total = t.total,
                    //         Emisor = string.IsNullOrEmpty(e.rfc) ? "" : seg.Desencriptar(e.rfc),
                    //         Receptor = string.IsNullOrEmpty(r.rfc) ? "" : seg.Desencriptar(r.rfc),
                    //         Impuestos = t.iva + (t.ieps ?? decimal.Zero)
                    //     }).ToList();

                    List<int> emisoresActuales = new List<int>();
                    if (this.Role() == "Admin")
                    {
                        emisoresActuales = (from em in db.emisor
                                            select em.idemisor).ToList();
                    }
                    else
                    {
                        emisoresActuales = (from em in db.emisor
                                            join ue in db.useremisor on em.idemisor equals ue.EmisorId
                                            where ue.UserId == HttpRuntime.Cache[SoapHeader.AuthenticatedToken].ToString()
                                            select em.idemisor).ToList();
                    }
                    facturas =
                    (from f in db.factura
                     join t in db.ticket on f.ticket equals t.idticket
                     join e in db.emisor on t.emisor equals e.idemisor
                     join r in db.receptor on f.receptor equals r.idreceptor
                     where f.fecha_timbrado >= parametros.FechaInicial
                        && f.fecha_timbrado < parametros.FechaFinal
                        && f.uuid.Contains(parametros.FolioFiscal) && emisoresActuales.Contains(e.idemisor)
                     select new Facturas
                     {
                                  FolioFiscal = f.uuid,
                                  FechaTimbrado = f.fecha_timbrado,
                                  Ticket = t.serie + t.folio,
                                  Subtotal = t.subtotal,
                                  Total = t.total,
                                  Emisor = string.IsNullOrEmpty(e.rfc) ? "" : seg.Desencriptar(e.rfc),
                                  Receptor = string.IsNullOrEmpty(r.rfc) ? "" : seg.Desencriptar(r.rfc),
                                  Impuestos = t.iva + (t.ieps ?? decimal.Zero)
                     }).OrderBy(x => x.FechaTimbrado).ToList();

                    facturas = facturas.Where(x => x.Emisor.Contains(parametros.EmisorRFC) && x.Receptor.Contains(parametros.ReceptorRFC)).ToList();
                }
                return facturas;
            }
            catch
            {
                return null;
            }
        }

        private string EncodeBase64(string file)
        {
            string base64 = string.Empty;
            if (!string.IsNullOrEmpty(file))
            {
                FileStream fs = new FileStream(file,
                                               FileMode.Open,
                                               FileAccess.Read);
                byte[] filebytes = new byte[fs.Length];
                fs.Read(filebytes, 0, Convert.ToInt32(fs.Length));
                base64 = Convert.ToBase64String(filebytes, Base64FormattingOptions.InsertLineBreaks);
            }
            return base64;
        }

        private void DecodeBase64(string base64, string rutaArchivo)
        {
            if (!string.IsNullOrEmpty(base64))
            {
                byte[] filebytes = Convert.FromBase64String(base64);
                FileStream fs = new FileStream(rutaArchivo,
                                               FileMode.CreateNew,
                                               FileAccess.Write,
                                               FileShare.None);
                fs.Write(filebytes, 0, filebytes.Length);
                fs.Close();
            }
        }

        public class Emisor
        {
            public int Id { get; set; }
            public string RFC { get; set; }
            public string RazonSocial { get; set; }
            public string RegimenFiscal { get; set; }
            public string Logo { get; set; }
            public string NombreLogo { get; set; }
            public Domicilio domicilio { get; set; }
            public Certificado certificado { get; set; }
        }

        public class Domicilio
        {
            public int Id { get; set; }
            public string Calle { get; set; }
            public string NoExterior { get; set; }
            public string NoInterior { get; set; }
            public string Colonia { get; set; }
            public string Municipio { get; set; }
            public string Localidad { get; set; }
            public string Referencia { get; set; }
            public string Estado { get; set; }
            public string Pais { get; set; }
            public string CodigoPostal { get; set; }
        }

        public class Certificado
        {
            public int Id { get; set; }
            public string LlavePrivada { get; set; }
            public string Contrasenia { get; set; }
            public string CSD { get; set; }
            public string nombreLlave { get; set; }
            public string nombreCSD { get; set; }
        }

        public class Facturas
        {
            public string FolioFiscal { get; set; }
            public string Ticket { get; set; }
            public DateTime FechaTimbrado { get; set; }
            public string Emisor { get; set; }
            public string Receptor { get; set; }
            public decimal Subtotal { get; set; }
            public decimal Impuestos { get; set; }
            public decimal Total { get; set; }
        }

        public class ParametrosFacturas
        {
            public string EmisorRFC { get; set; }
            public string ReceptorRFC { get; set; }
            public string FolioFiscal { get; set; }
            public DateTime FechaInicial { get; set; }
            public DateTime FechaFinal { get; set; }
        }
    }
}
