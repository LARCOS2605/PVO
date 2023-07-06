@Code
    ViewData("Title") = "Registrar Cliente"
    ViewData("I_CC") = "class=active"
    ViewData("SubPage") = "Clientes"

    Dim db As New PVO_NetsuiteEntities
    Dim Ambiente = (From n In db.Parametros Where n.Clave = "Ambiente").FirstOrDefault()
    Dim TipoAmbiente As String = ""

    If Not IsNothing(Ambiente) Then
        TipoAmbiente = Ambiente.Valor
    End If
End Code


<div class="alert alert-warning" role="alert">
    <strong>@Idiomas.My.Resources.Resource.Alert_Recordatorio</strong> @Idiomas.My.Resources.Resource.Alert_CrearCliente
</div>

<div class="card">
    <div class="card-header">
        <header><h4 class="header-title">@ViewData("Title")</h4></header>
    </div>
    <div class="card-body">
        

        <div class="embed-responsive embed-responsive-21by9">
            @If TipoAmbiente = "QA" Then
                @<iframe Class="embed-responsive-item" src="https://6236630-sb1.extforms.netsuite.com/app/site/crm/externalleadpage.nl?compid=6236630_SB1&formid=4&h=AAFdikaINMd3ytVXHm3LTnjyuvfrNdMLmFDz0zjHQe44Tl4icNw"></iframe>
            ElseIf TipoAmbiente = "PRD" Then
                @<iframe Class="embed-responsive-item" src="https://6236630.extforms.netsuite.com/app/site/crm/externalleadpage.nl?compid=6236630&formid=4&h=AAFdikaIeEhZtZ_CLj3Voh6FdfVlDkAcRW2GDpPewJHlDG5FCmA"></iframe>
            End If
        </div>

    </div>
</div>
