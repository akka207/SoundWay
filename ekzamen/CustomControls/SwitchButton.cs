using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace soundway.CustomControls
{
    internal class SwitchButton : Button
    {
        #region Attached properties area
        public bool ActiveState
        {
            get { return (bool)GetValue(ActiveStateProperty); }
            set { SetValue(ActiveStateProperty, value); }
        }
        public static readonly DependencyProperty ActiveStateProperty =
            DependencyProperty.Register("ActiveState", typeof(bool), typeof(SwitchButton), new UIPropertyMetadata(false));

        public string MouseOverColor
        {
            get { return (string)GetValue(MouseOverColorProperty); }
            set { SetValue(MouseOverColorProperty, value); }
        }
        public static readonly DependencyProperty MouseOverColorProperty =
            DependencyProperty.Register("MouseOverColor", typeof(string), typeof(SwitchButton), new UIPropertyMetadata("White"));

        public bool DoColorSwitch
        {
            get { return (bool)GetValue(DoColorSwitchProperty); }
            set { SetValue(DoColorSwitchProperty, value); }
        }
        public static readonly DependencyProperty DoColorSwitchProperty =
            DependencyProperty.Register("DoColorSwitch", typeof(bool), typeof(SwitchButton), new UIPropertyMetadata(true));

        public string UnactiveIconPath
        {
            get { return (string)GetValue(UnactiveIconPathProperty); }
            set { SetValue(UnactiveIconPathProperty, value); }
        }
        public static readonly DependencyProperty UnactiveIconPathProperty =
            DependencyProperty.Register("UnactiveIconPath", typeof(string), typeof(SwitchButton), new UIPropertyMetadata(string.Empty));

        public string ActiveIconPath
        {
            get { return (string)GetValue(ActiveIconPathProperty); }
            set { SetValue(ActiveIconPathProperty, value); }
        }
        public static readonly DependencyProperty ActiveIconPathProperty =
            DependencyProperty.Register("ActiveIconPath", typeof(string), typeof(SwitchButton), new UIPropertyMetadata(string.Empty));
        #endregion
    }
}
