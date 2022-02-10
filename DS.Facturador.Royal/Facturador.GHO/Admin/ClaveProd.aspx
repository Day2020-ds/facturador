<%@ Page Title="Claves Productos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ClaveProd.aspx.cs" Inherits="Facturador.GHO.Admin.ClaveProd" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>.</h2>
    <script type="text/javascript">
        $(document).on('keypress keyup blur', '.numeric', function (event) {
            $(this).val($(this).val().replace(/[^0-9\.]/g, ''));
            if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57)) {
                event.preventDefault();
            }
        });
    </script>
    <div class="form-horizontal">
        <asp:UpdatePanel ID="upClavesProd" runat="server">
            <ContentTemplate>
                <p class="text-danger">
                    <asp:Literal runat="server" ID="ErrorMessage" />
                </p>

                <asp:Button runat="server" ID="btnRegistrar" CssClass="btn btn-primary" Text="Agregar Clave Producto" OnClick="btnRegistrar_Click"></asp:Button>
                <br></br>
                <asp:GridView ID="viewClavesProd" runat="server" CssClass="table table-striped table-bordered table-hover" AutoGenerateColumns="False" DataKeyNames="cClaveProdServ_id" OnRowCommand="viewClavesProd_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="cClaveProdServ_id" HeaderText="id" Visible="False" />
                        <asp:BoundField DataField="codigo" HeaderText="Codigo" />
                        <asp:BoundField DataField="descripcion" HeaderText="Descripción" />
                        <%--<asp:BoundField DataField="abreviatura" HeaderText="Abreviatura" />--%>
                        <asp:ButtonField Text="<span class='glyphicon glyphicon-pencil'></span>" ItemStyle-ForeColor="Black" CommandName="Editar" />
                        <asp:ButtonField Text="<span class='glyphicon glyphicon-remove'></span>" ItemStyle-ForeColor="Red" CommandName="Eliminar" />
                    </Columns>
                </asp:GridView>

            </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="btnClavesProd" />
            </Triggers>
        </asp:UpdatePanel>
    </div>

    <div class="modal fade" id="registro" tabindex="-1" role="dialog" aria-hidden="true" aria-labelledby="registroLabel">
        <div class="modal-dialog modal-md">
            <asp:UpdatePanel ID="upRegistrar" runat="server">
                <ContentTemplate>
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                            <h4 class="modal-title" id="registroLabel">
                                <asp:Label runat="server" ID="titleClaveProd" Text="" /></h4>
                        </div>
                        <div class="modal-body">
                            <div class="form-horizontal">
                                <asp:HiddenField runat="server" ID="cClaveProdServ_id" />
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="Codigo" CssClass="col-md-4 control-label">Clave Producto</asp:Label>
                                    <div class="col-md-8">
                                        <asp:TextBox runat="server" ID="Codigo" CssClass="form-control" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="Codigo" Display="Dynamic"
                                            CssClass="text-danger" ErrorMessage="El codigo es obligatorio." ValidationGroup="registrarGroup" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="Descripcion" CssClass="col-md-4 control-label">Descripcion</asp:Label>
                                    <div class="col-md-8">
                                        <asp:TextBox runat="server" ID="Descripcion" CssClass="form-control" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="Descripcion" Display="Dynamic"
                                            CssClass="text-danger" ErrorMessage="La descripcion es obligatoria." ValidationGroup="registrarGroup" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal">Cerrar</button>
                            <asp:Button ID="btnClavesProd" runat="server" ValidationGroup="registrarGroup" CssClass="btn btn-primary" Text="Registrar" OnClick="btnClavesProd_Click"></asp:Button>
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="viewClavesProd" EventName="RowCommand" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>
