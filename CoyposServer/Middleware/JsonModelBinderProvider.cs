using CoyposServer.Models.Sql;
using CoyposServer.Utils;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CoyposServer.Middleware;

public class JsonModelBinderProvider : IModelBinderProvider
{
    private readonly DatabaseContext _dbContext;
    
    public JsonModelBinderProvider(DatabaseContext databaseContext)
    {
        _dbContext = databaseContext;
    }
    
    public IModelBinder GetBinder(ModelBinderProviderContext context)
    {
        if (context == null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (context.Metadata.ModelType.AssemblyQualifiedName.Contains("CoyposServer.Models"))
        {
            return new JsonModelBinder(_dbContext);
        }

        return null;
    }
}