using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using TreeListView;

namespace Components
{
    public sealed class TreeViewBehavior
    {
        public static bool GetIsTransparent(TreeListViewItem element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            return (bool) element.GetValue(IsTransparentProperty);
        }
        public static void SetIsTransparent(TreeListViewItem element, bool value)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            element.SetValue(IsTransparentProperty, value);
        }
        public static readonly DependencyProperty IsTransparentProperty = DependencyProperty.RegisterAttached("IsTransparent", typeof(bool), typeof(TreeViewBehavior), new FrameworkPropertyMetadata(false, IsTransparent_PropertyChanged));
        private static void IsTransparent_PropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var tvi = (TreeListViewItem)sender;
            var isTransparent = System.Convert.ToBoolean(e.NewValue);

            if (isTransparent)
                tvi.Selected += tvi_Selected;
            else
                tvi.Selected -= tvi_Selected;
        }
        private static void tvi_Selected(object sender, RoutedEventArgs e)
        {
            var treeViewItem = (TreeListViewItem)sender;
            if (!treeViewItem.IsSelected)
                return;

            treeViewItem.Dispatcher.Invoke(() => treeViewItem.IsSelected = false, DispatcherPriority.Send);
        }
    }
}