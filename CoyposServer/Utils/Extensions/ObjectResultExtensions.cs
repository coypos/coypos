using System.Net;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CoyposServer.Utils.Extensions;

public static class ObjectResultExtensions
{
    public static bool CheckStatusCode(this ObjectResult objectResult, HttpStatusCode statusCode)
    {
        return CheckStatusCode(objectResult, (int)statusCode);
    }
    
    public static bool CheckStatusCode(this ObjectResult objectResult, int statusCode)
    {
        if (objectResult.StatusCode == statusCode)
            return true;
        throw new Exception($"Expected status code {statusCode}, got {objectResult.StatusCode} instead");
    }
    
    public static T YeldExpectedResult<T>(this ObjectResult objectResult)
    {
        if (objectResult.Value.GetType() == typeof(T))
            return (T)objectResult.Value;
        if (objectResult.Value.GetType() == typeof(ProblemDetails))
            throw new Exception("Unexpected result type. Result => " + JsonConvert.SerializeObject((ProblemDetails)objectResult.Value));
        if (objectResult.Value.GetType() == typeof(string))
            throw new Exception("Unexpected result type. Result => " + objectResult.Value.ToString());
        throw new Exception("Unhandled result type");
    }
}