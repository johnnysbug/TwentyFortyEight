using CocosSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TwentyFortyEight.Shared.Layers
{
	public class GameStartLayer : CCLayerColor
	{
		const int START_TILES = 2;
		const int GRID_SIZE = 4;
		const int WIN_TILE = 2048;

		const int SCORE_X_OFFSET = 270;

		readonly Tile[,] gridArray = new Tile[GRID_SIZE, GRID_SIZE];

		readonly CCSequence rotate = new CCSequence(new CCRotateBy (0.2f, 45), new CCDelayTime(0.2f), new CCRotateBy (0.2f, -95));
		CCCallFuncN removeTile = new CCCallFuncN (node => node.RemoveFromParent ());

		float tileMarginHorizontal;
		float tileMarginVertical;

		float columnHeight;
		float columnWidth;
		int score;

		CCSprite grid;
		CCLabel scoreLabel;
		CCLabel highScoreLabel;
		Tile emptyTile = new Tile();

		public static CCScene GameStartLayerScene (CCWindow mainWindow)
		{
			mainWindow.SetDesignResolutionSize(1080, 1920, CCSceneResolutionPolicy.ShowAll);

			var scene = new CCScene (mainWindow);
			var layer = new GameStartLayer();
			layer.Color = new CCColor3B(new CCColor4B(13.0f/255.0f, 107.0f/255.0f, 142.0f/255.0f, 1.0f));
			layer.Opacity = 255;
			scene.AddChild (layer);

			return scene;
		}

		protected override void AddedToScene()
		{
			base.AddedToScene();

			Scene.SceneResolutionPolicy = CCSceneResolutionPolicy.ShowAll;

			grid = new CCSprite("grid");

			grid.Position = VisibleBoundsWorldspace.Center;
			grid.ContentSize = new CCSize(900, 900);

			var header = new CCSprite("header");

			header.Position = VisibleBoundsWorldspace.Center.Offset(0, 480);

			var scoreTextLabel = new CCLabel("Score", "Lato-Black", 42);
			var highScoreTextLabel = new CCLabel("High Score", "Lato-Black", 42);
			scoreLabel = new CCLabel(Score.ToString(), "Lato-Black", 144);
			highScoreLabel = new CCLabel(Score.ToString(), "Lato-Black", 144);

			scoreTextLabel.Color = new CCColor3B(CCColor4B.White);
			highScoreTextLabel.Color = new CCColor3B(CCColor4B.White);
			scoreLabel.Color = new CCColor3B(CCColor4B.White);
			highScoreLabel.Color = new CCColor3B(CCColor4B.White);

			scoreTextLabel.Position = new CCPoint(VisibleBoundsWorldspace.MinX + SCORE_X_OFFSET, VisibleBoundsWorldspace.MaxY - 200);
			highScoreTextLabel.Position = new CCPoint(VisibleBoundsWorldspace.MaxX - SCORE_X_OFFSET, VisibleBoundsWorldspace.MaxY - 200);
			scoreLabel.Position = new CCPoint(VisibleBoundsWorldspace.MinX + SCORE_X_OFFSET, VisibleBoundsWorldspace.MaxY - 300);
			highScoreLabel.Position = new CCPoint(VisibleBoundsWorldspace.MaxX - SCORE_X_OFFSET, VisibleBoundsWorldspace.MaxY - 300);

			scoreTextLabel.AnchorPoint = CCPoint.AnchorMiddle;
			highScoreTextLabel.AnchorPoint = CCPoint.AnchorMiddle;
			scoreLabel.AnchorPoint = CCPoint.AnchorMiddle;
			highScoreLabel.AnchorPoint = CCPoint.AnchorMiddle;

			SetupBackground();

			InitializeGridArray();

			SpawnStartTiles();

			SetupGestureRecognizers();

			AddChild(scoreTextLabel);
			AddChild(highScoreTextLabel);
			AddChild(scoreLabel);
			AddChild(highScoreLabel);
			AddChild (grid);
			AddChild(header);
		}

		public int Score
		{
			get
			{
				return score;
			}
			set
			{
				score = value;
				UpdateScore(score);
			}
		}

		void UpdateScore(int newScore)
		{
			scoreLabel.Text = newScore.ToString();
		}

		void SetupBackground()
		{
			// load one tile to read the dimensions
			var tile = new Tile();

			columnWidth = tile.ContentSize.Width;
			columnHeight = tile.ContentSize.Height;

			// calculate the margin by subtracting the tile sizes from the grid size
			tileMarginHorizontal = (grid.ContentSize.Width - (4 * columnWidth)) / (4+1);
			tileMarginVertical = (grid.ContentSize.Height - (4 * columnWidth)) / (4+1);

			// set up initial x and y positions
			float x;
			float y = tileMarginVertical;
			for (int i = 0; i < 4; i++) {
				// iterate through each row
				x = tileMarginHorizontal;
				for (int j = 0; j < 4; j++) {
					//  iterate through each column in the current row
					var backgroundTile =  new CCSprite("empty");
					backgroundTile.ContentSize = new CCSize(columnWidth, columnHeight);
					backgroundTile.AnchorPoint = CCPoint.AnchorMiddle;
					backgroundTile.Position = new CCPoint(x + columnWidth / 2, y + columnHeight / 2);
					grid.AddChild(backgroundTile);
					x+= columnWidth + tileMarginHorizontal;
				}
				y += columnHeight + tileMarginVertical;
			}
		}

		void SetupGestureRecognizers()
		{
			var swipeLeft = new CCEventListenerTouchAllAtOnce();
			swipeLeft.OnTouchesEnded = HandleTouchesMoved;
			AddEventListener(swipeLeft, this);
		}

		void HandleTouchesMoved(List<CCTouch> touches, CCEvent touchEvent)
		{
			if (touches.Count == 1)
			{
				var xDiff = touches[0].Delta.X;
				var yDiff = touches[0].Delta.Y;

				var diff = xDiff - yDiff;

				if (Math.Abs(diff) > 25)
				{
					if (Math.Abs(xDiff) > Math.Abs(yDiff))
					{
						Move(new CCPoint((xDiff < 0) ? -1 : 1, 0));
					}
					else
					{
						Move(new CCPoint(0, (yDiff < 0) ? -1 : 1));
					}
				}
			}
		}

		void InitializeGridArray()
		{
			for (int i = 0; i < GRID_SIZE; i++)
			{
				for (int j = 0; j < GRID_SIZE; j++)
				{
					gridArray[i, j] = emptyTile;
				}
			}
		}

		void SpawnStartTiles() 
		{
			for (int i = 0; i < START_TILES; i++) 
			{
				SpawnRandomTile();
			}
		}

		void SpawnRandomTile()
		{
			var spawned = false;
			while (!spawned)
			{
				var randomRow = CCRandom.GetRandomInt(0, GRID_SIZE) % GRID_SIZE;
				var randomColumn = CCRandom.GetRandomInt(0, GRID_SIZE) % GRID_SIZE;
				var positionFree = (gridArray[randomColumn, randomRow] == emptyTile);
				if (positionFree)
				{
					AddTileAtColumn(randomColumn, randomRow);
					spawned = true;
				}
			}
		}

		void AddTileAtColumn(int column, int row)
		{
			var number = (new CCFastRandom().NextInt() % 2 + 1) * 2;
			var tile = new Tile(number);
			gridArray[column, row] = tile;
			tile.Scale = 0.0f;
			grid.AddChild(tile);
			tile.Position = PositionForColumn(column, row);
			var delay = new CCDelayTime(0.3f);
			var scaleUp = new CCScaleTo(0.2f, 1.0f);
			var sequence = new CCSequence(delay, scaleUp);
			tile.RunAction(sequence);
		}

		CCPoint PositionForColumn(int column, int row)
		{
			var x = tileMarginHorizontal + column * (tileMarginHorizontal + columnWidth);
			var y = tileMarginVertical + row * (tileMarginVertical + columnHeight);
			return new CCPoint(x + columnWidth / 2, y + columnHeight / 2);
		}

		bool IsIndexValidAndUnoccupied(int x, int y) 
		{
			var indexValid = IsIndexValid(x, y);
			return indexValid && gridArray[x, y] == emptyTile;
		}

		bool IsIndexValid(int x, int y)
		{
			var indexValid = true;
			indexValid &= x >= 0;
			indexValid &= y >= 0;
			if (indexValid)
			{
				indexValid &= x < gridArray.GetLength(0);
				if (indexValid)
				{
					indexValid &= y < gridArray.GetLength(1);
				}
			}
			return indexValid;
		}

		Tile TileForIndex(int x, int y) 
		{
			if (!IsIndexValid(x, y)) 
			{
				return emptyTile;
			} 
			else 
			{
				return gridArray[x, y];
			}
		}

		void Move(CCPoint direction)
		{
			var movedTilesThisRound = false;
			// apply negative vector until reaching boundary, this way 
			// we get the tile that is the furthest away bottom left corner
			var currentX = 0;
			var currentY = 0;
			// Move to relevant edge by applying direction until reaching border
			while (IsIndexValid(currentX, currentY)) 
			{
				var newX = currentX + (int)direction.X;
				var newY = currentY + (int)direction.Y;
				if (IsIndexValid(newX, newY)) 
				{
					currentX = newX;
					currentY = newY;
				} else {
					break;
				}
			}

			// store initial row value to reset after completing each column
			var initialY = currentY;
			// define changing of x and y value (moving left, up, down or right?)
			var xChange = (int)-direction.X;
			var yChange = (int)-direction.Y;
			if (xChange == 0) 
			{
				xChange = 1;
			}
			if (yChange == 0) 
			{
				yChange = 1;
			}

			// visit column for column
			while (IsIndexValid(currentX, currentY)) 
			{
				while (IsIndexValid(currentX, currentY)) 
				{
					// get tile at current index
					var tile = gridArray[currentX, currentY];
					if (tile == emptyTile) 
					{
						// if there is no tile at this index -> skip
						currentY += yChange;
						continue;
					}

					// store index in temp variables to change them and store new location of this tile
					var newX = currentX;
					var newY = currentY;

					/* find the farthest position by iterating in direction of the vector until we reach border of grid or an occupied cell*/
					while (IsIndexValidAndUnoccupied(newX + (int)direction.X, newY + (int)direction.Y)) 
					{
						newX += (int)direction.X;
						newY += (int)direction.Y;
					}

					var performMove = false;
					/* If we stopped moving in vector direction, but next index in vector direction is valid, this means the cell is occupied. Let's check if we can merge them*/
					if (IsIndexValid(newX + (int)direction.X, newY + (int)direction.Y)) 
					{
						// get the other tile
						var otherTileX = newX + (int)direction.X;
						var otherTileY = newY + (int)direction.Y;
						var otherTile = gridArray[otherTileX, otherTileY];
						// compare value of other tile and also check if the other thile has been merged this round
						if (tile.Value == otherTile.Value && !otherTile.MergedThisRound) 
						{
							// merge tiles
							MergeTileAtIndex(currentX, currentY, otherTileX, otherTileY);
							movedTilesThisRound = true;
						} else 
						{
							// we cannot merge so we want to perform a move
							performMove = true;
						}
					} 
					else 
					{
						// we cannot merge so we want to perform a move
						performMove = true;
					}
					if (performMove) {
						// Move tile to furthest position
						if (newX != currentX || newY !=currentY) {
							// only move tile if position changed
							MoveTile(tile, currentX, currentY, newX, newY);
							movedTilesThisRound = true;
						}
					}

					// move further in this column
					currentY += yChange;
				}
				// move to the next column, start at the inital row
				currentX += xChange;
				currentY = initialY;
			}

			if (movedTilesThisRound) {
				NextRound();
			}
			if (!IsMovePossible())
			{
				Lose();
			}
		}

		void MoveTile(CCNode tile, int currentX, int currentY, int newX, int newY)
		{
			gridArray[newX, newY] = gridArray[currentX, currentY];
			gridArray[currentX, currentY] = emptyTile;
			var newPosition = PositionForColumn(newX, newY);
			var moveTo = new CCMoveTo(0.2f, newPosition);
			tile.RunAction(moveTo);
		}

		void NextRound()
		{
			SpawnRandomTile();
			for (int i = 0; i < GRID_SIZE; i++)
			{
				for (int j = 0; j < GRID_SIZE; j++)
				{
					var tile = gridArray[i, j];
					if (tile != emptyTile)
					{
						// reset merged flag
						tile.MergedThisRound = false;
					}
				}
			}

			if (!IsMovePossible())
			{
				Lose();
			}
		}

		bool IsMovePossible()
		{
			for (int i = 0; i < GRID_SIZE; i++)
			{
				for (int j = 0; j < GRID_SIZE; j++)
				{
					var tile = gridArray[i, j];
					// no tile at this position
					if (tile == emptyTile)
					{
						// move possible, we have a free field
						return true;
					}
					else
					{
						// there is a tile at this position. Check if this tile could move
						var topNeighbor = TileForIndex(i, j + 1);
						var bottomNeighbor = TileForIndex(i, j - 1);
						var leftNeighbor = TileForIndex(i - 1, j);
						var rightNeighbor = TileForIndex(i + 1, j);
						var neighors = new [] { topNeighbor, bottomNeighbor, leftNeighbor, rightNeighbor };
						foreach (var neighborTile in neighors)
						{
							if (neighborTile != emptyTile)
							{
								var neighbor = neighborTile;
								if (neighbor.Value == tile.Value)
								{
									return true;
								}
							}
						}
					}
				}
			}
			return false;
		}

		void MergeTileAtIndex(int x, int y, int xOtherTile, int yOtherTile) 
		{
			// 1) update the game data
			var mergedTile = gridArray[x, y];
			var otherTile = gridArray[xOtherTile, yOtherTile];
			Score += mergedTile.Value + otherTile.Value;
			otherTile.MergedThisRound = true;
			if (otherTile.Value == WIN_TILE) 
			{
				Win();
			}

			gridArray[x, y] = emptyTile;

			// 2) update the UI
			var otherTilePosition = PositionForColumn(xOtherTile, yOtherTile);
			var moveTo = new CCMoveTo(0.2f, otherTilePosition);
			var remove = new CCRemoveSelf();
			var mergeTile = new CCCallFunc(() => 
			{
				otherTile.Value *= 2;
			});

			var sequence = new CCSequence(moveTo, mergeTile, remove);

			mergedTile.RunAction(sequence);
		}

		void Win()
		{
			//throw new NotImplementedException();
		}

		void Lose()
		{
			for (var column = GRID_SIZE - 1; column > -1; column--)
			{
				for (var row = GRID_SIZE - 1; row > -1; row--)
				{
					var tile = gridArray[column, row];
					tile.AnchorPoint = CCPoint.AnchorUpperLeft;
					var newPosition = PositionForColumn(column, row);
					newPosition.Y = 0;
					var delay = new CCDelayTime(1.0f);
					var moveTo = new CCMoveTo(1.8f, newPosition);
					tile.RunActions(delay, moveTo, removeTile);
					tile.RepeatForever(rotate);

				}
			}
		}
	}
}

