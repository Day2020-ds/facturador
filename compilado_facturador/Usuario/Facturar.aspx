<%@ Page Title="Facturación" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Facturar.aspx.cs" Inherits="Facturador.GHO.Usuario.Facturar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link rel="stylesheet" href="../Content/bootstrap-datepicker.css" type="text/css" />
    <script src="../Scripts/bootstrap-datepicker.js" type="text/javascript"></script>
    <script type="text/javascript">
        $(document).on('keypress keyup blur', '.numeric', function (event) {
            $(this).val($(this).val().replace(/[^0-9\.]/g, ''));
            if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57)) {
                event.preventDefault();
            }
        });
        $(document).on('keypress keyup blur', '.upper', function (event) {
            $(this).val($(this).val().replace(/[^a-zA-Z0-9&]/g, function (s) { return '' }));
            $(this).val($(this).val().toUpperCase());
        });
        $(document).on('click', '#msg_exito_close', function () {
            $('#msg_exito').hide();
        });
        $(document).on('click', '#msg_error_close', function () {
            $('#msg_error').hide();
            $('#msg_error_pie').hide();
        });
        $(document).on('click', '#msg_error_close_pie', function () {
            $('#msg_error').hide();
            $('#msg_error_pie').hide();
        });
        function pageLoad() {

            var checkin = $('#DateFechaPago').datepicker({
                format: "dd-mm-yyyy",
                language: "es",
                todayHighlight: true,
                startDate: '01/01/2014'
            }).on('changeDate', function (ev) {
                var newDate = new Date(ev.date)
                checkin.hide();
            }).data('datepicker');
        };
    </script>
    <h2>Nuevo Comprobante (CFDI)</h2>

    <p class="text-danger">
        <asp:Literal runat="server" ID="ErrorMessage" />
    </p>

    <asp:UpdatePanel ID="upAlertas" runat="server">

        <ContentTemplate>
            <div class="alert alert-success" aria-hidden="true" id="msg_exito" hidden="hidden">
                <a href="#" id="msg_exito_close" class="close">&times;</a>
                <asp:Label ID="lblExito" Text="" runat="server" />
            </div>
            <div class="alert alert-danger" aria-hidden="true" id="msg_error" hidden="hidden">
                <a href="#" id="msg_error_close" class="close">&times;</a>
                <asp:Label ID="lblError" Text="" runat="server" />
            </div>
        </ContentTemplate>
        <Triggers>
        </Triggers>
    </asp:UpdatePanel>

    <div class="form-horizontal">
        <hr />
        <h5><span class='glyphicon glyphicon-user'></span> Cliente:</h5>
        <div class="row" id="cliente">
            <asp:UpdatePanel ID="upCliente" runat="server">
                <ContentTemplate>
                    
                    <div class="col-md-3">
                        <div class="form-group" id="DivRFC">
                            <asp:Label runat="server" AssociatedControlID="RFC" CssClass="col-md-4 control-label">* RFC</asp:Label>
                            <div class="col-md-6">
                                <asp:TextBox runat="server" ID="RFC" CssClass="form-control upper" MaxLength="13" OnTextChanged="RFC_TextChanged" AutoPostBack="true" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="RFC" Display="Dynamic" ValidationGroup="vgFacturar"
                                    CssClass="text-danger" ErrorMessage="El RFC es obligatorio." />
                                <asp:RegularExpressionValidator ID="revRFC" runat="server" ErrorMessage="El RFC no es valido" Display="Dynamic"
                                    CssClass="text-danger" ValidationGroup="vgFacturar" ControlToValidate="RFC"
                                    ValidationExpression="[A-Z,Ñ,&]{3,4}[0-9]{2}[0-1][0-9][0-3][0-9][A-Z,0-9]?[A-Z,0-9]?[0-9,A-Z]?"></asp:RegularExpressionValidator>
                            </div>
                             <div class="col-md-2">
                                <asp:LinkButton style="background-color: #35c135;" ID="btnSearchClient" Text="<span class='glyphicon glyphicon-search'></span>" runat="server" CssClass="btn btn-info" OnClick="btnSearchClient_Click" />
                            </div>
                        </div>
                        
                    </div>
                    <div class="col-md-3">
                        <div class="form-group" id="DivRazonSocial">
                            <asp:Label runat="server" AssociatedControlID="RazonSocial" CssClass="col-md-4 control-label">* Nombre</asp:Label>
                            <div class="col-md-8">
                                <asp:TextBox runat="server" ID="RazonSocial" CssClass="form-control" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="RazonSocial" Display="Dynamic"
                                    CssClass="text-danger" ErrorMessage="La razón social es obligatoria." ValidationGroup="vgFacturar" />
                            </div>
                        </div> 
                      
                        
                   </div>
                    <div class="col-md-3">
                          
                    <%--   <div class="form-group">
                          <asp:Label runat="server" AssociatedControlID="CondicionesPago" CssClass="col-md-3 control-label">C pago</asp:Label>
                           <div class="col-md-9">
                           <asp:TextBox ID="CondicionesPago" runat="server" CssClass="form-control" />
                         </div>
                       </div> --%>
                    </div>
                    <div class="col-md-3">
                        <asp:HiddenField ID="idReceptor" runat="server" />
                        <div class="form-group">
                            
                            <div class="col-md-6">
                                <asp:TextBox runat="server" ID="Identificador" CssClass="form-control" OnTextChanged="Identificador_TextChanged" AutoPostBack="True" Visible="false" />
                            </div>
                           
                        </div>
                         <div class="form-group">
                           
                            <div class="col-md-8">
                                <asp:DropDownList runat="server" ID="ddlPais" CssClass="form-control" OnSelectedIndexChanged="ddlPais_SelectedIndexChanged" AutoPostBack="true" Visible="false" />
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="RFC" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
        
        <h5><span class='glyphicon glyphicon-list'></span> Detalles:</h5>
        <div class="row" id="cabecera">
            <div class="col-md-3">
                 <asp:UpdatePanel ID="upSerie" runat="server">
                    <ContentTemplate>
                        <asp:HiddenField ID="idEmpresa" runat="server" Value="" />
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="Serie" CssClass="col-md-4 control-label">Documento</asp:Label>
                            <div class="col-md-8">
                                <asp:DropDownList runat="server" ID="Serie" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="Serie_SelectedIndexChanged" >
                                </asp:DropDownList>
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="Serie" ValidationGroup="vgFacturar" Display="Dynamic"
                                    CssClass="text-danger" ErrorMessage="La serie es obligatoria." />
                            </div>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="Empresa" />
                    </Triggers>
                </asp:UpdatePanel>  
                 <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="ddlFormaPago" CssClass="col-md-4 control-label">* F / pago</asp:Label>
                    <div class="col-md-8">
                        <asp:DropDownList runat="server" ID="ddlFormaPago" CssClass="form-control" OnSelectedIndexChanged="ddlFormaPago_SelectedIndexChanged" AutoPostBack="true" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlFormaPago" ValidationGroup="vgFacturar" Display="Dynamic"
                            CssClass="text-danger" ErrorMessage="El método de pago es obligatorio." />
                    </div>
                </div>
                <div class="form-group">
                   
                    <div class="col-md-8">
                        <asp:DropDownList ID="Empresa" runat="server" CssClass="form-control" AutoPostBack="true" Visible="false" OnSelectedIndexChanged="Empresa_SelectedIndexChanged">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="Empresa" ValidationGroup="vgFacturar" Display="Dynamic"
                            CssClass="text-danger" ErrorMessage="La empresa es obligatoria." />
                    </div>
                </div>
                         
            </div>
            <div class="col-md-3">
               
               <div class="form-group">
                   <asp:Label runat="server" AssociatedControlID="ddlUsoCFDI" CssClass="col-md-4 control-label">* Uso</asp:Label>
                    <div class="col-md-8">
                                <asp:DropDownList runat="server" ID="ddlUsoCFDI" CssClass="form-control" OnSelectedIndexChanged="ddlUsoCFDI_SelectedIndexChanged" AutoPostBack="true" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlUsoCFDI" ValidationGroup="vgFacturar" Display="Dynamic"
                                    CssClass="text-danger" ErrorMessage="El Uso de CFDI es obligatorio." />
                            </div>
                    
                </div>
                <div class="form-group">
                              <asp:Label runat="server" AssociatedControlID="ddlMetodoPago" CssClass="col-md-4 control-label">* M / pago</asp:Label>
                          
                    <div class="col-md-8">
                        <asp:DropDownList runat="server" ID="ddlMetodoPago" CssClass="form-control" OnSelectedIndexChanged="ddlMetodoPago_SelectedIndexChanged" AutoPostBack="true" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlMetodoPago" ValidationGroup="vgFacturar" Display="Dynamic"
                            CssClass="text-danger" ErrorMessage="El método de pago es obligatorio." />
                    </div>
                </div>  
            </div>
             <div class="col-md-3">
                 <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="Moneda" CssClass="col-md-4 control-label">* Moneda</asp:Label>
                    <div class="col-md-8">
                        <asp:DropDownList runat="server" ID="Moneda" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="Moneda_SelectedIndexChanged">
                        </asp:DropDownList>
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="Moneda" ValidationGroup="vgFacturar" Display="Dynamic"
                            CssClass="text-danger" ErrorMessage="La moneda es obligatoria." />
                    </div>
                </div>
                 
            </div>
            <div class="col-md-3">
                  <div class="form-group">
                    <asp:UpdatePanel ID="upTipoCambio" runat="server">
                        <ContentTemplate>
                            <asp:Label runat="server" AssociatedControlID="TipoCambio" CssClass="col-md-4 control-label">T / cambio</asp:Label>
                            <div class="col-md-8">
                                <asp:TextBox ID="TipoCambio" runat="server" CssClass="form-control numeric" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="TipoCambio" ValidationGroup="vgFacturar" Display="Dynamic"
                                    CssClass="text-danger" ErrorMessage="El tipo de cambio es obligatorio." />
                            </div>
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="Moneda" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
               
            </div>
        </div>
         <h5><span class='glyphicon glyphicon-random'></span> Documento relacionado:</h5>
        <div class="row" id="relacionado">
             <div class="col-md-3">
                 <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="ddlTipoRelacion" CssClass="col-md-4 control-label">Relacion</asp:Label>
                        <div class="col-md-8">
                            <asp:DropDownList runat="server" ID="ddlTipoRelacion" CssClass="form-control" OnSelectedIndexChanged="ddlddlTipoRelacion_SelectedIndexChanged" AutoPostBack="true" />
                        </div>
                </div>
             </div>
             <div class="col-md-3">
                 <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="ddlUsoCFDI" CssClass="col-md-4 control-label">UUID</asp:Label>
                        <div class="col-md-8">
                            <asp:TextBox ID="CfdiRelacionado" runat="server" CssClass="form-control" />
                        </div>
                </div>
             </div>
        </div>
        <asp:UpdatePanel ID="upOriCon" runat="server">
            <ContentTemplate>
                <div class="row" id="ori_con">
                    <div class="col-md-6">
                        <div id="divOrigen" class="form-group" runat="server" style="display: none">
                            <asp:Label runat="server" AssociatedControlID="Origen" CssClass="col-md-4 control-label">Origen</asp:Label>
                            <div class="col-md-8">
                                <asp:TextBox runat="server" ID="Origen" CssClass="form-control numeric" />
                                <asp:RequiredFieldValidator ID="rfvOrigen" runat="server" ControlToValidate="Origen" Display="Dynamic" ValidationGroup="vgFacturar"
                                    CssClass="text-danger" ErrorMessage="El Origen es obligatorio." />
                                <asp:RegularExpressionValidator ID="revOrigen" runat="server" ErrorMessage="el origen debe contener 3 caracteres." Display="Dynamic"
                                    CssClass="text-danger" ValidationGroup="vgFacturar" ControlToValidate="Origen"
                                    ValidationExpression="\d{3}"></asp:RegularExpressionValidator>
                            </div>
                        </div>
                        <div id="divFechaPago" class="form-group" runat="server" style="display: none">
                            <asp:Label runat="server" AssociatedControlID="FechaPago" CssClass="col-md-4 control-label">Fecha de pago</asp:Label>
                            <div class="col-md-8">
                                <div class="input-group date" id='DateFechaPago'>
                                    <asp:TextBox ID="FechaPago" runat="server" CssClass="form-control date form_datetime" />
                                    <span class="input-group-addon" style="visibility: hidden"><span class="glyphicon glyphicon-calendar"></span></span>
                                    <asp:RequiredFieldValidator ID="rfvFechaPago" runat="server" ControlToValidate="FechaPago" Display="Dynamic" ValidationGroup="vgFacturar"
                                        CssClass="text-danger" ErrorMessage="La fecha de pago es obligatoria." />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-6">
                        <div id="divContrato" class="form-group" runat="server" style="display: none">
                            <asp:Label runat="server" AssociatedControlID="Contrato" CssClass="col-md-3 control-label">Contrato</asp:Label>
                            <div class="col-md-8">
                                <asp:TextBox runat="server" ID="Contrato" CssClass="form-control numeric" />
                                <asp:RequiredFieldValidator ID="rfvContrato" runat="server" ControlToValidate="Contrato" Display="Dynamic" ValidationGroup="vgFacturar"
                                    CssClass="text-danger" ErrorMessage="El contrato es obligatorio." />
                                <asp:RegularExpressionValidator ID="revContrato" runat="server" ErrorMessage="el contrato debe contener 8 caracteres." Display="Dynamic"
                                    CssClass="text-danger" ValidationGroup="vgFacturar" ControlToValidate="Contrato"
                                    ValidationExpression="\d{8}"></asp:RegularExpressionValidator>
                            </div>
                        </div>
                    </div>
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="Empresa" />
            </Triggers>
        </asp:UpdatePanel>
        

        <h5><span class='glyphicon glyphicon-shopping-cart'></span> Artículos:</h5>
        <asp:UpdatePanel ID="upConcepto" runat="server">
            <ContentTemplate>
                <div class="row" id="conceptos">
                    <div class="col-md-3">
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="ClaveCargo" CssClass="col-md-4 control-label">Codigo</asp:Label>
                            <div class="col-md-6">
                                <asp:TextBox ID="ClaveCargo" runat="server" CssClass="form-control" OnTextChanged="ClaveCargo_TextChanged" AutoPostBack="true" />
                            </div>
                            <div class="col-md-2">
                                <asp:LinkButton style="background-color: #35c135;" ID="btnSearchProducto" Text="<span class='glyphicon glyphicon-search'></span>" runat="server" CssClass="btn btn-info" OnClick="btnSearchProducto_Click" />
                            </div>
                        </div>
                         <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="Precio" CssClass="col-md-4 control-label">* Precio U</asp:Label>
                            <div class="col-md-8">
                                <asp:TextBox ID="Precio" runat="server" CssClass="form-control numeric" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="Precio" ValidationGroup="vgConcepto" Display="Dynamic"
                                    CssClass="text-danger" ErrorMessage="El precio unitario es obligatoria." />
                            </div>
                        </div>
                   
                    </div>
                    <div class="col-md-3">
                         <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="DdlcProdServ" CssClass="col-md-4 control-label">* Clave</asp:Label>
                            <div class="col-md-8">
                                <asp:DropDownList runat="server" ID="DdlcProdServ" CssClass="form-control" OnSelectedIndexChanged="DdlcProdServ_SelectedIndexChanged" AutoPostBack="true" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="DdlcProdServ" ValidationGroup="vgFacturar" Display="Dynamic"
                                    CssClass="text-danger" ErrorMessage="La clave de Unidad es obligatoria." />
                            </div>
                        </div>
                         <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="Descuento" CssClass="col-md-4 control-label">Desc. %</asp:Label>
                            <div class="col-md-8">
                                <asp:TextBox runat="server" ID="Descuento" CssClass="form-control numeric" />
                            </div>
                        </div>
                                          
                        <%--<div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="IVA" CssClass="col-md-3 control-label">IVA %</asp:Label>
                            <div class="col-md-9">
                                <asp:TextBox runat="server" ID="IVA" CssClass="form-control numeric" />
                            </div>
                        </div>--%>
                        
                        
                        <div class="form-group" runat="server" style="display: none" id="divIvaRetenido">
                          
                            <div class="col-md-6">
                                <asp:DropDownList runat="server" ID="DdlTasaoCuotaIVAret" CssClass="form-control" OnSelectedIndexChanged="DdlTasaoCuotaIVAret_SelectedIndexChanged" AutoPostBack="true" Visible="false" />
                            </div>
                        </div>
                        <div class="form-group" runat="server" style="display: none" id="divFacIvaRetenido">
                           
                            <div class="col-md-6">
                                <asp:DropDownList runat="server" ID="DdlTipoFactorIVAret" CssClass="form-control" OnSelectedIndexChanged="DdlTipoFactorIVAret_SelectedIndexChanged" AutoPostBack="true" Visible="false"  />
                            </div>
                        </div>

                        <div class="form-group" runat="server" style="display: none" id="divIsrRetenido">
                         
                            <div class="col-md-6">
                                <asp:DropDownList runat="server" ID="DdlTasaoCuotaISRret" CssClass="form-control" OnSelectedIndexChanged="DdlTasaoCuotaISRret_SelectedIndexChanged" AutoPostBack="true" Visible="false" />
                            </div>
                        </div>
                        <div class="form-group" runat="server" style="display: none" id="divFacIsrRetenido">
                          
                            <div class="col-md-6">
                                <asp:DropDownList runat="server" ID="DdlTipoFactorISRret" CssClass="form-control" OnSelectedIndexChanged="DdlTipoFactorISRret_SelectedIndexChanged" AutoPostBack="true" Visible="false" />
                            </div>
                        </div>
                        <%--<div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="DdlTasaoCuotaIEPS" CssClass="col-md-3 control-label">Tasa O Cuota IEPS</asp:Label>
                            <div class="col-md-6">
                                <asp:DropDownList runat="server" ID="DdlTasaoCuotaIEPS" CssClass="form-control" OnSelectedIndexChanged="DdlTasaoCuotaIEPS_SelectedIndexChanged" AutoPostBack="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="DdlTipoFactorIEPS" CssClass="col-md-3 control-label">Tipo Factor IEPS</asp:Label>
                            <div class="col-md-6">
                                <asp:DropDownList runat="server" ID="DdlTipoFactorIEPS" CssClass="form-control" OnSelectedIndexChanged="DdlTipoFactorIEPS_SelectedIndexChanged" AutoPostBack="true" />
                            </div>
                        </div>--%>
                       
                        <div class="form-group" runat="server" style="display: none" id="divIsnRetenido">
                            <asp:Label runat="server" AssociatedControlID="ISN" CssClass="col-md-4 control-label">ISN %</asp:Label>
                            <div class="col-md-9">
                                <asp:TextBox runat="server" ID="ISN" CssClass="form-control numeric" AutoPostBack="true"/>
                            </div>
                        </div>
                        <%-- </div>
                        </div>--%>
                        <%--<div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="Fecha" CssClass="col-md-3 control-label">Fecha Pago</asp:Label>
                            <div class="col-md-9">
                                <div class="input-group date" id='DateFecha'>
                                    <asp:TextBox ID="Fecha" runat="server" CssClass="form-control date form_datetime" />
                                    <span class="input-group-addon" style="visibility: hidden"><span class="glyphicon glyphicon-calendar"></span></span>
                                </div>
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="TipoCambioConcepto" CssClass="col-md-3 control-label">Tipo Cambio</asp:Label>
                            <div class="col-md-9">
                                <asp:TextBox ID="TipoCambioConcepto" runat="server" CssClass="form-control numeric" />
                            </div>
                        </div>--%>
                    </div>
                    <div class="col-md-3">
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="DdlUnidad" CssClass="col-md-4 control-label">* Clave U</asp:Label>
                            <div class="col-md-8">
                                <asp:DropDownList runat="server" ID="DdlUnidad" CssClass="form-control" OnSelectedIndexChanged="DdlUnidad_SelectedIndexChanged" AutoPostBack="true" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="DdlUnidad" ValidationGroup="vgFacturar" Display="Dynamic"
                                    CssClass="text-danger" ErrorMessage="La clave de Unidad es obligatoria." />
                            </div>
                        </div>
                         <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="DdlTipoFactorIVA" CssClass="col-md-4 control-label">* Factor</asp:Label>
                            <div class="col-md-8">
                                <asp:DropDownList runat="server" ID="DdlTipoFactorIVA" CssClass="form-control" OnSelectedIndexChanged="DdlTipoFactorIVA_SelectedIndexChanged" AutoPostBack="true" />
                                <%--<asp:RequiredFieldValidator runat="server" ControlToValidate="DdlTipoFactorIVA" ValidationGroup="vgFacturar" Display="Dynamic"
                                    CssClass="text-danger" ErrorMessage="La clave de Unidad es obligatoria." />--%>
                            </div>
                        </div>
                    </div>
                    <div class="col-md-3">                    
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="Cantidad" CssClass="col-md-4 control-label">* Cant</asp:Label>
                            <div class="col-md-8">
                                <asp:TextBox ID="Cantidad" runat="server" CssClass="form-control numeric" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="Cantidad" ValidationGroup="vgConcepto" Display="Dynamic"
                                    CssClass="text-danger" ErrorMessage="La cantidad es obligatoria." />
                            </div>
                        </div>
                         <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="DdlTasaoCuotaIVA" CssClass="col-md-4 control-label">* T / C</asp:Label>
                            <div class="col-md-8">
                                <asp:DropDownList runat="server" ID="DdlTasaoCuotaIVA" CssClass="form-control" OnSelectedIndexChanged="DdlTasaoCuotaIVA_SelectedIndexChanged" AutoPostBack="true" />
                                <%--<asp:RequiredFieldValidator runat="server" ControlToValidate="DdlTasaoCuotaIVA" ValidationGroup="vgFacturar" Display="Dynamic"
                                    CssClass="text-danger" ErrorMessage="La clave de Unidad es obligatoria." />--%>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="Descripcion" CssClass="col-md-2 control-label">Descripción</asp:Label>
                    <div class="col-md-6">
                        <asp:TextBox runat="server" ID="Descripcion" CssClass="form-control" TextMode="MultiLine" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="Descripcion" Display="Dynamic"
                            CssClass="text-danger" ErrorMessage="La descripción es obligatoria." ValidationGroup="registrarGroup" />
                    </div>
                     <div class="col-md-2">
                        <asp:Button ID="btnAgregarConcepto" Text="Agregar concepto" runat="server" CssClass="btn btn-success pull-right" ValidationGroup="vgConcepto" OnClick="btnAgregarConcepto_Click" />
                    </div>
                </div>
                <div class="form-group">
                   
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="ClaveCargo" />
                <asp:AsyncPostBackTrigger ControlID="Serie" />
            </Triggers>
        </asp:UpdatePanel>

        <div class="form-group">
            <asp:UpdatePanel ID="upConceptos" runat="server">
                <ContentTemplate>
                    <asp:GridView ID="viewFacturas" runat="server" CssClass="table table-striped table-bordered table-hover" AutoGenerateColumns="False" EmptyDataText="No se han agregado conceptos" OnRowCommand="viewFacturas_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="clave_prod_serv" HeaderText="Clave Producto Servicio" />
                            <asp:BoundField DataField="cantidad" HeaderText="Cantidad" />
                            <asp:BoundField DataField="clave_unidad" HeaderText="Clave Unidad" />
                            <asp:BoundField DataField="no_identificacion" HeaderText="Clave" />
                            <asp:BoundField DataField="descripcion" HeaderText="Descripcion" />
                            <asp:BoundField DataField="valor_unitario" HeaderText="Valor Unitario" />
                            <asp:BoundField DataField="importe" HeaderText="Importe" />
                            <asp:ButtonField Text="<span class='glyphicon glyphicon-pencil'></span>" ItemStyle-ForeColor="Black" CommandName="Editar" />
                            <asp:ButtonField Text="<span class='glyphicon glyphicon-remove'></span>" ItemStyle-ForeColor="Red" CommandName="Eliminar" />
                        </Columns>
                    </asp:GridView>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnAgregarConcepto" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
        <div class="row" id="totales">
            <asp:UpdatePanel ID="upTotales" runat="server">
                <ContentTemplate>
                    <div class="col-md-9">
                    </div>
                    <div class="col-md-3">
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="Subtotal" CssClass="col-md-4 control-label">Subtotal</asp:Label>
                            <div class="col-md-8">
                                <asp:TextBox ID="Subtotal" runat="server" CssClass="form-control numeric" ReadOnly="true" />
                            </div>
                        </div>
                        <div class="form-group" id="divDescuento" runat="server" style="display: none">
                            <asp:Label runat="server" AssociatedControlID="DescuentoTotal" CssClass="col-md-4 control-label">Descuento</asp:Label>
                            <div class="col-md-8">
                                <asp:TextBox ID="DescuentoTotal" runat="server" CssClass="form-control numeric" ReadOnly="true" />
                            </div>
                        </div>
                        <div class="form-group" id="divIVA" runat="server" style="display: none">
                            <asp:Label runat="server" AssociatedControlID="IvaTotal" CssClass="col-md-4 control-label">IVA</asp:Label>
                            <div class="col-md-8">
                                <asp:TextBox ID="IvaTotal" runat="server" CssClass="form-control numeric" ReadOnly="true" />
                            </div>
                        </div>
                        <%--<div class="form-group" id="divIEPS" runat="server" style="display: none">
                            <asp:Label runat="server" AssociatedControlID="IepsTotal" CssClass="col-md-4 control-label">IEPS</asp:Label>
                            <div class="col-md-8">
                                <asp:TextBox ID="IepsTotal" runat="server" CssClass="form-control numeric" ReadOnly="true" />
                            </div>
                        </div>--%>
                        <div class="form-group" id="divRetencionIVA" runat="server" style="display: none">
                            <asp:Label runat="server" AssociatedControlID="RetencionIvaTotal" CssClass="col-md-4 control-label">Retención IVA</asp:Label>
                            <div class="col-md-8">
                                <asp:TextBox ID="RetencionIvaTotal" runat="server" CssClass="form-control numeric" ReadOnly="true" />
                            </div>
                        </div>
                        <div class="form-group" id="divRetencionISR" runat="server" style="display: none">
                            <asp:Label runat="server" AssociatedControlID="RetencionIsrTotal" CssClass="col-md-4 control-label">Retención ISR</asp:Label>
                            <div class="col-md-8">
                                <asp:TextBox ID="RetencionIsrTotal" runat="server" CssClass="form-control numeric" ReadOnly="true" />
                            </div>
                        </div>
                        <div class="form-group" id="divISH" runat="server" style="display: none">
                            <asp:Label runat="server" AssociatedControlID="IshTotal" CssClass="col-md-4 control-label">ISH</asp:Label>
                            <div class="col-md-8">
                                <asp:TextBox ID="IshTotal" runat="server" CssClass="form-control numeric" ReadOnly="true" />
                            </div>
                        </div>
                        <div class="form-group" id="divISN" runat="server" style="display: none">
                            <asp:Label runat="server" AssociatedControlID="IsnTotal" CssClass="col-md-4 control-label">ISN</asp:Label>
                            <div class="col-md-8">
                                <asp:TextBox ID="IsnTotal" runat="server" CssClass="form-control numeric" ReadOnly="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="Total" CssClass="col-md-4 control-label">Total</asp:Label>
                            <div class="col-md-8">
                                <asp:TextBox ID="Total" runat="server" CssClass="form-control numeric" ReadOnly="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="col-md-11">
                                <asp:Button ID="btnGenerar" Text="Generar factura" runat="server" OnClientClick="if (Page_ClientValidate('vgFacturar')) {this.disabled=true;this.value = 'Generando...'}" UseSubmitBehavior="false" CssClass="btn btn-primary pull-right" ValidationGroup="vgFacturar" OnClick="btnGenerar_Click" />
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <%--<asp:AsyncPostBackTrigger ControlID="IvaRetenido" />
                    <asp:AsyncPostBackTrigger ControlID="IsrRetenido" />--%>
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>

    <asp:UpdatePanel ID="UpPanelFooter" runat="server">

        <ContentTemplate>
            <div class="alert alert-success" aria-hidden="true" id="msg_exito_pie" hidden="hidden">
                <a href="#" id="msg_exito_close_pie" class="close">&times;</a>
                <asp:Label ID="lblExitoPie" Text="" runat="server" />
            </div>
            <div class="alert alert-danger" aria-hidden="true" id="msg_error_pie" hidden="hidden">
                <a href="#" id="msg_error_close_pie" class="close">&times;</a>
                <asp:Label ID="lblErrorPie" Text="" runat="server" />
            </div>
        </ContentTemplate>
        <Triggers>
        </Triggers>
    </asp:UpdatePanel>

    <div class="modal fade" id="modalCliente" tabindex="-1" role="dialog" aria-hidden="true" aria-labelledby="clienteLabel">
        <div class="modal-dialog modal-lg">
            <asp:UpdatePanel ID="upModalCliente" runat="server">
                <ContentTemplate>
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                            <h4 class="modal-title" id="clienteLabel">
                                <asp:Label runat="server" ID="titleCliente" Text="" /></h4>
                        </div>
                        <div class="modal-body">
                            <div class="form-horizontal">
                                <asp:HiddenField runat="server" ID="idEmisor" />
                                <p class="text-danger">
                                    <asp:Literal runat="server" ID="ErrorCliente" />
                                </p>

                                <div class="row">
                                    <div class="col-md-4">
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="BuscarIdentificador" CssClass="col-md-4 control-label">Identificador</asp:Label>
                                            <div class="col-md-8">
                                                <asp:TextBox runat="server" ID="BuscarIdentificador" CssClass="form-control" />
                                            </div>
                                        </div>
                                       
                                    </div>
                                    <div class="col-md-4">
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="BuscarRazonSocial" CssClass="col-md-4 control-label">Nombre</asp:Label>
                                            <div class="col-md-8">
                                                <asp:TextBox runat="server" ID="BuscarRazonSocial" CssClass="form-control" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-md-4">
                                         <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="BuscarRFC" CssClass="col-md-4 control-label">RFC</asp:Label>
                                            <div class="col-md-8">
                                                <asp:TextBox runat="server" ID="BuscarRFC" CssClass="form-control" />
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <div class="col-md-12">
                                                <asp:Button runat="server" ID="btnBuscarCliente" CssClass="btn btn-primary pull-right" Text="Buscar" OnClick="btnBuscarCliente_Click"></asp:Button>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <br></br>
                                <asp:GridView ID="viewEmpresas" runat="server" CssClass="table table-striped table-bordered table-hover" AutoGenerateColumns="False" DataKeyNames="idreceptor" OnRowCommand="viewEmpresas_RowCommand">
                                    <Columns>
                                        <asp:BoundField DataField="idreceptor" HeaderText="idreceptor" Visible="False" />
                                        <asp:BoundField DataField="identificador" HeaderText="Identificador" />
                                        <asp:BoundField DataField="razon_social" HeaderText="Razon Social" />
                                        <asp:BoundField DataField="rfc" HeaderText="RFC" />
                                        <asp:ButtonField Text="<span class='glyphicon glyphicon-ok'></span>" ItemStyle-ForeColor="Green" CommandName="Seleccionar" />
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal">Cerrar</button>
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnSearchClient" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>

    <div class="modal fade" id="modalProducto" tabindex="-1" role="dialog" aria-hidden="true" aria-labelledby="productoLabel">
        <div class="modal-dialog modal-lg">
            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                <ContentTemplate>
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                            <h4 class="modal-title" id="productoLabel">
                                <asp:Label runat="server" ID="titleProducto" Text="" /></h4>
                        </div>
                        <div class="modal-body">
                            <div class="form-horizontal">
                                <asp:HiddenField runat="server" ID="idEmisorProducto" />
                                <p class="text-danger">
                                    <asp:Literal runat="server" ID="Literal1" />
                                </p>

                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="BuscarClave" CssClass="col-md-4 control-label">Clave</asp:Label>
                                            <div class="col-md-8">
                                                <asp:TextBox runat="server" ID="BuscarClave" CssClass="form-control" />
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <div class="col-md-12">
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="BuscarDescripcion" CssClass="col-md-4 control-label">Descripcion</asp:Label>
                                            <div class="col-md-8">
                                                <asp:TextBox runat="server" ID="BuscarDescripcion" CssClass="form-control" />
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <div class="col-md-12">
                                                <asp:Button runat="server" ID="btnBuscarProducto" CssClass="btn btn-primary pull-right" Text="Buscar" OnClick="btnBuscarProducto_Click"></asp:Button>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <br></br>
                                <asp:GridView ID="viewProductos" runat="server" CssClass="table table-striped table-bordered table-hover" AutoGenerateColumns="False" DataKeyNames="id" OnRowCommand="viewProductos_RowCommand">
                                    <Columns>
                                        <asp:BoundField DataField="id" HeaderText="id" Visible="False" />
                                        <asp:BoundField DataField="codigo" HeaderText="Codigo" />
                                        <asp:BoundField DataField="descripcion" HeaderText="Descripcion" />
                                        <asp:BoundField DataField="valor_unitario" HeaderText="Valor unitario" />
                                        <asp:BoundField DataField="iva" HeaderText="IVA" />
                                        <%--<asp:BoundField DataField="ieps" HeaderText="IEPS" />--%>
                                        
                                        <asp:ButtonField Text="<span class='glyphicon glyphicon-ok'></span>" ItemStyle-ForeColor="Green" CommandName="Seleccionar" />
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal">Cerrar</button>
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnSearchProducto" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>

</asp:Content>
