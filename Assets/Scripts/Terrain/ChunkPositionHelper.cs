using UnityEngine;

namespace PotentialRobot.Terrain
{
    public class ChunkPositionHelper
    {
        private readonly float _chunkSize;
        private readonly float _chunkHeight;

        public ChunkPositionHelper(float chunkSize, float slopeAngleDeg)
        {
            _chunkSize = chunkSize;

            float slopeAngleRad = Mathf.Deg2Rad * slopeAngleDeg;
            _chunkHeight = Mathf.Tan(slopeAngleRad) * _chunkSize;
        }

        public Vector3 CoordinatesToPosition(ChunkCoordinates coordinates)
        {
            float chunkX = coordinates.X * _chunkSize;
            float chunkY = coordinates.Y * -_chunkHeight;
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
