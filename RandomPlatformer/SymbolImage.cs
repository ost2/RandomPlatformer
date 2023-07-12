using System;
namespace RandomPlatformer
{
	public class SymbolImage
	{
		List<string> _fullImage;
		public List<string> FullImage { get { return _fullImage; } }

		public void createImage(string symbols)
		{
			_fullImage = new List<string>();
			_fullImage = symbols.Split(" ; ").ToList();
		}
		public SymbolImage(string symbols)
		{
			_fullImage = new List<string>();
            _fullImage = symbols.Split(" ; ").ToList();
        }

	}
}

