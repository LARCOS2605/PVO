Imports System.Web.Mvc

Public Class ErrorController
    Inherits AppBaseController

    '
    ' GET: /Error

    Function Index() As ViewResult
        Return View("Error")
    End Function

    Function NotFound() As ViewResult
        Response.StatusCode = 200
        Return View("NotFound")
    End Function

    'Function Index() As ViewResult
    '    Return View("Error")
    'End Function
End Class