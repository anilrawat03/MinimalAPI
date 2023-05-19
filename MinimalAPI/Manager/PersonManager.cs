using MinimalAPI.Models;

namespace MinimalAPI.Manager
{
    public class PersonManager : IPersonManager
    {
        public async Task AddProduct(PersonModel model)
        {
            Console.WriteLine("Model"+ model.Name);
        }
    }
}
