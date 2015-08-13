using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Input;

namespace Snake
{
    public class Snake
    {
        enum Directions
        {
            Left,
            Right,
            Up,
            Down
        }

        public struct EntityType
        {
            public const int Empty = 0;
            public const int Food  = -1;
            public const int BodyGenStart = 1;
        }

        struct Position
        {
            public int x;
            public int y;
        }

        private GameWindow Window;

        private Random rand = new Random(1234);

        private Directions Direction;
        private Position HeadPosition;
        public int Width;
        public int Height;
        public int[,] Map;
        private int Length;

        public void Reset(GameWindow window, int width, int height)
        {
            this.Window = window;
            this.Direction = Directions.Right;
            this.HeadPosition = new Position { x = 0, y = 0 };
            this.Width = width;
            this.Height = height;
            this.Map = new int[width, height];
            this.Map[HeadPosition.x, HeadPosition.y] = EntityType.BodyGenStart;
            this.Length = 0;
            GenerateFood();
        }

        public Snake(GameWindow window, int width, int height)
        {
            Reset(window, width, height);
        }

        private void GenerateFood()
        {
            int x = rand.Next(0, Width);
            int y = rand.Next(0, Height);

            while (Map[x, y] != EntityType.Empty)
            {
                x = rand.Next(0, Width);
                y = rand.Next(0, Height);
            }

            Map[x, y] = EntityType.Food;
        }

        public void Update(FrameEventArgs e)
        {
            if (Window.Keyboard[Key.Escape])
                Window.Exit();

            if (Window.Keyboard[Key.Right])
            {
                if (Direction != Directions.Left)
                    Direction = Directions.Right;
            }
            else if (Window.Keyboard[Key.Left])
            {
                if (Direction != Directions.Right)
                    Direction = Directions.Left;
            }
            else if (Window.Keyboard[Key.Up])
            {
                if (Direction != Directions.Down)
                    Direction = Directions.Up;
            }
            else if (Window.Keyboard[Key.Down])
            {
                if (Direction != Directions.Up)
                    Direction = Directions.Down;
            }

            switch (Direction)
            {
                case Directions.Right:
                    HeadPosition.x = (HeadPosition.x + 1) % Width;
                    break;
                case Directions.Left:
                    if (HeadPosition.x - 1 < 0)
                        HeadPosition.x = Width - 1;
                    else
                        HeadPosition.x = (HeadPosition.x - 1) % Width;
                    break;
                case Directions.Up:
                    if (HeadPosition.y - 1 < 0)
                        HeadPosition.y = Height - 1;
                    else
                        HeadPosition.y = (HeadPosition.y - 1) % Height;
                    break;
                case Directions.Down:
                    HeadPosition.y = (HeadPosition.y + 1) % Height;
                    break;
            }

            for (int y = 0; y < Height; ++y)
                for (int x = 0; x < Width; ++x)
                    if (Map[x, y] >= EntityType.BodyGenStart)
                        Map[x, y]--;

            if (Map[HeadPosition.x, HeadPosition.y] >= EntityType.BodyGenStart)
            {
                Reset(Window, Width, Height);
                return;
            }

            if (Map[HeadPosition.x, HeadPosition.y] == EntityType.Food)
            {
                Length++;
                GenerateFood();
            }

            Map[HeadPosition.x, HeadPosition.y] = EntityType.BodyGenStart + Length;
        }
    }
}
