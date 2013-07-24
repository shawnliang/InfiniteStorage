namespace Waveface.Model
{
	public interface IContent : IContentEntity
	{
		ContentType Type { get; }

		bool Liked { get; set; }
	}
}