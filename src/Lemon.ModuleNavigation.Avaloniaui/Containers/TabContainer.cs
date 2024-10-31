﻿using Avalonia.Controls;
using Lemon.ModuleNavigation.Core;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Lemon.ModuleNavigation.Avaloniaui.Containers
{
    public class TabContainer : AvaNavigationContainer
    {
        private readonly TabControl _tabControl;
        public TabContainer(TabControl tabControl)
        {
            _tabControl = tabControl;
            Contexts = [];
            Contexts.CollectionChanged += ViewContents_CollectionChanged;
            _tabControl.ContentTemplate = ContainerTemplate;
        }
        public object? SelectedItem
        {
            get
            {
                return _tabControl.SelectedItem;
            }
            set
            {
                _tabControl.SelectedItem = value;
            }
        }
        public override ObservableCollection<NavigationContext> Contexts
        {
            get;
        }

        public override void Activate(NavigationContext target)
        {
            if (!target.RequestNew)
            {
                var targetContext = Contexts.FirstOrDefault(context =>
                {
                    if (NavigationContext.ViewNameComparer.Equals(target, context))
                    {
                        return true;
                    }
                    return false;
                });
                if (targetContext == null)
                {
                    Contexts.Add(target);
                    SelectedItem = target;
                }
                else
                {
                    SelectedItem = targetContext;
                }
            }
            else
            {
                Contexts.Add(target);
                SelectedItem = target;
            }
        }
        public override void DeActivate(NavigationContext target)
        {
            Contexts.Remove(target);
        }
        public void Add(NavigationContext item)
        {
            Contexts.Add(item);
        }
        private void ViewContents_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                if (e.NewItems != null)
                {
                    foreach (var item in e.NewItems)
                    {
                        _tabControl.Items.Add(item);
                    }
                }
            }
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                if (e.OldItems != null)
                {
                    foreach (var item in e.OldItems)
                    {
                        _tabControl.Items.Remove(e.OldItems);
                    }
                }
            }
        }
    }
}
