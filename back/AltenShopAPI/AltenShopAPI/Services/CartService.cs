using AltenShopAPI.Models;
using static AltenShopAPI.Models.Cart;
using System.Text.Json;

namespace AltenShopAPI.Services
{
    public class CartService
    {
        private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "cart.json");

        // Méthode pour obtenir le panier
        public async Task<Cart> GetCartAsync()
        {
            if (!File.Exists(_filePath))
            {
                return new Cart();
            }

            var json = await File.ReadAllTextAsync(_filePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            try
            {
                var cart = JsonSerializer.Deserialize<Cart>(json, options);
                return cart ?? new Cart();
            }
            catch (JsonException ex)
            {
                // Log error
                Console.WriteLine($"Deserialization error: {ex.Message}");
                return new Cart();
            }
        }

        // Méthode pour ajouter un produit au panier
        public async Task AddProductToCartAsync(Product product)
        {
            var cart = await GetCartAsync();

            var existingItem = cart.Items.FirstOrDefault(i => i.Id == product.Id);
            cart.Items.Add(product);
            Console.WriteLine(product.Name + " added to cart");


            await SaveCartAsync(cart);
        }

        // Méthode pour supprimer un produit du panier
        public async Task RemoveProductFromCartAsync(int productId)
        {
            var cart = await GetCartAsync();
            var itemToRemove = cart.Items.FirstOrDefault(i => i.Id == productId);

            if (itemToRemove != null)
            {
                cart.Items.Remove(itemToRemove);
                await SaveCartAsync(cart);
            }
        }

        // Méthode pour enregistrer le panier dans le fichier
        private async Task SaveCartAsync(Cart cart)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(cart, options);
            await File.WriteAllTextAsync(_filePath, json);
        }
    }
}
