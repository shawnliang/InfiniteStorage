using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Waveface.Model
{
	public class ContentPropertyChangeEventArgs:EventArgs
	{
		#region Property
		public IContentEntity Content { get; private set; }

		public string PropertyName { get; private set; }
		#endregion

		#region Constructor
		public ContentPropertyChangeEventArgs(IContentEntity content, string propertyName)
		{
			this.Content = content;
			this.PropertyName = propertyName;
		}
		#endregion
	}
}
