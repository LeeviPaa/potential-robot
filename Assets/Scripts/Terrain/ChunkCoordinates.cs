namespace PotentialRobot.Terrain
{
    public struct ChunkCoordinates
    {
        /// <summary>
        /// Unity's X axis, aka "right"
        /// </summary>
        public int X { get; private set; }
        /// <summary>
        /// Unity's Z axis, aka "forward"
        /// </summary>
        public int Y { get; private set; }

        public ChunkCoordinates(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static ChunkCoordinates operator +(ChunkCoordinates a, ChunkCoordinates b)
            => new ChunkCoordinates(a.X + b.X, a.Y + b.Y);
    }
}
