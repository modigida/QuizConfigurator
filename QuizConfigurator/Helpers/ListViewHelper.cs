using System.Collections;
using System.Windows.Controls;
using System.Windows;

namespace QuizConfigurator.Helpers;
public static class ListViewHelper
{
    public static readonly DependencyProperty BindableSelectedItemsProperty =
            DependencyProperty.RegisterAttached("BindableSelectedItems", typeof(IList), typeof(ListViewHelper),
                new PropertyMetadata(null, OnBindableSelectedItemsChanged));

    private static void OnBindableSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is ListView listView)
        {
            if (e.OldValue is IList oldSelectedItems)
            {
                listView.SelectionChanged -= ListView_SelectionChanged;
            }
            listView.SelectionChanged += ListView_SelectionChanged;
        }
    }
    private static void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ListView listView)
        {
            IList selectedItems = GetBindableSelectedItems(listView);
            if (selectedItems == null) return;

            if (selectedItems.Count > 0)
            {
                selectedItems.Clear();
            }

            foreach (var item in listView.SelectedItems)
            {
                selectedItems.Add(item);
            }
        }
    }
    public static void SetBindableSelectedItems(DependencyObject element, IList value)
    {
        element.SetValue(BindableSelectedItemsProperty, value);
    }
    public static IList GetBindableSelectedItems(DependencyObject element)
    {
        return (IList)element.GetValue(BindableSelectedItemsProperty);
    }
}
