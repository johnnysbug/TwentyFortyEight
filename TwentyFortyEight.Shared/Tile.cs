using CocosSharp;

namespace TwentyFortyEight.Shared
{
	public class Tile : CCSprite
	{
		int _value;
		CCSprite _numberSprite;

		public Tile() : base("tile")
		{
			ContentSize = new CCSize(210, 210);
			AnchorPoint = CCPoint.AnchorMiddle;
		}

		public Tile(int number) : this()
		{
			Value = number;
		}

		public int Value
		{
			get
			{
				return _value;
			}
			set
			{
				_value = value;
				UpdateNumberSprite(_value);
			}
		}

		public bool MergedThisRound { get; set; }

		void UpdateNumberSprite(int number)
		{
			if (_numberSprite != null)
			{
				_numberSprite.RemoveFromParent();
			}
			var sprite = new CCSprite(string.Format("tiles/{0}", number));
			sprite.Color = CCColor3B.White;
			sprite.ContentSize = ContentSize;
			sprite.Position = ContentSize.Center;
			_numberSprite = sprite;
			AddChild(_numberSprite);
		}

		public override string ToString()
		{
			return string.Format("[Tile: Value={0}, MergedThisRound={1}]", Value, MergedThisRound);
		}
	}
}

