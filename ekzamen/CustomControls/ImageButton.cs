using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace soundway.CustomControls
{
    public class ImageButton : Button
    {
        #region Attached properties area
        public string IconPath
        {
            get { return (string)GetValue(IconPathProperty); }
            set { SetValue(IconPathProperty, value); }
        }
        public static readonly DependencyProperty IconPathProperty =
            DependencyProperty.Register("IconPath", typeof(string), typeof(ImageButton), new UIPropertyMetadata(string.Empty));
        #endregion
    }
}
