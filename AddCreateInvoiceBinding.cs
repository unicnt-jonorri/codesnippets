using DevExpress.Xpf.NavBar;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Uniconta.API.Plugin;
using Uniconta.API.System;
using Uniconta.ClientTools.Controls;
using Uniconta.Common;

namespace UnicontaIs.Plugins.PageEvents
{
    public class AddCreateInvoiceBinding : PageEventsBase
    {
        public override void Init(object page, CrudAPI api, UnicontaBaseEntity master)
        {
            base.Init(page, api, master);
            AddCheckbox(
                page,
                "Afhenda í verslun",
                "xDeliveredInStore",
                1,
                8,
                3,
                4
            );
        }

        public static void AddCheckbox(
               object pageRoot,
               string labelText,
               string bindingPath,
               int navGrpIndex,
               int row,
               int colLabel,
               int colEditor)
        {
            if (!(pageRoot is FrameworkElement fwe
                 &&  fwe.FindName("InputWindowOrder1") is NavBarControl ctrl 
                  && ctrl.Groups.ElementAtOrDefault(navGrpIndex) is NavBarGroup grp
                      && grp.Items.FirstOrDefault() is Grid grid))
                return;
            var label = new TextBlock
            {
                Foreground = (System.Windows.Media.Brush)fwe.FindResource("BodyTextColorBrush")
            };
            label.SetBinding(TextBlock.TextProperty, new Binding
            {
                Converter = (IValueConverter)fwe.FindResource("GlobalLocalizationValueConverter"),
                ConverterParameter = labelText
            });
            var editor = new CheckEditor
            {
                MaxWidth = 170,
                MinWidth = 170,
                Margin = new Thickness(5, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center
            };
            editor.SetBinding(CheckEditor.IsCheckedProperty, new Binding
            {
                Path = new PropertyPath(bindingPath),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged,
            });
            Grid.SetRow(label, row);
            Grid.SetColumn(label, colLabel);
            Grid.SetRow(editor, row);
            Grid.SetColumn(editor, colEditor);

            grid.Children.Add(label);
            grid.Children.Add(editor);
        }
    }
}
