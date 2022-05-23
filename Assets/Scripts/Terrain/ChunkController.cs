using System.Collections.Generic;
using System.Globalization;
using PotentialRobot.Terrain.Generation;
using UnityEngine;

namespace PotentialRobot.Terrain
{
    public class ChunkController : IChunkController
    {
        private readonly IChunkProvider _chunkProvider;
        private readonly ChunkVisibilityHelper _visibilityHelper;
        private readonly ChunkPositionHelper _positionHelper;
        private readonly Dictionary<ChunkCoordinates, GameObject> _chunks;

        private const string ChunkNameFormat = "Chunk [{0},{1}]";

        public ChunkController(Transform chunkParent, float chunkSize, float viewDistance, float slopeAngleDeg)
        {
            _chunkProvider = new ChunkProvider(chunkParent, chunkSize);
            _visibilityHelper = new ChunkVisibilityHelper(chunkSize, viewDistance);
            _positionHelper = new ChunkPositionHelper(chunkSize, slopeAngleDeg);
            _chunks = new Dictionary<ChunkCoordinates, GameObject>();
        }

        public void UpdateChunkVisibility(Vector3 viewerPosition)
        {
            CleanUpOutOfViewDistanceChunks(viewerPosition);
            CreateNewVisibleChunks(viewerPosition);
        }

        private void CleanUpOutOfViewDistanceChunks(Vector3 viewerPosition)
        {
            var cleanedUpCoordinates = new List<ChunkCoordinates>();
            foreach (KeyValuePair<ChunkCoordinates, GameObject> chunk in _chunks)
            {
                if (CleanUpIfNotVisible(chunk.Key, chunk.Value, viewerPosition))
                    cleanedUpCoordinates.Add(chunk.Key);
            }

            foreach (ChunkCoordinates coordinates in cleanedUpCoordinates)
                _ = _chunks.Remove(coordinates);
        }

        private bool CleanUpIfNotVisible(ChunkCoordinates coordinates, GameObject chunk, Vector3 viewerPosition)
        {
            Vector3 chunkPosition = _positionHelper.CoordinatesToPosition(coordinates);
            if (_visibilityHelper.IsVisible(chunkPosition, viewerPosition))
                return false;

            _chunkProvider.CleanUpChunk(chunk);
            return true;
        }

        private void CreateNewVisibleChunks(Vector3 viewerPosition)
        {
            ChunkCoordinates viewerCoordinates = _positionHelper.PositionToCoordinates(viewerPosition);

            foreach (ChunkCoordinates coordinates in _visibilityHelper
                .GetPotentiallyVisibleCoordinates(viewerCoordinates))
            {
                if (_chunks.ContainsKey(coordinates))
                    continue;

                Vector3 chunkPosition = _positionHelper.CoordinatesToPosition(coordinates);
                if (_visibilityHelper.IsVisible(chunkPosition, viewerPosition))
                    CreateChunk(coordinates, chunkPosition);
            }
        }

        private void CreateChunk(ChunkCoordinates coordinates, Vector3 position)
        {
            GameObject newChunk = _chunkProvider.GetChunk(position);
            newChunk.name = GetChunkName(coordinates);
            _chunks.Add(coordinates, newChunk);
        }

        private string GetChunkName(ChunkCoordinates coordinates)
            => string.Format(CultureInfo.InvariantCulture, ChunkNameFormat, coordinates.X, coordinates.Y);
    }
}
