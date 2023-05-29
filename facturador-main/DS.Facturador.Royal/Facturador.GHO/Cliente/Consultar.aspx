<%@ Page Title="Consultar" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Consultar.aspx.cs" Inherits="Facturador.GHO.Cliente.Consultar" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <script type="text/javascript">
        $(function () {
            $(".numeric").on("keypress keyup blur", function (event) {
                $(this).val($(this).val().replace(/[^0-9\.]/g, ''));
                if ((event.which != 46 || $(this).val().indexOf('.') != -1) && (event.which < 48 || event.which > 57)) {
                    event.preventDefault();
                }
            });
        });
    </script>
    <h2><%: Title %>.</h2>
    <p class="text-danger">
        <asp:Literal runat="server" ID="ErrorMessage" />
    </p>

    <div class="form-horizontal">
        <hr />
        <!-- Nav tabs -->
        <ul class="nav nav-tabs">
            <li class="active"><a href="#buscar_uuid" data-toggle="tab">Folio Fiscal</a></li>
            <li><a href="#buscar_ticket" data-toggle="tab">Ticket</a></li>
        </ul>

        <!-- Tab panes -->
        <div class="tab-content">
            <div class="tab-pane active" id="buscar_uuid">
                <h4>Buscar por UUID</h4>
                <br />
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="UUID" CssClass="col-md-2 control-label">UUID</asp:Label>
                    <div class="col-md-10">
                        <asp:TextBox runat="server" ID="UUID" CssClass="form-control" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="UUID"
                            CssClass="text-danger" ValidationGroup="consultaUUID" ErrorMessage="El Folio Fiscal es obligatorio." />
                    </div>
                </div>
                <div class="form-group">
                    <div class="col-md-offset-2 col-md-10">
                        <asp:Button runat="server" Text="Consultar" ValidationGroup="consultaUUID" CssClass="btn btn-primary" OnClick="ConsultarUUID" />
                    </div>
                </div>
            </div>
            <div class="tab-pane" id="buscar_ticket">
                <h4>Buscar por ticket</h4>
                <br />
                <div class="form-horizontal">
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="Empresa" CssClass="col-md-2 control-label">Empresa</asp:Label>
                        <div class="col-md-10">
                            <asp:DropDownList ID="Empresa" runat="server" CssClass="form-control">
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="Empresa"
                                CssClass="text-danger" ErrorMessage="La empresa es obligatoria." ValidationGroup="consultar" />
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="Ticket" CssClass="col-md-2 control-label">No. de ticket</asp:Label>
                        <div class="col-md-10">
                            <asp:TextBox runat="server" ID="Ticket" CssClass="form-control" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="Ticket"
                                CssClass="text-danger" ErrorMessage="El No. de Ticket es obligatorio." ValidationGroup="consultar" />
                        </div>
                    </div>
                    <div class="form-group">
                        <asp:Label runat="server" AssociatedControlID="Total" CssClass="col-md-2 control-label">Total</asp:Label>
                        <div class="col-md-10">
                            <asp:TextBox runat="server" ID="Total" CssClass="form-control numeric" />
                            <asp:RequiredFieldValidator runat="server" ControlToValidate="Total"
                                CssClass="text-danger" ErrorMessage="El Total es obligatorio." ValidationGroup="consultar" />
                        </div>
                    </div>
                    <div class="form-group">
                        <div class="col-md-offset-2 col-md-10">
                            <asp:Button runat="server" Text="Consultar" ValidationGroup="consultar" CssClass="btn btn-primary" OnClick="ConsultarTicket" />
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
