using System;
using System.Windows;
using System.Windows.Controls;

namespace Waveface.Client
{
	/// <summary>
	/// Interaction logic for StepPanel.xaml
	/// </summary>
	public partial class StepPanel : UserControl
	{

		#region Var

		public static readonly DependencyProperty _stepNo = DependencyProperty.Register("StepNo", typeof(Int32), typeof(StepPanel), null);

		#endregion

		#region Property
		public Int32 StepNo
		{
			get { return (Int32)GetValue(_stepNo); }
			set { SetValue(_stepNo, value); }
		}
		#endregion

		public StepPanel()
		{
			this.InitializeComponent();
		}
	}
}