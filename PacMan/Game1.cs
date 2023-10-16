using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PacMan
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        Random random;
        int gameTick;

        int GlobalOffset_X;
        int GlobalOffset_Y;

        int TileWidth;
        int TileHeight;
        Texture2D TileTexture_Wall_HorizontalFull;
        Texture2D TileTexture_Empty;

        Texture2D Texture_MapBase;
        Texture2D Texture_Map;
        List<List<Tile>> GameMap;
        int MapWidth;
        int MapHeight;

        Texture2D PacManTexture_Full;
        Texture2D PacManTexture_OpenFull_Right;
        Texture2D PacManTexture_OpenFull_Left;
        Texture2D PacManTexture_OpenFull_Up;
        Texture2D PacManTexture_OpenFull_Down;
        Player player;

        List<Ghost> Ghosts = new List<Ghost>();

        bool BeingPressed_M;
        bool DrawWallTiles;



        /////////////////////////////////////////

        #region Startup

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 1200;
            _graphics.PreferredBackBufferHeight = 1000;
            _graphics.ApplyChanges();

            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            random = new Random();
            gameTick = 0;

            TileWidth = 30;
            TileHeight = 30;

            MapWidth = 30;
            MapHeight = 33;
            GameMap = new List<List<Tile>>();

            player = new Player()
            {
                x = MapWidth / 2 * TileWidth,
                y = (MapHeight / 2 - 4) * TileHeight,

                Speed = 2
            };

            for (int i = 0; i < 4; i++)
            {
                Ghosts.Add(new Ghost()
                {
                    X = MapWidth / 2 * TileWidth,
                    Y = (MapHeight / 2 - 4) * TileHeight,

                    Speed = 2
                });
            }

            DrawWallTiles = false;

            GenerateMap();
            GenerateGlobalOffsets();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Texture_MapBase = Content.Load<Texture2D>("Map_BaseColour");
            Texture_Map = Content.Load<Texture2D>("MAP");

            TileTexture_Empty = Content.Load<Texture2D>("EmptyTile");
            TileTexture_Wall_HorizontalFull = Content.Load<Texture2D>("Wall_Horizontal_Full");

            PacManTexture_Full = Content.Load<Texture2D>("PacManFull");
            PacManTexture_OpenFull_Right = Content.Load<Texture2D>("PacManOpenFull_Right");
            PacManTexture_OpenFull_Left = Content.Load<Texture2D>("PacManOpenFull_Left");
            PacManTexture_OpenFull_Up = Content.Load<Texture2D>("PacManOpenFull_Up");
            PacManTexture_OpenFull_Down = Content.Load<Texture2D>("PacManOpenFull_Down");
        }

        #endregion

        /////////////////////////////////////////

        #region Main

        private void GenerateGlobalOffsets()
        {
            GlobalOffset_X = (_graphics.PreferredBackBufferWidth - (TileWidth * MapWidth)) / 2;
            GlobalOffset_Y = (_graphics.PreferredBackBufferHeight - (TileHeight * MapHeight)) / 2;
        }

        private void GenerateMap()
        {
            for (int Y = 0; Y < MapHeight; Y++)
            {
                GameMap.Add(new List<Tile>());
                for (int X = 0; X < MapWidth; X++)
                {
                    GameMap.Last().Add(new Tile() { x = X, y = Y, IsWall = false, TextureTag = "Empty" });
                }
            }

            if (File.Exists("Map.txt"))
            {
                List<string> MapRaw = File.ReadAllLines("Map.txt").ToList();

                for (int y = 0; y < MapRaw.Count() - 1; y++)
                {
                    String[] SplitRow = MapRaw[y].Split(',');

                    for (int x = 0; x < SplitRow.Count(); x++)
                    {
                        if (SplitRow[x] == "1")
                        {
                            try
                            {
                                GameMap[y][x].IsWall = true;
                            }
                            catch
                            {
                                Debug.WriteLine($"INDEX RANGE ERROR: X:{x}, MaxX: {GameMap[0].Count() - 1} \n Y:{y}, MaxY: {GameMap.Count() - 1}");
                            }
                        }
                    }
                }
            }
            else
            {
                File.Create("Map.txt");
            }
        }

        private Rectangle getRect(int x, int y, bool IsPlayer)
        {
            if (IsPlayer)
            {
                return new Rectangle(x + GlobalOffset_X - (TileWidth / 6), y + GlobalOffset_Y - (TileHeight / 6), Convert.ToInt32(TileWidth * 1.33), Convert.ToInt32(TileHeight * 1.33));
            }
            return new Rectangle(x * TileWidth + GlobalOffset_X, y * TileHeight + GlobalOffset_Y, TileWidth, TileHeight);
        }

        private Texture2D getTexture_Player()
        {

            if (player.TextureTag == 1)
            {
                if (player.Direction == "Right")
                {
                    return PacManTexture_OpenFull_Right;
                }
                if (player.Direction == "Left")
                {
                    return PacManTexture_OpenFull_Left;
                }
                if (player.Direction == "Up")
                {
                    return PacManTexture_OpenFull_Up;
                }
                if (player.Direction == "Down")
                {
                    return PacManTexture_OpenFull_Down;
                }
            }
            return PacManTexture_Full;
        }

        #endregion

        #region Controls/Movement

        private void KeyboardHandler()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.A))
            {
                player.DirectionAwait = "Left";
            }
            if (Keyboard.GetState().IsKeyDown(Keys.D))
            {
                player.DirectionAwait = "Right";
            }
            if (Keyboard.GetState().IsKeyDown(Keys.W))
            {
                player.DirectionAwait = "Up";
            }
            if (Keyboard.GetState().IsKeyDown(Keys.S))
            {
                player.DirectionAwait = "Down";
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F))
            {
                _graphics.ToggleFullScreen();
            }

            if (Keyboard.GetState().IsKeyDown(Keys.M) && !BeingPressed_M)
            {
                DrawWallTiles = !DrawWallTiles;
                BeingPressed_M = true;
            }
            else if (Keyboard.GetState().IsKeyUp(Keys.M))
            {
                BeingPressed_M = false;
            }
        }

        private void DirectionHandler()
        {
            if (player.DirectionAwait != player.Direction)
            {
                try
                {
                    if (player.DirectionAwait == "Left")
                    {
                        if (player.y % TileHeight == 0)
                        {
                            if (GameMap[player.y / TileHeight][player.x / TileWidth - 1].IsWall)
                            {
                                player.DirectionAwait = player.Direction;
                                return;
                            }
                            else
                            {
                                player.Direction = player.DirectionAwait;
                                return;
                            }
                        }
                    }
                    else if (player.DirectionAwait == "Up")
                    {
                        if (player.x % TileWidth == 0)
                        {
                            if (GameMap[player.y / TileHeight - 1][player.x / TileWidth].IsWall)
                            {
                                player.DirectionAwait = player.Direction;
                                return;
                            }
                            else
                            {
                                player.Direction = player.DirectionAwait;
                                return;
                            }
                        }
                    }
                    else if (player.DirectionAwait == "Right")
                    {
                        if (player.y % TileHeight == 0)
                        {
                            if (GameMap[player.y / TileHeight][player.x / TileWidth + 1].IsWall)
                            {
                                player.DirectionAwait = player.Direction;
                                return;
                            }
                            else
                            {
                                player.Direction = player.DirectionAwait;
                                return;
                            }
                        }
                    }
                    else if (player.DirectionAwait == "Down")
                    {
                        if (player.x % TileWidth == 0)
                        {
                            if (GameMap[player.y / TileHeight + 1][player.x / TileWidth].IsWall)
                            {
                                player.DirectionAwait = player.Direction;
                                return;
                            }
                            else
                            {
                                player.Direction = player.DirectionAwait;
                                return;
                            }
                        }
                    }
                }
                catch { }
            }
        }

        private void MovmentHandler(GameTime gameTime)
        {
            bool Moved = true;
            if (player.Direction == "Left")
            {
                Movement_WallCollisionHandler("Left", -player.Speed, 0);
            }
            if (player.Direction == "Right")
            {
                Movement_WallCollisionHandler("Right", player.Speed, 0);
            }
            if (player.Direction == "Up")
            {
                Movement_WallCollisionHandler("Up", 0, -player.Speed);
            }
            if (player.Direction == "Down")
            {
                Movement_WallCollisionHandler("Down", 0, player.Speed);
            }
            if (player.Direction == "Still")
            {
                Moved = false;
            }

            if (Moved && gameTick % 6 == 0)
            {
                player.TextureTag += 1;
                if (player.TextureTag > 1)
                {
                    player.TextureTag = 0;
                }
            }
        }

        private void Movement_WallCollisionHandler(string Direction, int DistanceX, int DistanceY)
        {
            int offset = 0;
            if (Direction == "Right")
            {
                offset = TileWidth;
            }
            if (Direction == "Down")
            {
                offset = TileHeight;
            }

            try
            {
                if (Direction == "Left" || Direction == "Right")
                {
                    if (player.x % TileWidth == 0)
                    {
                        if (GameMap[(player.y + DistanceY) / TileHeight][(player.x + DistanceX + offset) / TileWidth].IsWall)
                        {
                            player.Direction = "Still";
                        }
                        else
                        {
                            player.x += DistanceX;
                        }
                    }
                    else
                    {
                        player.x += DistanceX;
                    }
                }
                if (Direction == "Up" || Direction == "Down")
                {
                    if (player.y % TileHeight == 0)
                    {
                        if (GameMap[(player.y + DistanceY + offset) / TileHeight][(player.x + DistanceX) / TileWidth].IsWall)
                        {
                            player.Direction = "Still";
                        }
                        else
                        {
                            player.y += DistanceY;
                        }
                    }
                    else
                    {
                        player.y += DistanceY;
                    }
                }
            }
            catch 
            { 
                if (player.x < TileWidth)
                {
                    player.x = MapWidth * TileWidth - TileWidth * 2;
                }
                else if (player.x > TileWidth)
                {
                    player.x = TileWidth;
                }
            }
        }

        #endregion

        #region GhostMovement

        private void GhostMovementHandler()
        {
            foreach (Ghost ghost in Ghosts)
            {
                List<string> Directions = new List<string>();

                try
                {
                    if (ghost.X % TileWidth == 0 && ghost.Y % TileHeight == 0)
                    {
                        //Determines Available Directions
                        if (!GameMap[ghost.Y / TileHeight][ghost.X / TileWidth - 1].IsWall && ghost.Direction != "Right")
                        {
                            Directions.Add("Left");
                        }
                        if (!GameMap[ghost.Y / TileHeight][ghost.X / TileWidth + 1].IsWall && ghost.Direction != "Left")
                        {
                            Directions.Add("Right");
                        }
                        if (!GameMap[ghost.Y / TileHeight - 1][ghost.X / TileWidth].IsWall && ghost.Direction != "Down")
                        {
                            Directions.Add("Up");
                        }
                        if (!GameMap[ghost.Y / TileHeight + 1][ghost.X / TileWidth].IsWall && ghost.Direction != "Up")
                        {
                            Directions.Add("Down");
                        }

                        //Sets Ghost Direction
                        ghost.Direction = Directions[random.Next(0, Directions.Count())];
                    }

                    //Moves Ghost
                    if (ghost.Direction == "Left")
                    {
                        ghost.X -= ghost.Speed;
                    }
                    if (ghost.Direction == "Right")
                    {
                        ghost.X += ghost.Speed;
                    }
                    if (ghost.Direction == "Up")
                    {
                        ghost.Y -= ghost.Speed;
                    }
                    if (ghost.Direction == "Down")
                    {
                        ghost.Y += ghost.Speed;
                    }
                }
                catch
                {
                    //Moves Ghost to opposite side of screen when it takes a tunnel route
                    if (ghost.X < TileWidth)
                    {
                        ghost.X = MapWidth * TileWidth - TileWidth * 2;
                    }
                    else if (ghost.X > TileWidth)
                    {
                        ghost.X = TileWidth;
                    }
                }
            }
        }

        #endregion

        /////////////////////////////////////////

        #region Fundamentals

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();
            if (Mouse.GetState().RightButton == ButtonState.Pressed)
            {
                player.x = (Mouse.GetState().Position.X - GlobalOffset_X);
                player.y = (Mouse.GetState().Position.Y - GlobalOffset_Y);
            }


            KeyboardHandler();
            DirectionHandler();
            MovmentHandler(gameTime);

            GhostMovementHandler();


            gameTick++;
            if (gameTick > 1000000)
            {
                gameTick = 0;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkBlue);


            //// DRAW START
            _spriteBatch.Begin();
            //////////////////////


            //map base
            _spriteBatch.Draw(Texture_MapBase, new Rectangle(GlobalOffset_X, GlobalOffset_Y, MapWidth * TileWidth, MapHeight * TileHeight), Color.White);

            //map Texture
            _spriteBatch.Draw(Texture_Map, new Rectangle(GlobalOffset_X, GlobalOffset_Y, MapWidth * TileWidth, MapHeight * TileHeight), Color.White);

            //map
            if (DrawWallTiles)
            {
                foreach (List<Tile> yRow in GameMap)
                {
                    foreach (Tile tile in yRow)
                    {
                        if (tile.IsWall)
                        {
                            _spriteBatch.Draw(PacManTexture_Full, getRect(tile.x, tile.y, false), Color.White);
                        }
                        else
                        {
                            _spriteBatch.Draw(TileTexture_Empty, getRect(tile.x, tile.y, false), Color.White);
                        }
                    }
                }
            }

            //player
            if (player.TextureTag == 0)
            {
                _spriteBatch.Draw(getTexture_Player(), getRect(player.x, player.y, true), Color.Yellow);
            }
            else
            {
                _spriteBatch.Draw(getTexture_Player(), getRect(player.x, player.y, true), Color.Yellow);
            }

            //Ghosts
            foreach (Ghost ghost in Ghosts)
            {
                _spriteBatch.Draw(PacManTexture_OpenFull_Up, getRect(ghost.X, ghost.Y, true), Color.White);
                Debug.WriteLine("A");
            }
            

            //////////////////////
            _spriteBatch.End();
            //// DRAW END


            base.Draw(gameTime);
        }

        #endregion

        /////////////////////////////////////////
    }
}