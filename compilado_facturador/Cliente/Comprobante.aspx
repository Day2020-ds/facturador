<%@ Page Title="Factura" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Comprobante.aspx.cs" Inherits="Facturador.GHO.Cliente.Comprobante" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <h2><%: Title %>.</h2>

   <%-- <asp:UpdatePanel ID="upCrudGrid" runat="server">

        <ContentTemplate>--%>
            <p class="text-danger">
                <asp:Literal runat="server" ID="ErrorMessage" />
            </p>

            <div class="form-horizontal">
                <hr />
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="UUID" CssClass="col-md-2 control-label">UUID</asp:Label>
                    <div class="col-md-6">
                        <asp:Label runat="server" CssClass="form-control" ID="UUID"></asp:Label>
                    </div>
                </div>
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="Ticket" CssClass="col-md-2 control-label">No. de ticket</asp:Label>
                    <div class="col-md-6">
                        <asp:Label runat="server" ID="Ticket" CssClass="form-control"></asp:Label>
                    </div>
                </div>
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="EmisorRFC" CssClass="col-md-2 control-label">RFC Emisor</asp:Label>
                    <div class="col-md-6">
                        <asp:Label runat="server" ID="EmisorRFC" CssClass="form-control"></asp:Label>
                    </div>
                </div>
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="ReceptorRFC" CssClass="col-md-2 control-label">RFC Receptor</asp:Label>
                    <div class="col-md-6">
                        <asp:Label runat="server" ID="ReceptorRFC" CssClass="form-control"></asp:Label>
                    </div>
                </div>
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="Subtotal" CssClass="col-md-2 control-label">Subtotal</asp:Label>
                    <div class="col-md-6">
                        <asp:Label runat="server" ID="Subtotal" CssClass="form-control"></asp:Label>
                    </div>
                </div>
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="Total" CssClass="col-md-2 control-label">Total</asp:Label>
                    <div class="col-md-6">
                        <asp:Label runat="server" ID="Total" CssClass="form-control"></asp:Label>
                    </div>
                </div>
                <div class="form-group">
                    <div></div>
                    <div class="col-md-offset-2 col-md-10">
                        <asp:LinkButton runat="server" Text="<span class='glyphicon glyphicon-download'></span> XML" CssClass="btn btn-default" ID="btnXML" OnClick="DescargarXML" />

                        <asp:LinkButton runat="server" Text="<span class='glyphicon glyphicon-download'></span> PDF" CssClass="btn btn-default" ID="btnPDF" OnClick="DescargarPDF" />
                    </div>
                </div>
        <%--</ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnXML" />
            <asp:AsyncPostBackTrigger ControlID="btnPDF" />
        </Triggers>
    </asp:UpdatePanel>--%>
    </div>

    <div class="form-horizontal">
        <hr />
        <div class="row">
            <div class="col-md-6">
                <asp:Label runat="server" AssociatedControlID="Correo" CssClass="col-md-5 control-label">Correo Electrónico</asp:Label>
                <div class="col-md-7">
                    <asp:TextBox runat="server" ID="Correo" CssClass="form-control"></asp:TextBox>
                </div>
            </div>
            <div class="col-md-4">
                <asp:LinkButton runat="server" Text="<span class='glyphicon glyphicon-envelope'></span> Enviar" CssClass="btn btn-primary" OnClick="EnviarCorreo" />
            </div>
        </div>

    </div>
</asp:Content>
