using System.Collections.Generic;
using UnityEngine;

namespace PotentialRobot.Terrain
{
    public class ChunkVisibilityHelper
    {
        private readonly float _chunkSize;
        private readonly float _viewDistance;
        private readonly int _viewDistanceInChunks;
        private readonly float _viewDistanceSqr;

        public ChunkVisibilityHelper(float chunkSize, float viewDistance)
        {
            _chunkSize = chunkSize;
            _viewDistance = viewDistance;
            _viewDistanceInChunks = Mathf.RoundToInt(_viewDistance / _chunkSize);
            _viewDistanceSqr = _viewDistance * _viewDistance;
        }

        public bool IsVisible(Vector3 chunkPosition, Vector3 viewerPosition)
        {
            // Ignore distance on y-axis
            chunkPosition.y = 0;
            viewerPosition.y = 0;

            Vector3 viewerToChunkCenter = chunkPosition - viewerPosition;
            float distanceToChunkCenterSqr = Vector3.SqrMagnitude(viewerToChunkCenter);
            return distanceToChunkCenterSqr < _viewDistanceSqr;
        }

        public IEnumerable<ChunkCoordinates> GetPotentiallyVisibleCoordinates(ChunkCoordinates viewerChunk)
        {
            for (int i = -_viewDistanceInChunks; i <= _viewDistanceInChunks; i++)
            {
                for (int j = -_viewDistanceInChunks; j <= _viewDistanceInChunks; j++)
                {
                    var chunkOffset = new ChunkCoordinates(i, j);
                    yield return viewerChunk + chunkOffset;
                }
            }
        }
    }
}
