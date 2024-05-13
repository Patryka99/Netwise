using System;
using Microsoft.Extensions.DependencyInjection;

namespace NetWise
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = new ServiceCollection()
                .AddHttpClient()
                .AddTransient<CatService>()
                .BuildServiceProvider();

            var catFactService = serviceProvider.GetService<CatService>();

            catFactService.GetAndSaveCatFactAsync("Cat.txt").Wait();
        }
    }
}
