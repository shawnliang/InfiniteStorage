#region

using System;

#endregion

namespace Waveface.Model
{
	public class ContentPropertyChangeEventArgs : EventArgs
	{
		#region Property

		public IContentEntity Content { get; private set; }

		public string PropertyName { get; private set; }

		#endregion

		#region Constructor

		public ContentPropertyChangeEventArgs(IContentEntity content, string propertyName)
		{
			Content = content;
			PropertyName = propertyName;
		}

		#endregion
	}
}