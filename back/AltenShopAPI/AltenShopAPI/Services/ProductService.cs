using AltenShopAPI.Models;
using System.Text.Json;


// Le back-end doit gérer les API suivantes : 

// **/products**    -> POST : Create a new product | GET : Retrieve all products   
// **/products/:id** -> GET : Retrieve details for product id | PATCH : Update details of product 1 if it exists | DELETE : Remove product id


namespace AltenShopAPI.Services
{
    public class ProductService
    {
        private readonly string _filePath = Path.Combine(Directory.GetCurrentDirectory(), "Data", "products.json"); //Chemin du JSON

        public async Task<List<Product>> GetProductsAsync() //Lit les produits depuis le fichier JSON et les renvoie sous forme de liste.
        {
            if (!File.Exists(_filePath)) //Vérifie si le fichier existe
            {
                return new List<Product>(); //Sinon créé une liste vide
            }

            var json = await File.ReadAllTextAsync(_filePath); //On insère les produits dans une variable

            var options = new JsonSerializerOptions //Obligation de rajouter sinon les objets renvoyés sont nulls 
            {
                PropertyNameCaseInsensitive = true,
            };


            //Console.WriteLine($"JSON Content: {json}");
            Console.WriteLine("Products are up to date !");

            var products = JsonSerializer.Deserialize<List<Product>>(json, options); 

            return products;
        }

        public async Task<Product> GetProductByIdAsync(int id) //Trouve un produit par l'ID
        {
            var products = await GetProductsAsync();
            return products.Find(p => p.Id == id);
        }

        public async Task AddProductAsync(Product product) //Ajoute un nv produit
        {
            var products = await GetProductsAsync();
            products.Add(product);
            await SaveProductsAsync(products);
        }

        public async Task DeleteProductAsync(int id) //Supprime un produit par son ID
        {
            var products = await GetProductsAsync();
            var product = products.Find(p => p.Id == id);
            if (product != null)
            {
                products.Remove(product);
                await SaveProductsAsync(products);
            }
        }

        // Méthode pour mettre à jour un produit
        public async Task UpdateProductAsync(Product updatedProduct)
        {
            var products = await GetProductsAsync(); //recup tous les produits
            var existingProduct = products.FirstOrDefault(p => p.Id == updatedProduct.Id); //recherche le produit (id) et recupère les infos dans la liste

            if (existingProduct != null)
            {
                // met à jour les propriétés du produit
                existingProduct.Code = updatedProduct.Code;
                existingProduct.Name = updatedProduct.Name;
                existingProduct.Description = updatedProduct.Description;
                existingProduct.Image = updatedProduct.Image;
                existingProduct.Category = updatedProduct.Category;
                existingProduct.Price = updatedProduct.Price;
                existingProduct.Quantity = updatedProduct.Quantity;
                existingProduct.InternalReference = updatedProduct.InternalReference;
                existingProduct.ShellId = updatedProduct.ShellId;
                existingProduct.InventoryStatus = updatedProduct.InventoryStatus;
                existingProduct.Rating = updatedProduct.Rating;
                existingProduct.CreatedAt = updatedProduct.CreatedAt;
                existingProduct.UpdatedAt = updatedProduct.UpdatedAt;

                await SaveProductsAsync(products); //save produit
            }
        }

        private async Task SaveProductsAsync(List<Product> products) // Met a jour la liste JSON
        {
            var json = JsonSerializer.Serialize(products, new JsonSerializerOptions { WriteIndented = true });
            await File.WriteAllTextAsync(_filePath, json);
        }
    }
}
