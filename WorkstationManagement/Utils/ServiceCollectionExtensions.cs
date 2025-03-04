using Microsoft.Extensions.DependencyInjection;
using WorkstationManagement.ViewModels;

namespace WorkstationManagement.Utils;

public static class ServiceCollectionExtensions
{
    public static void AddCommonServices(this IServiceCollection collection)
    {
        collection.AddSingleton<NavigationService>();

        collection.AddTransient<MainWindowViewModel>();
        collection.AddTransient<LoginViewModel>();
        collection.AddTransient<AdminViewModel>();
        collection.AddTransient<UserViewModel>();
    }
}