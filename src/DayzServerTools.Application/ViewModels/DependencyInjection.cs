using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

using DayzServerTools.Application.ViewModels.Base;
using DayzServerTools.Application.ViewModels.ItemTypes;
using DayzServerTools.Application.ViewModels.Panes;
using DayzServerTools.Application.ViewModels.SpawnableTypes;
using DayzServerTools.Application.ViewModels.Trader;
using DayzServerTools.Library.Common;

namespace DayzServerTools.Application.ViewModels
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddViewModels(this IServiceCollection services)
        {
            services.AddSingleton<WorkspaceViewModel>();
            services.AddSingleton<ErrorsPaneViewModel>();
            services.AddSingleton<ILimitsDefinitionsProvider>(x =>
                x.GetRequiredService<WorkspaceViewModel>()
            );
            services.AddSingleton<IRandomPresetsProvider>(x =>
                x.GetRequiredService<WorkspaceViewModel>()
            );

            services.AddSingleton<ItemTypesViewModelsFactory>();
            services.AddSingleton<SpawnableTypesViewModelsFactory>();
            services.AddSingleton<TraderViewModelsFactory>();
            services.AddSingleton<ProjectFileViewModelFactory>();

            var factories = Assembly.GetAssembly(typeof(DependencyInjection))
                .ExportedTypes
                .Where(x => !x.IsInterface && x.GetInterfaces()
                    .Any(i => i.IsGenericType && typeof(IFileViewModelFactory<>).IsAssignableFrom(i.GetGenericTypeDefinition()))
                );

            foreach (var factory in factories)
            {
                services.AddSingleton(factory.GetInterfaces().First(), factory);
            }

            return services;
        }
    }
}
