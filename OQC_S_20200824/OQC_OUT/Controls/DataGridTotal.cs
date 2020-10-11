using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace OQC_OUT
{
    [TemplatePart(Name = "total_row", Type = typeof(Grid))]
    public class DataGridTotal : DataGrid
    {
        DataGrid TotalRow;
        List<object> totalRowItemSource;
        public override void OnApplyTemplate()
        {
            TotalRow = GetTemplateChild("TotalRow") as DataGrid;

            TotalRow.Background = new SolidColorBrush(Colors.AliceBlue);
            TotalRow.Visibility = Visibility.Visible;
            int displayindex = 0;
            if (TotalRow.Columns != null)
                TotalRow.Columns.Clear();

            foreach (var item in this.Columns)
            {

                DataGridTextColumn cl = new DataGridTextColumn();
                cl.Header = item.Header;
                cl.Width = item.Width;
                cl.DisplayIndex = item.DisplayIndex = displayindex++;

                Binding widthBd = new Binding();
                widthBd.Source = item;
                widthBd.Mode = BindingMode.TwoWay;
                widthBd.Path = new PropertyPath(DataGridColumn.WidthProperty);
                BindingOperations.SetBinding(cl, DataGridColumn.WidthProperty, widthBd);

                Binding visibleBd = new Binding();
                visibleBd.Source = item;
                visibleBd.Mode = BindingMode.TwoWay;
                visibleBd.Path = new PropertyPath(DataGridColumn.VisibilityProperty);
                BindingOperations.SetBinding(cl, DataGridColumn.VisibilityProperty, visibleBd);

                Binding indexBd = new Binding();
                indexBd.Source = item;
                indexBd.Mode = BindingMode.TwoWay;
                indexBd.Path = new PropertyPath(DataGridColumn.DisplayIndexProperty);
                BindingOperations.SetBinding(cl, DataGridColumn.DisplayIndexProperty, indexBd);

                cl.Binding = (item as DataGridTextColumn).Binding;

                TotalRow.Columns.Add(cl);
            }
            this.TotalRow.ItemsSource = totalRowItemSource;
            base.OnApplyTemplate();
        }
        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);

            Type itemType = null;
            totalRowItemSource = new List<object>();
            object obj = null;
            if (newValue == null)
            {
                return;
            }

            foreach (var item in newValue)
            {

                itemType = item.GetType();
                obj = Activator.CreateInstance(itemType, true);
                break;
            }
            if (itemType == null)
                return;

            PropertyInfo[] ps = itemType.GetProperties();
            foreach (var item in newValue)
            {

                foreach (PropertyInfo property in ps)
                {
                    object tmpValue = property.GetValue(item, null);
                    object totalValue = property.GetValue(obj, null);

                    if (property.PropertyType == typeof(int))
                    {
                        totalValue = (int)tmpValue + (int)totalValue;
                        property.SetValue(obj, totalValue, null);
                    }
                    else if (property.PropertyType == typeof(double))
                    {
                        totalValue = (double)tmpValue + (double)totalValue;
                        property.SetValue(obj, totalValue, null);
                    }
                }
            }
            totalRowItemSource.Add(obj);
            if (TotalRow != null)
                this.TotalRow.ItemsSource = totalRowItemSource;

        }
    }
}
