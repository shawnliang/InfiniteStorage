using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace InfiniteStorage
{
	[ToolboxBitmap(typeof(TabControl))]
	public class TabControlEx : TabControl
	{
		private bool m_HideTabs;


		#region Public Property
		/// <summary>
		/// Gets or sets the index of the page.
		/// </summary>
		/// <value>The index of the page.</value>
		public int PageIndex
		{
			get
			{
				return SelectedIndex + 1;
			}
			set
			{
				SelectedIndex = value - 1;
			}
		}

		/// <summary>
		/// Gets the page count.
		/// </summary>
		/// <value>The page count.</value>
		public int PageCount
		{
			get
			{
				return TabPages.Count;
			}
		}

		public Boolean IsLastPage
		{
			get
			{
				return PageIndex == this.PageCount;
			}
		}

		[DefaultValue(false), RefreshProperties(RefreshProperties.All)]
		public bool HideTabs
		{
			get { return m_HideTabs; }
			set
			{
				if (m_HideTabs == value)
					return;
				m_HideTabs = value;
				if (value)
					Multiline = true;
				UpdateStyles();
			}
		}

		[RefreshProperties(RefreshProperties.All)]
		public new bool Multiline
		{
			get
			{
				if (HideTabs)
					return true;
				return base.Multiline;
			}
			set
			{
				base.Multiline = HideTabs || value;
			}
		}

		public override Rectangle DisplayRectangle
		{
			get
			{
				if (HideTabs)
				{
					return new Rectangle(0, 0, Width, Height);
				}
				Int32 tabStripHeight = default(Int32);
				Int32 itemHeight = default(Int32);

				itemHeight = Alignment <= TabAlignment.Bottom ? ItemSize.Height : ItemSize.Width;

				if (Appearance == TabAppearance.Normal)
				{
					tabStripHeight = 5 + (itemHeight * RowCount);
				}
				else
				{
					tabStripHeight = (3 + itemHeight) * RowCount;
				}
				switch (Alignment)
				{
					case TabAlignment.Top:
						return new Rectangle(4, tabStripHeight, Width - 8, Height - tabStripHeight - 4);
					case TabAlignment.Bottom:
						return new Rectangle(4, 4, Width - 8, Height - tabStripHeight - 4);
					case TabAlignment.Left:
						return new Rectangle(tabStripHeight, 4, Width - tabStripHeight - 4, Height - 8);
					case TabAlignment.Right:
						return new Rectangle(4, 4, Width - tabStripHeight - 4, Height - 8);
				}
				return base.DisplayRectangle;
			}
		}
		#endregion


		#region Protected Method
		/// <summary>
		/// Processes a command key.
		/// </summary>
		/// <param name="msg">A <see cref="T:System.Windows.Forms.Message" />, passed by reference, that represents the window message to process.</param>
		/// <param name="keyData">One of the <see cref="T:System.Windows.Forms.Keys" /> values that represents the key to process.</param>
		/// <returns>
		/// true if the character was processed by the control; otherwise, false.
		/// </returns>
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (HideTabs)
			{
				if (keyData == (Keys.Control | Keys.Tab))
				{
					return true;
				}
				if (keyData == (Keys.Control | Keys.Shift | Keys.Tab))
				{
					return true;
				}
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
		#endregion

		#region Public Method
		/// <summary>
		/// Firsts the page.
		/// </summary>
		public void FirstPage()
		{
			PageIndex = 1;
		}

		/// <summary>
		/// Lasts the page.
		/// </summary>
		public void LastPage()
		{
			PageIndex = PageCount;
		}

		/// <summary>
		/// Previouses the page.
		/// </summary>
		public void PreviousPage()
		{
			var pageIndex = this.PageIndex;
			if (pageIndex <= 1)
				return;

			this.PageIndex = pageIndex - 1;
		}

		/// <summary>
		/// Nexts the page.
		/// </summary>
		public void NextPage()
		{
			var pageIndex = this.PageIndex;
			if (pageIndex >= PageCount)
				return;

			Cursor.Current = Cursors.WaitCursor;

			this.PageIndex = pageIndex + 1;
		}
		#endregion
	}
}
