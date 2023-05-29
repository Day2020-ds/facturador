<%@ Page Title="Consulta de Facturas" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Consulta.aspx.cs" Inherits="Facturador.GHO.Admin.Consulta" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <link rel="stylesheet" href="../Content/bootstrap-datepicker.css" type="text/css" />
    <script src="../Scripts/bootstrap-datepicker.js" type="text/javascript"></script>
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
   <h2>Consultar facturas</h2>
    <p class="text-danger">
        <asp:Literal runat="server" ID="ErrorMessage" />
    </p>

    <hr />

    <div class="row">
         <h4>Filtrar por:</h4>
         <div class="col-md-4">
              <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="FechaInicial" CssClass="col-md-4 control-label">Fecha Inicial</asp:Label>
                    <div class="col-md-8">
                        <div class="input-group date" id='DateFechaInicial'>
                            <asp:TextBox ID="FechaInicial" runat="server" CssClass="form-control date form_datetime col-md-8" />
                            <span class="input-group-addon" style="visibility: hidden"><span class="glyphicon glyphicon-calendar"></span></span>
                        </div>
                    </div>
              </div>
           <br>  <br />
             <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="UUID" CssClass="col-md-4 control-label">Folio Fiscal</asp:Label>
                    <div class="col-md-7">
                        <asp:TextBox runat="server" ID="UUID" CssClass="form-control" />
                    </div>
              </div>
         </div>
         <div class="col-md-4">
              <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="FechaFinal" CssClass="fechaFin col-md-4 control-label">Fecha Final</asp:Label>
                    <div class="col-md-8">
                        <div class='input-group date' id='DateFechaFinal'>
                            <asp:TextBox runat="server" ID="FechaFinal" CssClass="form-control" />
                            <span class="input-group-addon" style="visibility: hidden"><span class="glyphicon glyphicon-calendar"></span>
                            </span>
                        </div>
                    </div>
                </div>
             <br>  <br />
              <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="DDMetodoPago" CssClass="col-md-4 control-label">Metodo de Pago</asp:Label>
                    <div class="col-md-7">
                        <asp:DropDownList runat="server" ID="DDMetodoPago" CssClass="form-control" />
                    </div>
                </div>
         </div>
         
        <div class="col-md-4">
                <div class="form-group">
                    <asp:Label runat="server" AssociatedControlID="DDTipoComprobante" CssClass="col-md-4 control-label">Tipo de Comprobante</asp:Label>
                    <div class="col-md-8">
                        <asp:DropDownList runat="server" ID="DDTipoComprobante" CssClass="form-control" />
                    </div>
               </div>
                <div class="form-group">
                  
                    <div class="col-md-8">
                        <asp:DropDownList runat="server" ID="DDEmisor" CssClass="form-control"  Visible="false"/>
                    </div>
                </div>
                <br>  <br />
               
          
        </div>
        <div class="col-md-6">
            <div class="form-horizontal">
               
               
                <div class="form-group">
                    <div class="row">
                       
                        <div class="col-md-6">
                            
                            <div class="col-md-4">
                                <asp:Button runat="server" ID="btnConsultar" Text="Consultar" CssClass="btn btn-primary" OnClick="Consultar" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <br />
    <asp:UpdatePanel ID="upCrudGrid" runat="server">

        <ContentTemplate>
            <div class="form-horizontal">
                <div class="form-group">
                    <asp:GridView ID="viewFacturas" runat="server" CssClass="table table-striped table-bordered table-hover" AutoGenerateColumns="False" AllowPaging="True" OnPageIndexChanging="viewFacturas_PageIndexChanging" OnRowCommand="viewFacturas_RowCommand" DataKeyNames="uuid" OnRowCreated="viewFacturas_RowCreated">
                        <Columns>
                            <asp:BoundField HeaderStyle-BackColor="#3d618f" HeaderStyle-ForeColor="White" DataField="uuid" HeaderText="Folio Fiscal" />
                            <asp:BoundField HeaderStyle-BackColor="#3d618f" HeaderStyle-ForeColor="White" DataField="serie" HeaderText="Serie" />
                            <asp:BoundField HeaderStyle-BackColor="#3d618f" HeaderStyle-ForeColor="White" DataField="fecha_timbrado" HeaderText="Fecha timbrado" />
                            <asp:BoundField HeaderStyle-BackColor="#3d618f" HeaderStyle-ForeColor="White" DataField="emisor" HeaderText="Emisor" />
                            <asp:BoundField HeaderStyle-BackColor="#3d618f" HeaderStyle-ForeColor="White" DataField="receptor" HeaderText="Receptor" />
                            <asp:BoundField HeaderStyle-BackColor="#3d618f" HeaderStyle-ForeColor="White" DataField="subtotal" HeaderText="Subtotal" />
                            <asp:BoundField HeaderStyle-BackColor="#3d618f" HeaderStyle-ForeColor="White" DataField="iva" HeaderText="IVA" />
                            <asp:BoundField HeaderStyle-BackColor="#3d618f" HeaderStyle-ForeColor="White" DataField="ieps" HeaderText="IEPS" />
                            
                            <asp:BoundField HeaderStyle-BackColor="#3d618f" HeaderStyle-ForeColor="White" DataField="ret_iva" HeaderText="Ret. IVA" />
                            <asp:BoundField HeaderStyle-BackColor="#3d618f" HeaderStyle-ForeColor="White" DataField="ret_isr" HeaderText="Ret. ISR" />
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
                <div class="form-group">
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
            </div>

        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="btnConsultar" />
            <asp:PostBackTrigger ControlID="viewFacturas" />
            <asp:PostBackTrigger ControlID="viewcfdirip" />
        </Triggers>
    </asp:UpdatePanel>

    <div class="modal fade" id="Cancelacion" tabindex="-1" role="dialog" aria-hidden="true" aria-labelledby="cancelacionLabel">
        <div class="modal-dialog modal-sm">
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
                            <button type="button" class="btn btn-default" data-dismiss="modal">Cerrar</button>
                            <asp:Button ID="btnCancelar" runat="server" CssClass="btn btn-danger" Text="Aceptar" OnClick="btnCancelar_Click"></asp:Button>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </div>
</asp:Content>

