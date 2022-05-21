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
        }

        [SerializeField]
        private float _chunkSize = 1f;
        [SerializeField]
        private float _viewDistance = 5f;

        private Transform _transform;
        private List<GameObject> _chunks;
        private IChunkProvider _chunkProvider;

        private void Start()
        {
            Init();
            CreateVisibleChunks();
        }

        private void Init()
        {
            _transform = transform;
            _chunks = new List<GameObject>();
            _chunkProvider = new ChunkProvider();
        }

        private void CreateVisibleChunks()
        {
            int viewDistanceInChunks = Mathf.RoundToInt(_viewDistance / _chunkSize);

            for (int i = -viewDistanceInChunks; i <= viewDistanceInChunks; i++)
            {
                for (int j = -viewDistanceInChunks; j <= viewDistanceInChunks; j++)
                {
                    CreateChunkIfVisible(new ChunkCoordinates(i, j));
                }
            }
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

        private bool IsChunkVisible(Vector3 chunkPosition)
        {
            Vector3 viewerPosition = Vector3.zero;
            float viewDistanceSqr = _viewDistance * _viewDistance;

            Vector3 viewerToChunk = chunkPosition - viewerPosition;
            float distanceToChunkSqr = Vector3.SqrMagnitude(viewerToChunk);

            return distanceToChunkSqr < viewDistanceSqr;
        }

        private void CreateChunk(Vector3 position)
        {
            GameObject newChunk = _chunkProvider.GetNewChunk(position, _chunkSize, _transform);
            _chunks.Add(newChunk);
        }
    }
}
