﻿using Lemon.ModuleNavigation.Abstracts;

namespace Lemon.ModuleNavigation.Core
{
    public class NavigationService : INavigationService
    {
        private readonly List<IModuleNavigationHandler> _handlers = [];
        private readonly List<IViewNavigationHandler> _viewHandlers = [];
        public void RequestModuleNavigate(IModule module, NavigationParameters parameters)
        {
            foreach (var handler in _handlers)
            {
                if (handler is IModuleNavigationHandler<IModule> moduleHandler)
                {
                    moduleHandler.OnNavigateTo(module, parameters);
                }
            }
        }
        public void RequestModuleNavigate(string moduleName, NavigationParameters parameters)
        {
            foreach (var handler in _handlers)
            {
                handler.OnNavigateTo(moduleName, parameters);
            }
        }

        IDisposable IModuleNavigationService<IModule>.BindingNavigationHandler(IModuleNavigationHandler<IModule> moduleHandler)
        {
            _handlers.Add(moduleHandler);
            return new Cleanup<IModuleNavigationHandler>(_handlers, moduleHandler);
        }
        IDisposable IModuleNavigationService.BindingNavigationHandler(IModuleNavigationHandler handler)
        {
            _handlers.Add(handler);
            return new Cleanup<IModuleNavigationHandler>(_handlers, handler);
        }

        public void NavigateToView(string regionName, 
            string viewKey, 
            bool requestNew = false)
        {
            foreach (var handler in _viewHandlers)
            {
                handler.OnNavigateTo(regionName, viewKey, requestNew);
            }
        }

        IDisposable IViewNavigationService.BindingViewNavigationHandler(IViewNavigationHandler handler)
        {
            _viewHandlers.Add(handler);
            return new Cleanup<IViewNavigationHandler>(_viewHandlers, handler);
        }

        public void NavigateToView(string regionName, 
            string viewKey, 
            NavigationParameters parameters, 
            bool requestNew = false)
        {
            foreach (var handler in _viewHandlers)
            {
                handler.OnNavigateTo(regionName, viewKey, requestNew);
            }
        }

        private class Cleanup<T>(List<T> handlers, T handler)
            : IDisposable
        {
            public void Dispose()
            {
                handlers?.Remove(handler);
            }
        }
    }
}
