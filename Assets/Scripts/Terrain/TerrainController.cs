using System.Collections.Generic;
using PotentialRobot.Terrain.Generation;
using UnityEngine;

namespace PotentialRobot.Terrain
{
    public class TerrainController : MonoBehaviour
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

        [SerializeField]
        private float _chunkSize = 1f;
        [SerializeField]
        private float _viewDistance = 5f;
        [SerializeField]
        private Transform _viewer = null;

        private Transform _transform;
        private List<GameObject> _chunks;
        private IChunkProvider _chunkProvider;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            _transform = transform;
            _chunks = new List<GameObject>();
            _chunkProvider = new ChunkProvider();
        }

        private void Update()
        {
            CleanUpChunks();
            CreateVisibleChunks();
        }

        private void CleanUpChunks()
        {
            foreach (GameObject chunk in _chunks)
                _chunkProvider.CleanUpChunk(chunk);
        }

        private void CreateVisibleChunks()
        {
            int viewDistanceInChunks = Mathf.RoundToInt(_viewDistance / _chunkSize);

            for (int i = -viewDistanceInChunks; i <= viewDistanceInChunks; i++)
            {
                for (int j = -viewDistanceInChunks; j <= viewDistanceInChunks; j++)
                {
                    ChunkCoordinates viewerCoordinates = PositionToChunkCoordinate(_viewer.position);
                    ChunkCoordinates chunkCoordinates = viewerCoordinates + new ChunkCoordinates(i, j);
                    CreateChunkIfVisible(chunkCoordinates);
                }
            }
        }

        private ChunkCoordinates PositionToChunkCoordinate(Vector3 position)
        {
            int coordinateX = Mathf.RoundToInt(position.z / _chunkSize);
            int coordinateY = Mathf.RoundToInt(position.x / _chunkSize);

            return new ChunkCoordinates(coordinateX, coordinateY);
        }

        private void CreateChunkIfVisible(ChunkCoordinates coordinates)
        {
            Vector3 chunkPosition = CalculateChunkPosition(coordinates);
            if (IsChunkVisible(chunkPosition))
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
        private bool IsChunkVisible(Vector3 chunkPosition)
        {
            Vector3 viewerPosition = _viewer.position;
            float viewDistanceSqr = _viewDistance * _viewDistance;

            Vector3 viewerToChunk = chunkPosition - viewerPosition;
            float distanceToChunkSqr = Vector3.SqrMagnitude(viewerToChunk);

            return distanceToChunkSqr < viewDistanceSqr;
        }

        private void CreateChunk(Vector3 position)
        {
            GameObject newChunk = _chunkProvider.GetChunk(position, _chunkSize, _transform);
            _chunks.Add(newChunk);
        }
    }
}
