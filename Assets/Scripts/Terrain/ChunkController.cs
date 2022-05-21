using System.Collections.Generic;
using PotentialRobot.Terrain.Generation;
using UnityEngine;

namespace PotentialRobot.Terrain
{
    public class ChunkController
    {
        private struct ChunkCoordinates
        {
            public int X { get; private set; }
            public int Y { get; private set; }

            public ChunkCoordinates(int x, int y)
            {
                X = x;
                Y = y;
            }

            public static ChunkCoordinates operator +(ChunkCoordinates a, ChunkCoordinates b)
                => new ChunkCoordinates(a.X + b.X, a.Y + b.Y);

            public static ChunkCoordinates operator -(ChunkCoordinates a, ChunkCoordinates b)
                => new ChunkCoordinates(a.X - b.X, a.Y - b.Y);
        }

        private readonly float _chunkSize;
        private readonly float _viewDistance;
        private readonly Transform _chunkParent;
        private readonly List<GameObject> _chunks;
        private readonly IChunkProvider _chunkProvider;

        public ChunkController(Transform chunkParent, float chunkSize, float viewDistance)
        {
            _chunkParent = chunkParent;
            _chunkSize = chunkSize;
            _viewDistance = viewDistance;
            _chunks = new List<GameObject>();
            _chunkProvider = new ChunkProvider();
        }

        public void UpdateChunkVisibility(Vector3 viewerPosition)
        {
            CleanUpChunks();
            CreateVisibleChunks(viewerPosition);
        }

        private void CleanUpChunks()
        {
            foreach (GameObject chunk in _chunks)
                _chunkProvider.CleanUpChunk(chunk);
        }

        private void CreateVisibleChunks(Vector3 viewerPosition)
        {
            int viewDistanceInChunks = Mathf.RoundToInt(_viewDistance / _chunkSize);

            for (int i = -viewDistanceInChunks; i <= viewDistanceInChunks; i++)
            {
                for (int j = -viewDistanceInChunks; j <= viewDistanceInChunks; j++)
                {
                    var chunkOffset = new ChunkCoordinates(i, j);
                    ChunkCoordinates chunkCoordinates = ChunkCoordinatesFromOffset(viewerPosition, chunkOffset);
                    CreateChunkIfVisible(chunkCoordinates, viewerPosition);
                }
            }
        }

        private ChunkCoordinates ChunkCoordinatesFromOffset(Vector3 viewerPosition, ChunkCoordinates chunkOffsetFromViewer)
        {
            ChunkCoordinates viewerCoordinates = PositionToChunkCoordinate(viewerPosition);
            return viewerCoordinates + chunkOffsetFromViewer;
        }

        private ChunkCoordinates PositionToChunkCoordinate(Vector3 position)
        {
            int coordinateX = Mathf.RoundToInt(position.z / _chunkSize);
            int coordinateY = Mathf.RoundToInt(position.x / _chunkSize);

            return new ChunkCoordinates(coordinateX, coordinateY);
        }

        private void CreateChunkIfVisible(ChunkCoordinates coordinates, Vector3 viewerPosition)
        {
            Vector3 chunkPosition = CalculateChunkPosition(coordinates);
            if (IsChunkVisible(chunkPosition, viewerPosition))
                CreateChunk(chunkPosition);
        }

        private Vector3 CalculateChunkPosition(ChunkCoordinates coordinates)
        {
            float chunkX = coordinates.Y * _chunkSize;
            float chunkY = 0;
            float chunkZ = coordinates.X * _chunkSize;

            return new Vector3(chunkX, chunkY, chunkZ);
        }

        /// <summary>
        /// Calculates distance to chunk center, 
        /// instead of distance to nearest point in chunk.
        /// </summary>
        private bool IsChunkVisible(Vector3 chunkPosition, Vector3 viewerPosition)
        {
            float viewDistanceSqr = _viewDistance * _viewDistance;
            Vector3 viewerToChunk = chunkPosition - viewerPosition;
            float distanceToChunkSqr = Vector3.SqrMagnitude(viewerToChunk);
            return distanceToChunkSqr < viewDistanceSqr;
        }

        private void CreateChunk(Vector3 position)
        {
            GameObject newChunk = _chunkProvider.GetChunk(position, _chunkSize, _chunkParent);
            _chunks.Add(newChunk);
        }
    }
}
