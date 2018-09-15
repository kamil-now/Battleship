using Xamarin.Forms;

namespace Battleship
{
    public static class Extensions
    {
        public static bool IsLeftTo(this Point top, Point bottom) => top.X == bottom.X - 1 && top.Y == bottom.Y;
        public static bool IsAbove(this Point left, Point right) => left.X == right.X && left.Y == right.Y - 1;

        public static bool IsRightTo(this Point right, Point left) => left.IsLeftTo(right);
        public static bool IsUnder(this Point bottom, Point top) => top.IsAbove(bottom);

        public static Point Coordinates(this Button btn) => new Point(Grid.GetColumn(btn), Grid.GetRow(btn));
    }
}
