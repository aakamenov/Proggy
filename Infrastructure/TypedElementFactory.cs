using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Markup.Xaml.Templates;

namespace Proggy.Infrastructure
{
    public class TypedElementFactory : RecyclingElementFactory
    {
        public new IList<DataTemplate> Templates
        {
            get => templates;
            set
            {
                if (value is null)
                    return;

                AddItems(value);
            }
        }

        //The DataTemplates are added by using the Add() method at runtime
        //and not the setter so gotta use a hack here
        //in order to register them with the base class
        ObservableCollection<DataTemplate> templates;

        public TypedElementFactory()
        {
            templates = new ObservableCollection<DataTemplate>();
            templates.CollectionChanged += OnTemplateAdded;

            SelectTemplateKey += OnSelectTemplateKey;
        }

        private void OnSelectTemplateKey(object sender, SelectTemplateEventArgs e)
        {
            e.TemplateKey = e.DataContext.GetType().FullName;
        }

        private void OnTemplateAdded(object sender, NotifyCollectionChangedEventArgs e)
        {
            AddItems(e.NewItems.Cast<DataTemplate>());
        }

        private void AddItems(IEnumerable<DataTemplate> items)
        {
            foreach (var entry in items)
            {
                base.Templates.Add(entry.DataType.FullName, entry);
            }
        }
    }
}
