using AltenShopAPI.Models;
using AltenShopAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


// Le back-end doit gérer les API suivantes : 

// **/products**    -> POST : Create a new product | GET : Retrieve all products   
// **/products/:id** -> GET : Retrieve details for product id | PATCH : Update details of product 1 if it exists | DELETE : Remove product id

namespace AltenShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService; // Service pour gérer les opérations liées aux produits.

        public ProductsController(ProductService productService) // Constructeur pour injecter le service ProductService dans le contrôleur. Cela permet d'utiliser le service pour accéder aux données des produits.
        {
            _productService = productService;
        }

        [HttpGet] // -> GET "api/products".
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts() // get tous les produits 
        {
            var products = await _productService.GetProductsAsync(); // appel service pour obtenir la liste produits.
            if (products == null || products.Count == 0) //vérifie si liste nulle si oui -> 404
            {
                return NotFound("No products found.");
            }

            return Ok(products);  // retourne les produits avec un code HTTP 200 OK.
        }

        [HttpGet("{id}")]   // GET api/products/id ? (ex : 1004)
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _productService.GetProductByIdAsync(id); //appelle méthode produit par ID
            if (product == null) // si id non trouvé -> 404
            {
                return NotFound();
            }
            return Ok(product); // retourne le produits avec un code HTTP 200 OK.
        }

        [HttpPost] // -> POST "api/products".
        public async Task<ActionResult<Product>> AddProduct(Product product)
        {
            await _productService.AddProductAsync(product);
            Console.WriteLine("New product added : " + product.Name);
            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }

        [HttpDelete("{id}")] // -> DELETE "api/products/ID"
        public async Task<IActionResult> DeleteProduct(int id)
        {
            await _productService.DeleteProductAsync(id);
            Console.WriteLine("Product deleted : n°" + id);
            return NoContent();
        }

        [HttpPatch("{id}")] // -> PATCH "api/products/ID"
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductUpdateModel updateModel)
        {
            // on check si le produit existe
            var existingProduct = await _productService.GetProductByIdAsync(id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            // on met a jour avec les valeurs
            if (updateModel.Code != null) existingProduct.Code = updateModel.Code;
            if (updateModel.Name != null) existingProduct.Name = updateModel.Name;
            if (updateModel.Description != null) existingProduct.Description = updateModel.Description;
            if (updateModel.Image != null) existingProduct.Image = updateModel.Image;
            if (updateModel.Category != null) existingProduct.Category = updateModel.Category;
            if (updateModel.Price.HasValue) existingProduct.Price = updateModel.Price.Value;
            if (updateModel.Quantity.HasValue) existingProduct.Quantity = updateModel.Quantity.Value;
            if (updateModel.InternalReference != null) existingProduct.InternalReference = updateModel.InternalReference;
            if (updateModel.ShellId.HasValue) existingProduct.ShellId = updateModel.ShellId.Value;
            if (updateModel.InventoryStatus != null) existingProduct.InventoryStatus = updateModel.InventoryStatus;
            if (updateModel.Rating.HasValue) existingProduct.Rating = updateModel.Rating.Value;
            if (updateModel.CreatedAt.HasValue) existingProduct.CreatedAt = updateModel.CreatedAt.Value;
            if (updateModel.UpdatedAt.HasValue) existingProduct.UpdatedAt = updateModel.UpdatedAt.Value;

            await _productService.UpdateProductAsync(existingProduct);

            Console.WriteLine(existingProduct.Name + " Updated !");

            // réponse HTTP 204 -> mise à jour effectuée avec succès.
            return NoContent();
        }
        public class ProductUpdateModel
        {
            public string? Code { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public string? Image { get; set; }
            public string? Category { get; set; }
            public decimal? Price { get; set; }
            public int? Quantity { get; set; }
            public string? InternalReference { get; set; }
            public int? ShellId { get; set; }
            public string? InventoryStatus { get; set; }
            public int? Rating { get; set; }
            public long? CreatedAt { get; set; }
            public long? UpdatedAt { get; set; }
        }
    }

}
