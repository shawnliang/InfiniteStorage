using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Waveface.Client
{
	/// <summary>
	/// Interaction logic for StepPanel.xaml
	/// </summary>
	public partial class StepPanel : UserControl
	{

        #region Var

        public static readonly DependencyProperty _stepNo = DependencyProperty.Register("StepNo", typeof(int), typeof(StepPanel), null);

        #endregion

        #region Property
        public int StepNo
        {
            get { return (int)GetValue(_stepNo); }
            set { SetValue(_stepNo, value); }
        }
        #endregion

		public StepPanel()
		{
			this.InitializeComponent();
		}
	}
}