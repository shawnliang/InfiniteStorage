using InfiniteStorage.Properties;
using System;
using System.Windows.Forms;

namespace InfiniteStorage
{
	public partial class OrganizeSelectionControl : UserControl
	{
		public OrganizeSelectionControl()
		{
			InitializeComponent();
		}

		private void OrganizeSelectionControl_Load(object sender, EventArgs e)
		{
			if (!DesignMode)
			{
				var organizeMethod = (OrganizeMethod)Settings.Default.OrganizeMethod;
				OrganizeBy = organizeMethod;
			}

		}

		public OrganizeMethod OrganizeBy
		{
			get
			{
				if (radioYYYY.Checked)
					return OrganizeMethod.Year;
				else if (radioYYYYMM.Checked)
					return OrganizeMethod.YearMonth;
				else
					return OrganizeMethod.YearMonthDay;
			}

			set
			{
				switch (value)
				{
					case OrganizeMethod.Year:
						radioYYYY.Checked = true;
						break;
					case OrganizeMethod.YearMonth:
						radioYYYYMM.Checked = true;
						break;
					case OrganizeMethod.YearMonthDay:
						radioYYYYMMDD.Checked = true;
						break;
					default:
						throw new NotImplementedException();
				}
			}
		}
	}
}
