<%@ Page Language="C#" MasterPageFile="~/Site.Master" Title="Administración de usuarios" AutoEventWireup="true" CodeBehind="Usuario.aspx.cs" Inherits="Facturador.GHO.Admin.Usuario" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2><%: Title %>.</h2>

    <div class="form-horizontal">
        <h4>Consulta, edición y registro de usuarios.</h4>
        <hr />
        <p class="text-danger">
            <asp:Literal runat="server" ID="ErrorMessage" />
        </p>
        <p class="text-danger">
            <asp:UpdatePanel ID="upCrudGrid" runat="server">

                <ContentTemplate>
                    <button class="btn btn-default btn-primary" data-toggle="modal" data-target="#registro">
                        Agregar usuario
                    </button>
                    <br />
                    <br />
                    <asp:GridView ID="viewUsuarios" runat="server" CssClass="table table-striped table-bordered table-hover" AutoGenerateColumns="False" DataKeyNames="Id" OnRowCommand="viewUsuarios_RowCommand">
                        <Columns>
                            <asp:BoundField DataField="id" HeaderText="IdUsuario" Visible="False" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="ImgPeticion" runat="server" Text='<%# (Eval("IsAdmin").Equals(true) ? "<span class=\"glyphicon glyphicon-star\"></span>" : "")%>' />
                                </ItemTemplate>
                            </asp:TemplateField>
                            <asp:BoundField DataField="UserName" HeaderText="Usuario" />
                            <asp:BoundField DataField="Email" HeaderText="Correo Electrónico" />
                            <asp:BoundField DataField="Phone" HeaderText="Región" />
                            <asp:ButtonField Text="<span class='glyphicon glyphicon-pencil'></span>" ItemStyle-ForeColor="Black" CommandName="Editar" />
                            <asp:ButtonField Text="<span class='glyphicon glyphicon-remove'></span>" ItemStyle-ForeColor="Red" CommandName="Eliminar" />
                        </Columns>
                    </asp:GridView>

                </ContentTemplate>
                <Triggers>
                </Triggers>
            </asp:UpdatePanel>
        </p>
        <div class="modal fade" id="registro" tabindex="-1" role="dialog" aria-hidden="true" aria-labelledby="registroLabel">
            <div class="modal-dialog modal-lg">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                        <h4 class="modal-title" id="registroLabel">Registrar un usuario</h4>
                    </div>
                    <div class="modal-body">
                        <div class="form-horizontal">
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="UserName" CssClass="col-md-4 control-label">Nombre de usuario</asp:Label>
                                <div class="col-md-8">
                                    <asp:TextBox runat="server" ID="UserName" CssClass="form-control" />
                                    <asp:RequiredFieldValidator Display="Dynamic" runat="server" ControlToValidate="UserName"
                                        CssClass="text-danger" ErrorMessage="El nombre de usuario es obligatorio." ValidationGroup="registrarGroup" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="Correo" CssClass="col-md-4 control-label">Correo electrónico</asp:Label>
                                <div class="col-md-8">
                                    <asp:TextBox runat="server" ID="Correo" CssClass="form-control" />
                                    <asp:RequiredFieldValidator Display="Dynamic" runat="server" ControlToValidate="Correo"
                                        CssClass="text-danger" ErrorMessage="El correo electrónico es obligatoria." ValidationGroup="registrarGroup" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="Telefono" CssClass="col-md-4 control-label">Región</asp:Label>
                                <div class="col-md-8">
                                    <asp:TextBox runat="server" ID="Telefono" CssClass="form-control" />
                                </div>
                            </div>
                            <div class="form-group">
                                <asp:Label runat="server" AssociatedControlID="chkAdministrador" CssClass="col-md-4 control-label">Administrador</asp:Label>
                                <div class="col-md-8">
                                    <asp:CheckBox runat="server" ID="chkAdministrador" />
                                </div>
                            </div>
                            <asp:GridView ID="viewEmpresa" runat="server" CssClass="table table-striped table-bordered table-hover" AutoGenerateColumns="False" DataKeyNames="idemisor">
                                <Columns>
                                    <asp:BoundField DataField="idemisor" HeaderText="IdEmpresa" Visible="False" />
                                    <asp:BoundField DataField="identificador" HeaderText="Identificador" />
                                    <asp:BoundField DataField="razon_social" HeaderText="Nombre" />
                                    <asp:BoundField DataField="rfc" HeaderText="RFC" />
                                    <asp:TemplateField>
                                        <ItemTemplate>
                                            <asp:CheckBox ID="chkPermiso" runat="server" />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-dismiss="modal">Cerrar</button>
                        <asp:Button runat="server" ValidationGroup="registrarGroup" CssClass="btn btn-primary" Text="Registrar" OnClick="CreateUsuarios_Click"></asp:Button>
                    </div>
                </div>
            </div>
        </div>
        <div class="modal fade" id="editar" tabindex="-1" role="dialog" aria-hidden="true" aria-labelledby="editarLabel">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                        <h4 class="modal-title" id="editarLabel">Editar usuario</h4>
                    </div>
                    <div class="modal-body">
                        <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                            <ContentTemplate>
                                <div class="form-horizontal">
                                    <asp:HiddenField runat="server" ID="idEditar" />
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="EditarCorreo" CssClass="col-md-4 control-label">Correo electrónico</asp:Label>
                                        <div class="col-md-8">
                                            <asp:TextBox runat="server" ID="EditarCorreo" CssClass="form-control" />
                                            <asp:RequiredFieldValidator Display="Dynamic" runat="server" ControlToValidate="EditarCorreo"
                                                CssClass="text-danger" ErrorMessage="El correo electrónico es obligatoria." ValidationGroup="editarGroup" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="EditarTelefono" CssClass="col-md-4 control-label">Región</asp:Label>
                                        <div class="col-md-8">
                                            <asp:TextBox runat="server" ID="EditarTelefono" CssClass="form-control" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="chkEditAdministrador" CssClass="col-md-4 control-label">Administrador</asp:Label>
                                        <div class="col-md-8">
                                            <asp:CheckBox runat="server" ID="chkEditAdministrador" />
                                        </div>
                                    </div>
                                    <asp:GridView ID="viewEditarEmpresa" runat="server" CssClass="table table-striped table-bordered table-hover" AutoGenerateColumns="False" DataKeyNames="idemisor">
                                        <Columns>
                                            <asp:BoundField DataField="idemisor" HeaderText="IdEmpresa" Visible="False" />
                                            <asp:BoundField DataField="identificador" HeaderText="Identificador" />
                                            <asp:BoundField DataField="razon_social" HeaderText="Nombre" />
                                            <asp:BoundField DataField="rfc" HeaderText="RFC" />
                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:CheckBox ID="chkPermiso" Checked='<%#Eval("permisos") %>' runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="viewUsuarios" EventName="RowCommand" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-dismiss="modal">Cerrar</button>
                        <asp:Button runat="server" CssClass="btn btn-primary" Text="Guardar" OnClick="EditUsuarios_Click" ValidationGroup="editarGroup"></asp:Button>
                    </div>
                </div>
            </div>
        </div>
        <div class="modal fade" id="eliminar" tabindex="-1" role="dialog" aria-hidden="true" aria-labelledby="eliminarLabel">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                        <h4 class="modal-title" id="eliminarLabel">Eliminar usuario</h4>
                    </div>
                    <div class="modal-body">
                        <div class="form-horizontal">
                            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                <ContentTemplate>
                                    <asp:HiddenField runat="server" ID="idEliminar" />
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="EliminarCorreo" CssClass="col-md-4 control-label">Correo electrónico</asp:Label>
                                        <div class="col-md-8">
                                            <asp:TextBox runat="server" ID="EliminarCorreo" CssClass="form-control" />
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <asp:Label runat="server" AssociatedControlID="EliminarTelefono" CssClass="col-md-4 control-label">Región</asp:Label>
                                        <div class="col-md-8">
                                            <asp:TextBox runat="server" ID="EliminarTelefono" CssClass="form-control" />
                                        </div>
                                    </div>
                                </ContentTemplate>
                                <Triggers>
                                    <asp:AsyncPostBackTrigger ControlID="viewUsuarios" EventName="RowCommand" />
                                </Triggers>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-dismiss="modal">Cerrar</button>
                        <asp:Button runat="server" CausesValidation="false" CssClass="btn btn-danger" Text="Eliminar" OnClick="DeleteUsuarios_Click"></asp:Button>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

