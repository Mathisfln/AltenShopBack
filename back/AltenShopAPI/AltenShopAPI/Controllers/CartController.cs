using AltenShopAPI.Models;
using AltenShopAPI.Services;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AltenShopAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {

        private readonly CartService _cartService;
        private readonly ProductService _productService;

        public CartController(CartService cartService, ProductService productService)
        {
            _cartService = cartService;
            _productService = productService;
        }

        // Action pour obtenir le contenu du panier
        [HttpGet]
        public async Task<ActionResult<Cart>> GetCart()
        {
            //Console.WriteLine("GET CART");

            var cart = await _cartService.GetCartAsync();
            return Ok(cart);
        }

        // Action pour ajouter un produit au panier
        [HttpPost("add")]
        public async Task<IActionResult> AddProductToCart(int productId)
        {
            var product = await _productService.GetProductByIdAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            await _cartService.AddProductToCartAsync(product);
            return NoContent();
        }

        // Action pour supprimer un produit du panier
        [HttpDelete("remove/{productId}")]
        public async Task<IActionResult> RemoveProductFromCart(int productId)
        {
            await _cartService.RemoveProductFromCartAsync(productId);
            return NoContent();
        }

    }
}
