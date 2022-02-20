<%@ Page Title="Consulta de Facturas" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Consulta.aspx.cs" Inherits="Facturador.GHO.Admin.Consulta" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <link rel="stylesheet" href="../Content/bootstrap-datepicker.css" type="text/css" />
    <script src="../Scripts/bootstrap-datepicker.js" type="text/javascript"></script>
    <h2>Consultar facturas</h2>
    <p class="text-danger">
        <asp:Literal runat="server" ID="ErrorMessage" />
    </p>
    <hr />
    <script type="text/javascript">
        $(function () {
            var nowTemp = new Date();
            var now = new Date(nowTemp.getFullYear(), nowTemp.getMonth(), nowTemp.getDate(), 0, 0, 0, 0);

            var checkin = $('#DateFechaInicial').datepicker({
                format: "dd-mm-yyyy",
                language: "es",
                todayHighlight: true,
                startDate: '01/01/2014',
                endDate: nowTemp
            }).on('changeDate', function (ev) {
                var newDate = new Date(ev.date)
                if (ev.date.valueOf() > checkout.getDate()) {
                    $("#DateFechaFinal").datepicker("update", newDate);
                }
                $("#DateFechaFinal").datepicker("setStartDate ", newDate);
                checkin.hide();
                $('#DateFechaFinal')[0].focus();
            }).data('datepicker');
            var checkout = $('#DateFechaFinal').datepicker({
                format: "dd-mm-yyyy",
                language: "es",
                todayHighlight: true,
                startDate: '01/01/2014',
                endDate: nowTemp
            }).on('changeDate', function (ev) {
                var newDate = new Date(ev.date)
                if (ev.date.valueOf() < checkin.getDate()) {
                    $("#DateFechaInicial").datepicker("update", newDate);
                }
                checkout.hide();
            }).data('datepicker');
        });

    </script>
    <div>
        <div class="row">
            <div class="col-md-12 text-right">
                <button type="button" class="btn btn-primary" data-toggle="modal" data-target="#filterModal">
                    <i class="fas fa-filter"></i> Filtrar facturas
                </button>
            </div>
        </div>
        <br />
        <asp:UpdatePanel ID="upCrudGrid" runat="server">
                    <ContentTemplate>
                        <div class="table-responsive">
                            <asp:GridView ID="viewFacturas" runat="server" CssClass="table table-striped table-bordered table-hover" AutoGenerateColumns="False" AllowPaging="True" OnPageIndexChanging="viewFacturas_PageIndexChanging" OnRowCommand="viewFacturas_RowCommand" DataKeyNames="uuid" OnRowCreated="viewFacturas_RowCreated">
                                <Columns>
                                    <asp:BoundField HeaderStyle-BackColor="#3d618f" HeaderStyle-ForeColor="White" DataField="uuid" HeaderText="Folio Fiscal" />
                                    <asp:BoundField HeaderStyle-BackColor="#3d618f" HeaderStyle-ForeColor="White" DataField="serie" HeaderText="Serie" />
                                    <asp:BoundField HeaderStyle-BackColor="#3d618f" HeaderStyle-ForeColor="White" DataField="fecha_timbrado" HeaderText="Fecha timbrado" />
                                    <asp:BoundField HeaderStyle-BackColor="#3d618f" HeaderStyle-ForeColor="White" DataField="emisor" HeaderText="Emisor" />
                                    <asp:BoundField HeaderStyle-BackColor="#3d618f" HeaderStyle-ForeColor="White" DataField="receptor" HeaderText="Receptor" />
                                    
                                    <asp:BoundField HeaderStyle-BackColor="#3d618f" HeaderStyle-ForeColor="White" DataField="total" HeaderText="Total" />
                                    <asp:BoundField HeaderStyle-BackColor="#3d618f" HeaderStyle-ForeColor="White" DataField="tipo" HeaderText="Tipo" />
                                    <asp:BoundField HeaderStyle-BackColor="#3d618f" HeaderStyle-ForeColor="White" DataField="estatus" HeaderText="Estatus" />

                                    <asp:ButtonField HeaderStyle-BackColor="#3d618f" HeaderStyle-ForeColor="White" Text="<span class='glyphicon glyphicon-file'></span>" ItemStyle-ForeColor="Black" HeaderText="XML" CommandName="DescargarXML" />
                                    <asp:ButtonField HeaderStyle-BackColor="#3d618f" HeaderStyle-ForeColor="White" Text="<span class='glyphicon glyphicon-file'></span>" ItemStyle-ForeColor="Black" HeaderText="PDF" CommandName="DescargarPDF" />                            
                                    <asp:TemplateField HeaderStyle-BackColor="#3d618f" HeaderStyle-ForeColor="White" HeaderText="Cancelar">
                                        <ItemTemplate>
                                            <asp:LinkButton runat="server" ID="LinkCancelar" ForeColor="Red" Text="<span class='glyphicon glyphicon-remove'></span>" CommandName="Cancelar" CommandArgument='<%#Container.DisplayIndex %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>

                        <div class="table-responsive">
                            <asp:GridView ID="viewcfdirip" runat="server" CssClass="table table-striped table-bordered table-hover" AutoGenerateColumns="False" AllowPaging="True" OnPageIndexChanging="viewFacturas_PageIndexChanging" OnRowCommand="viewCFDIRIP_RowCommand" DataKeyNames="uuid" OnRowCreated="viewFacturas_RowCreated">
                                <Columns>
                                    <asp:BoundField DataField="uuid" HeaderText="Folio Fiscal" />
                                    <asp:BoundField DataField="fecha_timbrado" HeaderText="Fecha timbrado" />
                                    <asp:BoundField DataField="emisor" HeaderText="Emisor" />
                                    <asp:BoundField DataField="receptor" HeaderText="Receptor" />
                                    <asp:BoundField DataField="nacionalidad" HeaderText="Nacionalidad" />
                                    <asp:BoundField DataField="mesinicial" HeaderText="Mes inicial" />
                                    <asp:BoundField DataField="mesfinal" HeaderText="Mes final" />
                                    <asp:BoundField DataField="ejerciciofiscal" HeaderText="Año fiscal" />
                                    <asp:BoundField DataField="montototoperacion" HeaderText="Monto total" />
                                    <asp:BoundField DataField="montototgrav" HeaderText="Monto gravado" />
                                    <asp:BoundField DataField="montototexent" HeaderText="Monto exento" />
                                    <asp:BoundField DataField="montototret" HeaderText="Monto de las retenciones" />
                                    <asp:BoundField DataField="tipocomplemento" HeaderText="Complemento" />
                                    <asp:BoundField DataField="estatus" HeaderText="Estatus" />
                                    <asp:ButtonField Text="<span class='glyphicon glyphicon-search'></span>" ItemStyle-ForeColor="Black" HeaderText="Ver" CommandName="Ver" />
                                    <asp:ButtonField Text="<span class='glyphicon glyphicon-file'></span>" ItemStyle-ForeColor="Black" HeaderText="XML" CommandName="DescargarXML" />
                                    <asp:ButtonField Text="<span class='glyphicon glyphicon-file'></span>" ItemStyle-ForeColor="Black" HeaderText="PDF" CommandName="DescargarPDF" />                            
                                    <asp:TemplateField HeaderText="Cancelar">
                                        <ItemTemplate>
                                            <asp:LinkButton runat="server" ID="LinkCancelar" ForeColor="Red" Text="<span class='glyphicon glyphicon-remove'></span>" CommandName="Cancelar" CommandArgument='<%#Container.DisplayIndex %>' />
                                        </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="btnConsultar" />
                        <asp:PostBackTrigger ControlID="viewFacturas" />
                        <asp:PostBackTrigger ControlID="viewcfdirip" />
                    </Triggers>
                </asp:UpdatePanel>
    </div>

    <!-- ..:: Modales ::.. -->
    <!-- Modal Filtro -->
    <div class="modal fade" id="filterModal" tabindex="-1" role="dialog" aria-hidden="true" aria-labelledby="cancelacionLabel">
        <div class="modal-dialog modal-lg">
            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                            <h4 class="modal-title" id="titleFiltro">Filtrar facturas</h4>
                        </div>
                        <div class="modal-body">
                            <div class="form-horizontal">
                                <div class="row">
                                    <!-- Filter Form -->
                                    <!-- Fecha inicio -->
                                    <div class="col-md-3">
                                        <asp:Label runat="server" AssociatedControlID="FechaInicial" CssClass="control-label">Fecha Inicial</asp:Label>
                                        <asp:TextBox Type="Date" ID="FechaInicial" runat="server" CssClass="form-control date form_datetime col-md-8" />
                                    </div>
                                    <!-- Fecha final -->
                                    <div class="col-md-3">
                                        <asp:Label runat="server" AssociatedControlID="FechaFinal" CssClass="control-label">Fecha Final</asp:Label>
                                        <asp:TextBox Type="Date" runat="server" ID="FechaFinal" CssClass="form-control" />
                                    </div>
                                    <!-- Tipo comprobante -->
                                    <div class="col-md-4">
                                        <asp:Label runat="server" AssociatedControlID="DDTipoComprobante" CssClass="control-label">Tipo de Comprobante</asp:Label>
                                        <asp:DropDownList runat="server" ID="DDTipoComprobante" CssClass="form-control" />
                                        <!--
                                        <div class="form-group">
                                            <div class="col-md-8">
                                                <asp:DropDownList runat="server" ID="DDEmisor" CssClass="form-control"  Visible="false"/>
                                            </div>
                                        </div>
                                        -->
                                    </div>
                                    <!-- Folio Fiscal -->
                                    <div class="col-md-3">
                                        <asp:Label runat="server" AssociatedControlID="UUID" CssClass="control-label">Folio Fiscal</asp:Label>
                                        <asp:TextBox runat="server" ID="UUID" CssClass="form-control" />
                                    </div>
                                    <!-- Método de Pago -->
                                    <div class="col-md-4">
                                        <asp:Label runat="server" AssociatedControlID="DDMetodoPago" CssClass="control-label">Metodo de Pago</asp:Label>
                                        <asp:DropDownList runat="server" ID="DDMetodoPago" CssClass="form-control" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button runat="server" ID="btnConsultar" Text="Aplicar filtro" CssClass="btn btn-success" OnClick="Consultar" />
                            <button type="button" class="btn btn-danger" data-dismiss="modal"> <i class="fas fa-times"></i> Cerrar</button>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>


    <!-- Modal cancelar -->
    <div class="modal fade" id="Cancelacion" tabindex="-1" role="dialog" aria-hidden="true" aria-labelledby="cancelacionLabel">
        <div class="modal-dialog modal-md">
            <asp:UpdatePanel ID="upReporte" runat="server">
                <ContentTemplate>
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                            <h4 class="modal-title" id="reporteLabel">Cancelar comprobante</h4>
                        </div>
                        <div class="modal-body">
                            <div class="form-horizontal">
                                <p class="text-danger">
                                    <asp:Literal runat="server" ID="ErrorCancelar" />
                                </p>
                                <asp:HiddenField runat="server" ID="idCancelar" />
                                <div class="form-group">
                                    <div class="col-md-12">
                                       <asp:Label runat="server" ID="textoCancelar" Text="" />
                                    </div>
                                </div>
                                 <div class="form-group">
                                    <div class="col-md-12">
                                       <asp:Label style="color: red;" runat="server" ID="texto1" Text="" />
                                    </div>
                                </div>
                                 <div class="form-group">
                                    <div class="col-md-12">
                                       <asp:DropDownList runat="server" ID="DDTipocancelacion" CssClass="form-control" />
                                    </div>
                                </div>
                                 <div class="form-group">
                                    <div class="col-md-12">
                                       <asp:Label style="color: dodgerblue;" runat="server" ID="texto2" Text="" />
                                    </div>
                                </div>
                                 <div class="form-group">
                                    <div class="col-md-12">
                                       <asp:TextBox runat="server" ID="uuid_relacionado" CssClass="form-control" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="btnCancelar" runat="server" CssClass="btn btn-primary" Text="Aceptar" OnClick="btnCancelar_Click"></asp:Button>
                            <button type="button" class="btn btn-danger" data-dismiss="modal"> <i class="fas fa-times"></i> Cerrar</button>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>

