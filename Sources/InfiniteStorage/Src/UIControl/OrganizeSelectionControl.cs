using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using InfiniteStorage.Properties;

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
				switch(value)
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
