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

        private IChunkController _chunkController;

        private void Start()
        {
            _chunkController = new ChunkController(transform, _chunkSize, _viewDistance);
        }

        private void Update()
        {
            var viewerPosition = new Vector3(_viewer.position.x, 0, _viewer.position.z);
            _chunkController.UpdateChunkVisibility(viewerPosition);
        }
    }
}
