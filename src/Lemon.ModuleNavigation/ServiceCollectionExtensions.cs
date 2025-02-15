﻿using Lemon.ModuleNavigation.Abstracts;
using Lemon.ModuleNavigation.Core;
using Lemon.ModuleNavigation.Internals;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;

namespace Lemon.ModuleNavigation
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddModule<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TModule>(this IServiceCollection serviceDescriptors)
            where TModule : class, IModule
        {
            serviceDescriptors = serviceDescriptors
                .AddTransient<TModule>()
                .AddKeyedTransient<IModule>(typeof(TModule).Name, (sp, key) => sp.GetRequiredService<TModule>())
                .AddKeyedTransient<IModule, TModule>(nameof(IModule), (sp, key) => sp.GetRequiredService<TModule>())
                .AddKeyedTransient(typeof(TModule).Name, (sp, key) =>
                {
                    var module = sp.GetRequiredService<TModule>();
                    var view = ActivatorUtilities.CreateInstance(sp, module.ViewType);
                    return (view as IView)!;
                })
                .AddKeyedTransient(typeof(TModule).Name, (sp, key) =>
                {
                    var module = sp.GetRequiredService<TModule>();
                    var moduleSp = sp.GetKeyedService<IModuleServiceProvider>(typeof(TModule).Name);
                    if (moduleSp == null)
                    {
                        var viewModel = ActivatorUtilities.CreateInstance(sp, module.ViewModelType);
                        return (viewModel as IModuleNavigationAware)!;
                    }
                    else
                    {
                        var viewModel = ActivatorUtilities.CreateInstance(sp, module.ViewModelType, moduleSp);
                        return (viewModel as IModuleNavigationAware)!;
                    }
                })
                .AddKeyedTransient(typeof(TModule).Name, (sp, key) =>
                 {
                     var module = sp.GetRequiredService<TModule>();
                     if (module is IModuleScope moduleScope)
                     {
                         return new ModuleServiceProvider(moduleScope.ScopeServiceProvider!) as IModuleServiceProvider;
                     }
                     return default!;
                 });
            return serviceDescriptors;
        }

        private static IServiceCollection AddModulesBuilder(this IServiceCollection serviceDescriptors)
        {
            serviceDescriptors = serviceDescriptors.AddSingleton(sp => sp.GetKeyedServices<IModule>(nameof(IModule)));
            return serviceDescriptors;
        }

        public static IServiceCollection AddNavigationSupport(this IServiceCollection serviceDescriptors)
        {
            return serviceDescriptors
                .AddModulesBuilder()
                .AddSingleton<NavigationService>()
                .AddSingleton<IModuleNavigationService<IModule>>(sp => sp.GetRequiredService<NavigationService>())
                .AddSingleton<INavigationService>(sp => sp.GetRequiredService<NavigationService>())
                .AddSingleton<INavigationHandler, NavigationHandler>()
                .AddSingleton<IModuleManager, ModuleManager>()
                .AddSingleton<IRegionManager, RegionManager>();
        }

        public static IServiceCollection AddAppServiceProvider(this IServiceCollection serviceDescriptors,
            IServiceProvider serviceProvider)
        {
            return serviceDescriptors.AddSingleton<IServiceProviderDecorator>(_ => new ServiceProviderDecorator(serviceProvider));
        }
    }
}
