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
			AnchorPoint = CCPoint.AnchorLowerLeft;
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
				UpdateValueDisplay();
			}
		}

		public bool MergedThisRound { get; set; }

		void UpdateNumberSprite(int number)
		{
			RemoveChild(_numberSprite, false);
			var sprite = new CCSprite(string.Format("tiles/{0}", number));
			sprite.Color = CCColor3B.White;
			sprite.ContentSize = ContentSize;
			sprite.Position = ContentSize.Center;
			_numberSprite = sprite;
			AddChild(_numberSprite);
		}

		void UpdateValueDisplay()
		{
			CCColor3B color;

			switch (_value)
			{
				case 256:
					color = new CCColor3B(CCColor4B.Aquamarine);
					break;
				case 512:
					color = new CCColor3B(CCColor4B.Gray);
					break;
				case 1024:
					color = new CCColor3B(CCColor4B.Black);
					break;
				case 2048:
					color = new CCColor3B(CCColor4B.Transparent);
					break;
				default:
					color = new CCColor3B(CCColor4B.Transparent);
					break;
			}

			Color = color;
			UpdateNumberSprite(_value);
		}
	}
}

