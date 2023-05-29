<%@ Page Title="Monedas" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Moneda.aspx.cs" Inherits="Facturador.GHO.Admin.Moneda" %>

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
        <asp:UpdatePanel ID="upMonedas" runat="server">
            <ContentTemplate>
                <p class="text-danger">
                    <asp:Literal runat="server" ID="ErrorMessage" />
                </p>

                <asp:Button runat="server" ID="btnRegistrar" CssClass="btn btn-primary" Text="Agregar moneda" OnClick="btnRegistrar_Click"></asp:Button>
                <br></br>
                <asp:GridView ID="viewMonedas" runat="server" CssClass="table table-striped table-bordered table-hover" AutoGenerateColumns="False" DataKeyNames="id" OnRowCommand="viewMonedas_RowCommand">
                    <Columns>
                        <asp:BoundField DataField="id" HeaderText="id" Visible="False" />
                        <asp:BoundField DataField="nombre" HeaderText="Nombre" />
                        <asp:BoundField DataField="tipo_cambio" HeaderText="Tipo de cambio" />
                        <asp:BoundField DataField="abreviatura" HeaderText="Abreviatura" />
                        <asp:ButtonField Text="<span class='glyphicon glyphicon-pencil'></span>" ItemStyle-ForeColor="Black" CommandName="Editar" />
                        <asp:ButtonField Text="<span class='glyphicon glyphicon-remove'></span>" ItemStyle-ForeColor="Red" CommandName="Eliminar" />
                    </Columns>
                </asp:GridView>

            </ContentTemplate>
            <Triggers>
                <asp:PostBackTrigger ControlID="btnMoneda" />
            </Triggers>
        </asp:UpdatePanel>
    </div>

    <div class="modal fade" id="registro" tabindex="-1" role="dialog" aria-hidden="true" aria-labelledby="registroLabel">
        <div class="modal-dialog modal-sm">
            <asp:UpdatePanel ID="upRegistrar" runat="server">
                <ContentTemplate>
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                            <h4 class="modal-title" id="registroLabel">
                                <asp:Label runat="server" ID="titleMoneda" Text="" /></h4>
                        </div>
                        <div class="modal-body">
                            <div class="form-horizontal">
                                <asp:HiddenField runat="server" ID="idMoneda" />
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="Nombre" CssClass="col-md-4 control-label">Moneda</asp:Label>
                                    <div class="col-md-8">
                                        <asp:TextBox runat="server" ID="Nombre" CssClass="form-control" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="Nombre" Display="Dynamic"
                                            CssClass="text-danger" ErrorMessage="La moneda es obligatoria." ValidationGroup="registrarGroup" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="TipoCambio" CssClass="col-md-4 control-label">Tipo de cambio</asp:Label>
                                    <div class="col-md-8">
                                        <asp:TextBox runat="server" ID="TipoCambio" CssClass="form-control numeric" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="TipoCambio" Display="Dynamic"
                                            CssClass="text-danger" ErrorMessage="El tipo de cambio es obligatorio." ValidationGroup="registrarGroup" />
                                    </div>
                                </div>
                                <div class="form-group">
                                    <asp:Label runat="server" AssociatedControlID="Abreviatura" CssClass="col-md-4 control-label">Abreviatura</asp:Label>
                                    <div class="col-md-8">
                                        <asp:TextBox runat="server" ID="Abreviatura" CssClass="form-control" />
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="Abreviatura" Display="Dynamic"
                                            CssClass="text-danger" ErrorMessage="La abreviatura es obligatoria." ValidationGroup="registrarGroup" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal">Cerrar</button>
                            <asp:Button ID="btnMoneda" runat="server" ValidationGroup="registrarGroup" CssClass="btn btn-primary" Text="Registrar" OnClick="btnMoneda_Click"></asp:Button>
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="viewMonedas" EventName="RowCommand" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>
