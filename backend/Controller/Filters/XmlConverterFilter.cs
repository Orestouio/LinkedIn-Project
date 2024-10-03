using System.Text;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace BackendApp.Controllers.Filters;

public class XmlConverterFilterAttribute : ActionFilterAttribute

{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        if(!this.VerifyXmlParameter(filterContext.HttpContext)) return;
        filterContext.HttpContext.Response.OnStarting(
            () => SetHeaders(filterContext)
        );
    }

    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
    }

    public override void OnResultExecuting(ResultExecutingContext filterContext)
    {
        if(!this.VerifyXmlParameter(filterContext.HttpContext)) return;
        FormatResponseIntoXml(filterContext);
    }
    private static Task<string> SetHeaders(ActionExecutingContext filterContext)
    {
        return Task.Run( () => filterContext.HttpContext.Response.ContentType = "application/xml; charset=utf-8" );
        
    }
    private static void FormatResponseIntoXml(ResultExecutingContext filterContext)
    {  
        if (filterContext.Result is not JsonResult result) return;     

        var jsonString = JsonConvert.SerializeObject(result);
        var xmlDoc = JsonConvert.DeserializeXmlNode(jsonString, "root");
        if(xmlDoc is null) return;
        FilterXmlDoc(xmlDoc);

        File.AppendAllLines("./log.txt", [filterContext.HttpContext.Response.StatusCode.ToString()]);
        //await filterContext.HttpContext.Response.Body.WriteAsync(memoryStream.ToArray().AsMemory(0, (int)memoryStream.Length));
        filterContext.Result = new ContentResult()
        {
            Content = xmlDoc.OuterXml,
            ContentType = "application/xml",
            StatusCode = filterContext.HttpContext.Response.StatusCode
        }; //Prevents further processing     
        
    }

    private static void FilterXmlDoc(XmlDocument xmlDoc)
    {
        var elementToRemove = xmlDoc.GetElementsByTagName("StatusCode")[0];
        if(elementToRemove is not null)
            xmlDoc.DocumentElement?.RemoveChild(elementToRemove);
        elementToRemove = xmlDoc.GetElementsByTagName("ContentType")[0];
        if(elementToRemove is not null)
            xmlDoc.DocumentElement?.RemoveChild(elementToRemove);
        elementToRemove = xmlDoc.GetElementsByTagName("SerializerSettings")[0];
        if(elementToRemove is not null)
            xmlDoc.DocumentElement?.RemoveChild(elementToRemove);
    }

    private bool VerifyXmlParameter(HttpContext context)
    {
        string? value = context.Request.Query["format"];
        if(value is null) return false;
        return value == "xml";
    }
}
