using MinimalAPI.Models;

namespace MinimalAPI.Manager
{
    public interface IPersonManager
    {
        Task AddProduct(PersonModel model);
    }
}
