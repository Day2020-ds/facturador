<%@ Page Title="Recuperar contraseña" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Recover.aspx.cs" Inherits="Facturador.GHO.Account.Recover" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2><%: Title %>.</h2>
    <p class="text-danger">
        <asp:Literal runat="server" ID="ErrorMessage" />
    </p>

    <div class="form-horizontal">
        <h4>Introduzca la siguiente información para regenerar la contraseña.</h4>
        <hr />
        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="UserName" CssClass="col-md-2 control-label">Nombre de usuario</asp:Label>
            <div class="col-md-10">
                <asp:TextBox runat="server" ID="UserName" CssClass="form-control" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="UserName" Display="Dynamic"
                    CssClass="text-danger" ErrorMessage="El campo nombre de usuario es obligatorio." />
            </div>
        </div>
        <div class="form-group">
            <asp:Label runat="server" AssociatedControlID="Correo" CssClass="col-md-2 control-label">Correo electrónico</asp:Label>
            <div class="col-md-10">
                <asp:TextBox runat="server" ID="Correo" CssClass="form-control" />
                <asp:RequiredFieldValidator runat="server" ControlToValidate="Correo" Display="Dynamic"
                    CssClass="text-danger" ErrorMessage="El campo de correo es obligatorio." />
                <asp:RegularExpressionValidator runat="server" ErrorMessage="El correo no es valido." Display="Dynamic"
                    CssClass="text-danger" ControlToValidate="Correo" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" />
            </div>
        </div>
        <div class="form-group">
            <div class="col-md-offset-2 col-md-10">
                <asp:Button runat="server" ID="Recuperar" OnClick="Recover_Click" Text="Enviar" CssClass="btn btn-primary" />
            </div>
        </div>
        <br />
        <p>La contraseña sera enviada al correo proporcionado</p>
    </div>
</asp:Content>
