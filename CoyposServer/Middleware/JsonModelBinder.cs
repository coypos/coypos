using System.Net;
using System.Reflection;
using System.Text;
using CoyposServer.Utils;
using Newtonsoft.Json.Linq;

namespace CoyposServer.Middleware;

using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

public class JsonModelBinder : IModelBinder
{
    private readonly DatabaseContext _dbContext;
    
    public JsonModelBinder(DatabaseContext databaseContext)
    {
        _dbContext = databaseContext;
    }
    
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {
        try
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            // Read the request body into a memory stream
            using (var memoryStream = new MemoryStream())
            {
                await bindingContext.HttpContext.Request.Body.CopyToAsync(memoryStream);
                memoryStream.Seek(0, SeekOrigin.Begin);

                // Read the memory stream as a string
                using (var reader = new StreamReader(memoryStream, Encoding.UTF8))
                {
                    var requestBodyString = await reader.ReadToEndAsync();
                    if (requestBodyString == "")
                        requestBodyString = "{}";
                    var requestBody = JObject.Parse(requestBodyString);

                    var virtualProperties = bindingContext.ModelType.GetProperties().Where(p =>
                    {
                        var getMethod = p.GetGetMethod();
                        return getMethod is not null && getMethod.IsVirtual;
                    });

                    // deserialize
                    var result = JsonConvert.DeserializeObject(requestBodyString, bindingContext.ModelType,
                        new JsonSerializerSettings()
                        {
                            MissingMemberHandling = MissingMemberHandling.Ignore,
                            Error = (sender, args) => { args.ErrorContext.Handled = true; }
                        });

                    foreach (var virtualProperty in virtualProperties)
                    {
                        var foundDbContextProperty = typeof(DatabaseContext).GetProperties()
                            .FirstOrDefault(p =>
                            {
                                var genericArguments = p.PropertyType.GetGenericArguments();
                                if (genericArguments.Length > 0)
                                    return p.PropertyType.GetGenericArguments()[0] == virtualProperty.PropertyType;
                                return default;
                            });
                        if (foundDbContextProperty is null)
                            continue;

                        var list = (foundDbContextProperty.GetValue(_dbContext) as IQueryable).Cast<object>().ToList();

                        if (list is IEnumerable<object> enumerableList)
                        {
                            int? val = null;
                            foreach (var keyValuePair in requestBody)
                            {
                                if (keyValuePair.Key.ToLower() == virtualProperty.Name.ToLower())
                                {
                                    if (keyValuePair.Value.Type != JTokenType.Integer)
                                        continue;
                                    val = keyValuePair.Value.Value<int>();
                                    break;
                                }
                            }

                            if (val is null)
                                continue;


                            // Iterate through each element in the list
                            var found = false;
                            foreach (var item in enumerableList)
                            {
                                // You can use reflection to access the "ID" property and check its value
                                var idProperty = item.GetType().GetProperty("ID");

                                if (idProperty != null && idProperty.GetValue(item).Equals(val))
                                {
                                    _dbContext.Attach(item);
                                    result.GetType().GetProperty(virtualProperty.Name).SetValue(result, item);
                                    found = true;
                                    break;
                                }
                            }

                            var v = bindingContext.ValueProvider.GetValue("filter");
                            if (!found && v.FirstValue is not null && v.FirstValue.ToUpper().Equals("ISNULL"))
                            {
                                result.GetType().GetProperty(virtualProperty.Name)
                                    .SetValue(result, Activator.CreateInstance(result.GetType()));
                            }
                            else if (!found)
                                throw new Exception(
                                "One of the request properties tried to refer to a non-existent object in the database.");
                        }
                    }

                    // Set the bound value to the result
                    bindingContext.Result = ModelBindingResult.Success(result);
                }
            }
        }
        catch (Exception e)
        {
            Log.Wrn("⚠️ Aborting the request in the model binder.");
            Log.Wrn("⚠️ This will throw an exception right about... now!");
            bindingContext.HttpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
            bindingContext.HttpContext.Response.ContentType = "application/json";
            await bindingContext.HttpContext.Response.WriteAsync("{\"Title\": \""+e.Message+"\"}");
            await bindingContext.HttpContext.Response.CompleteAsync();
        }
    }
}