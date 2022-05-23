using UnityEngine;

namespace PotentialRobot.Terrain
{
    public class TerrainController : MonoBehaviour
    {
        [SerializeField]
        private float _chunkSize = 1f;
        [SerializeField]
        private float _viewDistance = 5f;
        [SerializeField]
        private Transform _viewer = null;
        [SerializeField]
        [Range(0, 89)]
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
