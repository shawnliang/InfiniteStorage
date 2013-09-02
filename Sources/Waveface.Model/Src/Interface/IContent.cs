using System;
namespace Waveface.Model
{
	public interface IContent : IContentEntity
	{
		ContentType Type { get; }

		Boolean Liked { get; set; }
	}
}