using UnityEngine;

namespace PotentialRobot.Terrain
{
    public class ChunkPositionHelper
    {
        private readonly float _chunkSize;

        public ChunkPositionHelper(float chunkSize)
        {
            _chunkSize = chunkSize;
        }

        public Vector3 CoordinatesToPosition(ChunkCoordinates coordinates)
        {
            float chunkX = coordinates.X * _chunkSize;
            float chunkY = 0;
            float chunkZ = coordinates.Y * _chunkSize;

            return new Vector3(chunkX, chunkY, chunkZ);
        }

        public ChunkCoordinates PositionToCoordinates(Vector3 position)
        {
            int coordinateX = Mathf.RoundToInt(position.x / _chunkSize);
            int coordinateY = Mathf.RoundToInt(position.z / _chunkSize);

            return new ChunkCoordinates(coordinateX, coordinateY);
        }
    }
}
