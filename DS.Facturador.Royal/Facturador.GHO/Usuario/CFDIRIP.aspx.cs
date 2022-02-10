using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using System.Text;
using System.IO;
using Facturador.GHO.CFDRIP;
using MySql.Data.MySqlClient;
using Facturador.GHO.Controllers;
//using Facturador.GHO.Servicio;
using Daysoft.DICI.Facturador.DFacture;
//using DS.CFDI.Utils;
//using DS.RetencionPagov1;
using System.Xml;
using System.Xml.Serialization;
using System.Data;


namespace Facturador.GHO.Usuario
{
    public partial class CFDIRIP : System.Web.UI.Page
    {
        Seguridad seg;
        private string rutaCertificado = @"/Facturacion/Empresas/Certificados/";
        private string rutaComprobantesRIP = @"/Facturacion/Empresas/ComprobantesRIP/";
        string certificadoCsd, certificadoLlavePrivada, certificadoContrasenia;

        protected void Page_Load(object sender, EventArgs e)
        {
            seg = new Seguridad();
            rutaCertificado = Server.MapPath("~" + System.Configuration.ConfigurationManager.AppSettings["rutaCertificado"]);
            rutaComprobantesRIP = Server.MapPath("~" + System.Configuration.ConfigurationManager.AppSettings["rutaComprobantesRIP"]);

            if (!IsPostBack)
            {
                //Empresa
                LlenarddlEmpresa();
                //Datos
                LlenarddlClaveRetencion();
                //Cliente
                LlenarddlNacionalidad();
                //Periodo
                LlenarddlMesInicial();
                LlenarddlEjercicioFiscal();
                //Totales
                LlenarddlTipoPagoRetencion();
                LlenarddlImpuesto();
                //Complemento
                LlenarddlComplemento();
            }
        }

