<%@ Page Title="Clientes" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Cliente.aspx.cs" Inherits="Facturador.GHO.Usuario.Cliente" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="../Content/jQuery.FileUpload/css/jquery.fileupload.css" rel="stylesheet" />
    <h2>Mis clientes</h2>
    <hr />
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
    </script>
    <div>
        <!-- Empresa -->
        <div class="row">
            <div class="col-md-12">
                <asp:Label runat="server" AssociatedControlID="Empresa" CssClass="col-md-2 control-label">Empresa:</asp:Label>
                <asp:DropDownList ID="Empresa" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="Empresa_SelectedIndexChanged">
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="Empresa" ValidationGroup="vgFacturar" Display="Dynamic"
                    CssClass="text-danger" ErrorMessage="La empresa es obligatoria." />
            </div>
        </div>
       
        <asp:UpdatePanel ID="upEmpresas" runat="server">
            <ContentTemplate>
                <p class="text-danger">
                    <asp:Literal runat="server" ID="ErrorMessage" />
                </p>
                 <div class="form-group">
                   <div class="col-md-2 marginpadding">
                       <asp:Button runat="server" ID="btnRegistrar" CssClass="btn btn-primary" Text="Nuevo Cliente" OnClick="btnRegistrar_Click"></asp:Button>
                   </div>
                   <div class="col-md-2 marginpadding" style="padding-left: 13px;">
                       <span class="btn btn-success fileinput-button">
                           <i class="glyphicon glyphicon-plus"></i>
                           <span>Lista clientes.</span>
                           <input id="file1" type="file" name="fileUpload" accept="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet xlsx" runat="server" />
                       </span>
                   </div>
                   <div class="col-md-2 marginpadding">
                       <asp:Button ID="Importar" runat="server" CssClass="btn btn-default" Text="Importar lista" OnClick="Importar_Click" />
                   </div>
                 </div>
                <br></br>
                <asp:GridView ID="viewEmpresas" runat="server" CssClass="table table-striped table-bordered table-hover" AutoGenerateColumns="False" DataKeyNames="idreceptor" OnRowCommand="viewEmpresas_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="idreceptor" HeaderText="idreceptor" Visible="False" HeaderStyle-BackColor="#3d618f" HeaderStyle-ForeColor="White" />
                        <asp:BoundField DataField="identificador" HeaderText="Identificador" HeaderStyle-BackColor="#3d618f" HeaderStyle-ForeColor="White" />
                        <asp:BoundField DataField="razon_social" HeaderText="Razon Social" HeaderStyle-BackColor="#3d618f" HeaderStyle-ForeColor="White" />
                        <asp:BoundField DataField="rfc" HeaderText="RFC" HeaderStyle-BackColor="#3d618f" HeaderStyle-ForeColor="White" />
                        <asp:BoundField DataField="curp" HeaderText="CURP" HeaderStyle-BackColor="#3d618f" HeaderStyle-ForeColor="White"/>
                        <asp:ButtonField Text="<span class='glyphicon glyphicon-pencil'></span>" HeaderStyle-BackColor="#3d618f" HeaderStyle-ForeColor="White" ItemStyle-ForeColor="Black" CommandName="Editar" />
                        <asp:ButtonField Text="<span class='glyphicon glyphicon-remove'></span>" HeaderStyle-BackColor="#3d618f" HeaderStyle-ForeColor="White" ItemStyle-ForeColor="Red" CommandName="Eliminar" />
                    </Columns>
                </asp:GridView>

            </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="btnEmisor" />
                <asp:AsyncPostBackTrigger ControlID="Empresa" />
                <asp:PostBackTrigger ControlID="file1" />
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
                            <h4>Añadir nuevo cliente</h4>
                        </div>
                        <div class="modal-body">
                            <div class="form-horizontal">
                                <asp:HiddenField runat="server" ID="idEmisor" />
                                <div class="row">
                                    <!-- Identificador -->
                                    <div class="col-md-4">
                                        <asp:Label runat="server" AssociatedControlID="Identificador" CssClass="control-label">Cliente</asp:Label>
                                        <asp:TextBox runat="server" ID="Identificador" CssClass="form-control" placeholder="Alias o identificador"/>
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="Identificador" Display="Dynamic"
                                            CssClass="text-danger" ErrorMessage="El identificador es obligatorio." ValidationGroup="registrarGroup" />
                                    </div>
                                    <!-- Razón social -->
                                    <div class="col-md-5">
                                        <asp:Label runat="server" AssociatedControlID="RazonSocial" CssClass="control-label">Razón social</asp:Label>
                                        <asp:TextBox runat="server" ID="RazonSocial" CssClass="form-control" placeholder="Nombre o razón social"/>
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="RazonSocial" Display="Dynamic"
                                            CssClass="text-danger" ErrorMessage="la razón social es obligatoria." ValidationGroup="registrarGroup" />
                                    </div>
                                    <!-- RFC -->
                                    <div class="col-md-3">
                                        <asp:Label runat="server" AssociatedControlID="RFC" CssClass="control-label">RFC</asp:Label>
                                        <asp:TextBox runat="server" ID="RFC" CssClass="form-control" placeholder="XAXX010101000"/>
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="RFC" Display="Dynamic"
                                            CssClass="text-danger" ErrorMessage="El RFC es obligatorio." ValidationGroup="registrarGroup" />
                                        <asp:RegularExpressionValidator ID="revRFC" runat="server" ErrorMessage="El RFC no es valido" Display="Dynamic"
                                            CssClass="text-danger" ValidationGroup="registrarGroup" ControlToValidate="RFC"
                                            ValidationExpression="[A-Z,Ñ,&]{3,4}[0-9]{2}[0-1][0-9][0-3][0-9][A-Z,0-9]?[A-Z,0-9]?[0-9,A-Z]?"></asp:RegularExpressionValidator>
                                    </div>
                                    <!-- CURP -->
                                    <div class="col-md-3">
                                        <asp:UpdatePanel ID="updFormularioCliente" runat="server">
                                            <ContentTemplate>
                                                <div id="divClienteNacional" runat="server">
                                                    <asp:Label runat="server" AssociatedControlID="CURP" CssClass="control-label">CURP</asp:Label>
                                                    <asp:TextBox runat="server" ID="CURP" MaxLength="18" CssClass="form-control" placeholder="XAXX010101000XXXXX"/>
                                                    <asp:RegularExpressionValidator ID="revCURP" runat="server" ErrorMessage="El CURP no es valido" Display="Dynamic"
                                                        CssClass="text-danger" ValidationGroup="registrarGroup" ControlToValidate="CURP"
                                                        ValidationExpression="([A-Z][A,E,I,O,U,X][A-Z]{2}[0-9]{2}[0-1][0-9][0-3][0-9][M,H][A-Z]{2}[B,C,D,F,G,H,J,K,L,M,N,Ñ,P,Q,R,S,T,V,W,X,Y,Z]{3}[0-9,A-Z][0-9])?"></asp:RegularExpressionValidator>
                                                </div>
                                                <div runat="server" id="divClienteExtranjero" class="form-group" visible="false">
                                                    <asp:Label runat="server" AssociatedControlID="txbNumRegIdentFiscalCliente" CssClass="control-label">Num de registro de id fiscal </asp:Label>
                                                    <asp:TextBox ID="txbNumRegIdentFiscalCliente" runat="server" CssClass="form-control" MaxLength="20"></asp:TextBox>
                                                </div>
                                            </ContentTemplate>
                                            <%--<Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="ddlNacionalidad" />
                                            </Triggers>--%>
                                        </asp:UpdatePanel>
                                    </div>
                                    <!-- Calle -->
                                    <div class="col-md-4">
                                        <asp:Label runat="server" AssociatedControlID="Calle" CssClass="control-label">Calle</asp:Label>
                                        <asp:TextBox runat="server" ID="Calle" CssClass="form-control" placeholder="Nombre calle o avenida"/>
                                    </div>
                                    <!-- No. Exterior -->
                                    <div class="col-md-2">
                                        <asp:Label runat="server" AssociatedControlID="NoExterior" CssClass="control-label">No ext</asp:Label>
                                        <asp:TextBox runat="server" ID="NoExterior" CssClass="form-control" placeholder="Ej. 5"/>
                                    </div>
                                    <!-- No. Interior -->
                                    <div class="col-md-2">
                                        <asp:Label runat="server" AssociatedControlID="NoInterior" CssClass="control-label">No int</asp:Label>
                                        <asp:TextBox runat="server" ID="NoInterior" CssClass="form-control" placeholder="Ej. A"/>
                                    </div>
                                    <!-- Localidad -->
                                    <div class="col-md-3">
                                        <asp:Label runat="server" AssociatedControlID="Localidad" CssClass="control-label">Localidad</asp:Label>
                                        <asp:TextBox runat="server" ID="Localidad" CssClass="form-control" />
                                    </div>
                                    <!-- Colonia -->
                                    <div class="col-md-3">
                                        <asp:Label runat="server" AssociatedControlID="Colonia" CssClass="control-label">Colonia</asp:Label>
                                        <asp:TextBox runat="server" ID="Colonia" CssClass="form-control" />
                                    </div>
                                    <!-- Municipio -->
                                    <div class="col-md-3">
                                        <asp:Label runat="server" AssociatedControlID="Municipio" CssClass="control-label">Municipio</asp:Label>
                                        <asp:TextBox runat="server" ID="Municipio" CssClass="form-control" />
                                    </div>
                                    <!-- Estado -->
                                    <div class="col-md-3">
                                        <asp:Label runat="server" AssociatedControlID="Estado" CssClass="control-label">Estado</asp:Label>
                                        <asp:TextBox runat="server" ID="Estado" CssClass="form-control" />
                                    </div>
                                    <!-- País -->
                                    <div class="col-md-3">
                                        <asp:Label runat="server" AssociatedControlID="Pais" CssClass="control-label">País</asp:Label>
                                        <asp:TextBox runat="server" ID="Pais" CssClass="form-control" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="Pais" Display="Dynamic"
                                            CssClass="text-danger" ErrorMessage="El País es obligatorio." ValidationGroup="registrarGroup" />
                                    </div>
                                    <!-- Código Postal -->
                                    <div class="col-md-2">
                                        <asp:Label runat="server" AssociatedControlID="CodigoPostal" CssClass="control-label">C.P.</asp:Label>
                                        <asp:TextBox runat="server" ID="CodigoPostal" CssClass="form-control numeric" placeholder="00000"/>
                                        <asp:RegularExpressionValidator runat="server" ControlToValidate="CodigoPostal" ValidationGroup="registrarGroup" Display="Dynamic"
                                            CssClass="text-danger" ErrorMessage="El codigo postal debe tener 5 digitos" ValidationExpression="\d{5}" />
                                    </div>
                                    <!-- Referencias -->
                                    <div class="col-md-6">
                                        <asp:Label runat="server" AssociatedControlID="Referencia" CssClass="control-label">Referencias</asp:Label>
                                        <asp:TextBox runat="server" ID="Referencia" CssClass="form-control" placeholder="Entre calles, edificio, fachada, etc."/>
                                    </div>
                                    <div class="col-md-3">
                                        <div class="form-group">
                                            <%--<asp:Label runat="server" AssociatedControlID="ddlNacionalidad" CssClass="col-md-4 control-label">Nacionalidad </asp:Label>
                                            <div class="col-md-8">
                                                <asp:DropDownList ID="ddlNacionalidad" runat="server" AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="ddlNacionalidad_SelectedIndexChanged"></asp:DropDownList>
                                                <asp:RequiredFieldValidator ID="rfvddlNacionalidad" runat="server" ControlToValidate="ddlNacionalidad" CssClass="text-danger" ValidationGroup="vgFacturar" Display="Dynamic" ErrorMessage="La Nacionalidad es obligatoria."></asp:RequiredFieldValidator>
                                            </div>--%>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="btnEmisor" runat="server" ValidationGroup="registrarGroup" CssClass="btn btn-primary" Text="Registrar" OnClick="btnEmisor_Click"></asp:Button>
                            <button type="button" class="btn btn-danger" data-dismiss="modal"> <i class="fas fa-times"></i> Cerrar</button>
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="viewEmpresas" EventName="RowCommand" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>
