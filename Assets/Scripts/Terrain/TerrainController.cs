using UnityEngine;

namespace PotentialRobot.Terrain
{
    public class TerrainController : MonoBehaviour
    {
        [SerializeField]
        private Transform _viewer = null;
        [SerializeField]
        [Range(TerrainConstants.MinChunkSize, TerrainConstants.MaxChunkSize)]
        private float _chunkSize = 1f;
        [SerializeField]
        [Range(TerrainConstants.MinViewDistance, TerrainConstants.MaxViewDistance)]
        private float _viewDistance = 5f;
        [SerializeField]
        [Range(TerrainConstants.MinSlopeAngleDeg, TerrainConstants.MaxSlopeAngleDeg)]
        private float _slopeAngleDeg = 30f;

        private IChunkController _chunkController;

        private void Start()
        {
            _chunkController = new ChunkController(transform, _chunkSize, _viewDistance, _slopeAngleDeg);
        }

        private void Update()
        {
            _chunkController.UpdateChunkVisibility(_viewer.position);
        }
    }
}
