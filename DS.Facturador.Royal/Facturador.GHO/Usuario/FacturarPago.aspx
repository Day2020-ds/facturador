<%@ Page Title="Facturación" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FacturarPago.aspx.cs" Inherits="Facturador.GHO.Usuario.FacturarPago" %>

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
                format: "yyyy-mm-dd",
                language: "es",
                todayHighlight: true,
                startDate: '01/01/2014'
            }).on('changeDate', function (ev) {
                var newDate = new Date(ev.date)
                checkin.hide();
            }).data('datepicker');
        };
    </script>
    <h2>Complemento de pago</h2>
    <hr />

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

    <div>
        <h5><span class='glyphicon glyphicon-lock'></span> Emisor:</h5>
        <div class="row" id="cabecera">
            <!-- Empresa -->
            <div class="col-md-5">
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="Empresa" CssClass="control-label">Empresa</asp:Label>
                    <asp:DropDownList ID="Empresa" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="Empresa_SelectedIndexChanged">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="Empresa" ValidationGroup="vgFacturar" Display="Dynamic"
                        CssClass="text-danger" ErrorMessage="La empresa es obligatoria." />
                </div>
            </div>
            <!-- Serie -->
            <div class="col-md-5">
                 <asp:UpdatePanel ID="upSerie" runat="server">
                    <ContentTemplate>
                        <asp:HiddenField ID="idEmpresa" runat="server" Value="" />
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="Serie" CssClass="control-label">Serie</asp:Label>
                            <asp:DropDownList runat="server" ID="Serie" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="Serie_SelectedIndexChanged" >
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="Serie" ValidationGroup="vgFacturar" Display="Dynamic"
                                CssClass="text-danger" ErrorMessage="La serie es obligatoria." />
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="Empresa" />
                    </Triggers>
                </asp:UpdatePanel>
            </div>
             <div class="col-md-3">
            </div>
             <div class="col-md-3">
            </div>
        </div>
        <!-- Receptor -->
        <h5><span class='glyphicon glyphicon-lock'></span> Receptor:</h5>
        <div class="row" id="cliente">
            <asp:UpdatePanel ID="upCliente" runat="server">
                <ContentTemplate>
                    <!-- Buscar -->
                    <div class="col-md-12">
                        <div class="row">
                            <!-- Cliente -->
                            <div class="col-md-3" style="padding: 0px;">
                                <asp:LinkButton style="background-color: #35c135;width:100%;" ID="btnSearchClient" Text="<span class='glyphicon glyphicon-search'></span> Buscar cliente" runat="server" CssClass="btn btn-info" OnClick="btnSearchClient_Click" />
                            </div>
                            <!-- Comprobante -->
                            <div class="col-md-3" style="padding: 0px;">
                                <asp:LinkButton style="margin-left:15px; width:100%;" ID="LinkButton1" Text="<i class='fas fa-file-invoice'></i> Buscar por CFDI" runat="server" CssClass="btn btn-primary" OnClick="btnSearchClient_Click" />
                            </div>
                        </div>
                    </div>
                    <!-- Cliente -->
                    <div class="col-md-3">
                        <asp:HiddenField ID="idReceptor" runat="server" />
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="Identificador" CssClass="control-label">Identificador</asp:Label>
                            <asp:TextBox runat="server" ID="Identificador" CssClass="form-control" OnTextChanged="Identificador_TextChanged" AutoPostBack="True" disabled/>
                        </div>
                    </div>
                    <!-- RFC -->
                    <div class="col-md-3">
                        <div class="form-group" id="DivRFC">
                            <asp:Label runat="server" AssociatedControlID="RFC" CssClass="control-label">* RFC</asp:Label>
                            <asp:TextBox runat="server" ID="RFC" CssClass="form-control upper" MaxLength="13" OnTextChanged="RFC_TextChanged" AutoPostBack="true" disabled/>
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="RFC" Display="Dynamic" ValidationGroup="vgFacturar"
                                CssClass="text-danger" ErrorMessage="El RFC es obligatorio." />
                            <asp:RegularExpressionValidator ID="revRFC" runat="server" ErrorMessage="El RFC no es valido" Display="Dynamic"
                                CssClass="text-danger" ValidationGroup="vgFacturar" ControlToValidate="RFC"
                                ValidationExpression="[A-Z,Ñ,&]{3,4}[0-9]{2}[0-1][0-9][0-3][0-9][A-Z,0-9]?[A-Z,0-9]?[0-9,A-Z]?"></asp:RegularExpressionValidator>
                        </div>
                    </div>
                    <!-- Razón social -->
                    <div class="col-md-3">
                        <div class="form-group" id="DivRazonSocial">
                            <asp:Label runat="server" AssociatedControlID="RazonSocial" CssClass="control-label">* Razón social</asp:Label>
                            <asp:TextBox runat="server" ID="RazonSocial" CssClass="form-control" disabled/>
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="RazonSocial" Display="Dynamic"
                                CssClass="text-danger" ErrorMessage="La razón social es obligatoria." ValidationGroup="vgFacturar" />
                        </div>
                    </div>
                    <!-- País -->
                    <div class="col-md-3">
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="ddlPais" CssClass="control-label">Pais</asp:Label>
                            <asp:DropDownList runat="server" ID="ddlPais" CssClass="form-control" OnSelectedIndexChanged="ddlPais_SelectedIndexChanged" AutoPostBack="true" />
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="RFC" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
        <!-- Información -->
        <h5><span class='glyphicon glyphicon-list'></span> Información:</h5>
        <div class="row" id="pago">
            <!-- Forma de pago -->
            <div class="col-md-5">
                <asp:Label runat="server" AssociatedControlID="ddlFormaPago" CssClass="control-label">* Forma de pago</asp:Label>
                <asp:DropDownList runat="server" ID="ddlFormaPago" CssClass="form-control" OnSelectedIndexChanged="ddlFormaPago_SelectedIndexChanged" AutoPostBack="true" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="ddlFormaPago" ValidationGroup="vgFacturar" Display="Dynamic"
                    CssClass="text-danger" ErrorMessage="El método de pago es obligatorio." />
            </div>
            <!-- Moneda -->
            <div class="col-md-3">
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="Moneda" CssClass="control-label">* Moneda</asp:Label>
                    <asp:DropDownList runat="server" ID="Moneda" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="Moneda_SelectedIndexChanged">
                    </asp:DropDownList>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="Moneda" ValidationGroup="vgFacturar" Display="Dynamic"
                        CssClass="text-danger" ErrorMessage="La moneda es obligatoria." />
                </div>
            </div>
            <!-- Tipo de cambio -->
             <div class="col-md-2">
                <div class="form-group">
                    <asp:UpdatePanel ID="upTipoCambio" runat="server">
                        <ContentTemplate>
                            <asp:Label runat="server" AssociatedControlID="TipoCambio" CssClass="control-label">Tipo de cambio</asp:Label>
                            <asp:TextBox ID="TipoCambio" runat="server" CssClass="form-control numeric" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="TipoCambio" ValidationGroup="vgFacturar" Display="Dynamic"
                                CssClass="text-danger" ErrorMessage="El tipo de cambio es obligatorio." />
                        </ContentTemplate>
                        <Triggers>
                            <asp:AsyncPostBackTrigger ControlID="Moneda" />
                        </Triggers>
                    </asp:UpdatePanel>
                </div>
            </div>
            <!-- Monto -->
            <div class="col-md-2">
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="Monto" CssClass="control-label">* Monto</asp:Label>
                    <asp:TextBox ID="Monto" runat="server" CssClass="form-control numeric" placeholder="$ 0.00"/>
                    <asp:RequiredFieldValidator runat="server" ControlToValidate="Monto" ValidationGroup="vgConcepto" Display="Dynamic"
                        CssClass="text-danger" ErrorMessage="El Monto es obligatorio." />
                </div>
            </div>
            <!-- Fecha -->
            <div class="col-md-2">
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="FechaPago" CssClass="control-label">* Fecha</asp:Label>
                    <asp:TextBox Type="Date" ID="FechaPago" runat="server" style="width: 170px;" CssClass="form-control date form_datetime" />
                    <!-- 
                    <span class="input-group-addon" style="visibility: hidden"><span class="glyphicon glyphicon-calendar"></span></span>
                    -->
                </div>
            </div>
            <!-- Operación -->
            <div class="col-md-3">
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="NumOperacion" CssClass="control-label">Operación</asp:Label>
                    <asp:TextBox ID="NumOperacion" runat="server" CssClass="form-control numeric" />
                </div>
            </div>
            <!-- RFC CO -->
            <div class="col-md-3">
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="RFCOrd" CssClass="control-label">RFC Ordenante</asp:Label>
                    <asp:TextBox runat="server" ID="RFCOrd" CssClass="form-control upper" MaxLength="13" AutoPostBack="true" />
                    <asp:RegularExpressionValidator ID="revRFC2" runat="server" ErrorMessage="El RFC no es valido" Display="Dynamic"
                        CssClass="text-danger" ValidationGroup="vgFacturar" ControlToValidate="RFCOrd"
                        ValidationExpression="[A-Z,Ñ,&]{3,4}[0-9]{2}[0-1][0-9][0-3][0-9][A-Z,0-9]?[A-Z,0-9]?[0-9,A-Z]?"></asp:RegularExpressionValidator>
                </div>
            </div>
            <!-- RFC CB -->
            <div class="col-md-3">
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="RFCBen" CssClass="control-label">RFC Beneficiario</asp:Label>
                    <asp:TextBox runat="server" ID="RFCBen" CssClass="form-control upper" MaxLength="13" AutoPostBack="true" />
                    <asp:RegularExpressionValidator ID="revRFC3" runat="server" ErrorMessage="El RFC no es valido" Display="Dynamic"
                        CssClass="text-danger" ValidationGroup="vgFacturar" ControlToValidate="RFCBen"
                        ValidationExpression="[A-Z,Ñ,&]{3,4}[0-9]{2}[0-1][0-9][0-3][0-9][A-Z,0-9]?[A-Z,0-9]?[0-9,A-Z]?"></asp:RegularExpressionValidator>
                </div>
            </div>
            <!-- Banco O -->
            <div class="col-md-3">
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="NomBancoOrdExt" CssClass="control-label">Banco Ordenante</asp:Label>
                    <asp:TextBox runat="server" ID="NomBancoOrdExt" CssClass="form-control" AutoPostBack="true" />
                </div>
            </div>
            <!-- Cta O -->
            <div class="col-md-3">
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="CtaOrdenante" CssClass="control-label">Cuenta Ordenante</asp:Label>
                    <asp:TextBox runat="server" ID="CtaOrdenante" CssClass="form-control" AutoPostBack="true" />
                </div>
            </div>
            <!-- Cta B -->
            <div class="col-md-3">
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="CtaBeneficiario" CssClass="control-label">Cuenta Beneficiario</asp:Label>
                    <asp:TextBox runat="server" ID="CtaBeneficiario" CssClass="form-control" AutoPostBack="true" />
                </div>
            </div>
            <!-- Parcialidad -->
            <div class="col-md-3">
                 <div class="form-group">
                     <asp:Label runat="server" AssociatedControlID="Monto" CssClass="control-label">Parcialidad</asp:Label>
                     <asp:TextBox ID="parcialidad" runat="server" CssClass="form-control numeric" value="1" />
                     <%--<asp:RequiredFieldValidator runat="server" ControlToValidate="ImportePagado" ValidationGroup="vgConcepto" Display="Dynamic"
                        CssClass="text-danger" ErrorMessage="El Monto es obligatorio." />--%>
                 </div>  
             </div>
            <!-- Saldo Anterior -->
            <div class="col-md-3">
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="Monto" CssClass="control-label">* Saldo Anterior</asp:Label>
                    <asp:TextBox ID="SaldoAnterior" runat="server" CssClass="form-control numeric" placeholder="$ 0.00" />
                    <%--<asp:RequiredFieldValidator runat="server" ControlToValidate="SaldoAnterior" ValidationGroup="vgConcepto" Display="Dynamic"
                            CssClass="text-danger" ErrorMessage="El Saldo Anterior es obligatorio." />--%>
                </div>
            </div>
            <!-- Saldo anterior -->
            <div class="col-md-3">
                <div class="form-group">
                     <asp:Label runat="server" AssociatedControlID="Monto" CssClass="control-label">* Importe Pagado</asp:Label>
                     <asp:TextBox ID="ImportePagado" runat="server" CssClass="form-control numeric" placeholder="$ 0.00" />
                     <%--<asp:RequiredFieldValidator runat="server" ControlToValidate="ImportePagado" ValidationGroup="vgConcepto" Display="Dynamic"
                            CssClass="text-danger" ErrorMessage="El Monto es obligatorio." />--%>
                </div>
            </div>
            <!-- Btn consultar -->
            <!--
            <div class="col-md-3" style="padding-top:26px;">
                <asp:Button runat="server" ID="btnConsultar" Text="Consultar" CssClass="btn btn-primary" OnClick="Consultar" />
            </div>
            -->
        </div>
        <br />
    <asp:UpdatePanel ID="upCrudGrid" runat="server">
        <ContentTemplate>
            <div class="form-horizontal">
                <div class="form-group">
                    <asp:GridView ID="viewFacturas" runat="server" CssClass="table table-striped table-bordered table-hover" AutoGenerateColumns="False" AllowPaging="True" OnPageIndexChanging="viewFacturas_PageIndexChanging" OnRowCommand="viewFacturas_RowCommand" DataKeyNames="uuid" OnRowCreated="viewFacturas_RowCreated">
                        <Columns>
                            <asp:BoundField DataField="uuid" HeaderText="Folio Fiscal" />
                            <asp:BoundField DataField="serie" HeaderText="Serie" />
                            <asp:BoundField DataField="folio" HeaderText="Folio" />
                            <asp:BoundField DataField="fecha_timbrado" HeaderText="Fecha timbrado" />
                            <asp:BoundField DataField="moneda" HeaderText="Moneda" />
                            <asp:BoundField DataField="tipo_cambio" HeaderText="Tipo Cambio" />
                            <asp:BoundField DataField="subtotal" HeaderText="Subtotal" />
                            <asp:BoundField DataField="total" HeaderText="Total" />
                            <asp:BoundField DataField="estatus" HeaderText="Estatus" />
                            <asp:ButtonField Text="<span class='glyphicon glyphicon-ok'></span>" ItemStyle-ForeColor="Green" HeaderText="Agregar" CommandName="Agregar" />
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnConsultar" />
            <asp:PostBackTrigger ControlID="viewFacturas" />
        </Triggers>

    </asp:UpdatePanel>
        <div class="col-md-12">
                
        </div>
        <div class="col-md-12">
           
        </div>
        <div class="col-md-12">
               
        </div>
        <div class="form-group">
            <asp:UpdatePanel ID="upRelacionados" runat="server">
                <ContentTemplate>
                    <asp:GridView ID="viewRelacionados" runat="server" CssClass="table table-striped table-bordered table-hover" AutoGenerateColumns="False" EmptyDataText="No se han agregado conceptos" OnRowCommand="viewFacturas_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="uuid" HeaderText="Folio Fiscal" />
                            <asp:BoundField DataField="serie" HeaderText="Serie" />
                            <asp:BoundField DataField="folio" HeaderText="Folio" />
                            <asp:BoundField DataField="moneda" HeaderText="Moneda" />
                            <asp:BoundField DataField="tipo_cambio" HeaderText="Tipo Cambio" />
                            <asp:BoundField DataField="saldoAnterior" HeaderText="Saldo Anterior" />
                            <asp:BoundField DataField="importePagado" HeaderText="Imp Pagado" />
                            <%--<asp:BoundField DataField="total" HeaderText="Imp Pagado" />--%>
                            <asp:ButtonField Text="<span class='glyphicon glyphicon-remove'></span>" ItemStyle-ForeColor="Red" CommandName="Eliminar" />
                        </Columns>
                    </asp:GridView>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnConsultar" />
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
                                <asp:TextBox ID="Subtotal" runat="server" CssClass="form-control numeric" ReadOnly="true" placeholder="$ 0.00" />
                            </div>
                        </div>
                        <div class="form-group" id="divIVA" runat="server" style="display: none">
                            <asp:Label runat="server" AssociatedControlID="IvaTotal" CssClass="col-md-4 control-label">IVA</asp:Label>
                            <div class="col-md-8">
                                <asp:TextBox ID="IvaTotal" runat="server" CssClass="form-control numeric" ReadOnly="true" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="Total" CssClass="col-md-4 control-label">Total</asp:Label>
                            <div class="col-md-8">
                                <asp:TextBox ID="Total" runat="server" CssClass="form-control numeric" ReadOnly="true"  placeholder="$ 0.00"/>
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="col-md-11" style="padding: 8px 0px;">
                                <asp:Button ID="btnGenerar" Text="Generar factura" runat="server" OnClientClick="if (Page_ClientValidate('vgFacturar')) {this.disabled=true;this.value = 'Generando...'}" UseSubmitBehavior="false" CssClass="btn btn-primary pull-right" ValidationGroup="vgFacturar" OnClick="btnGenerar_Click" />
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
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
                                <asp:Label runat="server" ID="titleCliente" Text="" />
                            </h4>
                        </div>
                        <div class="modal-body">
                            <div class="form-horizontal">
                                <asp:HiddenField runat="server" ID="idEmisor" />
                                <p class="text-danger">
                                    <asp:Literal runat="server" ID="ErrorCliente" />
                                </p>
                                <div class="row">
                                    <!-- Identificador -->
                                    <div class="col-md-4">
                                            <asp:Label runat="server" AssociatedControlID="BuscarIdentificador" CssClass="control-label">Identificador</asp:Label>
                                            <asp:TextBox runat="server" ID="BuscarIdentificador" CssClass="form-control" placeholder="Alias o razón social"/>
                                    </div>
                                    <!-- Nombre o razón social -->
                                     <div class="col-md-4">
                                             <asp:Label runat="server" AssociatedControlID="BuscarRazonSocial" CssClass="control-label">Nombre</asp:Label>
                                             <asp:TextBox runat="server" ID="BuscarRazonSocial" CssClass="form-control" placeholder="Razón social" />
                                    </div>
                                    <!-- RFC -->
                                    <div class="col-md-4">
                                        <asp:Label runat="server" AssociatedControlID="BuscarRFC" CssClass="control-label">RFC</asp:Label>
                                        <asp:TextBox runat="server" ID="BuscarRFC" CssClass="form-control" placeholder="XAXX010101000 "/>
                                    </div>
                                    <!-- Btn Buscar -->
                                    <div class="col-md-9"></div>
                                    <div class="col-md-3" style="padding-top:15px;">
                                        <asp:Button runat="server" ID="btnBuscarCliente" CssClass="btn btn-primary pull-right" Text="Buscar" OnClick="btnBuscarCliente_Click"></asp:Button>
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
                            <button type="button" class="btn btn-danger" data-dismiss="modal"> <i class="fas fa-times"></i> Cerrar</button>
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
                                        <asp:BoundField DataField="ish" HeaderText="ISH" />
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
               <%-- <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnSearchProducto" />
                </Triggers>--%>
            </asp:UpdatePanel>
        </div>
    </div>

</asp:Content>
