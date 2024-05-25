using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace soundway.CustomControls
{
    internal class CustomTextBlock:TextBlock
    {
        #region Attached properties area
        public bool ActiveState
        {
            get { return (bool)GetValue(ActiveStateProperty); }
            set { SetValue(ActiveStateProperty, value); }
        }
        public static readonly DependencyProperty ActiveStateProperty =
            DependencyProperty.Register("ActiveState", typeof(bool), typeof(CustomTextBlock), new UIPropertyMetadata(false));
        #endregion
    }
}
