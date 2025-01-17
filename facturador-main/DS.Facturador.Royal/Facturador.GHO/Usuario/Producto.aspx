﻿<%@ Page Title="Productos" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Producto.aspx.cs" Inherits="Facturador.GHO.Usuario.Producto" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <link href="../Content/jQuery.FileUpload/css/jquery.fileupload.css" rel="stylesheet" />
    <h2>Mis productos</h2>
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
                <asp:Label runat="server" AssociatedControlID="Empresa" CssClass="col-md-2 control-label">Empresa</asp:Label>
                <asp:DropDownList ID="Empresa" runat="server" CssClass="form-control" AutoPostBack="true" OnSelectedIndexChanged="Empresa_SelectedIndexChanged">
                </asp:DropDownList>
                <asp:RequiredFieldValidator runat="server" ControlToValidate="Empresa" ValidationGroup="vgFacturar" Display="Dynamic"
                    CssClass="text-danger" ErrorMessage="La empresa es obligatoria." />
            </div>
        </div>
       
        <asp:UpdatePanel ID="upProductos" runat="server">
            <ContentTemplate>
                <p class="text-danger">
                    <asp:Literal runat="server" ID="ErrorMessage" />
                </p>
                 <div class="form-group">
                    <div class="col-md-2 marginpadding">
                        <asp:Button runat="server" ID="btnRegistrar" CssClass="btn btn-primary" Text="Nuevo producto" OnClick="btnRegistrar_Click"></asp:Button>
                    </div>
                    <div class="col-md-2 marginpadding"  style="padding-left: 13px;">
                        <span class="btn btn-success fileinput-button">
                            <i class="glyphicon glyphicon-plus"></i>
                            <span>Lista productos.</span>
                            <input id="file1" type="file" name="fileUpload" accept="application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" runat="server" />
                        </span>
                    </div>
                    <div class="col-md-2 marginpadding" style="padding-left: 13px;">
                          <asp:Button ID="Importar" runat="server" CssClass="btn btn-default" Text="Importar" OnClick="Importar_Click" />
                    </div>
                 </div>
               
                <br></br>
                <asp:GridView ID="viewProductos" runat="server" CssClass="table table-striped table-bordered table-hover" AutoGenerateColumns="False" DataKeyNames="id" OnRowCommand="viewProductos_RowCommand">
                    <Columns>
                        <asp:BoundField HeaderStyle-BackColor="#3d618f" HeaderStyle-ForeColor="White" DataField="id" HeaderText="id" Visible="False" />
                        <asp:BoundField HeaderStyle-BackColor="#3d618f" HeaderStyle-ForeColor="White" DataField="codigo" HeaderText="Codigo" />
                        <asp:BoundField HeaderStyle-BackColor="#3d618f" HeaderStyle-ForeColor="White" DataField="descripcion" HeaderText="Descripcion" />
                        <asp:BoundField HeaderStyle-BackColor="#3d618f" HeaderStyle-ForeColor="White" DataField="valor_unitario" HeaderText="Valor unitario" />
                        <asp:BoundField HeaderStyle-BackColor="#3d618f" HeaderStyle-ForeColor="White" DataField="iva" HeaderText="IVA" />
                        <asp:BoundField HeaderStyle-BackColor="#3d618f" HeaderStyle-ForeColor="White" DataField="ieps" HeaderText="IEPS" />
                        <asp:ButtonField HeaderStyle-BackColor="#3d618f" HeaderStyle-ForeColor="White" Text="<span class='glyphicon glyphicon-pencil'></span>" ItemStyle-ForeColor="Black" CommandName="Editar" />
                        <asp:ButtonField HeaderStyle-BackColor="#3d618f" HeaderStyle-ForeColor="White" Text="<span class='glyphicon glyphicon-remove'></span>" ItemStyle-ForeColor="Red" CommandName="Eliminar" />
                    </Columns>
                </asp:GridView>

            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnProducto" />
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
                             <h4 >Añadir nuevo producto</h4>
                        </div>
                        <div class="modal-body">
                            <div class="form-horizontal">
                                <asp:HiddenField runat="server" ID="idProducto" />
                                <div class="row">
                                    <!-- Cóodigo SKU -->
                                    <div class="col-md-3">
                                        <asp:Label runat="server" AssociatedControlID="Codigo" CssClass="control-label">SKU</asp:Label>
                                        <asp:TextBox runat="server" ID="Codigo" CssClass="form-control" placeholder="Clave o SKU"/>
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="Codigo" Display="Dynamic"
                                            CssClass="text-danger" ErrorMessage="El codigo es obligatorio." ValidationGroup="registrarGroup" />                                        
                                    </div>
                                    <!-- Unidad -->
                                    <div class="col-md-3">
                                        <asp:Label runat="server" AssociatedControlID="UnidadMedida" CssClass="control-label">Unidad</asp:Label>
                                        <asp:TextBox runat="server" ID="UnidadMedida" CssClass="form-control" placeholder="Unidad"/>
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="UnidadMedida" Display="Dynamic"
                                            CssClass="text-danger" ErrorMessage="La unidad de medida es obligatoria." ValidationGroup="registrarGroup" />
                                    </div>
                                    <!-- Precio -->
                                    <div class="col-md-3">
                                        <asp:Label runat="server" AssociatedControlID="ValorUnitario" CssClass="control-label">Precio</asp:Label>
                                        <asp:TextBox runat="server" ID="ValorUnitario" CssClass="form-control numeric" placeholder="$ 0.00"/>
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="ValorUnitario" Display="Dynamic"
                                            CssClass="text-danger" ErrorMessage="El valor unitario es obligatorio." ValidationGroup="registrarGroup" />
                                    </div>
                                    <!-- IVA -->
                                    <div class="col-md-3">
                                        <asp:Label runat="server" AssociatedControlID="IVA" CssClass="control-label">IVA%</asp:Label>
                                        <asp:TextBox runat="server" ID="IVA" CssClass="form-control numeric" placeholder="16.00 %" />
                                    </div>
                                    <!-- IEPS -->
                                    <div class="col-md-3">
                                        <asp:Label runat="server" AssociatedControlID="IEPS" CssClass="control-label">IEPS %</asp:Label>
                                        <asp:TextBox runat="server" ID="IEPS" CssClass="form-control numeric" placeholder="00.00 %"/>
                                    </div>
                                    <!-- Descuento -->
                                    <div class="col-md-3">
                                        <asp:Label runat="server" AssociatedControlID="Descuento" CssClass="control-label">Desc %</asp:Label>
                                        <asp:TextBox runat="server" ID="Descuento" CssClass="form-control numeric" placeholder="0.00 %" />
                                    </div>
                                    <!-- Descripción -->
                                    <div class="col-md-6">
                                        <asp:Label runat="server" AssociatedControlID="Descripcion" CssClass="control-label">Descripción</asp:Label>
                                        <asp:TextBox runat="server" ID="Descripcion" CssClass="form-control" TextMode="MultiLine" placeholder="Descripción del producto o servicio..."/>
                                        <asp:RequiredFieldValidator runat="server" ControlToValidate="Descripcion" Display="Dynamic"
                                            CssClass="text-danger" ErrorMessage="La descripción es obligatoria." ValidationGroup="registrarGroup" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="btnProducto" runat="server" ValidationGroup="registrarGroup" CssClass="btn btn-primary" Text="Registrar" OnClick="btnProducto_Click"></asp:Button>
                            <button type="button" class="btn btn-danger" data-dismiss="modal"> <i class="fas fa-times"></i> Cerrar</button>
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="viewProductos" EventName="RowCommand" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>
