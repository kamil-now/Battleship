using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace Battleship
{
    public class Ship
    {
        public int Size { get; }
        public bool IsVertical { get; set; }
        public List<Point> HitPoints { get; private set; } = new List<Point>();
        public List<Point> Points { get; private set; } = new List<Point>();

        public bool Initialized => Points.Count == Size;
        public bool Sank => Points.Intersect(HitPoints).Count() == Points.Count;
        public Point Top => Points.OrderBy(p => p.Y).First();
        public Point Bottom => Points.OrderBy(p => p.Y).Last();
        public Point Left => Points.OrderBy(p => p.X).First();
        public Point Right => Points.OrderBy(p => p.X).Last();

        public Ship(int size)
        {
            Size = size;
        }
        public void AddPoint(Point newPoint)
        {
            if (!Initialized)
            {
                Points.Add(newPoint);
                if (Points.Count == 2)
                {
                    var first = Points.ElementAt(0);
                    var second = Points.ElementAt(1);


                    IsVertical = first.IsAbove(second) || first.IsUnder(second);
                }

            }
        }
    }
}
