<div class="row">
    <div><input type="hidden" name="Carrito.index" value="<%= contador %>" /></div>
    <div><input type="hidden" name="Carrito[<%= contador %>].ClaveProducto" value="<%= Carrito.ClaveProducto %>" /></div>
    <div><input type="hidden" name="Carrito[<%= contador %>].Descripcion" value="<%= Carrito.Descripcion %>" /></div>
    <div><input type="hidden" name="Carrito[<%= contador %>].Cantidad" value="<%= Carrito.Cantidad %>" /></div>
    <div><input type="hidden" name="Carrito[<%= contador %>].NumLote" value="<%= Carrito.NumLote %>" /></div>
    <div><input type="hidden" name="Carrito[<%= contador %>].IssueInventory" value="<%= Carrito.IssueInventory %>" /></div>
    <div><input type="hidden" name="Carrito[<%= contador %>].ID_NS" value="<%= Carrito.ID_NS %>" /></div>
    <div><input type="hidden" name="Carrito[<%= contador %>].idPr" value="<%= Carrito.idPr %>" /></div>
    <div><input type="hidden" name="Carrito[<%= contador %>].TipoMaterial" value="<%= Carrito.TipoMaterial %>" /></div>
    <div><input type="hidden" name="Carrito[<%= contador %>].PrecioU" value="<%= Carrito.PrecioU %>" /></div>
    <div><input type="hidden" name="Carrito[<%= contador %>].Total" value="<%= Carrito.Total %>" /></div>
</div>