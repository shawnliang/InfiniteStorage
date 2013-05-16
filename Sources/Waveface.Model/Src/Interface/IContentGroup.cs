using System.Collections.Generic;

namespace Waveface.Model
{
	public interface IContentGroup : IContentEntity
	{
		#region Property
		IEnumerable<IContentEntity> Contents { get; }
		#endregion
	}
}
