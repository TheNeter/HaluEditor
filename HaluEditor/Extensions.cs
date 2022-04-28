using System.Windows;

namespace ngprojects.HaluEditor
{
    public static class Extensions
    {
        public static bool PointIsNull(this Point p)
        {
            if (p.X == -1 && p.Y == -1)
            {
                return true;
            }
            return false;
        }
    }
}