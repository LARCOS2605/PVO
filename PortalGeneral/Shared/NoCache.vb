Imports System.ComponentModel
Imports System.Data.Entity
Imports AutoMapper
Imports NetsuiteLibrary.Clases

Public Class NoCacheAttribute
    Inherits ActionFilterAttribute

    Public Overrides Sub OnResultExecuting(ByVal filterContext As ResultExecutingContext)
        filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1))
        filterContext.HttpContext.Response.Cache.SetValidUntilExpires(False)
        filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches)
        filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache)
        filterContext.HttpContext.Response.Cache.SetNoStore()
        filterContext.HttpContext.Response.AppendHeader("pragma", "no-cache")
        MyBase.OnResultExecuting(filterContext)
    End Sub
End Class