        #region Llenar DropDownList de CFDIRIP y Complementos
        //Empresa
        protected void LlenarddlEmpresa() {
            try
            {
                CFDIRIPConexionBD objConexion = new CFDIRIPConexionBD();
                MySqlConnection conexionString = objConexion.ObtenerConexion();
                MySqlCommand sql = new MySqlCommand("SELECT idemisor, identificador, razon_social From emisor", conexionString);
                MySqlDataReader leersql = sql.ExecuteReader();
                ddlEmpresa.Items.Clear();
                ddlEmpresa.Items.Add(new ListItem("", null));
                while (leersql.Read())
                {
                    ddlEmpresa.Items.Add(new ListItem((leersql.GetString(1)+" - "+seg.Desencriptar(leersql.GetString(2))), leersql.GetString(0)));
                }
                conexionString.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        //Datos
        protected void LlenarddlClaveRetencion()
        {
            ddlClaveRetencion.Items.Clear();
            ddlClaveRetencion.Items.Add(new ListItem("", null));
            ddlClaveRetencion.Items.Add(new ListItem("Adquisición de bienes consignada en escritura pública", "Item11"));
            ddlClaveRetencion.Items.Add(new ListItem("Adquisición de desperdicios industriales", "Item10"));
            ddlClaveRetencion.Items.Add(new ListItem("Adquisición de otros bienes, no consignada en escritura pública", "Item12"));
            ddlClaveRetencion.Items.Add(new ListItem("Arrendamiento", "Item05"));
            ddlClaveRetencion.Items.Add(new ListItem("Arrendamiento en fideicomiso", "Item17"));
            ddlClaveRetencion.Items.Add(new ListItem("Autotransporte terrestre de carga", "Item03"));
            ddlClaveRetencion.Items.Add(new ListItem("Dividendos o utilidades distribuidas", "Item14"));
            ddlClaveRetencion.Items.Add(new ListItem("Enajenación de acciones", "Item06"));
            ddlClaveRetencion.Items.Add(new ListItem("Enajenación de acciones u operaciones en bolsa de valores", "Item19"));
            ddlClaveRetencion.Items.Add(new ListItem("Enajenación de bienes inmuebles consignada en escritura pública", "Item08"));
            ddlClaveRetencion.Items.Add(new ListItem("Enajenación de bienes objeto de la LIEPS", "Item07"));
            ddlClaveRetencion.Items.Add(new ListItem("Enajenación de otros bienes, no consignada en escritura pública", "Item09"));
            ddlClaveRetencion.Items.Add(new ListItem("Fideicomisos que no realizan actividades empresariales", "Item21"));
            ddlClaveRetencion.Items.Add(new ListItem("Intereses", "Item16"));
            ddlClaveRetencion.Items.Add(new ListItem("Intereses reales deducibles por créditos hipotecarios", "Item23"));
            ddlClaveRetencion.Items.Add(new ListItem("Obtención de premios", "Item20"));
            ddlClaveRetencion.Items.Add(new ListItem("Operaciones Financieras Derivadas de Capital", "Item24"));
            ddlClaveRetencion.Items.Add(new ListItem("Otro tipo de retenciones", "Item25"));
            ddlClaveRetencion.Items.Add(new ListItem("Otros retiros de AFORE", "Item13"));
            ddlClaveRetencion.Items.Add(new ListItem("Pagos realizados a favor de residentes en el extranjero", "Item18"));
            ddlClaveRetencion.Items.Add(new ListItem("Planes personales de retiro", "Item22"));
            ddlClaveRetencion.Items.Add(new ListItem("Regalías por derechos de autor", "Item02"));
            ddlClaveRetencion.Items.Add(new ListItem("Remanente distribuible", "Item15"));
            ddlClaveRetencion.Items.Add(new ListItem("Servicios prestados por comisionistas", "Item04"));
            ddlClaveRetencion.Items.Add(new ListItem("Servicios profesionales", "Item01"));
        }

        //Cliente
        protected void LlenarddlNacionalidad() {
            ddlNacionalidad.Items.Clear();
            ddlNacionalidad.Items.Add(new ListItem("", null));
            ddlNacionalidad.Items.Add(new ListItem("Nacional", "Nacional"));
            ddlNacionalidad.Items.Add(new ListItem("Extranjero", "Extranjero"));
        }

        protected void LlenarddlCliente()
        {
            if (!(string.IsNullOrEmpty(ddlEmpresa.SelectedValue) && string.IsNullOrEmpty(ddlNacionalidad.SelectedValue)))
            {
                try
                {
                    CFDIRIPConexionBD objConexion = new CFDIRIPConexionBD();
                    MySqlConnection conexionString = objConexion.ObtenerConexion();
                    MySqlCommand sql = new MySqlCommand("SELECT idreceptor, identificador, razon_social From receptor WHERE(nacionalidad='" + ddlNacionalidad.SelectedValue + "' && emisor='" + ddlEmpresa.SelectedValue + "')", conexionString);
                    MySqlDataReader leersql = sql.ExecuteReader();
                    ddlIdentificadorCliente.Items.Clear();
                    ddlIdentificadorCliente.Items.Add(new ListItem("", null));
                    while (leersql.Read())
                    {
                        ddlIdentificadorCliente.Items.Add(new ListItem((leersql.GetString(1) + " - " + seg.Desencriptar(leersql.GetString(2))), leersql.GetString(0)));
                    }
                    conexionString.Close();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }
        
        //Periodo
        protected void LlenarddlMesInicial() {
            ddlMesInicial.Items.Clear();
            ddlMesInicial.Items.Add(new ListItem("", null));
            for (int mesInicial = 1; mesInicial <= 12; mesInicial++ )
            {
                ddlMesInicial.Items.Add(new ListItem(Convert.ToString(mesInicial), Convert.ToString(mesInicial)));
            }
        }

        protected void LlenarddlMesFinal(int mes) {
            ddlMesFinal.Items.Add(new ListItem("", null));
            for (int mesFinal = mes; mesFinal <= 12; mesFinal++)
            {
                ddlMesFinal.Items.Add(new ListItem(Convert.ToString(mesFinal), Convert.ToString(mesFinal)));
            }
        }

        protected void LlenarddlEjercicioFiscal() {
            ddlEjercicioFiscal.Items.Clear();
            ddlEjercicioFiscal.Items.Add(new ListItem("", null));
            for (int ejercicioFiscal = 2004; ejercicioFiscal <= 2024; ejercicioFiscal++)
            {
                ddlEjercicioFiscal.Items.Add(new ListItem(Convert.ToString(ejercicioFiscal), Convert.ToString(ejercicioFiscal)));
            }
        }

        //Impuestos Retenidos
        protected void LlenarddlImpuesto() {
            ddlImpuesto.Items.Clear();
            ddlImpuesto.Items.Add(new ListItem("", null));
            ddlImpuesto.Items.Add(new ListItem("ISR", "Item01"));
            ddlImpuesto.Items.Add(new ListItem("IVA", "Item02"));
            ddlImpuesto.Items.Add(new ListItem("IEPS", "Item03"));
        }
        
        protected void LlenarddlTipoPagoRetencion() {
            ddlTipoPagoRetencion.Items.Clear();
            ddlTipoPagoRetencion.Items.Add(new ListItem("", null));
            ddlTipoPagoRetencion.Items.Add(new ListItem("Pago definitivo", "Pagodefinitivo"));
            ddlTipoPagoRetencion.Items.Add(new ListItem("Pago provisional", "Pagoprovisional"));
        }

        //Compleentos
        protected void LlenarddlComplemento()
        {
            ddlComplemento.Items.Clear();
            ddlComplemento.Items.Add(new ListItem("", null));
            ddlComplemento.Items.Add(new ListItem("Arrendamiento en fideicomiso", "1"));
            ddlComplemento.Items.Add(new ListItem("Dividendos", "2"));
            ddlComplemento.Items.Add(new ListItem("Enajenación de acciones", "3"));
            ddlComplemento.Items.Add(new ListItem("Fideicomiso no empresarial", "4"));
            ddlComplemento.Items.Add(new ListItem("Intereses", "5"));
            ddlComplemento.Items.Add(new ListItem("Intereses hipotecarios", "6"));
            ddlComplemento.Items.Add(new ListItem("Operaciones con derivados", "7"));
            ddlComplemento.Items.Add(new ListItem("Pagos a extranjeros", "8"));
            ddlComplemento.Items.Add(new ListItem("Planes de retiro", "9"));
            ddlComplemento.Items.Add(new ListItem("Premios", "10"));
            ddlComplemento.Items.Add(new ListItem("Sector financiero", "11"));
            OcultarComplementos();
        }

        //Complemento Dividendos
        protected void LLenarddlCveTipDivOUtil()
        {
            ddlCveTipDivOUtil.Items.Clear();
            ddlCveTipDivOUtil.Items.Add(new ListItem("", null));
            ddlCveTipDivOUtil.Items.Add(new ListItem("Proviene de CUFIN", "Item01"));
            ddlCveTipDivOUtil.Items.Add(new ListItem("No proviene de CUFIN", "Item02"));
            ddlCveTipDivOUtil.Items.Add(new ListItem("Reembolso o reducción de capital", "Item03"));
            ddlCveTipDivOUtil.Items.Add(new ListItem("Liquidación de la persona moral", "Item04"));
            ddlCveTipDivOUtil.Items.Add(new ListItem("CUFINRE", "Item05"));
            ddlCveTipDivOUtil.Items.Add(new ListItem("Proviene de CUFIN al 31 de diciembre 2013", "Item06"));
        }

        protected void LlenarddlTipoSocDistrDiv()
        {
            ddlTipoSocDistrDiv.Items.Clear();
            ddlTipoSocDistrDiv.Items.Add(new ListItem("", null));
            ddlTipoSocDistrDiv.Items.Add(new ListItem("Sociedad Nacional", "SociedadNacional"));
            ddlTipoSocDistrDiv.Items.Add(new ListItem("Sociedad Extranjera", "SociedadExtranjera"));
        }

        //Complemento Pagos a extranjeros        
        protected void LlenarddlPaisDeResidParaEfecFisc()
        {
            try
            {
                CFDIRIPConexionBD objConexion = new CFDIRIPConexionBD();
                MySqlConnection conexion = objConexion.ObtenerConexion();
                MySqlCommand consultarPais = new MySqlCommand("SELECT id, pais FROM catalogopaises", conexion);
                MySqlDataReader agregar = consultarPais.ExecuteReader();
                ddlPaisDeResidParaEfecFiscNoBeneficiario.Items.Clear();
                ddlPaisDeResidParaEfecFiscNoBeneficiario.Items.Add(new ListItem("", null));
                while (agregar.Read())
                {
                    ddlPaisDeResidParaEfecFiscNoBeneficiario.Items.Add(new ListItem(agregar.GetString(1), agregar.GetString(0)));
                }
                conexion.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        protected void LLenarddlConceptoPagoPagosaExtranjeros()
        {
            ddlConceptoPagoPagosaExtranjeros.Items.Clear();
            ddlConceptoPagoPagosaExtranjeros.Items.Add(new ListItem("", null));
            ddlConceptoPagoPagosaExtranjeros.Items.Add(new ListItem("Agentes pagadores", "Item8"));
            ddlConceptoPagoPagosaExtranjeros.Items.Add(new ListItem("Artistas, deportistas y espectáculos públicos", "Item1"));
            ddlConceptoPagoPagosaExtranjeros.Items.Add(new ListItem("Asociación en participación", "Item5"));
            ddlConceptoPagoPagosaExtranjeros.Items.Add(new ListItem("Fideicomiso", "Item4"));
            ddlConceptoPagoPagosaExtranjeros.Items.Add(new ListItem("Organizaciones exentas", "Item7"));
            ddlConceptoPagoPagosaExtranjeros.Items.Add(new ListItem("Organizaciones Internacionales o de gobierno", "Item6"));
            ddlConceptoPagoPagosaExtranjeros.Items.Add(new ListItem("Otras personas físicas", "Item2"));
            ddlConceptoPagoPagosaExtranjeros.Items.Add(new ListItem("Otros ", "Item9"));
            ddlConceptoPagoPagosaExtranjeros.Items.Add(new ListItem("Persona moral", "Item3"));
        }

        //Complemento Premios
        protected void LlenarddlEntidadFederativaPremios()
        {
            ddlEntidadFederativaPremios.Items.Clear();
            ddlEntidadFederativaPremios.Items.Add(new ListItem("", null));
            ddlEntidadFederativaPremios.Items.Add(new ListItem("AGUASCALIENTES", "Item01"));
            ddlEntidadFederativaPremios.Items.Add(new ListItem("BAJA CALIFORNIA", "Item02"));
            ddlEntidadFederativaPremios.Items.Add(new ListItem("BAJA CALIFORNIA SUR", "Item03"));
            ddlEntidadFederativaPremios.Items.Add(new ListItem("CAMPECHE", "Item04"));
            ddlEntidadFederativaPremios.Items.Add(new ListItem("COAHUILA", "Item05"));
            ddlEntidadFederativaPremios.Items.Add(new ListItem("COLIMA", "Item06"));
            ddlEntidadFederativaPremios.Items.Add(new ListItem("CHIAPAS", "Item07"));
            ddlEntidadFederativaPremios.Items.Add(new ListItem("CHIHUAHUA", "Item08"));
            ddlEntidadFederativaPremios.Items.Add(new ListItem("DISTRITO FEDERAL", "Item09"));
            ddlEntidadFederativaPremios.Items.Add(new ListItem("DURANGO", "Item10"));
            ddlEntidadFederativaPremios.Items.Add(new ListItem("GUANAJUATO", "Item11"));
            ddlEntidadFederativaPremios.Items.Add(new ListItem("GUERRERO", "Item12"));
            ddlEntidadFederativaPremios.Items.Add(new ListItem("HIDALGO", "Item13"));
            ddlEntidadFederativaPremios.Items.Add(new ListItem("JALISCO", "Item14"));
            ddlEntidadFederativaPremios.Items.Add(new ListItem("MEXICO", "Item15"));
            ddlEntidadFederativaPremios.Items.Add(new ListItem("MICHOACAN", "Item16"));
            ddlEntidadFederativaPremios.Items.Add(new ListItem("MORELOS", "Item17"));
            ddlEntidadFederativaPremios.Items.Add(new ListItem("NAYARIT", "Item18"));
            ddlEntidadFederativaPremios.Items.Add(new ListItem("NUEVO LEON", "Item19"));
            ddlEntidadFederativaPremios.Items.Add(new ListItem("OAXACA", "Item20"));
            ddlEntidadFederativaPremios.Items.Add(new ListItem("PUEBLA", "Item21"));
            ddlEntidadFederativaPremios.Items.Add(new ListItem("QUERETARO", "Item22"));
            ddlEntidadFederativaPremios.Items.Add(new ListItem("QUINTANA ROO", "Item23"));
            ddlEntidadFederativaPremios.Items.Add(new ListItem("SAN LUIS POTOSI", "Item24"));
            ddlEntidadFederativaPremios.Items.Add(new ListItem("SINALOA", "Item25"));
            ddlEntidadFederativaPremios.Items.Add(new ListItem("SONORA", "Item26"));
            ddlEntidadFederativaPremios.Items.Add(new ListItem("TABASCO", "Item27"));
            ddlEntidadFederativaPremios.Items.Add(new ListItem("TAMAULIPAS", "Item28"));
            ddlEntidadFederativaPremios.Items.Add(new ListItem("TLAXCALA", "Item29"));
            ddlEntidadFederativaPremios.Items.Add(new ListItem("VERACRUZ", "Item30"));
            ddlEntidadFederativaPremios.Items.Add(new ListItem("YUCATAN", "Item31"));
            ddlEntidadFederativaPremios.Items.Add(new ListItem("ZACATECAS", "Item32"));
        }
        #endregion

        protected void OcultarComplementos() {
            //Ocultar complemento arrendamiento en fideicomiso y sus validaciones
            divArrendamientoenFideicomiso.Visible = false;
            rfvPagProvEfecPorFiduc.Enabled = false;
            rfvDeduccCorresp.Enabled = false;
            rfvRendimFideicom.Enabled = false;
            //Ocultar complemento dividendos y sus validaciones
            divDividendos.Visible = false;
            rfvddlCveTipDivOUtil.Enabled = false;
            rfvtxbMontISRAcredRetMexico.Enabled = false;
            rfvtxbMontISRAcredRetExtranjero.Enabled = false;
            rfvddlTipoSocDistrDiv.Enabled = false;
            //Ocultar complemento enajenacion de acciones y sus validaciones
            divEnajenaciondeAcciones.Visible = false;
            rfvtxbContratoIntermediacion.Enabled = false;
            rfvtxbGanancia.Enabled = false;
            rfvtxbPerdida.Enabled = false;
            //Ocultar complemento fideicomiso no empresarial y sus validaciones
            divFideicomisoNoEmpresarial.Visible = false;
            rfvtxbMontTotEntradasPeriodoIngresosOEntradas.Enabled = false;
            rfvtxbPartPropAcumDelFideicomIngresosOEntradas.Enabled = false;
            rfvtxbPropDelMontTotIngresosOEntradas.Enabled = false;
            rfvtxbConceptoIngresosOEntradas.Enabled = false;
            rfvtxbMontTotEgresPeriodoDeduccOSalidas.Enabled = false;
            rfvtxbPartPropDelFideicomDeduccOSalidas.Enabled = false;
            rfvtxbPropDelMontTotDeduccOSalidas.Enabled = false;
            rfvtxbConceptoDeduccOSalidas.Enabled = false;
            rfvtxbMontRetRelPagFideicRetEfectFideicomiso.Enabled = false;
            rfvtxbDescRetRelPagFideicRetEfectFideicomiso.Enabled = false;
            //Ocultar complemento intereses y sus validaciones
            divIntereses.Visible = false;
            rfvtxbMontIntNominal.Enabled = false;
            rfvtxbMontIntReal.Enabled = false;
            rfvtxbPerdidaIntereses.Enabled = false;
            //Ocultar complemento intereses hipotecarios y sus validaciones
            divInteresesHipotecarios.Visible = false;
            rfvtxbSaldoInsoluto.Enabled = false;
            //Ocultar complemento operaciones con derivados y sus validaciones
            divOperacionesconDerivados.Visible = false;
            rfvtxbMontGanAcum.Visible = false;
            rfvtxbMontPerdDed.Visible = false;
            //Ocultar complemento pagos a extranjeros y sus validaciones
            divPagosaExtranjeros.Visible = false;
            rfvddlPaisDeResidParaEfecFiscNoBeneficiario.Enabled = false;
            rfvtxbRFCBeneficiario.Enabled = false;
            rfvtxbCURPBeneficiario.Enabled = false;
            rfvtxbNomDenRazSocBeneficiario.Enabled = false;
            rfvddlConceptoPagoPagosaExtranjeros.Enabled = false;
            rfvtbxDescripcionConceptoPagoPagosaExtranjeros.Enabled = false;
            revtxbRFCBeneficiario.Enabled = false;
            revtxbCURPBeneficiario.Enabled = false;
            //Ocultar complemento planes de retiro y sus validaciones
            divPlanesdeRetiro.Visible = false;
            rfvtxbMontIntRealesDevengAniooInmAnt.Enabled = false;
            //Ocultar complemento premios y sus validaciones
            divPremios.Visible = false;
            rfvddlEntidadFederativaPremios.Enabled = false;
            rfvtxbMontTotPagoPremios.Enabled = false;
            rfvtxbMontTotPagoGravPremios.Enabled = false;
            rfvtxbMontTotPagoExentPremios.Enabled = false;
            //Ocultar complemento sector financiero y sus validaciones
            divSectorFinanciero.Visible = false;
            rfvtxbIdFideicom.Enabled = false;
            rfvtxbDescripFideicom.Enabled = false;
        }

        //ValidacionesDecimales
        #region ValidacionesDecimales
        protected Boolean ValidarDecimalesenComplementoArrendamientoenFideicomiso()
        {
            Boolean valido=true;
            if (ValidarFormatoDecimal(txbPagProvEfecPorFiduc.Text))
                lblValidacionPagProvEfecPorFiduc.Visible = false;
            else
            {
                lblValidacionPagProvEfecPorFiduc.Visible = true;
                valido = false;
            }
            if (ValidarFormatoDecimal(txbRendimFideicom.Text))
                lblValidacionRendimFideicom.Visible = false;
            else
            {
                lblValidacionRendimFideicom.Visible = true;
                valido = false;
            }
            if (ValidarFormatoDecimal(txbDeduccCorresp.Text))
                lblValidacionDeduccCorresp.Visible = false;
            else
            {
                lblValidacionDeduccCorresp.Visible = true;
                valido = false;
            }
            if (!string.IsNullOrEmpty(txbMontTotRet.Text))
            {
                if (ValidarFormatoDecimal(txbMontTotRet.Text))
                    lblValidacionMontTotRet.Visible = false;
                else
                {
                    lblValidacionMontTotRet.Visible = true;
                    valido = false;
                }
            }
            if (!string.IsNullOrEmpty(txbMontResFiscDistFibras.Text))
            {
                if (ValidarFormatoDecimal(txbMontResFiscDistFibras.Text))
                    lblValidacionMontResFiscDistFibras.Visible = false;
                else
                {
                    lblValidacionMontResFiscDistFibras.Visible = true;
                    valido = false;
                }
            }
            if (!string.IsNullOrEmpty(txbMontOtrosConceptDistr.Text))
            {
                if (ValidarFormatoDecimal(txbMontOtrosConceptDistr.Text))
                    lblValidacionMontOtrosConceptDistr.Visible = false;
                else
                {
                    lblValidacionMontOtrosConceptDistr.Visible = true;
                    valido = false;
                }
            }
        return valido;
        }

        protected Boolean ValidarDecimalesenComplementoDividendos() {
            Boolean valido=true;
            if (ValidarFormatoDecimal(txbMontISRAcredRetMexico.Text))
                lblValidaciontxbMontISRAcredRetMexico.Visible = false;
            else {
                lblValidaciontxbMontISRAcredRetMexico.Visible = true;
                valido = false;
            }
            if (ValidarFormatoDecimal(txbMontISRAcredRetExtranjero.Text))
                lblValidaciontxbMontISRAcredRetExtranjero.Visible = false;
            else
            {
                lblValidaciontxbMontISRAcredRetExtranjero.Visible = true;
                valido = false;
            }
            if (!string.IsNullOrEmpty(txbMontRetExtDivExt.Text)) {
                if (ValidarFormatoDecimal(txbMontRetExtDivExt.Text))
                    lblValidaciontxbMontRetExtDivExt.Visible = false;
                else
                {
                    lblValidaciontxbMontRetExtDivExt.Visible = true;
                    valido = false;
                }
            }
            if (!string.IsNullOrEmpty(txbMontISRAcredNal.Text))
            {
                if (ValidarFormatoDecimal(txbMontISRAcredNal.Text))
                    lblValidaciontxbMontISRAcredNal.Visible = false;
                else
                {
                    lblValidaciontxbMontISRAcredNal.Visible = true;
                    valido = false;
                }
            }
            if (!string.IsNullOrEmpty(txbMontDivAcumNal.Text))
            {
                if (ValidarFormatoDecimal(txbMontDivAcumNal.Text))
                    lblValidaciontxbMontDivAcumNal.Visible = false;
                else
                {
                    lblValidaciontxbMontDivAcumNal.Visible = true;
                    valido = false;
                }
            }
            if (!string.IsNullOrEmpty(txbMontDivAcumExt.Text))
            {
                if (ValidarFormatoDecimal(txbMontDivAcumExt.Text))
                    lblValidaciontxbMontDivAcumExt.Visible = false;
                else
                {
                    lblValidaciontxbMontDivAcumExt.Visible = true;
                    valido = false;
                }
            }
            if (!string.IsNullOrEmpty(txbProporcionRem.Text))
            {
                if (ValidarFormatoDecimal(txbProporcionRem.Text))
                    lblValidaciontxbProporcionRem.Visible = false;
                else
                {
                    lblValidaciontxbProporcionRem.Visible = true;
                    valido = false;
                }
            }
            return valido;
        }

        protected Boolean ValidarDecimalesenComplementoEnajenaciondeAcciones() {
            Boolean valido=true;
            if (ValidarFormatoDecimal(txbGanancia.Text))
                lblValidaciontxbGanancia.Visible = false;
            else
            {
                lblValidaciontxbGanancia.Visible = true;
                valido = false;
            }
            if (ValidarFormatoDecimal(txbPerdida.Text))
                lblValidaciontxbPerdida.Visible = false;
            else{
                lblValidaciontxbPerdida.Visible = true;
                valido = false;
            }
            return valido;
        }

        protected Boolean ValidarDecimalesenComplementoFideicomisonoEmpresarial() {
            Boolean valido = true;
            if (ValidarFormatoDecimal(txbMontTotEntradasPeriodoIngresosOEntradas.Text))
                lblValidaciontxbMontTotEntradasPeriodoIngresosOEntradas.Visible = false;
            else {
                lblValidaciontxbMontTotEntradasPeriodoIngresosOEntradas.Visible = true;
                valido = false;
            }
            if (ValidarFormatoDecimal(txbPartPropAcumDelFideicomIngresosOEntradas.Text))
                lblValidaciontxbPartPropAcumDelFideicomIngresosOEntradas.Visible = false;
            else
            {
                lblValidaciontxbPartPropAcumDelFideicomIngresosOEntradas.Visible = true;
                valido = false;
            }
            if (ValidarFormatoDecimal(txbPropDelMontTotIngresosOEntradas.Text))
                lblValidaciontxbPropDelMontTotIngresosOEntradas.Visible = false;
            else
            {
                lblValidaciontxbPropDelMontTotIngresosOEntradas.Visible = true;
                valido = false;
            }
            if (ValidarFormatoDecimal(txbMontTotEgresPeriodoDeduccOSalidas.Text))
                lblValidaciontxbMontTotEgresPeriodoDeduccOSalidas.Visible = false;
            else
            {
                lblValidaciontxbMontTotEgresPeriodoDeduccOSalidas.Visible = true;
                valido = false;
            }
            if (ValidarFormatoDecimal(txbPartPropDelFideicomDeduccOSalidas.Text))
                lblValidaciontxbPartPropDelFideicomDeduccOSalidas.Visible = false;
            else
            {
                lblValidaciontxbPartPropDelFideicomDeduccOSalidas.Visible = true;
                valido = false;
            }
            if (ValidarFormatoDecimal(txbPropDelMontTotDeduccOSalidas.Text))
                lblValidaciontxbPropDelMontTotDeduccOSalidas.Visible = false;
            else
            {
                lblValidaciontxbPropDelMontTotDeduccOSalidas.Visible = true;
                valido = false;
            }
            if (ValidarFormatoDecimal(txbMontRetRelPagFideicRetEfectFideicomiso.Text))
                lblValidaciotxbMontRetRelPagFideicRetEfectFideicomiso.Visible = false;
            else
            {
                lblValidaciotxbMontRetRelPagFideicRetEfectFideicomiso.Visible = true;
                valido = false;
            }
            return valido;
        }

        protected Boolean ValidarDecimalesenComplementoIntereses() {
            Boolean valido = true;
            if (ValidarFormatoDecimal(txbMontIntNominal.Text))
                lblValidaciontxbMontIntNominal.Visible = false;
            else{
                lblValidaciontxbMontIntNominal.Visible = true;
                valido = false;
            }
            if (ValidarFormatoDecimal(txbMontIntReal.Text))
                lblValidaciontxbMontIntReal.Visible = false;
            else
            {
                lblValidaciontxbMontIntReal.Visible = true;
                valido = false;
            }
            if (ValidarFormatoDecimal(txbPerdidaIntereses.Text))
                lblValidaciontxbPerdidaIntereses.Visible = false;
            else
            {
                lblValidaciontxbPerdidaIntereses.Visible = true;
                valido = false;
            }
            return valido;
        }

        protected Boolean ValidarDecimalesenComplementoInteresesHipotecarios() {
            Boolean valido = true;
            if (ValidarFormatoDecimal(txbSaldoInsoluto.Text))
                lblValidaciontxbSaldoInsoluto.Visible = false;
            else {
                lblValidaciontxbSaldoInsoluto.Visible = true;
                valido = false;
            }
            if (!string.IsNullOrEmpty(txbPropDeducDelCredit.Text))
            {
                if (ValidarFormatoDecimal(txbPropDeducDelCredit.Text))
                    lblValidaciontxbPropDeducDelCredit.Visible = false;
                else
                {
                    lblValidaciontxbPropDeducDelCredit.Visible = true;
                    valido = false;
                }
            }
            if (!string.IsNullOrEmpty(txbMontTotIntNominalesDev.Text))
            {
                if (ValidarFormatoDecimal(txbMontTotIntNominalesDev.Text))
                    lblValidaciontxbMontTotIntNominalesDev.Visible = false;
                else
                {
                    lblValidaciontxbMontTotIntNominalesDev.Visible = true;
                    valido = false;
                }
            }
            if (!string.IsNullOrEmpty(txbMontTotIntNominalesDevYPag.Text))
            {
                if (ValidarFormatoDecimal(txbMontTotIntNominalesDevYPag.Text))
                    lblValidaciontxbMontTotIntNominalesDevYPag.Visible = false;
                else
                {
                    lblValidaciontxbMontTotIntNominalesDevYPag.Visible = true;
                    valido = false;
                }
            }
            if (!string.IsNullOrEmpty(txbMontTotIntRealPagDeduc.Text))
            {
                if (ValidarFormatoDecimal(txbMontTotIntRealPagDeduc.Text))
                    lblValidaciontxbMontTotIntRealPagDeduc.Visible = false;
                else
                {
                    lblValidaciontxbMontTotIntRealPagDeduc.Visible = true;
                    valido = false;
                }
            }
            return valido;
        }

        protected Boolean ValidarDecimalesenComplementoOperacionesconDerivados() {
            Boolean valido = true;
            if (ValidarFormatoDecimal(txbMontGanAcum.Text))
                lblValidaciontxbMontGanAcum.Visible = false;
            else
            {
                lblValidaciontxbMontGanAcum.Visible = true;
                valido = false;
            }
            if (ValidarFormatoDecimal(txbMontPerdDed.Text))
                lblValidaciontxbMontPerdDed.Visible = false;
            else
            {
                lblValidaciontxbMontPerdDed.Visible = true;
                valido = false;
            }
            return valido;
        }

        protected Boolean ValidarDecimalesenComplementoPlanesdeRetiro(){
            Boolean valido = true;
            if (!string.IsNullOrEmpty(txbMontTotAportAnioInmAnterior.Text))
            {
                if (ValidarFormatoDecimal(txbMontTotAportAnioInmAnterior.Text))
                    lblValidaciontxbMontTotAportAnioInmAnterior.Visible = false;
                else
                {
                    lblValidaciontxbMontTotAportAnioInmAnterior.Visible = true;
                    valido = false;
                }
            }
            if (ValidarFormatoDecimal(txbMontIntRealesDevengAniooInmAnt.Text))
                lblValidaciontxbMontIntRealesDevengAniooInmAnt.Visible = false;
            else
            {
                lblValidaciontxbMontIntRealesDevengAniooInmAnt.Visible = true;
                valido = false;
            }
            if (!string.IsNullOrEmpty(txbMontTotRetiradoAnioInmAntPer.Text))
            {
                if (ValidarFormatoDecimal(txbMontTotRetiradoAnioInmAntPer.Text))
                    lblValidaciontxbMontTotRetiradoAnioInmAntPer.Visible = false;
                else
                {
                    lblValidaciontxbMontTotRetiradoAnioInmAntPer.Visible = true;
                    valido = false;
                }
            }
            if (!string.IsNullOrEmpty(txbMontTotExentRetiradoAnioInmAnt.Text))
            {
                if (ValidarFormatoDecimal(txbMontTotExentRetiradoAnioInmAnt.Text))
                    lblValidaciontxbMontTotExentRetiradoAnioInmAnt.Visible = false;
                else
                {
                    lblValidaciontxbMontTotExentRetiradoAnioInmAnt.Visible = true;
                    valido = false;
                }
            }
            if (!string.IsNullOrEmpty(txbMontTotExedenteAnioInmAnt.Text))
            {
                if (ValidarFormatoDecimal(txbMontTotExedenteAnioInmAnt.Text))
                    lblValidaciontxbMontTotExedenteAnioInmAnt.Visible = false;
                else
                {
                    lblValidaciontxbMontTotExedenteAnioInmAnt.Visible = true;
                    valido = false;
                }
            }
            if (!string.IsNullOrEmpty(txbMontTotRetiradoAnioInmAnt.Text))
            {
                if (ValidarFormatoDecimal(txbMontTotRetiradoAnioInmAnt.Text))
                    lblValidaciontxbMontTotRetiradoAnioInmAnt.Visible = false;
                else
                {
                    lblValidaciontxbMontTotRetiradoAnioInmAnt.Visible = true;
                    valido = false;
                }
            }
            return valido;
        }

        protected Boolean ValidarDecimalesenComplementoPremios() {
            Boolean valido = true;
            if (ValidarFormatoDecimal(txbMontTotPagoPremios.Text))
                lblValidaciontxbMontTotPagoPremios.Visible = false;
            else
            {
                lblValidaciontxbMontTotPagoPremios.Visible = true;
                valido = false;
            }
            if (ValidarFormatoDecimal(txbMontTotPagoGravPremios.Text))
                lblValidaciontxbMontTotPagoGravPremios.Visible = false;
            else
            {
                lblValidaciontxbMontTotPagoGravPremios.Visible = true;
                valido = false;
            }
            if (ValidarFormatoDecimal(txbMontTotPagoExentPremios.Text))
                lblValidaciontxbMontTotPagoExentPremios.Visible = false;
            else
            {
                lblValidaciontxbMontTotPagoExentPremios.Visible = true;
                valido = false;
            }
            return valido;
        }

        protected Boolean ValidarFormatoDecimal(string valor)
        {
            try
            {
                var valorDecimal = Convert.ToDecimal(valor);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        #endregion

        protected void BuscarDatosCertificado()
        {
            try
            {
                CFDIRIPConexionBD objConexion = new CFDIRIPConexionBD();
                MySqlConnection conexionString = objConexion.ObtenerConexion();
                MySqlCommand sql = new MySqlCommand("SELECT csd, contrasenia, llave_privada  FROM certificado WHERE(idcertificado=(SELECT cetificado FROM emisor WHERE(idemisor='" + ddlEmpresa.SelectedValue + "')))", conexionString);
                MySqlDataReader leersql = sql.ExecuteReader();
                leersql.Read();
                certificadoCsd = leersql.GetString(0);
                certificadoContrasenia = seg.Desencriptar(leersql.GetString(1));
                certificadoLlavePrivada = leersql.GetString(2);
                conexionString.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //protected void GenerarCFDRIP()
        //{
        //    DS.RetencionPagov1.Retenciones cfdirip = new DS.RetencionPagov1.Retenciones();
        //    if (!string.IsNullOrEmpty(txbFolioInterno.Text))
        //        cfdirip.FolioInt = txbFolioInterno.Text;
        //    BuscarDatosCertificado();
        //    cfdirip.Cert = Convert.ToBase64String(File.ReadAllBytes(rutaCertificado + ddlEmpresa.SelectedValue + "/" + certificadoCsd));
        //    cfdirip.NumCert = Util.GetNumberCertificate(File.ReadAllBytes(rutaCertificado + ddlEmpresa.SelectedValue + "/" + certificadoCsd));
        //    DateTime fechaExp = DateTime.Now;

        //    cfdirip.FechaExp = Convert.ToDateTime(fechaExp.ToString("yyyy-MM-ddTHH:mm:sszzz"));
        //    try
        //    {
        //        c_Retenciones cvReten;
        //        if (Enum.TryParse(ddlClaveRetencion.SelectedValue, out cvReten))
        //            cfdirip.CveRetenc = cvReten;
        //        else
        //            MessageBox.Show("Error el valor dentro del campo clave de la retencion no esta dentro de la lista");
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    if(!string.IsNullOrEmpty(txbDescripcionRetencion.Text))
        //        cfdirip.DescRetenc = txbDescripcionRetencion.Text;
        //    //Datos del Emisor
        //    cfdirip.Emisor = new RetencionesEmisor();
        //    cfdirip.Emisor.RFCEmisor = txbRFCEmisor.Text;
        //    if (!string.IsNullOrEmpty(txbRazonSocial.Text))
        //        cfdirip.Emisor.NomDenRazSocE = txbRazonSocial.Text;
        //    if (!string.IsNullOrEmpty(txbCURPEmisor.Text))
        //        cfdirip.Emisor.CURPE = txbCURPEmisor.Text;
        //    //Datos del Receptor
        //    cfdirip.Receptor = new RetencionesReceptor();
        //    try
        //    {
        //        RetencionesReceptorNacionalidad nacionalidadReceptor;
        //        if (Enum.TryParse(ddlNacionalidad.SelectedValue, out nacionalidadReceptor))
        //            cfdirip.Receptor.Nacionalidad = nacionalidadReceptor;
        //        else
        //            MessageBox.Show("Error, el valor en el campo Nacionalidad no esta dentro de la lista");
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    //Nodo Nacional
        //    cfdirip.Receptor.Item = new object();
        //    if (ddlNacionalidad.SelectedValue.Equals("Nacional"))
        //    {
        //        RetencionesReceptorNacional retencionNacional = new RetencionesReceptorNacional();
        //        retencionNacional.RFCRecep = txbRFCCliente.Text;
        //        if(!string.IsNullOrEmpty(txbRazonSocialCliente.Text))
        //            retencionNacional.NomDenRazSocR = txbRazonSocialCliente.Text;
        //        if(!string.IsNullOrEmpty(txbCURPCliente.Text))
        //            retencionNacional.CURPR = txbCURPCliente.Text;
        //        cfdirip.Receptor.Item = retencionNacional;
        //    }
        //    //Nodo Extrangero
        //    if (ddlNacionalidad.SelectedValue.Equals("Extranjero"))
        //    {
        //        RetencionesReceptorExtranjero retencionExtranjero = new RetencionesReceptorExtranjero();
        //        if(!string.IsNullOrEmpty(txbNumRegIdentFiscalCliente.Text))
        //            retencionExtranjero.NumRegIdTrib = txbNumRegIdentFiscalCliente.Text;
        //        retencionExtranjero.NomDenRazSocR = txbRazonSocialCliente.Text;
        //        cfdirip.Receptor.Item = retencionExtranjero;
        //    }
        //    //Periodo
        //    cfdirip.Periodo = new RetencionesPeriodo();
        //    cfdirip.Periodo.MesIni = Convert.ToInt32(ddlMesInicial.SelectedValue);
        //    cfdirip.Periodo.MesFin = Convert.ToInt32(ddlMesFinal.SelectedValue);
        //    cfdirip.Periodo.Ejerc = Convert.ToInt32(ddlEjercicioFiscal.SelectedValue);
        //    //Montos totales
        //    cfdirip.Totales = new RetencionesTotales();
        //    cfdirip.Totales.montoTotOperacion = Convert.ToDecimal(txbMontoTotalDeOperacion.Text);
        //    cfdirip.Totales.montoTotExent = Convert.ToDecimal(txbMontototalExento.Text);
        //    cfdirip.Totales.montoTotGrav = Convert.ToDecimal(txbMontoTotalGravado.Text);
        //    cfdirip.Totales.montoTotRet = Convert.ToDecimal(txbMontoTotalRetenciones.Text);
        //    if (Session["objImpuestosRetenidos"] != null)
        //    {
        //        List<RetencionesTotalesImpRetenidos> lstImpuestosRetenidos = new List<RetencionesTotalesImpRetenidos>();
        //        lstImpuestosRetenidos = Session["objImpuestosRetenidos"] as List<RetencionesTotalesImpRetenidos>;
        //        cfdirip.Totales.ImpRetenidos = new RetencionesTotalesImpRetenidos[lstImpuestosRetenidos.Count()];
        //        cfdirip.Totales.ImpRetenidos = lstImpuestosRetenidos.ToArray();
        //    }
        //    try
        //    {
        //        switch (ddlComplemento.SelectedValue)
        //        {
        //            case "1":
        //                #region Arrendamiento en fideicomiso *
        //                Arrendamientoenfideicomiso arrendamientoenFideicomiso = new Arrendamientoenfideicomiso();
        //                arrendamientoenFideicomiso.PagProvEfecPorFiduc = Convert.ToDecimal(txbPagProvEfecPorFiduc.Text);
        //                arrendamientoenFideicomiso.RendimFideicom = Convert.ToDecimal(txbRendimFideicom.Text);
        //                arrendamientoenFideicomiso.DeduccCorresp = Convert.ToDecimal(txbDeduccCorresp.Text);
        //                if (!string.IsNullOrEmpty(txbMontTotRet.Text))
        //                {
        //                    arrendamientoenFideicomiso.MontTotRet = Convert.ToDecimal(txbMontTotRet.Text);
        //                    arrendamientoenFideicomiso.MontTotRetSpecified = true;
        //                }else
        //                    arrendamientoenFideicomiso.MontTotRetSpecified = false;
        //                if (!string.IsNullOrEmpty(txbMontResFiscDistFibras.Text))
        //                {
        //                    arrendamientoenFideicomiso.MontResFiscDistFibras = Convert.ToDecimal(txbMontResFiscDistFibras.Text);
        //                    arrendamientoenFideicomiso.MontResFiscDistFibrasSpecified = true;
        //                }else
        //                    arrendamientoenFideicomiso.MontResFiscDistFibrasSpecified = false;
        //                if (!string.IsNullOrEmpty(txbMontOtrosConceptDistr.Text))
        //                {
        //                    arrendamientoenFideicomiso.MontOtrosConceptDistr = Convert.ToDecimal(txbMontOtrosConceptDistr.Text);
        //                    arrendamientoenFideicomiso.MontOtrosConceptDistrSpecified = true;
        //                }else
        //                    arrendamientoenFideicomiso.MontOtrosConceptDistrSpecified = false;
        //                if (!string.IsNullOrEmpty(txbDescrMontOtrosConceptDistr.Text))
        //                    arrendamientoenFideicomiso.DescrMontOtrosConceptDistr = txbDescrMontOtrosConceptDistr.Text;
        //                cfdirip.SchemaLocation += " http://www.sat.gob.mx/esquemas/retencionpago/1/arrendamientoenfideicomiso http://www.sat.gob.mx/esquemas/retencionpago/1/arrendamientoenfideicomiso/arrendamientoenfideicomiso.xsd";
        //                cfdirip.Complemento = new RetencionesComplemento();
        //                XmlSerializerNamespaces namespacesarrendamientoenFideicomiso = new XmlSerializerNamespaces();
        //                namespacesarrendamientoenFideicomiso.Add("arrendamientoenfideicomiso", "http://www.sat.gob.mx/esquemas/retencionpago/1/arrendamientoenfideicomiso");
        //                XmlSerializer serializerarrendamientoenFideicomiso = new XmlSerializer(typeof(Arrendamientoenfideicomiso));
        //                StringWriter writerarrendamientoenFideicomiso = new StringWriter();
        //                serializerarrendamientoenFideicomiso.Serialize(writerarrendamientoenFideicomiso, arrendamientoenFideicomiso, namespacesarrendamientoenFideicomiso);
        //                XmlDocument xmlarrendamientoenFideicomiso = new XmlDocument();
        //                xmlarrendamientoenFideicomiso.LoadXml(writerarrendamientoenFideicomiso.ToString());
        //                cfdirip.Complemento.Any = new XmlElement[1];
        //                cfdirip.Complemento.Any[0] = xmlarrendamientoenFideicomiso.DocumentElement;
        //                #endregion
        //                break;
        //            case "2":
        //                #region Dividendos *
        //                Dividendos dividendos = new Dividendos();
        //                if (!string.IsNullOrEmpty(ddlCveTipDivOUtil.SelectedValue))
        //                {
        //                    dividendos.DividOUtil = new DividendosDividOUtil();
        //                    try
        //                    {
        //                        c_TipoDividendoOUtilidadDistribuida enumCveTipDivOUtil;
        //                        if (Enum.TryParse(ddlCveTipDivOUtil.SelectedValue, out enumCveTipDivOUtil))
        //                            dividendos.DividOUtil.CveTipDivOUtil = enumCveTipDivOUtil;
        //                        else
        //                            MessageBox.Show("Error, el valor en el campo \"Tipo de dividendo\"no esta dentro de la lista");
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        MessageBox.Show(ex.Message);
        //                    }
        //                    dividendos.DividOUtil.MontISRAcredRetMexico = Convert.ToDecimal(txbMontISRAcredRetMexico.Text);
        //                    dividendos.DividOUtil.MontISRAcredRetExtranjero = Convert.ToDecimal(txbMontISRAcredRetExtranjero.Text);
        //                    if (!string.IsNullOrEmpty(txbMontRetExtDivExt.Text))
        //                    {
        //                        dividendos.DividOUtil.MontRetExtDivExt = Convert.ToDecimal(txbMontRetExtDivExt.Text);
        //                        dividendos.DividOUtil.MontRetExtDivExtSpecified = true;
        //                    }else
        //                        dividendos.DividOUtil.MontRetExtDivExtSpecified = false;
        //                    try
        //                    {
        //                        DividendosDividOUtilTipoSocDistrDiv enumTipoSocDistrDiv;
        //                        if (Enum.TryParse(ddlTipoSocDistrDiv.SelectedValue, out enumTipoSocDistrDiv))
        //                            dividendos.DividOUtil.TipoSocDistrDiv = enumTipoSocDistrDiv;
        //                        else
        //                            MessageBox.Show("Error, el valor en el campo \"Sociedad de distribucion del dividendo\"no esta dentro de la lista");
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        MessageBox.Show(ex.Message);
        //                    }
        //                    if (!string.IsNullOrEmpty(txbMontISRAcredNal.Text))
        //                    {
        //                        dividendos.DividOUtil.MontISRAcredNal = Convert.ToDecimal(txbMontISRAcredNal.Text);
        //                        dividendos.DividOUtil.MontISRAcredNalSpecified = true;
        //                    }else
        //                        dividendos.DividOUtil.MontISRAcredNalSpecified = false;
        //                    if (!string.IsNullOrEmpty(txbMontDivAcumNal.Text))
        //                    {
        //                        dividendos.DividOUtil.MontDivAcumNal = Convert.ToDecimal(txbMontDivAcumNal.Text);
        //                        dividendos.DividOUtil.MontDivAcumNalSpecified = true;
        //                    }else
        //                        dividendos.DividOUtil.MontDivAcumNalSpecified = false;
        //                    if (!string.IsNullOrEmpty(txbMontDivAcumExt.Text))
        //                    {
        //                        dividendos.DividOUtil.MontDivAcumExt = Convert.ToDecimal(txbMontDivAcumExt.Text);
        //                        dividendos.DividOUtil.MontDivAcumExtSpecified = true;
        //                    }else
        //                        dividendos.DividOUtil.MontDivAcumExtSpecified = false;
        //                }
        //                if (!string.IsNullOrEmpty(txbProporcionRem.Text))
        //                {
        //                    dividendos.Remanente = new DividendosRemanente();
        //                    dividendos.Remanente.ProporcionRem = Convert.ToDecimal(txbProporcionRem.Text);
        //                }
        //                cfdirip.SchemaLocation += "http://www.sat.gob.mx/esquemas/retencionpago/1/dividendos http://www.sat.gob.mx/esquemas/retencionpago/1/dividendos/dividendos.xsd";
        //                cfdirip.Complemento = new RetencionesComplemento();
        //XmlSerializerNamespaces namespacesdividendos = new XmlSerializerNamespaces();
        //                namespacesdividendos.Add("dividendos", "http://www.sat.gob.mx/esquemas/retencionpago/1/dividendos");
        //                XmlSerializer serializerdividendos = new XmlSerializer(typeof(Dividendos));
        //                StringWriter writerdividendos = new StringWriter();
        //                serializerdividendos.Serialize(writerdividendos, dividendos, namespacesdividendos);
        //                XmlDocument xmldividendos = new XmlDocument();
        //                xmldividendos.LoadXml(writerdividendos.ToString());
        //                cfdirip.Complemento.Any = new XmlElement[1];
        //                cfdirip.Complemento.Any[0] = xmldividendos.DocumentElement;
        //                #endregion
        //                break;
        //            case "3":
        //                #region Enajenación de acciones *
        //                EnajenaciondeAcciones enajenaciondeAcciones = new EnajenaciondeAcciones();
        //                enajenaciondeAcciones.ContratoIntermediacion = txbContratoIntermediacion.Text;
        //                enajenaciondeAcciones.Ganancia = Convert.ToDecimal(txbGanancia.Text);
        //                enajenaciondeAcciones.Perdida = Convert.ToDecimal(txbPerdida.Text);
        //                cfdirip.SchemaLocation += "http://www.sat.gob.mx/esquemas/retencionpago/1/enajenaciondeacciones http://www.sat.gob.mx/esquemas/retencionpago/1/enajenaciondeacciones/enajenaciondeacciones.xsd";
        //                cfdirip.Complemento = new RetencionesComplemento();
        //                XmlSerializerNamespaces namespacesenajenaciondeAcciones = new XmlSerializerNamespaces();
        //                namespacesenajenaciondeAcciones.Add("enajenaciondeacciones", "http://www.sat.gob.mx/esquemas/retencionpago/1/enajenaciondeacciones");
        //                XmlSerializer serializerenajenaciondeAcciones = new XmlSerializer(typeof(EnajenaciondeAcciones));
        //                StringWriter writerenajenaciondeAcciones = new StringWriter();
        //                serializerenajenaciondeAcciones.Serialize(writerenajenaciondeAcciones, enajenaciondeAcciones, namespacesenajenaciondeAcciones);
        //                XmlDocument xmlenajenaciondeAcciones = new XmlDocument();
        //                xmlenajenaciondeAcciones.LoadXml(writerenajenaciondeAcciones.ToString());
        //                cfdirip.Complemento.Any = new XmlElement[1];
        //                cfdirip.Complemento.Any[0] = xmlenajenaciondeAcciones.DocumentElement;
        //                #endregion
        //                break;
        //            case "4":
        //                #region Fideicomiso no empresarial *
        //                Fideicomisonoempresarial fideicomisonoEmpresarial = new Fideicomisonoempresarial();
        //                fideicomisonoEmpresarial.IngresosOEntradas = new FideicomisonoempresarialIngresosOEntradas();
        //                fideicomisonoEmpresarial.IngresosOEntradas.MontTotEntradasPeriodo = Convert.ToDecimal(txbMontTotEntradasPeriodoIngresosOEntradas.Text);
        //                fideicomisonoEmpresarial.IngresosOEntradas.PartPropAcumDelFideicom = Convert.ToDecimal(txbPartPropAcumDelFideicomIngresosOEntradas.Text);
        //                fideicomisonoEmpresarial.IngresosOEntradas.PropDelMontTot = Convert.ToDecimal(txbPropDelMontTotIngresosOEntradas.Text);
        //                fideicomisonoEmpresarial.IngresosOEntradas.IntegracIngresos = new FideicomisonoempresarialIngresosOEntradasIntegracIngresos();
        //                fideicomisonoEmpresarial.IngresosOEntradas.IntegracIngresos.Concepto = txbConceptoIngresosOEntradas.Text;
        //                fideicomisonoEmpresarial.DeduccOSalidas = new FideicomisonoempresarialDeduccOSalidas();
        //                fideicomisonoEmpresarial.DeduccOSalidas.MontTotEgresPeriodo = Convert.ToDecimal(txbMontTotEgresPeriodoDeduccOSalidas.Text);
        //                fideicomisonoEmpresarial.DeduccOSalidas.PartPropDelFideicom = Convert.ToDecimal(txbPartPropDelFideicomDeduccOSalidas.Text);
        //                fideicomisonoEmpresarial.DeduccOSalidas.PropDelMontTot = Convert.ToDecimal(txbPropDelMontTotDeduccOSalidas.Text);
        //                fideicomisonoEmpresarial.DeduccOSalidas.IntegracEgresos = new FideicomisonoempresarialDeduccOSalidasIntegracEgresos();
        //                fideicomisonoEmpresarial.DeduccOSalidas.IntegracEgresos.ConceptoS = txbConceptoDeduccOSalidas.Text;
        //                fideicomisonoEmpresarial.RetEfectFideicomiso = new FideicomisonoempresarialRetEfectFideicomiso();
        //                fideicomisonoEmpresarial.RetEfectFideicomiso.MontRetRelPagFideic = Convert.ToDecimal(txbMontRetRelPagFideicRetEfectFideicomiso.Text);
        //                fideicomisonoEmpresarial.RetEfectFideicomiso.DescRetRelPagFideic = txbDescRetRelPagFideicRetEfectFideicomiso.Text;
        //                cfdirip.SchemaLocation += "http://www.sat.gob.mx/esquemas/retencionpago/1/fideicomisonoempresarial http://www.sat.gob.mx/esquemas/retencionpago/1/fideicomisonoempresarial/fideicomisonoempresarial.xsd";
        //                cfdirip.Complemento = new RetencionesComplemento();
        //                XmlSerializerNamespaces namespacesfideicomisonoempresarial = new XmlSerializerNamespaces();
        //                namespacesfideicomisonoempresarial.Add("fideicomisonoempresarial", "http://www.sat.gob.mx/esquemas/retencionpago/1/fideicomisonoempresarial");
        //                XmlSerializer serializerfideicomisonoempresarial = new XmlSerializer(typeof(Fideicomisonoempresarial));
        //                StringWriter writerfideicomisonoempresarial = new StringWriter();
        //                serializerfideicomisonoempresarial.Serialize(writerfideicomisonoempresarial, fideicomisonoEmpresarial, namespacesfideicomisonoempresarial);
        //                XmlDocument xmlfideicomisonoempresarial = new XmlDocument();
        //                xmlfideicomisonoempresarial.LoadXml(writerfideicomisonoempresarial.ToString());
        //                cfdirip.Complemento.Any = new XmlElement[1];
        //                cfdirip.Complemento.Any[0] = xmlfideicomisonoempresarial.DocumentElement;
        //                #endregion
        //                break;
        //            case "5":
        //                #region Intereses *
        //                Intereses intereses = new Intereses();
        //                string respSistFinanciero, respRetiroAORESRetInt, respOperFinancDerivad;
        //                try
        //                {
        //                    InteresesSistFinanciero enumSistFinanciero;
        //                    InteresesRetiroAORESRetInt enumRetiroAORESRetInt;
        //                    InteresesOperFinancDerivad enumOperFinancDerivad;
        //                    //SistFinanciero
        //                    if (chkSistFinanciero.Checked)
        //                        respSistFinanciero = "SI";
        //                    else
        //                        respSistFinanciero = "NO";
        //                    Session["respSistFinanciero"] = respSistFinanciero;
        //                    if (Enum.TryParse(respSistFinanciero, out enumSistFinanciero))
        //                        intereses.SistFinanciero = enumSistFinanciero;
        //                    else
        //                        MessageBox.Show("Error, el valor en el campo \"Intereses obtenidos en el periodo provienen del sistema financiero\"no esta dentro de la lista");
        //                    //RetiroAORESRetInt
        //                    if (chklRetiroAORESRetInt.Checked)
        //                        respRetiroAORESRetInt = "SI";
        //                    else
        //                        respRetiroAORESRetInt = "NO";
        //                    Session["respRetiroAORESRetInt"] = respRetiroAORESRetInt;
        //                    if (Enum.TryParse(respRetiroAORESRetInt, out enumRetiroAORESRetInt))
        //                        intereses.RetiroAORESRetInt = enumRetiroAORESRetInt;
        //                    else
        //                        MessageBox.Show("Error, el valor en el campo \"Intereses obtenidos fueron retirados en el periodo\"no esta dentro de la lista");
        //                    //OperFinancDerivad
        //                    if (chkOperFinancDerivad.Checked)
        //                        respOperFinancDerivad = "SI";
        //                    else
        //                        respOperFinancDerivad = "NO";
        //                    Session["respOperFinancDerivad"] = respOperFinancDerivad;
        //                    if (Enum.TryParse(respOperFinancDerivad, out enumOperFinancDerivad))
        //                        intereses.OperFinancDerivad = enumOperFinancDerivad;
        //                    else
        //                        MessageBox.Show("Error, el valor en el campo \"Intereses obtenidos corresponden a operaciones financieras derivadas\"no esta dentro de la lista");
        //                }
        //                catch (Exception ex)
        //                {
        //                    MessageBox.Show(ex.Message);
        //                }
        //                intereses.MontIntNominal = Convert.ToDecimal(txbMontIntNominal.Text);
        //                intereses.MontIntReal = Convert.ToDecimal(txbMontIntReal.Text);
        //                intereses.Perdida = Convert.ToDecimal(txbPerdidaIntereses.Text);
        //                cfdirip.SchemaLocation += "http://www.sat.gob.mx/esquemas/retencionpago/1/intereses http://www.sat.gob.mx/esquemas/retencionpago/1/intereses/intereses.xsd";
        //                cfdirip.Complemento = new RetencionesComplemento();
        //                XmlSerializerNamespaces namespacesintereses = new XmlSerializerNamespaces();
        //                namespacesintereses.Add("intereses", "http://www.sat.gob.mx/esquemas/retencionpago/1/intereses");
        //                XmlSerializer serializerintereses = new XmlSerializer(typeof(Intereses));
        //                StringWriter writerintereses = new StringWriter();
        //                serializerintereses.Serialize(writerintereses, intereses, namespacesintereses);
        //                XmlDocument xmlintereses = new XmlDocument();
        //                xmlintereses.LoadXml(writerintereses.ToString());
        //                cfdirip.Complemento.Any = new XmlElement[1];
        //                cfdirip.Complemento.Any[0] = xmlintereses.DocumentElement;
        //                #endregion
        //                break;
        //            case "6":
        //                #region Intereses hipotecarios *
        //                Intereseshipotecarios interesesHipotecarios = new Intereseshipotecarios();
        //                string respCreditoDeInstFinanc;
        //                try
        //                {
        //                    IntereseshipotecariosCreditoDeInstFinanc enumCreditoDeInstFinanc;
        //                    if (chkCreditoDeInstFinanc.Checked)
        //                        respCreditoDeInstFinanc = "SI";
        //                    else
        //                        respCreditoDeInstFinanc = "NO";
        //                    Session["respCreditoDeInstFinanc"] = respCreditoDeInstFinanc;
        //                    if (Enum.TryParse(respCreditoDeInstFinanc, out enumCreditoDeInstFinanc))
        //                        interesesHipotecarios.CreditoDeInstFinanc = enumCreditoDeInstFinanc;
        //                    else
        //                        MessageBox.Show("Error, el valor en el campo \"El crédito otorgado fue por institución financiera\" no esta dentro de la lista");
        //                }
        //                catch (Exception ex)
        //                {
        //                    MessageBox.Show(ex.Message);
        //                }
        //                interesesHipotecarios.SaldoInsoluto = Convert.ToDecimal(txbSaldoInsoluto.Text);
        //                //PropDeducDelCredit 
        //                if (!string.IsNullOrEmpty(txbPropDeducDelCredit.Text))
        //                {
        //                    interesesHipotecarios.PropDeducDelCredit = Convert.ToDecimal(txbPropDeducDelCredit.Text);
        //                    interesesHipotecarios.PropDeducDelCreditSpecified=true;
        //                }else
        //                    interesesHipotecarios.PropDeducDelCreditSpecified = false;
        //                //MontTotIntNominalesDev
        //                if (!string.IsNullOrEmpty(txbMontTotIntNominalesDev.Text))
        //                {
        //                    interesesHipotecarios.MontTotIntNominalesDev = Convert.ToDecimal(txbMontTotIntNominalesDev.Text);
        //                    interesesHipotecarios.MontTotIntNominalesDevSpecified = true;
        //                }else
        //                    interesesHipotecarios.MontTotIntNominalesDevSpecified = false;
        //                //MontTotIntNominalesDevYPagSpecified
        //                if (!string.IsNullOrEmpty(txbMontTotIntNominalesDevYPag.Text))
        //                {
        //                    interesesHipotecarios.MontTotIntNominalesDevYPag = Convert.ToDecimal(txbMontTotIntNominalesDevYPag.Text);
        //                    interesesHipotecarios.MontTotIntNominalesDevYPagSpecified = true;
        //                }else
        //                    interesesHipotecarios.MontTotIntNominalesDevYPagSpecified = false;
        //                //MontTotIntRealPagDeduc
        //                if (!string.IsNullOrEmpty(txbMontTotIntRealPagDeduc.Text))
        //                {
        //                    interesesHipotecarios.MontTotIntRealPagDeduc = Convert.ToDecimal(txbMontTotIntRealPagDeduc.Text);
        //                    interesesHipotecarios.MontTotIntRealPagDeducSpecified = true; 
        //                }else
        //                    interesesHipotecarios.MontTotIntRealPagDeducSpecified = false;
        //                //NumContrato 
        //                if (!string.IsNullOrEmpty(txbNumContrato.Text))
        //                    interesesHipotecarios.NumContrato = txbNumContrato.Text;
        //                cfdirip.SchemaLocation += "http://www.sat.gob.mx/esquemas/retencionpago/1/intereseshipotecarios http://www.sat.gob.mx/esquemas/retencionpago/1/intereseshipotecarios/intereseshipotecarios.xsd";
        //                cfdirip.Complemento = new RetencionesComplemento();
        //                XmlSerializerNamespaces namespacesintereseshipotecarios = new XmlSerializerNamespaces();
        //                namespacesintereseshipotecarios.Add("intereseshipotecarios", "http://www.sat.gob.mx/esquemas/retencionpago/1/intereseshipotecarios");
        //                XmlSerializer serializerintereseshipotecarios = new XmlSerializer(typeof(Intereseshipotecarios));
        //                StringWriter writerintereseshipotecarios = new StringWriter();
        //                serializerintereseshipotecarios.Serialize(writerintereseshipotecarios, interesesHipotecarios, namespacesintereseshipotecarios);
        //                XmlDocument xmlintereseshipotecarios = new XmlDocument();
        //                xmlintereseshipotecarios.LoadXml(writerintereseshipotecarios.ToString());
        //                cfdirip.Complemento.Any = new XmlElement[1];
        //                cfdirip.Complemento.Any[0] = xmlintereseshipotecarios.DocumentElement;
        //                #endregion
        //                break;
        //            case "7":
        //                #region Operaciones con derivados *
        //                Operacionesconderivados operacionesconDerivados = new Operacionesconderivados();
        //                operacionesconDerivados.MontGanAcum = Convert.ToDecimal(txbMontGanAcum.Text);
        //                operacionesconDerivados.MontPerdDed = Convert.ToDecimal(txbMontPerdDed.Text);
        //                cfdirip.SchemaLocation += "http://www.sat.gob.mx/esquemas/retencionpago/1/operacionesconderivados http://www.sat.gob.mx/esquemas/retencionpago/1/operacionesconderivados/operacionesconderivados.xsd";
        //                cfdirip.Complemento = new RetencionesComplemento();
        //                XmlSerializerNamespaces namespacesoperacionesconDerivados = new XmlSerializerNamespaces();
        //                namespacesoperacionesconDerivados.Add("operacionesconderivados", "http://www.sat.gob.mx/esquemas/retencionpago/1/operacionesconderivados");
        //                XmlSerializer serializeroperacionesconDerivados = new XmlSerializer(typeof(Operacionesconderivados));
        //                StringWriter writeroperacionesconDerivados = new StringWriter();
        //                serializeroperacionesconDerivados.Serialize(writeroperacionesconDerivados, operacionesconDerivados, namespacesoperacionesconDerivados);
        //                XmlDocument xmloperacionesconDerivados = new XmlDocument();
        //                xmloperacionesconDerivados.LoadXml(writeroperacionesconDerivados.ToString());
        //                cfdirip.Complemento.Any = new XmlElement[1];
        //                cfdirip.Complemento.Any[0] = xmloperacionesconDerivados.DocumentElement;
        //                #endregion
        //                break;
        //            case "8":
        //                #region Pagos a extranjeros *
        //                Pagosaextranjeros pagosaExtranjeros = new Pagosaextranjeros();
        //                c_TipoContribuyenteSujetoRetencion enumConceptoPago;
        //                string respEsBenefEfectDelCobro;
        //                if (chkEsBenefEfectDelCobro.Checked)
        //                {
        //                    #region Beneficiario
        //                    respEsBenefEfectDelCobro = "SI";
        //                    try
        //                    {
        //                        pagosaExtranjeros.Beneficiario = new PagosaextranjerosBeneficiario();
        //                        pagosaExtranjeros.Beneficiario.RFC = txbRFCBeneficiario.Text;
        //                        pagosaExtranjeros.Beneficiario.CURP = txbCURPBeneficiario.Text;
        //                        pagosaExtranjeros.Beneficiario.NomDenRazSocB = txbNomDenRazSocBeneficiario.Text;
        //                        if (Enum.TryParse(ddlConceptoPagoPagosaExtranjeros.SelectedValue, out enumConceptoPago))
        //                            pagosaExtranjeros.Beneficiario.ConceptoPago = enumConceptoPago;
        //                        else
        //                            MessageBox.Show("Error, el valor en el campo \"Tipo de contribuyente sujeto a la retención\" no esta dentro de la lista");
        //                        pagosaExtranjeros.Beneficiario.DescripcionConcepto = tbxDescripcionConceptoPagoPagosaExtranjeros.Text;
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        MessageBox.Show(ex.Message);
        //                    }
        //                    #endregion
        //                }
        //                else
        //                {
        //                    #region No beneficiario
        //                    respEsBenefEfectDelCobro = "NO";
        //                    try
        //                    {
        //                        pagosaExtranjeros.NoBeneficiario = new PagosaextranjerosNoBeneficiario();
        //                        c_Pais enumPaisDeResidParaEfecFiscNoBeneficiario;
        //                        if (Enum.TryParse(ddlPaisDeResidParaEfecFiscNoBeneficiario.SelectedValue, out enumPaisDeResidParaEfecFiscNoBeneficiario))
        //                            pagosaExtranjeros.NoBeneficiario.PaisDeResidParaEfecFisc = enumPaisDeResidParaEfecFiscNoBeneficiario;
        //                        else
        //                            MessageBox.Show("Error, el valor en el campo \"Clave del país de residencia del extranjero\" no esta dentro de la lista");
        //                        if (Enum.TryParse(ddlConceptoPagoPagosaExtranjeros.SelectedValue, out enumConceptoPago))
        //                            pagosaExtranjeros.NoBeneficiario.ConceptoPago = enumConceptoPago;
        //                        else
        //                            MessageBox.Show("Error, el valor en el campo \"Tipo de contribuyente sujeto a la retención\" no esta dentro de la lista");
        //                        pagosaExtranjeros.NoBeneficiario.DescripcionConcepto = tbxDescripcionConceptoPagoPagosaExtranjeros.Text;
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        MessageBox.Show(ex.Message);
        //                    }
        //                    #endregion
        //                }
        //                Session["respEsBenefEfectDelCobro"] = respEsBenefEfectDelCobro;
        //                PagosaextranjerosEsBenefEfectDelCobro enumEsBenefEfectDelCobro;
        //                if (Enum.TryParse(respEsBenefEfectDelCobro, out enumEsBenefEfectDelCobro))
        //                    pagosaExtranjeros.EsBenefEfectDelCobro = enumEsBenefEfectDelCobro;
        //                else
        //                    MessageBox.Show("Error, el valor en el campo \"El beneficiario del pago es la misma persona que retiene\" no esta dentro de la lista");
        //                cfdirip.SchemaLocation += "http://www.sat.gob.mx/esquemas/retencionpago/1/pagosaextranjeros http://www.sat.gob.mx/esquemas/retencionpago/1/pagosaextranjeros/pagosaextranjeros.xsd";
        //                cfdirip.Complemento = new RetencionesComplemento();
        //                XmlSerializerNamespaces namespacespagosaExtranjeros = new XmlSerializerNamespaces();
        //                namespacespagosaExtranjeros.Add("pagosaextranjeros", "http://www.sat.gob.mx/esquemas/retencionpago/1/pagosaextranjeros");
        //                XmlSerializer serializerpagosaExtranjeros = new XmlSerializer(typeof(Pagosaextranjeros));
        //                StringWriter writerpagosaExtranjeros = new StringWriter();
        //                serializerpagosaExtranjeros.Serialize(writerpagosaExtranjeros, pagosaExtranjeros, namespacespagosaExtranjeros);
        //                XmlDocument xmlpagosaExtranjeros = new XmlDocument();
        //                xmlpagosaExtranjeros.LoadXml(writerpagosaExtranjeros.ToString());
        //                cfdirip.Complemento.Any = new XmlElement[1];
        //                cfdirip.Complemento.Any[0] = xmlpagosaExtranjeros.DocumentElement;
        //                #endregion
        //                break;
        //            case "9":
        //                #region Planes de retiro *
        //                string resSistemaFinanc, resHuboRetirosAnioInmAntPer, resHuboRetirosAnioInmAnt;
        //                Planesderetiro planesderetiro = new Planesderetiro();
        //                PlanesderetiroSistemaFinanc enumSistemaFinanc;
        //                PlanesderetiroHuboRetirosAnioInmAntPer enumHuboRetirosAnioInmAntPer;
        //                PlanesderetiroHuboRetirosAnioInmAnt enumHuboRetirosAnioInmAnt;
        //                if (chkSistemaFinanc.Checked)
        //                    resSistemaFinanc = "SI";
        //                else
        //                    resSistemaFinanc = "NO";
        //                Session["resSistemaFinanc"] = resSistemaFinanc;
        //                if (Enum.TryParse(resSistemaFinanc, out enumSistemaFinanc))
        //                    planesderetiro.SistemaFinanc = enumSistemaFinanc;
        //                else
        //                    MessageBox.Show("Error, el valor en el campo \"Los planes personales de retiro son del sistema financiero\" no esta dentro de la lista");
        //                if (!string.IsNullOrEmpty(txbMontTotAportAnioInmAnterior.Text))
        //                {
        //                    planesderetiro.MontTotAportAnioInmAnterior = Convert.ToDecimal(txbMontTotAportAnioInmAnterior.Text);
        //                    planesderetiro.MontTotAportAnioInmAnteriorSpecified = true;
        //                }
        //                else
        //                    planesderetiro.MontTotAportAnioInmAnteriorSpecified = false;
        //                planesderetiro.MontIntRealesDevengAniooInmAnt=Convert.ToDecimal(txbMontIntRealesDevengAniooInmAnt.Text);
        //                //HuboRetirosAnioInmAntPer
        //                if (chkHuboRetirosAnioInmAntPer.Checked)
        //                    resHuboRetirosAnioInmAntPer = "SI";
        //                else
        //                    resHuboRetirosAnioInmAntPer = "NO";
        //                Session["resHuboRetirosAnioInmAntPer"] = resHuboRetirosAnioInmAntPer;
        //                if (Enum.TryParse(resHuboRetirosAnioInmAntPer, out enumHuboRetirosAnioInmAntPer))
        //                    planesderetiro.HuboRetirosAnioInmAntPer = enumHuboRetirosAnioInmAntPer;
        //                else
        //                    MessageBox.Show("Error, el valor en el campo \"Se realizaron retiros de recursos invertidos y sus rendimientos\" no esta dentro de la lista");
        //                //MontTotRetiradoAnioInmAntPer
        //                if(!string.IsNullOrEmpty(txbMontTotRetiradoAnioInmAntPer.Text)){
        //                    planesderetiro.MontTotRetiradoAnioInmAntPer = Convert.ToDecimal(txbMontTotRetiradoAnioInmAntPer.Text);
        //                    planesderetiro.MontTotRetiradoAnioInmAntPerSpecified = true;
        //                }else
        //                    planesderetiro.MontTotRetiradoAnioInmAntPerSpecified = false;
        //                //MontTotExentRetiradoAnioInmAnt
        //                if (!string.IsNullOrEmpty(txbMontTotExentRetiradoAnioInmAnt.Text))
        //                {
        //                    planesderetiro.MontTotExentRetiradoAnioInmAnt = Convert.ToDecimal(txbMontTotExentRetiradoAnioInmAnt.Text);
        //                    planesderetiro.MontTotExentRetiradoAnioInmAntSpecified = true;
        //                }else
        //                    planesderetiro.MontTotExentRetiradoAnioInmAntSpecified = false;
        //                //MontTotExedenteAnioInmAnt
        //                if (!string.IsNullOrEmpty(txbMontTotExedenteAnioInmAnt.Text))
        //                {
        //                    planesderetiro.MontTotExedenteAnioInmAnt = Convert.ToDecimal(txbMontTotExedenteAnioInmAnt.Text);
        //                    planesderetiro.MontTotExedenteAnioInmAntSpecified = true;
        //                }
        //                else
        //                    planesderetiro.MontTotExedenteAnioInmAntSpecified = false;
        //                if (chkHuboRetirosAnioInmAnt.Checked)
        //                    resHuboRetirosAnioInmAnt= "SI";
        //                else
        //                    resHuboRetirosAnioInmAnt= "NO";
        //                Session["resHuboRetirosAnioInmAnt"] = resHuboRetirosAnioInmAnt;
        //                if (Enum.TryParse(resHuboRetirosAnioInmAnt, out enumHuboRetirosAnioInmAnt))
        //                    planesderetiro.HuboRetirosAnioInmAnt = enumHuboRetirosAnioInmAnt;
        //                else
        //                    MessageBox.Show("Error, el valor en el campo \"Se realizaron retiros en el ejercicio inmediato anterior\" no esta dentro de la lista");
        //                //MontTotRetiradoAnioInmAnt
        //                if (!string.IsNullOrEmpty(txbMontTotRetiradoAnioInmAnt.Text))
        //                {
        //                    planesderetiro.MontTotRetiradoAnioInmAnt = Convert.ToDecimal(txbMontTotRetiradoAnioInmAnt.Text);
        //                    planesderetiro.MontTotRetiradoAnioInmAntSpecified = true;
        //                }else
        //                    planesderetiro.MontTotRetiradoAnioInmAntSpecified = false;
        //                cfdirip.SchemaLocation += " http://www.sat.gob.mx/esquemas/retencionpago/1/planesderetiro http://www.sat.gob.mx/esquemas/retencionpago/1/planesderetiro/planesderetiro.xsd";
        //                cfdirip.Complemento = new RetencionesComplemento();
        //                XmlSerializerNamespaces namespacesPlanesderetiro = new XmlSerializerNamespaces();
        //                namespacesPlanesderetiro.Add("planesderetiro", "http://www.sat.gob.mx/esquemas/retencionpago/1/planesderetiro");
        //                XmlSerializer serializerPlanesderetiro = new XmlSerializer(typeof(Planesderetiro));
        //                StringWriter writerPlanesderetiro = new StringWriter();
        //                serializerPlanesderetiro.Serialize(writerPlanesderetiro, planesderetiro, namespacesPlanesderetiro);
        //                XmlDocument xmlPlanesderetiro = new XmlDocument();
        //                xmlPlanesderetiro.LoadXml(writerPlanesderetiro.ToString());
        //                cfdirip.Complemento.Any = new XmlElement[1];
        //                cfdirip.Complemento.Any[0] = xmlPlanesderetiro.DocumentElement;
        //                #endregion
        //                break;
        //            case "10":
        //                #region Premios *
        //                Premios premios = new Premios();
        //                c_EntidadesFederativas enumEntidadFederativa;
        //                if (Enum.TryParse(ddlEntidadFederativaPremios.SelectedValue, out enumEntidadFederativa))
        //                    premios.EntidadFederativa= enumEntidadFederativa;
        //                else
        //                    MessageBox.Show("Error, el valor en el campo \"Entidad federativa\" no esta dentro de la lista");
        //                premios.MontTotPago = Convert.ToDecimal(txbMontTotPagoPremios.Text);
        //                premios.MontTotPagoGrav = Convert.ToDecimal(txbMontTotPagoGravPremios.Text);
        //                premios.MontTotPagoExent = Convert.ToDecimal(txbMontTotPagoExentPremios.Text);
        //                cfdirip.SchemaLocation += " http://www.sat.gob.mx/esquemas/retencionpago/1/premios http://www.sat.gob.mx/esquemas/retencionpago/1/premios/premios.xsd";
        //                cfdirip.Complemento = new RetencionesComplemento();
        //                XmlSerializerNamespaces namespacesPremios = new XmlSerializerNamespaces();
        //                namespacesPremios.Add("premios", "http://www.sat.gob.mx/esquemas/retencionpago/1/premios");
        //                XmlSerializer serializerPremios = new XmlSerializer(typeof(Premios));
        //                StringWriter writerPremios = new StringWriter();
        //                serializerPremios.Serialize(writerPremios, premios, namespacesPremios);
        //                XmlDocument xmlPremios = new XmlDocument();
        //                xmlPremios.LoadXml(writerPremios.ToString());
        //                cfdirip.Complemento.Any = new XmlElement[1];
        //                cfdirip.Complemento.Any[0] = xmlPremios.DocumentElement;
        //                #endregion
        //                break;
        //            case "11":
        //                #region Sector financiero *
        //                SectorFinanciero sectorFinanciero = new SectorFinanciero();
        //                sectorFinanciero.IdFideicom = txbIdFideicom.Text;
        //                if (!string.IsNullOrEmpty(txbNomFideicom.Text))
        //                    sectorFinanciero.NomFideicom = txbNomFideicom.Text;
        //                sectorFinanciero.DescripFideicom = txbDescripFideicom.Text;
        //                cfdirip.SchemaLocation += " http://www.sat.gob.mx/esquemas/retencionpago/1/sectorfinanciero http://www.sat.gob.mx/esquemas/retencionpago/1/sectorfinanciero/sectorfinanciero.xsd";
        //                cfdirip.Complemento = new RetencionesComplemento();
        //                XmlSerializerNamespaces namespacesSectorFinanciero = new XmlSerializerNamespaces();
        //                namespacesSectorFinanciero.Add("sectorfinanciero", "http://www.sat.gob.mx/esquemas/retencionpago/1/sectorfinanciero");
        //                XmlSerializer serializerSectorFinanciero = new XmlSerializer(typeof(SectorFinanciero));
        //                StringWriter writerSectorFinanciero = new StringWriter();
        //                serializerSectorFinanciero.Serialize(writerSectorFinanciero, sectorFinanciero, namespacesSectorFinanciero);
        //                XmlDocument xmlSectorFinanciero = new XmlDocument();
        //                xmlSectorFinanciero.LoadXml(writerSectorFinanciero.ToString());
        //                cfdirip.Complemento.Any = new XmlElement[1];
        //                cfdirip.Complemento.Any[0] = xmlSectorFinanciero.DocumentElement;
        //                #endregion
        //                break;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    //TimbradoDS timbre = new TimbradoDS();
        //    DSTimbrado timbre = new DSTimbrado();
        //    try
        //    {
        //        cfdirip.Sign(File.ReadAllBytes(rutaCertificado + ddlEmpresa.SelectedValue + "/" + certificadoLlavePrivada), Encoding.UTF8.GetBytes(certificadoContrasenia));
        //        //timbre.RFC = cfdirip.Emisor.RFCEmisor;
        //        //timbre.Titulo = ddlComplemento.Items.FindByValue(ddlComplemento.SelectedValue).ToString();

        //        //if (cfdirip.Emisor.RFCEmisor == "AAA010101AAA")
        //        //    timbre.TipoTimbrado = Servicio.wsDaySoft.TypeCFD.Test;
        //        //else
        //        //{
        //        //    timbre.TipoTimbrado = Servicio.wsDaySoft.TypeCFD.Production;

        //        if (cfdirip.Emisor.RFCEmisor != "AAA010101AAA")
        //            timbre.TipoTimbrado = Daysoft.DICI.Facturador.DFacture.wsDaySoft.TypeCFD.Production;
        //        else
        //            timbre.TipoTimbrado = Daysoft.DICI.Facturador.DFacture.wsDaySoft.TypeCFD.Test;

        //        //CFDIRIPConexionBD objConexion = new CFDIRIPConexionBD();
        //        //    MySqlConnection conexionString = objConexion.ObtenerConexion();
        //        //    MySqlCommand sql = new MySqlCommand("SELECT usuario, contrasenia, token FROM servisim ", conexionString);
        //        //    MySqlDataReader leer;
        //        //    leer = sql.ExecuteReader();
        //        //    if (!leer.Read())
        //        //    {
        //        //        timbre.Usuario = leer.GetString(0);
        //        //        timbre.Password = string.IsNullOrWhiteSpace(leer.GetString(1)) ? "" : seg.Desencriptar(leer.GetString(1));
        //        //        timbre.Token = string.IsNullOrWhiteSpace(leer.GetString(2)) ? "" : seg.Desencriptar(leer.GetString(2));
        //        //    }
        //        //    leer.Close();
        //        //}
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    timbre.TimbrarCFDI(cfdirip.ToXml());
        //    string rutaXML = rutaComprobantesRIP + Session["identificador"] + "/"  +timbre.ObtenerFechaTimbrado().ToString("yyyy-MM") + "/";
        //    if (!Directory.Exists(rutaXML))
        //        Directory.CreateDirectory(rutaXML);
        //    rutaXML = rutaXML + ddlComplemento.Items.FindByValue(ddlComplemento.SelectedValue).ToString() + timbre.ObtenerUUID() + ".xml";
        //    timbre.GuardarCFDTimbrado(rutaXML);
        //    GuardarenBD(cfdirip, timbre);
        //    LimpiarDatos();
        //}

        //protected void GuardarenBD(DS.RetencionPagov1.Retenciones cfdirip, DSTimbrado timbrecfdirip)
        //{
        //    try
        //    {
        //        CFDIRIPConexionBD objConexion = new CFDIRIPConexionBD();
        //        MySqlConnection conexionString = objConexion.ObtenerConexion();
        //        MySqlCommand sql= new MySqlCommand("",conexionString);
        //        MySqlDataReader leer;
        //        string complementoId = string.Empty;
        //        string nomTablaComplementoId = string.Empty;
        //        //Se llena la tabla que corresponda al complemento
        //        switch (ddlComplemento.SelectedValue)
        //        {
        //            case "1":
        //                #region Guarda en la tabla Arrendamiento en fideicomiso
        //                string insertMontTotRet = string.IsNullOrEmpty(txbMontTotRet.Text) ? null : ", "+txbMontTotRet.Text;
        //                string tituloMontTotRet = string.IsNullOrEmpty(insertMontTotRet) ? null : ", monttotret";
        //                string insertMontResFiscDistFibras = string.IsNullOrEmpty(txbMontResFiscDistFibras.Text) ? null : ", "+txbMontResFiscDistFibras.Text;
        //                string tituloMontResFiscDistFibras = string.IsNullOrEmpty(insertMontResFiscDistFibras) ? null : ", montresfiscdistfibras";
        //                string insertMontOtrosConceptDistr = string.IsNullOrEmpty(txbMontOtrosConceptDistr.Text) ? null : ", "+txbMontOtrosConceptDistr.Text;
        //                string tituloMontOtrosConceptDistr = string.IsNullOrEmpty(insertMontOtrosConceptDistr) ? null : ", montotrosconceptdistr";
        //                string insertDescrMontOtrosConceptDistr = string.IsNullOrEmpty(txbDescrMontOtrosConceptDistr.Text) ? null : ", '"+txbDescrMontOtrosConceptDistr.Text+"'";
        //                string tituloDescrMontOtrosConceptDistr = string.IsNullOrEmpty(insertDescrMontOtrosConceptDistr) ? null : ", descrmontotrosconceptdistr";
        //                sql.CommandText = "INSERT INTO arrendamientoenfideicomiso (pagprovefecporfiduc, rendimfideicom, deducccorresp" + tituloMontTotRet + "" + tituloMontResFiscDistFibras + "" + tituloMontOtrosConceptDistr + "" + tituloDescrMontOtrosConceptDistr + ") VALUES (" + txbPagProvEfecPorFiduc.Text + ", " + txbRendimFideicom.Text + ", " + txbDeduccCorresp.Text + "" + insertMontTotRet + "" + insertMontResFiscDistFibras + "" + insertMontOtrosConceptDistr + "" + insertDescrMontOtrosConceptDistr + ")";
        //                nomTablaComplementoId = "arrendamientoenfideicomiso";
        //                #endregion
        //                break;
        //            case "2":
        //                #region Guarda en la tabla Dividendos
        //                string insertMontRetExtDivExt = string.IsNullOrEmpty(txbMontRetExtDivExt.Text) ? null : ", " + txbMontRetExtDivExt.Text;
        //                string tituloMontRetExtDivExt = string.IsNullOrEmpty(insertMontRetExtDivExt) ? null : ", montretextdivext";
        //                string insertMontISRAcredNal = string.IsNullOrEmpty(txbMontISRAcredNal.Text) ? null : ", " + txbMontISRAcredNal.Text;
        //                string tituloMontISRAcredNal = string.IsNullOrEmpty(insertMontISRAcredNal) ? null : ", montisracrednal";
        //                string insertMontDivAcumNal = string.IsNullOrEmpty(txbMontDivAcumNal.Text) ? null : ", " + txbMontDivAcumNal.Text;
        //                string tituloMontDivAcumNal = string.IsNullOrEmpty(insertMontDivAcumNal) ? null : ", montdivacumnal";
        //                string insertMontDivAcumExt = string.IsNullOrEmpty(txbMontDivAcumExt.Text) ? null : ", " + txbMontDivAcumExt.Text;
        //                string tituloMontDivAcumExt = string.IsNullOrEmpty(insertMontDivAcumExt) ? null : ", montdivacumext";
        //                string insertProporcionRem = string.IsNullOrEmpty(txbProporcionRem.Text) ? null : ", " + txbProporcionRem.Text;
        //                string tituloProporcionRem = string.IsNullOrEmpty(insertProporcionRem) ? null : ", proporcionrem";
        //                sql.CommandText = "INSERT INTO dividendos (cvetipdivoutil, montisracredretmexico, montisracredretextranjero" + tituloMontRetExtDivExt + ", tiposocdistrdiv" + tituloMontISRAcredNal + "" + tituloMontDivAcumNal + "" + tituloMontDivAcumExt + "" + tituloProporcionRem + ") VALUES ('" + ddlCveTipDivOUtil.SelectedValue + "', " + txbMontISRAcredRetMexico.Text + ", " + txbMontISRAcredRetExtranjero.Text + "" + insertMontRetExtDivExt + ", '" + ddlTipoSocDistrDiv.SelectedValue + "'" + insertMontISRAcredNal + "" + insertMontDivAcumNal + "" + insertMontDivAcumExt + "" + insertProporcionRem + ")";
        //                nomTablaComplementoId = "dividendos";
        //                #endregion
        //                break;
        //            case "3":
        //                #region Guarda en la tabla Enajenacion de acciones
        //                sql.CommandText = "INSERT INTO enajenaciondeacciones (contratointermediacion, Ganancia, Perdida) VALUES ('" + txbContratoIntermediacion.Text + "', " + txbGanancia.Text + ", " + txbPerdida.Text + ")";
        //                nomTablaComplementoId = "enajenaciondeacciones";
        //                #endregion
        //                break;
        //            case "4":
        //                #region Guarda en la tabla Fideicomiso no empresarial
        //                sql.CommandText = "INSERT INTO fideicnoempresarial (monttotentradasperiodo, partpropacumdelfideicom, propdelmonttotingresosoentradas, concepto, monttotegresperiodo, partpropdelfideicom, propdelmonttotdeduccosalida, concepto_s, montretrelpagfideic, descretrelpagfideic) VALUES (" + txbMontTotEntradasPeriodoIngresosOEntradas.Text + ", " + txbPartPropAcumDelFideicomIngresosOEntradas.Text + ", " + txbPropDelMontTotIngresosOEntradas.Text + ", '" + txbConceptoIngresosOEntradas.Text + "', " + txbMontTotEgresPeriodoDeduccOSalidas.Text + ", " + txbPartPropDelFideicomDeduccOSalidas.Text + ", " + txbPropDelMontTotDeduccOSalidas.Text + ", '" + txbConceptoDeduccOSalidas.Text + "', " + txbMontRetRelPagFideicRetEfectFideicomiso.Text + ", '" + txbDescRetRelPagFideicRetEfectFideicomiso.Text + "')";
        //                nomTablaComplementoId = "fideicnoempresarial";
        //                #endregion
        //                break;
        //            case "5":
        //                #region Guarda en la tabla Intereses
        //                sql.CommandText = "INSERT INTO intereses (sistemafinanciero, retiroaoresretint, operfinancderivad, montintnominal, montintreal, perdida) VALUES ('" + Session["respSistFinanciero"] + "', '" + Session["respRetiroAORESRetInt"] + "', '" + Session["respOperFinancDerivad"] + "', " + txbMontIntNominal.Text + ", " + txbMontIntReal.Text + ", " + txbPerdidaIntereses.Text + ")";
        //                nomTablaComplementoId = "intereses";
        //                #endregion
        //                break;
        //            case "6":
        //                #region Guarda en la tabla Intereses hipotecarios
        //                string insertPropDeducDelCredit = string.IsNullOrEmpty(txbPropDeducDelCredit.Text) ? null : ", "+txbPropDeducDelCredit.Text;
        //                string tituloPropDeducDelCredit = string.IsNullOrEmpty(insertPropDeducDelCredit) ? null : ", propdeducdelcredit";
        //                string insertMontTotIntNominalesDev = string.IsNullOrEmpty(txbMontTotIntNominalesDev.Text) ? null : ", "+txbMontTotIntNominalesDev.Text;
        //                string tituloMontTotIntNominalesDev = string.IsNullOrEmpty(insertMontTotIntNominalesDev) ? null : ", monttotintnominalesdev";
        //                string insertMontTotIntNominalesDevYPag = string.IsNullOrEmpty(txbMontTotIntNominalesDevYPag.Text) ? null : ", "+txbMontTotIntNominalesDevYPag.Text;
        //                string tituloMontTotIntNominalesDevYPag = string.IsNullOrEmpty(insertMontTotIntNominalesDevYPag) ? null : ", monttotintnominalesdevypag";
        //                string insertMontTotIntRealPagDeduc = string.IsNullOrEmpty(txbMontTotIntRealPagDeduc.Text) ? null : ", " + txbMontTotIntRealPagDeduc.Text;
        //                string tituloMontTotIntRealPagDeduc = string.IsNullOrEmpty(insertMontTotIntRealPagDeduc) ? null : ", monttotintrealpagdeduc";
        //                string insertNumContrato = string.IsNullOrEmpty(txbNumContrato.Text) ? null : ", '"+txbNumContrato.Text+"'";
        //                string tituloNumContrato = string.IsNullOrEmpty(insertNumContrato) ? null : ", numcontrato";
        //                sql.CommandText = "INSERT INTO intereseshipotecarios (creditodeInstfinanc, saldoinsoluto" + tituloPropDeducDelCredit + "" + tituloMontTotIntNominalesDev + "" + tituloMontTotIntNominalesDevYPag + "" + tituloMontTotIntRealPagDeduc + "" + tituloNumContrato + ") VALUES ('" + Session["respCreditoDeInstFinanc"] + "', " + txbSaldoInsoluto.Text + "" + insertPropDeducDelCredit + "" + insertMontTotIntNominalesDev + "" + insertMontTotIntNominalesDevYPag + "" + insertMontTotIntRealPagDeduc + "" + insertNumContrato + ")";
        //                nomTablaComplementoId = "intereseshipotecarios";
        //                #endregion
        //                break;
        //            case "7":
        //                #region Guardar en la tabla Operaciones con derivados
        //                sql.CommandText = "INSERT INTO operacionesconderivados (montganacum, montperdded) VALUES ("+txbMontGanAcum.Text+", " + txbMontPerdDed .Text+ ")";
        //                nomTablaComplementoId = "operacionesconderivados";
        //                #endregion
        //                break;
        //            case "8":
        //                #region Guardar en la tabla Pagos a extranjeros
        //                string insertPaisDeResidParaEfecFiscNoBeneficiario = string.IsNullOrEmpty(ddlPaisDeResidParaEfecFiscNoBeneficiario.SelectedValue) ? null : ", '"+ddlPaisDeResidParaEfecFiscNoBeneficiario.SelectedValue+"'";
        //                string tituloPaisDeResidParaEfecFiscNoBeneficiario = string.IsNullOrEmpty(insertPaisDeResidParaEfecFiscNoBeneficiario) ? null : ", paisderesidparaefecfisc";
        //                string insertRFCBeneficiario = string.IsNullOrEmpty(txbRFCBeneficiario.Text) ? null : ", '"+txbRFCBeneficiario.Text+"'";
        //                string tituloRFCBeneficiario = string.IsNullOrEmpty(insertRFCBeneficiario) ? null : ", rfc";
        //                string insertCURPBeneficiario = string.IsNullOrEmpty(txbCURPBeneficiario.Text) ? null : ", '"+txbCURPBeneficiario.Text+"'";
        //                string tituloCURPBeneficiario = string.IsNullOrEmpty(insertCURPBeneficiario) ? null : ", curp";
        //                string insertNomDenRazSocBeneficiario = string.IsNullOrEmpty(txbNomDenRazSocBeneficiario.Text) ? null : ", '"+txbNomDenRazSocBeneficiario.Text+"'";
        //                string tituloNomDenRazSocBeneficiario = string.IsNullOrEmpty(insertNomDenRazSocBeneficiario) ? null : ", nomdenrazsocb";
        //                sql.CommandText = "INSERT INTO pagosaextranjeros (esbenefefectdelcobro" + tituloPaisDeResidParaEfecFiscNoBeneficiario + "" + tituloRFCBeneficiario + "" + tituloCURPBeneficiario + "" + tituloNomDenRazSocBeneficiario + ", conceptopago, descripcionconcepto) VALUES ('" + Session["respEsBenefEfectDelCobro"] + "'" + insertPaisDeResidParaEfecFiscNoBeneficiario + "" + insertRFCBeneficiario + "" + insertCURPBeneficiario + "" + insertNomDenRazSocBeneficiario + ", '" + ddlConceptoPagoPagosaExtranjeros.SelectedValue + "', '" + tbxDescripcionConceptoPagoPagosaExtranjeros.Text + "')";
        //                nomTablaComplementoId = "pagosaextranjeros";
        //                #endregion
        //                break;
        //            case "9":
        //                #region Guardar en la tabla Planes de retiro
        //                string insertMontTotAportAnioInmAnterior = string.IsNullOrEmpty(txbMontTotAportAnioInmAnterior.Text) ? null : ", "+txbMontTotAportAnioInmAnterior.Text;
        //                string tituloMontTotAportAnioInmAnterior = string.IsNullOrEmpty(insertMontTotAportAnioInmAnterior) ? null : ", monttotaportanioinmanterior";
        //                string insertMontTotRetiradoAnioInmAntPer = string.IsNullOrEmpty(txbMontTotRetiradoAnioInmAntPer.Text) ? null : ", " + txbMontTotRetiradoAnioInmAntPer.Text;
        //                string tituloMontTotRetiradoAnioInmAntPer = string.IsNullOrEmpty(insertMontTotRetiradoAnioInmAntPer) ? null : ", monttotretiradoanioinmantper";
        //                string insertMontTotExentRetiradoAnioInmAnt = string.IsNullOrEmpty(txbMontTotExentRetiradoAnioInmAnt.Text) ? null : ", " + txbMontTotExentRetiradoAnioInmAnt.Text;
        //                string tituloMontTotExentRetiradoAnioInmAnt = string.IsNullOrEmpty(insertMontTotExentRetiradoAnioInmAnt) ? null : ", monttotexentretiradoanioinmant";
        //                string insertMontTotExedenteAnioInmAnt = string.IsNullOrEmpty(txbMontTotExedenteAnioInmAnt.Text) ? null : ", " + txbMontTotExedenteAnioInmAnt.Text;
        //                string tituloMontTotExedenteAnioInmAnt = string.IsNullOrEmpty(insertMontTotExedenteAnioInmAnt) ? null : ", monttotexedenteanioinmant";
        //                string insertMontTotRetiradoAnioInmAnt = string.IsNullOrEmpty(txbMontTotRetiradoAnioInmAnt.Text) ? null : ", " + txbMontTotRetiradoAnioInmAnt.Text;
        //                string tituloMontTotRetiradoAnioInmAnt = string.IsNullOrEmpty(insertMontTotRetiradoAnioInmAnt) ? null : ", monttotretiradoanioInmant";
        //                sql.CommandText = "INSERT INTO planesderetiro (sistemafinanc" + tituloMontTotAportAnioInmAnterior + ", montintrealesdevengnniooinmant, huboretirosanioInmantper" + tituloMontTotRetiradoAnioInmAntPer + "" + tituloMontTotExentRetiradoAnioInmAnt + "" + tituloMontTotExedenteAnioInmAnt + ", huboretirosanioinmant" + tituloMontTotRetiradoAnioInmAnt + ") VALUES ('" + Session["resSistemaFinanc"] + "'" + insertMontTotAportAnioInmAnterior + ", " + txbMontIntRealesDevengAniooInmAnt.Text + ", '" + Session["resHuboRetirosAnioInmAntPer"] + "'" + insertMontTotRetiradoAnioInmAntPer + "" + insertMontTotExentRetiradoAnioInmAnt + "" + insertMontTotExedenteAnioInmAnt + ", '" + Session["resHuboRetirosAnioInmAnt"] + "'" + insertMontTotRetiradoAnioInmAnt + ")";
        //                nomTablaComplementoId = "planesderetiro";
        //                #endregion
        //                break;
        //            case "10":
        //                #region Guardar en la tabla Premios
        //                sql.CommandText = "INSERT INTO premios (entidadfederativa, monttotpago, monttotpagograv, monttotpagoexent) VALUES ('" + ddlEntidadFederativaPremios.SelectedValue + "', " + txbMontTotPagoPremios.Text + ", " + txbMontTotPagoGravPremios.Text + ", " + txbMontTotPagoExentPremios.Text + ")";
        //                nomTablaComplementoId = "premios";
        //                #endregion
        //                break;
        //            case "11":
        //                #region Guardar en la tabla Sector financiero
        //                string insertNomFideicom = string.IsNullOrEmpty(txbNomFideicom.Text) ? null : ", '"+txbNomFideicom.Text+"'";
        //                string tituloNomFideicom = string.IsNullOrEmpty(insertNomFideicom) ? null : ", nomfideicom";
        //                sql.CommandText = "INSERT INTO sectorfinanciero (idfideicom" + tituloNomFideicom + ", descripfideicom) VALUES ('" + txbIdFideicom.Text + "'" + insertNomFideicom + ", '" + txbDescripFideicom.Text + "')";
        //                nomTablaComplementoId = "sectorfinanciero";
        //                #endregion
        //                break;
        //        }
        //        sql.ExecuteNonQuery();
        //        sql.CommandText = "SELECT MAX(id) FROM " + nomTablaComplementoId;
        //        leer = sql.ExecuteReader();
        //        leer.Read();
        //        complementoId = leer.GetString(0);
        //        leer.Close();

        //        //Se llena la tabla CFDIRIP de la BD
        //        string insertfoliointerno = string.IsNullOrEmpty(cfdirip.FolioInt) ? null : ", '"+cfdirip.FolioInt+"'";
        //        string titulofoliointerno = string.IsNullOrEmpty(insertfoliointerno) ? null : ", foliointerno";
        //        string insertDescRetenc = string.IsNullOrEmpty(cfdirip.DescRetenc) ? null : ", '" + cfdirip.DescRetenc + "'";
        //        string tituloDescRetenc = string.IsNullOrEmpty(insertDescRetenc) ? null : ", descripcion";
        //        string fechaBD =  cfdirip.FechaExp.ToString("yyyy-MM-dd HH:mm:ss");
        //        sql.CommandText = "INSERT INTO cfdirip (emisor, receptor" + titulofoliointerno + ", fechaexp, clave" + tituloDescRetenc + ", mesinicial, mesfinal, ejerciciofiscal, montototoperacion, montototgrav, montototexent, montototret, " + nomTablaComplementoId + ") VALUES ('" + ddlEmpresa.SelectedValue + "', '" + ddlIdentificadorCliente.SelectedValue + "'" + insertfoliointerno + ", '" + fechaBD + "', '" + cfdirip.CveRetenc.ToString() + "'" + insertDescRetenc + ", " + cfdirip.Periodo.MesIni.ToString() + "," + cfdirip.Periodo.MesFin.ToString() + "," + cfdirip.Periodo.Ejerc.ToString() + ", " + cfdirip.Totales.montoTotOperacion + ", " + cfdirip.Totales.montoTotGrav + ", " + cfdirip.Totales.montoTotExent + ", " + cfdirip.Totales.montoTotRet + ", " + complementoId + ")";
        //        sql.ExecuteNonQuery();
        //        sql.CommandText = "SELECT MAX( id ) FROM cfdirip";
        //        leer = sql.ExecuteReader();
        //        leer.Read();
        //        string idTablacfdirip = leer.GetString(0);
        //        leer.Close();
        //        if (Session["objImpuestosRetenidos"] != null)
        //        {
        //            List<RetencionesTotalesImpRetenidos> lstImpuestosRetenidos = new List<RetencionesTotalesImpRetenidos>();
        //            lstImpuestosRetenidos = Session["objImpuestosRetenidos"] as List<RetencionesTotalesImpRetenidos>;
        //            string insertBaseRet, insertImpuesto, tituloBaseRet, tituloImpuesto;
        //            //Se llena la tabla impuestosretenidos de la BD
        //            foreach (RetencionesTotalesImpRetenidos lstImpuestosRetenido in lstImpuestosRetenidos) {
        //                insertBaseRet= lstImpuestosRetenido.BaseRetSpecified ? null : lstImpuestosRetenido.BaseRet.ToString()+", ";
        //                tituloBaseRet = string.IsNullOrEmpty(insertBaseRet) ? null : "base, ";
        //                insertImpuesto= lstImpuestosRetenido.ImpuestoSpecified ? null : "'"+lstImpuestosRetenido.Impuesto.ToString()+"', ";
        //                tituloImpuesto = string.IsNullOrEmpty(insertImpuesto) ? null : "impuesto, ";
        //                sql.CommandText = "INSERT INTO impuestosretenidos (" + tituloBaseRet + "" + tituloImpuesto + "monto, tipopago, cfdrip) VALUES (" + insertBaseRet + "" + insertImpuesto + "" + lstImpuestosRetenido.montoRet + ", '" + lstImpuestosRetenido.TipoPagoRet.ToString() + "', " + idTablacfdirip + ")";
        //                sql.ExecuteNonQuery();
        //            }
        //        }
        //        //se llena la tabla facturacfdirip
        //        sql.CommandText = "INSERT INTO facturacfdirip (uuid, fecha_timbrado, cfdirip) VALUES ('" + timbrecfdirip.ObtenerUUID() + "', " + timbrecfdirip.ObtenerFechaTimbrado() + ", " + idTablacfdirip + ")";
        //        sql.ExecuteNonQuery();
        //        conexionString.Close();
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show(e.Message);
        //    }
        //}

        protected void LimpiarDatos() {
            ddlEmpresa.SelectedValue = "";
            txbRFCEmisor.Text = "";
            txbRazonSocial.Text = "";
            txbCURPEmisor.Text = "";
            txbFolioInterno.Text = "";
            ddlClaveRetencion.SelectedValue = "";
            txbDescripcionRetencion.Text = "";
            ddlNacionalidad.SelectedValue = "";
            ddlIdentificadorCliente.SelectedValue = "";
            txbRazonSocialCliente.Text = "";
            txbRFCCliente.Text = "";
            txbCURPCliente.Text = "";
            txbNumRegIdentFiscalCliente.Text = "";
            ddlMesInicial.SelectedValue = "";
            ddlMesFinal.SelectedValue = "";
            ddlEjercicioFiscal.SelectedValue = "";
            txbMontoTotalDeOperacion.Text = "";
            txbMontototalExento.Text = "";
            txbMontoTotalGravado.Text = "";
            txbMontoTotalRetenciones.Text = "";
            ddlComplemento.SelectedValue = "";
            LimpiarImportesRetenidos();
            grvImpRetenidos.DataSource = null;
            grvImpRetenidos.DataBind();
            Session["objImpuestosRetenidos"] = null;
            LimpiarComplementos();
        }

        protected void LimpiarImportesRetenidos() {
            txbBaseImpuesto.Text = "";
            ddlImpuesto.SelectedValue = "";
            txbMontoRetencion.Text = "";
            ddlTipoPagoRetencion.SelectedValue = "";
        }

        protected void LimpiarComplementos() { 
        #region Limpiar Arrendamiento en Fideicomiso
            txbPagProvEfecPorFiduc.Text = "";
            txbRendimFideicom.Text = "";
            txbDeduccCorresp.Text = "";
            txbMontTotRet.Text = "";
            txbMontResFiscDistFibras.Text = "";
            txbMontOtrosConceptDistr.Text = "";
            txbDescrMontOtrosConceptDistr.Text = "";
        #endregion
        #region Limpiar Dividendos
            ddlCveTipDivOUtil.SelectedValue = "";
            txbMontISRAcredRetMexico.Text = "";
            txbMontISRAcredRetExtranjero.Text = "";
            txbMontRetExtDivExt.Text = "";
            ddlTipoSocDistrDiv.SelectedValue = "";
            txbMontISRAcredNal.Text = "";
            txbMontDivAcumNal.Text = "";
            txbMontDivAcumExt.Text = "";
            txbProporcionRem.Text = "";
        #endregion
        #region Limpiar Enajenacion de Acciones
            txbContratoIntermediacion.Text = "";
            txbGanancia.Text = "";
            txbPerdida.Text = "";
        #endregion
        #region Limpiar Fideicomiso no Empresarial
            txbMontTotEntradasPeriodoIngresosOEntradas.Text = "";
            txbPartPropAcumDelFideicomIngresosOEntradas.Text = "";
            txbPropDelMontTotIngresosOEntradas.Text = "";
            txbConceptoIngresosOEntradas.Text = "";
            txbMontTotEgresPeriodoDeduccOSalidas.Text = "";
            txbPartPropDelFideicomDeduccOSalidas.Text = "";
            txbPropDelMontTotDeduccOSalidas.Text = "";
            txbConceptoDeduccOSalidas.Text = "";
            txbMontRetRelPagFideicRetEfectFideicomiso.Text = "";
            txbDescRetRelPagFideicRetEfectFideicomiso.Text = "";
        #endregion
        #region Limpiar Intereses
            chkSistFinanciero.Checked = false;
            chklRetiroAORESRetInt.Checked = false;
            chkOperFinancDerivad.Checked = false;
            txbMontIntNominal.Text = "";
            txbMontIntReal.Text = "";
            txbPerdidaIntereses.Text = "";
        #endregion
        #region Limpiar Intereses Hipotecarios
            chkCreditoDeInstFinanc.Checked = false;
            txbSaldoInsoluto.Text = "";
            txbPropDeducDelCredit.Text = "";
            txbMontTotIntNominalesDev.Text = "";
            txbMontTotIntNominalesDevYPag.Text = "";
            txbMontTotIntRealPagDeduc.Text = "";
            txbNumContrato.Text = "";
        #endregion
        #region Limpiar Operaciones con Derivados
            txbMontGanAcum.Text = "";
            txbMontPerdDed.Text = "";
        #endregion
        #region Limpiar Pagos a Extranjeros
            chkEsBenefEfectDelCobro.Checked = false;
            ddlPaisDeResidParaEfecFiscNoBeneficiario.SelectedValue = "";
            txbRFCBeneficiario.Text = "";
            txbCURPBeneficiario.Text = "";
            txbNomDenRazSocBeneficiario.Text = "";
            ddlConceptoPagoPagosaExtranjeros.SelectedValue = "";
            tbxDescripcionConceptoPagoPagosaExtranjeros.Text = "";
        #endregion
        #region Limpiar Planes de Retiro
            chkSistemaFinanc.Checked = false;
            txbMontTotAportAnioInmAnterior.Text = "";
            txbMontIntRealesDevengAniooInmAnt.Text = "";
            chkHuboRetirosAnioInmAntPer.Checked = false;
            txbMontTotRetiradoAnioInmAntPer.Text = "";
            txbMontTotExentRetiradoAnioInmAnt.Text = "";
            txbMontTotExedenteAnioInmAnt.Text = "";
            chkHuboRetirosAnioInmAnt.Checked = false;
            txbMontTotRetiradoAnioInmAnt.Text = "";
        #endregion
        #region Limpiar Premios
            ddlEntidadFederativaPremios.SelectedValue = "";
            txbMontTotPagoPremios.Text = "";
            txbMontTotPagoGravPremios.Text = "";
            txbMontTotPagoExentPremios.Text = "";
        #endregion
        #region Limpiar Sector Financiero() {
            txbIdFideicom.Text = "";
            txbNomFideicom.Text = "";
            txbDescripFideicom.Text = "";
        #endregion
        }
        
        //Selecciona el emisor
        protected void ddlEmpresa_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlEmpresa.SelectedValue))
            {
                try
                {
                    CFDIRIPConexionBD objConexion = new CFDIRIPConexionBD();
                    MySqlConnection conexionString = objConexion.ObtenerConexion();
                    MySqlCommand sql = new MySqlCommand("SELECT rfc, razon_social, curp, identificador From emisor WHERE(idemisor='" + ddlEmpresa.SelectedValue + "')", conexionString);
                    MySqlDataReader leersql = sql.ExecuteReader();
                    leersql.Read();
                    txbRFCEmisor.Text = seg.Desencriptar(leersql.GetString(0));
                    txbRazonSocial.Text = seg.Desencriptar(leersql.GetString(1));
                    txbCURPEmisor.Text = string.IsNullOrEmpty(leersql.GetValue(2).ToString()) ? "" : leersql.GetString(2);
                    Session["identificador"] = leersql.GetString(3);
                    conexionString.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                txbRFCEmisor.Text = "";
                txbRazonSocial.Text = "";
                txbCURPEmisor.Text = "";
            }
        }

        //Selecciona la nacionalidad del cliente
        protected void ddlNacionalidad_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlNacionalidad.SelectedValue))
            {
                LlenarddlCliente();
                if (ddlNacionalidad.SelectedValue.Equals("Nacional"))
                {
                    divClienteExtranjero.Visible = false;
                    divClienteNacional.Visible = true;
                }
                else
                {
                    divClienteNacional.Visible = false;
                    divClienteExtranjero.Visible = true;
                }
            }
            else
            {
                divClienteNacional.Visible = false;
                divClienteExtranjero.Visible = false;
                ddlIdentificadorCliente.Items.Clear();
                txbRazonSocialCliente.Text = "";
                txbRFCCliente.Text = "";
                txbCURPCliente.Text = "";
                txbNumRegIdentFiscalCliente.Text = "";
            }
        }

        //Selecciona el cliente
        protected void ddlIdentificadorCliente_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlIdentificadorCliente.SelectedValue))
            {
                try
                {
                    CFDIRIPConexionBD objConexion = new CFDIRIPConexionBD();
                    MySqlConnection conexionString = objConexion.ObtenerConexion();
                    MySqlCommand sql;
                    sql = new MySqlCommand("SELECT razon_social, rfc, curp, numregidtrib From receptor WHERE(idreceptor='" + ddlIdentificadorCliente.SelectedValue + "')", conexionString);
                    MySqlDataReader leersql = sql.ExecuteReader();
                    leersql.Read();

                    txbRazonSocialCliente.Text = seg.Desencriptar(leersql.GetString(0));
                    //Nacional
                    txbRFCCliente.Text = seg.Desencriptar(leersql.GetString(1));
                    if (!string.IsNullOrEmpty((leersql.GetValue(2).ToString())))
                        txbCURPCliente.Text = leersql.GetString(2);
                    else
                        txbCURPCliente.Text = "";
                    //Extranjero
                    if (!string.IsNullOrEmpty(leersql.GetValue(3).ToString()))
                        txbNumRegIdentFiscalCliente.Text = leersql.GetString(3);
                    else
                        txbNumRegIdentFiscalCliente.Text = "";
                    conexionString.Close();
                }
                catch (Exception ed)
                {
                    MessageBox.Show(ed.Message);
                }

            }
            else
            {
                txbRazonSocialCliente.Text = "";
                txbRFCCliente.Text = "";
                txbCURPCliente.Text = "";
                txbNumRegIdentFiscalCliente.Text = "";
            }
        }

        protected void chkEsBenefEfectDelCobro_CheckedChanged(object sender, EventArgs e)
        {
            if (chkEsBenefEfectDelCobro.Checked)
            {
                divNoBeneficiario.Visible = false;
                divBeneficiario.Visible = true;
            }
            else
            {
                divNoBeneficiario.Visible = true;
                divBeneficiario.Visible = false;
            }
        }

        protected void ddlMesInicial_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ddlMesInicial.SelectedValue))
            {
                ddlMesFinal.Items.Clear();
                LlenarddlMesFinal(Convert.ToInt32(ddlMesInicial.SelectedValue));
            }
        }

