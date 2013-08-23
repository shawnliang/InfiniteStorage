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