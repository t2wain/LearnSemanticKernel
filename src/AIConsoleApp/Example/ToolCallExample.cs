using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace AIConsoleApp.Example
{
    public static class ToolCallExample
    {
        public class OrderPizzaPlugin
        {
            [KernelFunction("get_pizza_menu")]
            public async Task<int> GetPizzaMenuAsync() => await Task.FromResult(0);

            [KernelFunction("add_pizza_to_cart")]
            [Description("Add a pizza to the user's cart; returns the new item and updated cart")]
            public async Task<int> AddPizzaToCart(int quantity = 1, string specialInstructions = "") =>
                await Task.FromResult(0);

            [KernelFunction("remove_pizza_from_cart")]
            public async Task<int> RemovePizzaFromCart(int pizzaId) => await Task.FromResult(0);

            [KernelFunction("get_pizza_from_cart")]
            [Description("Returns the specific details of a pizza in the user's cart; " 
                + "use this instead of relying on previous messages " 
                + "since the cart may have changed since then.")]
            public async Task<int> GetPizzaFromCart(int pizzaId) => await Task.FromResult(0);

            [KernelFunction("get_cart")]
            [Description("Returns the user's current cart, including the total price and items in the cart.")]
            public async Task<int> GetCart() => await Task.FromResult(0);

            [KernelFunction("checkout")]
            [Description("Checkouts the user's cart; this function will retrieve the payment from the user and complete the order.")]
            public async Task<int> Checkout() => await Task.FromResult(0);
        }


        public static void EX1()
        {
            IKernelBuilder kernelBuilder = Kernel.CreateBuilder();
            kernelBuilder.AddAzureOpenAIChatCompletion(
                 deploymentName: "NAME_OF_YOUR_DEPLOYMENT",
                 apiKey: "YOUR_API_KEY",
                 endpoint: "YOUR_AZURE_ENDPOINT"
            );
            kernelBuilder.Plugins.AddFromType<OrderPizzaPlugin>("OrderPizza");
            Kernel kernel = kernelBuilder.Build();
        }

    }
}