        protected void ddlComplemento_SelectedIndexChanged(object sender, EventArgs e)
        {
            LimpiarComplementos();
            OcultarComplementos();
            switch (ddlComplemento.SelectedValue)
            {
                case "1":
                    //Activa el complemento arrendamiento en fideicomiso y sus validaciones
                    divArrendamientoenFideicomiso.Visible = true;
                    rfvPagProvEfecPorFiduc.Enabled = true;
                    rfvDeduccCorresp.Enabled = true;
                    rfvRendimFideicom.Enabled = true;
                    break;
                case "2":
                    //Activa el complemento dividendos y sus validaciones
                    divDividendos.Visible = true;
                    LLenarddlCveTipDivOUtil();
                    LlenarddlTipoSocDistrDiv();
                    rfvddlCveTipDivOUtil.Enabled = true;
                    rfvtxbMontISRAcredRetMexico.Enabled = true;
                    rfvtxbMontISRAcredRetExtranjero.Enabled = true;
                    rfvddlTipoSocDistrDiv.Enabled = true;
                    break;
                case "3":
                    //Activa el complemento enajenacion de acciones y sus validaciones
                    divEnajenaciondeAcciones.Visible = true;
                    rfvtxbContratoIntermediacion.Enabled = true;
                    rfvtxbGanancia.Enabled = true;
                    rfvtxbPerdida.Enabled = true;
                    break;
                case "4":
                    //Activa el complemento fideicomiso no empresarial y sus validaciones
                    divFideicomisoNoEmpresarial.Visible = true;
                    rfvtxbMontTotEntradasPeriodoIngresosOEntradas.Enabled = true;
                    rfvtxbPartPropAcumDelFideicomIngresosOEntradas.Enabled = true;
                    rfvtxbPropDelMontTotIngresosOEntradas.Enabled = true;
                    rfvtxbConceptoIngresosOEntradas.Enabled = true;
                    rfvtxbMontTotEgresPeriodoDeduccOSalidas.Enabled = true;
                    rfvtxbPartPropDelFideicomDeduccOSalidas.Enabled = true;
                    rfvtxbPropDelMontTotDeduccOSalidas.Enabled = true;
                    rfvtxbConceptoDeduccOSalidas.Enabled = true;
                    rfvtxbMontRetRelPagFideicRetEfectFideicomiso.Enabled = true;
                    rfvtxbDescRetRelPagFideicRetEfectFideicomiso.Enabled = true;
                    break;
                case "5":
                    //Activa el complemento intereses y sus validaciones
                    divIntereses.Visible = true;
                    divIntereses.Visible = true;
                    rfvtxbMontIntNominal.Enabled = true;
                    rfvtxbMontIntReal.Enabled = true;
                    rfvtxbPerdidaIntereses.Enabled = true;
                    break;
                case "6":
                    //Activa el complemento intereses hipotecarios y sus validaciones
                    divInteresesHipotecarios.Visible = true;
                    rfvtxbSaldoInsoluto.Enabled = true;
                    break;
                case "7":
                    //Activa el complemento operaciones con derivados y sus validaciones
                    divOperacionesconDerivados.Visible = true;
                    rfvtxbMontGanAcum.Visible = true;
                    rfvtxbMontPerdDed.Visible = true;
                    break;
                case "8":
                    //Activa el complemento pagos a extranjeros y sus validaciones
                    divPagosaExtranjeros.Visible = true;
                    divBeneficiario.Visible = true;
                    rfvddlPaisDeResidParaEfecFiscNoBeneficiario.Enabled = true;
                    rfvtxbRFCBeneficiario.Enabled = true;
                    rfvtxbCURPBeneficiario.Enabled = true;
                    rfvtxbNomDenRazSocBeneficiario.Enabled = true;
                    rfvddlConceptoPagoPagosaExtranjeros.Enabled = true;
                    rfvtbxDescripcionConceptoPagoPagosaExtranjeros.Enabled = true;
                    revtxbRFCBeneficiario.Enabled = true;
                    revtxbCURPBeneficiario.Enabled = true;
                    LlenarddlPaisDeResidParaEfecFisc();
                    LLenarddlConceptoPagoPagosaExtranjeros();
                    break;
                case "9":
                    //Activa el complemento planes de retiro y sus validaciones
                    divPlanesdeRetiro.Visible = true;
                    rfvtxbMontIntRealesDevengAniooInmAnt.Enabled = true;
                    break;
                case "10":
                    //Activa el complemento premios y sus validaciones
                    divPremios.Visible = true;
                    rfvddlEntidadFederativaPremios.Enabled = true;
                    rfvtxbMontTotPagoPremios.Enabled = true;
                    rfvtxbMontTotPagoGravPremios.Enabled = true;
                    rfvtxbMontTotPagoExentPremios.Enabled = true;
                    LlenarddlEntidadFederativaPremios();
                    break;
                case "11":
                    //Activa el complemento sector financiero y sus validaciones
                    divSectorFinanciero.Visible = true;
                    rfvtxbIdFideicom.Enabled = true;
                    rfvtxbDescripFideicom.Enabled = true;
                    break;
            }                  
        }

