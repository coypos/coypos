using System.Net;
using System.Text;
using CoyposServer.Models;
using CoyposServer.Models.Sql;
using CoyposServer.Utils;
using CoyposServer.Utils.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NUnit.Framework;

namespace CoyposServer.Controllers;

public class ProductController : ControllerBase
{
    private readonly DatabaseContext _dbContext;

    public ProductController(DatabaseContext dbContext) =>
        _dbContext = dbContext;

    /// <summary>
    /// Returns products
    /// </summary>
    /// <param name="productFilter">filter to use</param>
    /// <param name="filter">filter type to use (AND/OR/NOR)</param>
    /// <param name="itemsPerPage">number of items per page</param>
    /// <param name="page">page number</param>
    [HttpGet]
    [Route("products")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(RichResponse<List<Product>>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public ObjectResult GetProductsWithFilter([FromBody] Product productFilter, string filter = "AND", int itemsPerPage = 50, int page = 1, string language = "", bool loadImages = false)
    {
        try
        {
            var images = loadImages ? _dbContext.Images.ToList() : new List<Image>();
            var products = _dbContext.Products.ToList();
            
            for (var i = 0; i < products.Count; i++)
                if (!products[i].Name.IsNullOrEmpty())
                    products[i].Name = LanguageHelpers.Translate(products[i].Name, language);

            var filteredProducts = products.Filter(productFilter, filter);
            var pagefiedProducts = filteredProducts.Pagefy(itemsPerPage, page, out var totalPages);
            
            for (var i = 0; i < pagefiedProducts.Count; i++)
            {
                if (!pagefiedProducts[i].Image.IsNullOrEmpty() && loadImages)
                    pagefiedProducts[i].Image =
                        images.FirstOrDefault(_ => _.ID.ToString() == pagefiedProducts[i].Image).Img;
                else
                    pagefiedProducts[i].Image = null;
            }

            var promotions = _dbContext.Promotions.ToList();

            for (var i = 0; i < pagefiedProducts.Count; i++)
            {
                var promotionsWithThisItem = promotions.Where(_ =>
                {
                    var id = pagefiedProducts[i].ID.ToString();
                    return _.Ids is not null 
                           && _.Ids.Split(',').Any(v => v == id);
                }).ToList();
                var activePromotions =
                    promotionsWithThisItem.Where(_ => _.StartDate < DateTime.Now && _.EndDate > DateTime.Now).ToList();
                var bestPromotion = activePromotions.MinBy(_ => _.DiscountPercentage);
                if (bestPromotion is null)
                    continue;
                pagefiedProducts[i].AppliedPromotion = bestPromotion;
#pragma warning disable CS8629
                var pr = (decimal)(pagefiedProducts[i].Price - pagefiedProducts[i].Price * bestPromotion.DiscountPercentage / 100);
#pragma warning restore CS8629
                pagefiedProducts[i].DiscountedPrice = Math.Round(pr, 2);
            }

            return StatusCode((int)HttpStatusCode.OK, new RichResponse<List<Product>>(pagefiedProducts)
            {
                Page = page,
                TotalPages = totalPages,
                ItemsPerPage = itemsPerPage,
                TotalItems = products.Count,
                TotalItemsFiltered = pagefiedProducts.Count
            });
        }
        catch (ArgumentOutOfRangeException e)
        {
            return StatusCode((int)HttpStatusCode.BadRequest, new ProblemDetails() { Title = e.Message });
        }
        catch (Exception e)
        {
            return StatusCode((int)HttpStatusCode.InternalServerError, new ProblemDetails() { Title = e.Message });
        }
    }

    /// <summary>
    /// Updates a product
    /// </summary>
    /// <param name="product">new product data</param>
    /// <param name="id">product ID</param>
    [HttpPut]
    [Route("product/{id}")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ObjectResult> Put([FromBody] Product product, int id)
    {
        Product? productFromDb;
        try
        {
            if (!product.Image.IsNullOrEmpty())
            {
                var imageSize = Encoding.UTF8.GetBytes(product.Image).Length;
                if (imageSize > 25*1024)
                    throw new Exception(
                        $"Provided image is too large ({imageSize} bytes). Maximum image size: {25*1024} bytes.");
                var imageResult = _dbContext.Images.Add(new Image() { Img = product.Image });
                await _dbContext.ForceSaveChangesAsync("Images");
                product.Image = imageResult.Entity.ID.ToString();
            }
            
            productFromDb = _dbContext.Products.FirstOrDefault(p => p.ID == id);
            if (productFromDb is null)
                throw new Exception("No known product with such ID");
            product.ID = id;
            product.CreateDate = productFromDb.CreateDate;
            product = ObjectHelpers.CopyNonNullValues(productFromDb, product);
            //_dbContext.AttachVirtualProperties(productFromDb);
            _dbContext.Entry(productFromDb).CurrentValues.SetValues(product);
            
            await _dbContext.ForceSaveChangesAsync("Products");
        }
        catch (Exception e)
        {
            var message = e.InnerException is not null ? e.InnerException.Message : e.Message;
            var response = e.InnerException is not null
                ? HttpStatusCode.BadRequest
                : HttpStatusCode.InternalServerError;
            return StatusCode((int)response, new ProblemDetails() { Title = message });
        }

        return StatusCode((int)HttpStatusCode.OK, productFromDb);
    }

    /// <summary>
    /// Deletes a product
    /// </summary>
    /// <param name="id">product ID</param>
    [HttpDelete]
    [Route("product/{id}")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ObjectResult> Delete(int id)
    {
        try
        {
            var productFromDb = _dbContext.Products.FirstOrDefault(p => p.ID == id);
            if (productFromDb is null)
                throw new Exception("No known product with such ID");
            _dbContext.AttachVirtualProperties(id);
            _dbContext.Remove(productFromDb);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            var message = e.InnerException is not null ? e.InnerException.Message : e.Message;
            var response = e.InnerException is not null
                ? HttpStatusCode.BadRequest
                : HttpStatusCode.InternalServerError;
            return StatusCode((int)response, new ProblemDetails() { Title = message });
        }

        return StatusCode((int)HttpStatusCode.OK, $"Product {id} deleted");
    }

    /// <summary>
    /// Creates a product entry
    /// </summary>
    /// <param name="product">new product data</param>
    /// <returns></returns>
    [HttpPost]
    [Route("product")]
    [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
    [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ObjectResult> Post([FromBody] Product product)
    {
        EntityEntry<Product> result;

        try
        {
            if (!product.Image.IsNullOrEmpty())
            {
                var imageSize = Encoding.UTF8.GetBytes(product.Image).Length;
                if (imageSize > 25*1024)
                    throw new Exception(
                        $"Provided image is too large ({imageSize} bytes). Maximum image size: {25*1024} bytes.");
                var imageResult = _dbContext.Images.Add(new Image() { Img = product.Image });
                await _dbContext.ForceSaveChangesAsync("Images");
                product.Image = imageResult.Entity.ID.ToString();
            }
            
            // overwritten values:
            product.CreateDate = DateTime.Now;
            product.UpdateDate = DateTime.Now;
            _dbContext.AttachVirtualProperties(product);
            // </>

            result = _dbContext.Products.Add(product);
            
            await _dbContext.ForceSaveChangesAsync("Products");
        }
        catch (Exception e)
        {
            var message = e.InnerException is not null ? e.InnerException.Message : e.Message;
            var response = e.InnerException is not null
                ? HttpStatusCode.BadRequest
                : HttpStatusCode.InternalServerError;
            return StatusCode((int)response, new ProblemDetails() { Title = message });
        }

        return StatusCode((int)HttpStatusCode.OK, result.Entity);
    }
}