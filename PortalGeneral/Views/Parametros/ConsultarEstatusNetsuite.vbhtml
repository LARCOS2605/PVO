@Code
    ViewData("Title") = "Estatus Netsuite"
    ViewData("I_EN") = "class=active"
    ViewData("SubPage") = "Configuración"
End Code


<style>

    .responsive-iframe {
        position: fixed;
        top: 0;
        left: 0;
        bottom: 0;
        right: 0;
        width: 100%;
        height: 100%;
        border: none;
    }
</style>

<div class="card">
    <div class="card-header">
        <header><h4 class="header-title">@ViewData("Title")</h4></header>
    </div>
    <div class="card-body">
        

        <div class="embed-responsive embed-responsive-21by9">
            <iframe class="embed-responsive-item" src="https://status.netsuite.com/"></iframe>
        </div>

    </div>
</div>
