<%@ Page Title="Administrar empresas" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Empresas.aspx.cs" Inherits="Facturador.GHO.Admin.Empresa" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2><%: Title %>.</h2>
    <script type="text/javascript">
        $(document).on('keypress keyup blur', '.numeric', function (event) {
            $(this).val($(this).val().replace(/[^0-9\.]/g, ''));
            if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57)) {
                event.preventDefault();
            }
        });
        $(document).on('keypress keyup blur', '.upper', function (event) {
            $(this).val($(this).val().replace(/[^a-zA-Z0-9&]/g, function (s) { return '' }));
            //$(this).val($(this).val().replace(/([a-z])/, function (s) { return s.toUpperCase() }));
            $(this).val($(this).val().toUpperCase());
        });
    </script>
    <div class="form-horizontal">
        <asp:UpdatePanel ID="upEmpresas" runat="server">
            <ContentTemplate>
                <p class="text-danger">
                    <asp:Literal runat="server" ID="ErrorMessage" />
                </p>

                <asp:Button runat="server" ID="btnRegistrar" CssClass="btn btn-primary" Text="Agregar empresa" OnClick="btnRegistro_Click"></asp:Button>
                <br></br>
                <asp:GridView ID="viewEmpresas" runat="server" CssClass="table table-striped table-bordered table-hover" AutoGenerateColumns="False" DataKeyNames="idemisor" OnRowCommand="viewEmpresas_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="idemisor" HeaderText="idemisor" Visible="False" />
                        <asp:BoundField DataField="identificador" HeaderText="Identificador" />
                        <asp:BoundField DataField="razon_social" HeaderText="Razon Social" />
                        <asp:BoundField DataField="rfc" HeaderText="RFC" />
                        <asp:BoundField DataField="curp" HeaderText="CURP" />
                        <asp:ButtonField Text="<span class='glyphicon glyphicon-picture'></span>" ItemStyle-ForeColor="Black" CommandName="Pdf" HeaderText="PDF" />
                        <asp:ButtonField Text="<span class='glyphicon glyphicon-list'></span>" ItemStyle-ForeColor="Black" CommandName="Folio" HeaderText="Folios" />
                        <asp:ButtonField Text="<span class='glyphicon glyphicon-list'></span>" ItemStyle-ForeColor="Black" CommandName="cProdServ" HeaderText="Claves Producto o Servicio" />
                        <asp:ButtonField Text="<span class='glyphicon glyphicon-pencil'></span>" ItemStyle-ForeColor="Black" CommandName="Editar" />
                        <asp:ButtonField Text="<span class='glyphicon glyphicon-remove'></span>" ItemStyle-ForeColor="Red" CommandName="Eliminar" />
                    </Columns>
                </asp:GridView>

            </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="btnEmisor" />
            </Triggers>
        </asp:UpdatePanel>
    </div>

    <div class="modal fade" id="registro" tabindex="-1" role="dialog" aria-hidden="true" aria-labelledby="registroLabel">
        <div class="modal-dialog modal-lg">
            <asp:UpdatePanel ID="upRegistrar" runat="server">
                <ContentTemplate>
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                            <h4 class="modal-title" id="registroLabel">
                                <asp:Label runat="server" ID="titleEmisor" Text="" /></h4>
                        </div>
                        <div class="modal-body">
                            <div class="form-horizontal">
                                <asp:HiddenField runat="server" ID="idEmisor" />
                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="Identificador" CssClass="col-md-4 control-label">Identificador</asp:Label>
                                            <div class="col-md-8">
                                                <asp:TextBox runat="server" ID="Identificador" CssClass="form-control" />
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="Identificador" Display="Dynamic"
                                                    CssClass="text-danger" ErrorMessage="El identificador es obligatoria." ValidationGroup="registrarGroup" />
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="RazonSocial" CssClass="col-md-4 control-label">Razon social</asp:Label>
                                            <div class="col-md-8">
                                                <asp:TextBox runat="server" ID="RazonSocial" CssClass="form-control" />
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="RazonSocial" Display="Dynamic"
                                                    CssClass="text-danger" ErrorMessage="la razón social es obligatoria." ValidationGroup="registrarGroup" />
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="RFC" CssClass="col-md-4 control-label">RFC</asp:Label>
                                            <div class="col-md-8">
                                                <asp:TextBox runat="server" ID="RFC" CssClass="form-control upper" />
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="RFC" Display="Dynamic"
                                                    CssClass="text-danger" ErrorMessage="El RFC es obligatorio." ValidationGroup="registrarGroup" />
                                                <asp:RegularExpressionValidator ID="revRFC" runat="server" ErrorMessage="El RFC no es valido" Display="Dynamic"
                                                    CssClass="text-danger" ValidationGroup="registrarGroup" ControlToValidate="RFC"
                                                    ValidationExpression="[A-Z,Ñ,&]{3,4}[0-9]{2}[0-1][0-9][0-3][0-9][A-Z,0-9]?[A-Z,0-9]?[0-9,A-Z]?"></asp:RegularExpressionValidator>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="CURP" CssClass="col-md-4 control-label">CURP</asp:Label>
                                            <div class="col-md-8">
                                                <asp:TextBox runat="server" ID="CURP" MaxLength="18" CssClass="form-control upper" />
                                                <asp:RegularExpressionValidator ID="revCURP" runat="server" ErrorMessage="El CURP no es valido" Display="Dynamic"
                                                    CssClass="text-danger" ValidationGroup="registrarGroup" ControlToValidate="CURP"
                                                    ValidationExpression="([A-Z][A,E,I,O,U,X][A-Z]{2}[0-9]{2}[0-1][0-9][0-3][0-9][M,H][A-Z]{2}[B,C,D,F,G,H,J,K,L,M,N,Ñ,P,Q,R,S,T,V,W,X,Y,Z]{3}[0-9,A-Z][0-9])?"></asp:RegularExpressionValidator>
                                        </div>
                                        </div>
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="FileLogo" CssClass="col-md-4 control-label">Logotipo</asp:Label>
                                            <div class="col-md-8">
                                                <asp:FileUpload ID="FileLogo" runat="server" CssClass="form-control" />
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="FileCedula" CssClass="col-md-4 control-label">Cedula</asp:Label>
                                            <div class="col-md-8">
                                                <asp:FileUpload ID="FileCedula" runat="server" CssClass="form-control" />
                                            </div>
                                        </div>

                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="CodigoPostal" CssClass="col-md-4 control-label">Codigo Postal</asp:Label>
                                            <div class="col-md-8">
                                                <asp:TextBox runat="server" ID="CodigoPostal" CssClass="form-control numeric" />
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="CodigoPostal" Display="Dynamic"
                                                    CssClass="text-danger" ErrorMessage="El codigo postal es obligatorio." ValidationGroup="registrarGroup" />
                                                <asp:RegularExpressionValidator runat="server" ControlToValidate="CodigoPostal" ValidationGroup="registrarGroup" Display="Dynamic"
                                                    CssClass="text-danger" ErrorMessage="El codigo postal debe tener 5 digitos" ValidationExpression="\d{5}" />
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="LlavePrivada" CssClass="col-md-4 control-label">Llave privada</asp:Label>
                                            <div class="col-md-8">
                                                <asp:FileUpload ID="LlavePrivada" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvLLavePrivada" runat="server" ControlToValidate="LlavePrivada"
                                                    CssClass="text-danger" ErrorMessage="La llave privada es obligatoria." ValidationGroup="registrarGroup" Display="Dynamic" />
                                                <asp:RegularExpressionValidator runat="server" ControlToValidate="LlavePrivada" ValidationGroup="registrarGroup" Display="Dynamic"
                                                    CssClass="text-danger" ErrorMessage="no selecciono un archivo valido (.key)" ValidationExpression="^(([a-zA-Z]:)|(\\{2}\w+)\$?)(\\(\w[\w].*))(.key)$"></asp:RegularExpressionValidator>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="Contrasenia" CssClass="col-md-4 control-label">Contraseña</asp:Label>
                                            <div class="col-md-8">
                                                <asp:TextBox runat="server" ID="Contrasenia" CssClass="form-control" TextMode="Password" />
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="Contrasenia" Display="Dynamic"
                                                    CssClass="text-danger" ErrorMessage="La contraseña es obligatoria." ValidationGroup="registrarGroup" />
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="Certificado" CssClass="col-md-4 control-label">Certificado</asp:Label>
                                            <div class="col-md-8">
                                                <asp:FileUpload ID="Certificado" runat="server" CssClass="form-control" />
                                                <asp:RequiredFieldValidator ID="rfvCertificado" runat="server" ControlToValidate="Certificado" Display="Dynamic"
                                                    CssClass="text-danger" ErrorMessage="El certificado es obligatorio." ValidationGroup="registrarGroup" />
                                                <asp:RegularExpressionValidator runat="server" ControlToValidate="Certificado" ValidationGroup="registrarGroup" Display="Dynamic"
                                                    CssClass="text-danger" ErrorMessage="no selecciono un archivo valido (.cer)" ValidationExpression="^(([a-zA-Z]:)|(\\{2}\w+)\$?)(\\(\w[\w].*))(.cer)$"></asp:RegularExpressionValidator>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="cRegimen" CssClass="col-md-4 control-label">Régimen Fiscal</asp:Label>
                                            <div class="col-md-8">
                                                <asp:DropDownList runat="server" ID="cRegimen" CssClass="form-control">
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="cRegimen" Display="Dynamic"
                                                    CssClass="text-danger" ErrorMessage="El tipo de documento es obligatorio." ValidationGroup="registrarGroup" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <br />
                                <%--<div class="form-group">
                                    <asp:CheckBox ID="chkSucursal" runat="server" Text="Sucursal" OnCheckedChanged="chkSucursal_CheckedChanged" AutoPostBack="True" />
                                </div>
                                <div id="divSucursal" runat="server" style="display: none">
                                    <div class="row">
                                        <div class="col-md-6">
                                            <div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="SucursalCalle" CssClass="col-md-4 control-label">Calle</asp:Label>
                                                <div class="col-md-8">
                                                    <asp:TextBox runat="server" ID="SucursalCalle" CssClass="form-control" />
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="SucursalNoExterior" CssClass="col-md-4 control-label">Número exterior</asp:Label>
                                                <div class="col-md-8">
                                                    <asp:TextBox runat="server" ID="SucursalNoExterior" CssClass="form-control" />
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="SucursalNoInterior" CssClass="col-md-4 control-label">Número interior</asp:Label>
                                                <div class="col-md-8">
                                                    <asp:TextBox runat="server" ID="SucursalNoInterior" CssClass="form-control" />
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="SucursalColonia" CssClass="col-md-4 control-label">Colonia</asp:Label>
                                                <div class="col-md-8">
                                                    <asp:TextBox runat="server" ID="SucursalColonia" CssClass="form-control" />
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="SucursalLocalidad" CssClass="col-md-4 control-label">Localidad</asp:Label>
                                                <div class="col-md-8">
                                                    <asp:TextBox runat="server" ID="SucursalLocalidad" CssClass="form-control" />
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-md-6">
                                            <div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="SucursalReferencia" CssClass="col-md-4 control-label">Referencia</asp:Label>
                                                <div class="col-md-8">
                                                    <asp:TextBox runat="server" ID="SucursalReferencia" CssClass="form-control" />
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="SucursalMunicipio" CssClass="col-md-4 control-label">Municipio</asp:Label>
                                                <div class="col-md-8">
                                                    <asp:TextBox runat="server" ID="SucursalMunicipio" CssClass="form-control" />
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="SucursalEstado" CssClass="col-md-4 control-label">Estado</asp:Label>
                                                <div class="col-md-8">
                                                    <asp:TextBox runat="server" ID="SucursalEstado" CssClass="form-control" />
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="SucursalPais" CssClass="col-md-4 control-label">País</asp:Label>
                                                <div class="col-md-8">
                                                    <asp:TextBox runat="server" ID="SucursalPais" CssClass="form-control" />
                                                    <asp:RequiredFieldValidator ID="rfvSucursalPais" runat="server" ControlToValidate="SucursalPais" Display="Dynamic" Enabled="false"
                                                        CssClass="text-danger" ErrorMessage="El País es obligatorio." ValidationGroup="registrarGroup" />
                                                </div>
                                            </div>
                                            <div class="form-group">
                                                <asp:Label runat="server" AssociatedControlID="SucursalCodigoPostal" CssClass="col-md-4 control-label">Codigo Postal</asp:Label>
                                                <div class="col-md-8">
                                                    <asp:TextBox runat="server" ID="SucursalCodigoPostal" CssClass="form-control numeric" />
                                                    <asp:RegularExpressionValidator runat="server" ControlToValidate="SucursalCodigoPostal" ValidationGroup="registrarGroup" Display="Dynamic"
                                                        CssClass="text-danger" ErrorMessage="El codigo postal debe tener 5 digitos" ValidationExpression="\d{5}" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>--%>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal">Cerrar</button>
                            <asp:Button ID="btnEmisor" runat="server" ValidationGroup="registrarGroup" CssClass="btn btn-primary" Text="Registrar" OnClick="Registrar"></asp:Button>
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="viewEmpresas" EventName="RowCommand" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>

    <div class="modal fade" id="folios" tabindex="-1" role="dialog" aria-hidden="true" aria-labelledby="foliosLabel">
        <div class="modal-dialog modal-lg">
            <asp:UpdatePanel ID="upFolios" runat="server">
                <ContentTemplate>
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                            <h4 class="modal-title" id="foliosLabel">Asignar folios</h4>
                        </div>
                        <div class="modal-body">
                            <div class="form-horizontal">
                                <p class="text-danger">
                                    <asp:Literal runat="server" ID="ErrorFolio" />
                                </p>
                                <asp:HiddenField runat="server" ID="idFolioEmpresa" />
                                <asp:HiddenField runat="server" ID="idFolioInstruccion" />
                                <asp:HiddenField runat="server" ID="idFolio" />
                                <div class="row">
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="Serie" CssClass="col-md-4 control-label">Serie</asp:Label>
                                            <div class="col-md-8">
                                                <asp:TextBox runat="server" ID="Serie" CssClass="form-control" />
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="Serie" Display="Dynamic"
                                                    CssClass="text-danger" ErrorMessage="La serie es obligatoria." ValidationGroup="folioGroup" />
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="FolioInicio" CssClass="col-md-4 control-label">Folio inicio</asp:Label>
                                            <div class="col-md-8">
                                                <asp:TextBox runat="server" ID="FolioInicio" CssClass="form-control" />
                                                <asp:RegularExpressionValidator runat="server" ControlToValidate="FolioInicio" ValidationGroup="folioGroup" Display="Dynamic"
                                                    ErrorMessage="El folio debe de ser numerico." ValidationExpression="^\d*$" CssClass="text-danger"></asp:RegularExpressionValidator>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-md-6">
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="FolioActual" CssClass="col-md-4 control-label">Folio actual</asp:Label>
                                            <div class="col-md-8">
                                                <asp:TextBox runat="server" ID="FolioActual" CssClass="form-control" />
                                                <asp:RegularExpressionValidator runat="server" ControlToValidate="FolioActual" ValidationGroup="folioGroup" Display="Dynamic"
                                                    ErrorMessage="El folio debe de ser numerico." ValidationExpression="^\d*$" CssClass="text-danger"></asp:RegularExpressionValidator>
                                                <asp:CompareValidator runat="server" ControlToValidate="FolioActual" ControlToCompare="FolioInicio" ValidationGroup="folioGroup" Display="Dynamic"
                                                    ErrorMessage="El folio actual debe de ser mayor o igual al folio inicio." Operator="GreaterThanEqual" CssClass="text-danger"></asp:CompareValidator>
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="TipoDocumento" CssClass="col-md-4 control-label">Tipo de documento</asp:Label>
                                            <div class="col-md-8">
                                                <asp:DropDownList runat="server" ID="TipoDocumento" CssClass="form-control">
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="TipoDocumento" Display="Dynamic"
                                                    CssClass="text-danger" ErrorMessage="El tipo de documento es obligatorio." ValidationGroup="folioGroup" />
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <div class="col-md-4">
                                            </div>
                                            <div class="col-md-4">
                                                <asp:LinkButton ID="btnCancelar" Text="<span class='glyphicon glyphicon-remove'></span> Cancelar" runat="server" CssClass="btn btn-default pull-right" OnClick="btnCancelar_Click" />
                                            </div>
                                            <div class="col-md-4">
                                                <asp:LinkButton ID="btnAgregarFolio" Text="<span class='glyphicon glyphicon-plus'></span> Agregar Folio" runat="server" CssClass="btn btn-primary pull-right" ValidationGroup="folioGroup" OnClick="btnAgregarFolio_Click" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <br />
                                <asp:GridView ID="viewFolios" runat="server" CssClass="table table-striped table-bordered table-hover" AutoGenerateColumns="False" DataKeyNames="id" OnRowCommand="viewFolios_RowCommand">
                                    <Columns>
                                        <asp:BoundField DataField="id" HeaderText="Id" Visible="false" />
                                        <asp:BoundField DataField="serie" HeaderText="Serie" />
                                        <asp:BoundField DataField="folio_inicio" HeaderText="Folio inicio" />
                                        <asp:BoundField DataField="folio_actual" HeaderText="Folio actual" />
                                        <asp:BoundField DataField="tipo_comprobante" HeaderText="Tipo" />
                                        <asp:ButtonField Text="<span class='glyphicon glyphicon-pencil'></span>" ItemStyle-ForeColor="Black" CommandName="Editar" />
                                        <asp:ButtonField Text="<span class='glyphicon glyphicon-remove'></span>" ItemStyle-ForeColor="Red" CommandName="Eliminar" />
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
                    <asp:AsyncPostBackTrigger ControlID="viewEmpresas" EventName="RowCommand" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>

    <div class="modal fade" id="cProdServ" tabindex="-1" role="dialog" aria-hidden="true" aria-labelledby="cProdServLabel">
        <div class="modal-dialog modal-lg">
            <asp:UpdatePanel ID="uocProdServ" runat="server">
                <ContentTemplate>
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                            <h4 class="modal-title" id="clavesLabel">Asignar Clave de Producto o Servicio</h4>
                        </div>
                        <div class="modal-body">
                            <div class="form-horizontal">
                                <p class="text-danger">
                                    <asp:Literal runat="server" ID="ErrorClave" />
                                </p>
                                <asp:HiddenField runat="server" ID="idClaveProdServEmpresa" />
                                <asp:HiddenField runat="server" ID="idcProdServInstruccion" />
                                <asp:HiddenField runat="server" ID="idClave" />
                                <div class="row">
                                    <div class="col-md-8">
                                        <div class="form-group">
                                            <asp:Label runat="server" AssociatedControlID="CclaveProdServ" CssClass="col-md-4 control-label">Clave</asp:Label>
                                            <div class="col-md-8">
                                                <asp:DropDownList runat="server" ID="CclaveProdServ" CssClass="form-control">
                                                </asp:DropDownList>
                                                <asp:RequiredFieldValidator runat="server" ControlToValidate="CclaveProdServ" Display="Dynamic"
                                                    CssClass="text-danger" ErrorMessage="El tipo de documento es obligatorio." ValidationGroup="folioGroup" />
                                            </div>
                                        </div>
                                        <div class="form-group">
                                            <div class="col-md-4">
                                            </div>
                                            <div class="col-md-4">
                                                <asp:LinkButton ID="btnAgregarcProdServ" Text="<span class='glyphicon glyphicon-plus'></span> Agregar Clave" runat="server" CssClass="btn btn-primary pull-right" ValidationGroup="cProdServGroup" OnClick="btnAgregarcProdServ_Click" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <br />
                                <asp:GridView ID="viewcProdServ" runat="server" CssClass="table table-striped table-bordered table-hover" AutoGenerateColumns="False" DataKeyNames="cClaveProdServ_id" OnRowCommand="viewcProdServ_RowCommand">
                                    <Columns>
                                        <asp:BoundField DataField="cClaveProdServ_id" HeaderText="Id" Visible="false" />
                                        <asp:BoundField DataField="codigo" HeaderText="Codigo" />
                                        <asp:BoundField DataField="descripcion" HeaderText="Descripcion" />
                                        <asp:ButtonField Text="<span class='glyphicon glyphicon-remove'></span>" ItemStyle-ForeColor="Red" CommandName="Eliminar" />
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
                    <asp:AsyncPostBackTrigger ControlID="viewEmpresas" EventName="RowCommand" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>

    <div class="modal fade" id="reporte" tabindex="-1" role="dialog" aria-hidden="true" aria-labelledby="reporteLabel">
        <div class="modal-dialog">
            <asp:UpdatePanel ID="upReporte" runat="server">
                <ContentTemplate>
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                            <h4 class="modal-title" id="reporteLabel">Seleccionar reporte</h4>
                        </div>
                        <div class="modal-body">
                            <div class="form-horizontal">
                                <p class="text-danger">
                                    <asp:Literal runat="server" ID="ErrorReporte" />
                                </p>
                                <asp:HiddenField runat="server" ID="idReporteEmpresa" />
                                <div class="form-group">
                                    <div class="col-md-12">
                                        <asp:RadioButtonList  ID="rblReporte" runat="server" OnSelectedIndexChanged="rblReporte_SelectedIndexChanged" AutoPostBack="true">
                                            <asp:ListItem Value="1" Text="Corporativo" Selected="True" />
                                            <asp:ListItem Value="2" Text="Corporativo + Origen / contrato" />
                                            <asp:ListItem Value="3" Text="Corporativo + Fecha pago / Tipo Cambio" /> 
                                            <asp:ListItem Value="4" Text="Corporativo + Origen / contrato + Fecha pago / Tipo Cambio" />
                                            <asp:ListItem Value="5" Text="Presidencia" />
                                            <asp:ListItem Value="6" Text="Hoteleria" />
                                            <asp:ListItem Value="7" Text="Persona Fisica" />
                                            <asp:ListItem Value="8" Text="Donataria Fundacion Royal" />
                                            <asp:ListItem Value="9" Text="Reporte sin tipo de cambio" />
                                            <asp:ListItem Value="10" Text="Donataria Balóm por Valor" />
                                        </asp:RadioButtonList>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal">Cerrar</button>
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="viewEmpresas" EventName="RowCommand"  />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>

