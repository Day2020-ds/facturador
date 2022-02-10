using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.IO;

namespace Facturador.GHO.Controllers
{
    public class CorreoElectronico
    {
        /// <summary>
        /// Clase correo
        /// </summary>
        private MailMessage mensajeCorreo;
        /// <summary>
        /// Clase Simple Mail Transfer Protocol
        /// </summary>
        private SmtpClient smtpCliente;
        /// <summary>
        /// Credenciales de Red
        /// </summary>
        private NetworkCredential credencial;

        private List<string> destinatarios = null;
        private List<Attachment> adjuntos = null;
        private string remitenteCorreo;
        private string remitenteNombre;
        private Seguridad seg = null;

        public CorreoElectronico()
        {
            try
            {
                this.mensajeCorreo = new MailMessage();
                this.smtpCliente = new SmtpClient();
                this.credencial = new NetworkCredential();
                this.smtpCliente.Credentials = credencial;
                this.destinatarios = new List<string>();
                this.adjuntos = new List<Attachment>();
                seg = new Seguridad();

                using (var db = new DataModel.OstarDB())
                {
                    var info = db.config.FirstOrDefault();

                    if (info != null)
                    {
                        this.smtpCliente.Host = info.servidor;
                        this.smtpCliente.Port = info.puerto;
                        this.credencial.UserName = info.correo.Replace("@royal-holiday.com", "");
                        this.credencial.Password = seg.Desencriptar(info.contrasenia);
                        this.smtpCliente.EnableSsl = info.ssl == 1 ? true : false;
                        this.remitenteCorreo = info.correo;
                        this.remitenteNombre = info.nombre;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error al instanciar correo." + ex.Message, ex);
            }
        }

        /// <summary>
        /// Agrega un destinatario al correo.
        /// </summary>
        /// <param name="email"></param>
        public void AgregarDestinatario(string email)
        {
            try
            {
                if (String.IsNullOrEmpty(email))
                    throw new Exception("Destinatario no puede ser nulo o vacio.");
                if (!this.EsEmail(email))
                    throw new Exception(String.Format("Destinatario '{0}' no es un correo valido.", email));
                this.destinatarios.Add(email);
            }
            catch (Exception ex)
            {
                throw new Exception("Error al agregar destinatario. " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Agrega un archivo adjunto al correo.
        /// </summary>
        /// <param name="ruta"></param>
        public void AgregarAdjunto(string ruta, string nombre)
        {
            try
            {
                if (!File.Exists(ruta))
                    throw new Exception(String.Format("El archivo '{0}' no existe.", ruta));
                //this.adjuntos.Add(new Attachment(ruta);
                this.adjuntos.Add(new Attachment(File.Open(ruta, FileMode.Open), nombre));
            }
            catch (Exception ex)
            {
                throw new Exception("Error al agregar archivo adjunto. " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Envia un correo electrónico
        /// </summary>
        /// <param name="remitente">Correo Electronico del Emisor</param>
        /// <param name="asunto">Asunto del Correo Electronico</param>
        /// <param name="mensaje">Mensaje del Correo Electronico</param>
        public void EnviarCorreo(string asunto, string mensaje)
        {
            try
            {
                this.EnviarCorreo(asunto, mensaje, false);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Verifica que la cadena eMail corresponda con una direccion de EMail
        /// </summary>
        /// <param name="eMail"></param>
        /// <returns>True si la cadena coincide con un email, false si no coincide</returns>
        private bool EsEmail(string eMail)
        {
            try
            {
                return Regex.IsMatch(eMail, @"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
            }
            catch (Exception ex)
            {
                throw new Exception("Error al validar e-mail. " + ex.Message, ex);
            }
        }

        public void EnviarCorreo(string nombre, string asunto, string mensaje, bool mensajeHtml)
        {
            remitenteNombre = nombre;
            EnviarCorreo(asunto, mensaje, mensajeHtml);
        }

        public void EnviarCorreo(string asunto, string mensaje, bool mensajeHtml)
        {
            try
            {
                //smtpCliente.UseDefaultCredentials = true;
                if (String.IsNullOrEmpty(remitenteCorreo))
                    throw new Exception("Remitente no puede ser nulo o vacio.");
                if (!this.EsEmail(remitenteCorreo))
                    throw new Exception(String.Format("Remitente '{0}' no es un correo valido.", remitenteCorreo));
                //
                this.mensajeCorreo.From = new MailAddress(remitenteCorreo, remitenteNombre);

                if (this.destinatarios.Count == 0)
                    throw new Exception("Deben de existir destinatarios.");
                //
                foreach (string destinatario in this.destinatarios)
                {
                    this.mensajeCorreo.To.Add(new MailAddress(destinatario));
                }
                //
                foreach (Attachment adjunto in this.adjuntos)
                {
                    this.mensajeCorreo.Attachments.Add(adjunto);
                }
                //
                this.mensajeCorreo.Subject = asunto;
                this.mensajeCorreo.Body = mensaje;
                this.mensajeCorreo.IsBodyHtml = mensajeHtml;
                this.smtpCliente.Send(this.mensajeCorreo);
                this.mensajeCorreo.Attachments.Dispose();
            }
            catch (Exception ex)
            {
                try
                {
                    this.mensajeCorreo.Attachments.Dispose();
                }
                catch { }
                throw new Exception("Error al enviar correo. " + ex.Message, ex);
            }
        }  
    }
}