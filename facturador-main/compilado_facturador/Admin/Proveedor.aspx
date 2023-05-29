<%@ Page Title="Proveedores" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Proveedor.aspx.cs" Inherits="Facturador.GHO.Admin.Proveedor" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2><%: Title %>.</h2>

    <div class="form-horizontal">
        <hr />
        <p class="text-danger">
            <asp:Literal runat="server" ID="ErrorMessage" />
            <asp:UpdatePanel ID="upCrudGrid" runat="server">

                <ContentTemplate>
                    <asp:GridView ID="viewProveedores" runat="server" CssClass="table" AutoGenerateColumns="False" DataKeyNames="idproveedor" OnRowCommand="viewProveedores_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="idproveedor" HeaderText="idproveedor" Visible="False" />
                            <asp:BoundField DataField="rfc" HeaderText="RFC" />
                            <asp:BoundField DataField="razon_social" HeaderText="Razon Social" />
                            <asp:BoundField DataField="correo_electronico" HeaderText="Correo Electrónico" />
                            <asp:BoundField DataField="contacto" HeaderText="Contacto" />
                            <asp:BoundField DataField="telefono" HeaderText="Telefono" />
                            <asp:ButtonField Text="Editar" CommandName="Editar" />
                            <asp:ButtonField Text="Eliminar" CommandName="Eliminar" />
                        </Columns>
                    </asp:GridView>
                    <button class="btn btn-default" data-toggle="modal" data-target="#registro">
                        Agregar proveedor
                    </button>

                </ContentTemplate>
                <Triggers>
                </Triggers>
            </asp:UpdatePanel>
        </p>
        <div class="modal fade" id="registro" tabindex="-1" role="dialog" aria-hidden="true" aria-labelledby="registroLabel">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                        <h4 class="modal-title" id="registroLabel">Registrar un proveedor</h4>
                    </div>
                    <div class="modal-body">
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="RazonSocial" CssClass="col-md-2 control-label">Razon social</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="RazonSocial" CssClass="form-control" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="RazonSocial"
                                    CssClass="text-danger" ErrorMessage="la razón social es obligatoria." ValidationGroup="registrarGroup" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="RFC" CssClass="col-md-2 control-label">RFC</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="RFC" CssClass="form-control" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="RFC"
                                    CssClass="text-danger" ErrorMessage="El RFC es obligatorio." ValidationGroup="registrarGroup" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="Correo" CssClass="col-md-2 control-label">Correo electrónico</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="Correo" CssClass="form-control" />
                                <asp:RequiredFieldValidator runat="server" ControlToValidate="Correo"
                                    CssClass="text-danger" ErrorMessage="El correo electrónico es obligatoria." ValidationGroup="registrarGroup" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="Contacto" CssClass="col-md-2 control-label">Contacto</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="Contacto" CssClass="form-control" />
                            </div>
                        </div>
                        <div class="form-group">
                            <asp:Label runat="server" AssociatedControlID="Telefono" CssClass="col-md-2 control-label">Telefono</asp:Label>
                            <div class="col-md-10">
                                <asp:TextBox runat="server" ID="Telefono" CssClass="form-control" />
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-dismiss="modal">Cerrar</button>
                        <asp:Button runat="server" ValidationGroup="registrarGroup" CssClass="btn btn-primary" Text="Registrar" OnClick="Registrar"></asp:Button>
                    </div>
                </div>
            </div>
        </div>
        <div class="modal fade" id="editar" tabindex="-1" role="dialog" aria-hidden="true" aria-labelledby="editarLabel">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                        <h4 class="modal-title" id="editarLabel">Editar proveedor</h4>
                    </div>
                    <div class="modal-body">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <asp:HiddenField runat="server" ID="idEditar" />
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="EditarRazonSocial" CssClass="col-md-2 control-label">Razon social</asp:Label>
                                    <div class="col-md-10">
                                        <asp:TextBox runat="server" ID="EditarRazonSocial" CssClass="form-control" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="EditarRazonSocial"
                                            CssClass="text-danger" ErrorMessage="la razón social es obligatoria." ValidationGroup="editarGroup" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="EditarRFC" CssClass="col-md-2 control-label">RFC</asp:Label>
                                    <div class="col-md-10">
                                        <asp:TextBox runat="server" ID="EditarRFC" CssClass="form-control" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="EditarRFC"
                                            CssClass="text-danger" ErrorMessage="El RFC es obligatorio." ValidationGroup="editarGroup" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="EditarCorreo" CssClass="col-md-2 control-label">Correo electrónico</asp:Label>
                                    <div class="col-md-10">
                                        <asp:TextBox runat="server" ID="EditarCorreo" CssClass="form-control" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="EditarCorreo"
                                            CssClass="text-danger" ErrorMessage="El correo electrónico es obligatoria." ValidationGroup="editarGroup" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="EditarContacto" CssClass="col-md-2 control-label">Contacto</asp:Label>
                                    <div class="col-md-10">
                                        <asp:TextBox runat="server" ID="EditarContacto" CssClass="form-control" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="EditarTelefono" CssClass="col-md-2 control-label">Telefono</asp:Label>
                                    <div class="col-md-10">
                                        <asp:TextBox runat="server" ID="EditarTelefono" CssClass="form-control" />
                                    </div>
                                </div>
                                </div>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="viewProveedores" EventName="RowCommand" />
                            </Triggers>
                        </asp:UpdatePanel>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal">Cerrar</button>
                            <asp:Button runat="server" CssClass="btn btn-primary" Text="Guardar" OnClick="Editar" ValidationGroup="editarGroup"></asp:Button>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal fade" id="eliminar" tabindex="-1" role="dialog" aria-hidden="true" aria-labelledby="eliminarLabel">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                            <h4 class="modal-title" id="eliminarLabel">Eliminar proveedor</h4>
                        </div>
                        <div class="modal-body">
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                <ContentTemplate>
                                    <asp:HiddenField runat="server" ID="idEliminar" />
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="EliminarRazonSocial" CssClass="col-md-2 control-label">Razon social</asp:Label>
                                        <div class="col-md-10">
                                            <asp:TextBox runat="server" ID="EliminarRazonSocial" CssClass="form-control" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="EliminarRFC" CssClass="col-md-2 control-label">RFC</asp:Label>
                                        <div class="col-md-10">
                                            <asp:TextBox runat="server" ID="EliminarRFC" CssClass="form-control" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="EliminarCorreo" CssClass="col-md-2 control-label">Correo electrónico</asp:Label>
                                        <div class="col-md-10">
                                            <asp:TextBox runat="server" ID="EliminarCorreo" CssClass="form-control" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="EliminarContacto" CssClass="col-md-2 control-label">Contacto</asp:Label>
                                        <div class="col-md-10">
                                            <asp:TextBox runat="server" ID="EliminarContacto" CssClass="form-control" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="EliminarTelefono" CssClass="col-md-2 control-label">Telefono</asp:Label>
                                        <div class="col-md-10">
                                            <asp:TextBox runat="server" ID="EliminarTelefono" CssClass="form-control" />
                                        </div>
                                    </div>
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="viewProveedores" EventName="RowCommand" />
                                </Triggers>
                            </asp:UpdatePanel>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal">Cerrar</button>
                            <asp:Button runat="server" CausesValidation="false" CssClass="btn btn-primary" Text="Eliminar" OnClick="Eliminar"></asp:Button>
                        </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

