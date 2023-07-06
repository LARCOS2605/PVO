@code
    Layout = Nothing
End Code
@If ViewData.Model Is Nothing Then
    @<span>NA</span>
Else
    @<span class="glyphicon glyphicon-@IIF(ViewData.Model, "check", "unchecked")" aria-hidden="true" style="font-size:1.5em" title="@IIf(ViewData.Model, "Enviado", "Sin Enviar")"></span>
End If