        //protected void btnImpuestosRetenidos_Click(object sender, EventArgs e)
        //{
        //    Boolean valido = true;
        //    RetencionesTotalesImpRetenidos objImpuestosRetenidos = new RetencionesTotalesImpRetenidos();
        //    RetencionesTotalesImpRetenidosImpuesto enumImpuestos;
        //    RetencionesTotalesImpRetenidosTipoPagoRet enumTipoPagoRetencion;
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(txbBaseImpuesto.Text))
        //        {
        //            if (ValidarFormatoDecimal(txbBaseImpuesto.Text))
        //            {
        //                objImpuestosRetenidos.BaseRet = Convert.ToDecimal(txbBaseImpuesto.Text);
        //                objImpuestosRetenidos.BaseRetSpecified = true;
        //                lblValidaciontxbBaseImpuesto.Visible = false;
        //            }
        //            else
        //            {
        //                lblValidaciontxbBaseImpuesto.Visible = true;
        //                valido = false;
        //            }
        //        }
        //        else
        //            objImpuestosRetenidos.BaseRetSpecified = false;
        //        if (!string.IsNullOrEmpty(ddlImpuesto.SelectedValue))
        //        {
        //            if (Enum.TryParse(ddlImpuesto.SelectedValue, out enumImpuestos))
        //            {
        //                objImpuestosRetenidos.Impuesto = enumImpuestos;
        //                objImpuestosRetenidos.ImpuestoSpecified = true;
        //            }
        //            else
        //            {
        //                MessageBox.Show("Error, el valor en el campo \"Impuesto\" no esta dentro de la lista");
        //                return;
        //            }
        //        }
        //        else
        //            objImpuestosRetenidos.ImpuestoSpecified = false;
        //        if (ValidarFormatoDecimal(txbMontoRetencion.Text))
        //            objImpuestosRetenidos.montoRet = Convert.ToDecimal(txbMontoRetencion.Text);
        //        else
        //        {
        //            lblValidaciontxbMontoRetencion.Visible = true;
        //            valido = false;
        //        }
        //        if (Enum.TryParse(ddlTipoPagoRetencion.SelectedValue, out enumTipoPagoRetencion))
        //            objImpuestosRetenidos.TipoPagoRet = enumTipoPagoRetencion;
        //        else
        //        {
        //            MessageBox.Show("Error, el valor en el campo \"Tipo de pago de la retención\" no esta dentro de la lista");
        //            return;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message);
        //    }
        //    if (valido)
        //    {
        //        List<RetencionesTotalesImpRetenidos> lstImpuestosRetenidos = new List<RetencionesTotalesImpRetenidos>();
        //        if (Session["objImpuestosRetenidos"] != null)
        //        {
        //            lstImpuestosRetenidos = Session["objImpuestosRetenidos"] as List<RetencionesTotalesImpRetenidos>;
        //            lstImpuestosRetenidos.Add(objImpuestosRetenidos);
        //            Session["objImpuestosRetenidos"] = lstImpuestosRetenidos;
        //        }
        //        else
        //        {
        //            lstImpuestosRetenidos = new List<RetencionesTotalesImpRetenidos>();
        //            lstImpuestosRetenidos.Add(objImpuestosRetenidos);
        //            Session["objImpuestosRetenidos"] = lstImpuestosRetenidos;
        //        }
        //        DataTable tblImpuestosRetenidos = new DataTable();
        //        tblImpuestosRetenidos.Columns.Add("BaseRet");
        //        tblImpuestosRetenidos.Columns.Add("impuetsto");
        //        tblImpuestosRetenidos.Columns.Add("montoRet");
        //        tblImpuestosRetenidos.Columns.Add("TipoPagoRet");
        //        foreach (RetencionesTotalesImpRetenidos impuestosRetenidos in lstImpuestosRetenidos)
        //        {
        //            DataRow dr = tblImpuestosRetenidos.NewRow();
        //            if (impuestosRetenidos.BaseRetSpecified)
        //                dr["BaseRet"] = impuestosRetenidos.BaseRet.ToString();
        //            if (impuestosRetenidos.ImpuestoSpecified)
        //            {
        //                string nombreImpuestosRetenidosImpuesto = ddlImpuesto.Items.FindByValue(impuestosRetenidos.Impuesto.ToString()).ToString();
        //                dr["impuetsto"] = nombreImpuestosRetenidosImpuesto;
        //            }
        //            dr["montoRet"] = impuestosRetenidos.montoRet.ToString();
        //            string nombreimpuestosRetenidosTipoPagoRet = ddlTipoPagoRetencion.Items.FindByValue(impuestosRetenidos.TipoPagoRet.ToString()).ToString();
        //            dr["TipoPagoRet"] = nombreimpuestosRetenidosTipoPagoRet;

        //            tblImpuestosRetenidos.Rows.Add(dr);
        //        }
        //        grvImpRetenidos.DataSource = tblImpuestosRetenidos;
        //        grvImpRetenidos.DataBind();
        //    }
        //    LimpiarImportesRetenidos();
        //}

        protected void btnGenerarCFDRIP_Click(object sender, EventArgs e)
        {
            #region Validacion de deciamles
            Boolean valido;
            switch (ddlComplemento.SelectedValue)
            {
                case "1":
                    valido=ValidarDecimalesenComplementoArrendamientoenFideicomiso();
                    break;
                case "2":
                    valido=ValidarDecimalesenComplementoDividendos();
                    break;
                case "3":
                    valido=ValidarDecimalesenComplementoEnajenaciondeAcciones();
                    break;
                case "4":
                    valido = ValidarDecimalesenComplementoFideicomisonoEmpresarial();
                    break;
                case "5":
                    valido = ValidarDecimalesenComplementoIntereses();
                    break;
                case "6":
                    valido = ValidarDecimalesenComplementoInteresesHipotecarios();
                    break;
                case "7":
                    valido = ValidarDecimalesenComplementoOperacionesconDerivados();
                    break;
                case "9":
                    valido = ValidarDecimalesenComplementoPlanesdeRetiro();
                    break;
                case "10":
                    valido = ValidarDecimalesenComplementoPremios();
                    break;
                default:
                    valido = true;
                    break;
            }
            #endregion
            if (valido)
            {
                //GenerarCFDRIP();
            }
        }
    }
}