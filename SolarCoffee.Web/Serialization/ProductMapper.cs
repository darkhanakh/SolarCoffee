using SolarCoffee.Web.ViewModels;

namespace SolarCoffee.Web.Serialization
{
    public static class ProductMapper
    {
        /// <summary>
        /// Maps a Product data model to a ProductModel view model
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public static ProductModel SerializeProductModel(Data.Models.Product product)
        {
            return new ProductModel
            {
                Id = product.Id,
                CreateOn = product.CreateOn,
                UpdatedOn = product.UpdatedOn,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                IsTaxable = product.IsTaxable,
                IsArchived = product.IsArchived
            };
        }
        
        /// <summary>
        /// Maps a ProductModel view model to Product data model
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        public static Data.Models.Product SerializeProductModel(ProductModel product)
        {
            return new Data.Models.Product
            {
                Id = product.Id,
                CreateOn = product.CreateOn,
                UpdatedOn = product.UpdatedOn,
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                IsTaxable = product.IsTaxable,
                IsArchived = product.IsArchived
            };
        }
    }
}