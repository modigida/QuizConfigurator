using System.Collections;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;

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
                listView.MouseDoubleClick -= ListView_MouseDoubleClick;
            }
            listView.SelectionChanged += ListView_SelectionChanged;
            listView.MouseDoubleClick += ListView_MouseDoubleClick;
        }
    }
    private static void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ListView listView)
        {
            IList selectedItems = GetBindableSelectedItems(listView);
            if (selectedItems != null)
            {

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
    }
    private static void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is ListView listView)
        {
            var clickedItem = (e.OriginalSource as FrameworkElement)?.DataContext;

            if (clickedItem != null && listView.Items.Contains(clickedItem))
            {
                listView.SelectedItems.Clear();

                listView.SelectedItems.Add(clickedItem);

                IList selectedItems = GetBindableSelectedItems(listView);
                if (selectedItems != null)
                {
                    selectedItems.Clear();
                    selectedItems.Add(clickedItem);
                }
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
