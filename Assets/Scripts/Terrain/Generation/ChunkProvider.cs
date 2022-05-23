using PotentialRobot.Common;
using UnityEngine;

namespace PotentialRobot.Terrain.Generation
{
    public class ChunkProvider : IChunkProvider
    {
        private const float DefaultRotationEulerX = 90f;
        private const float DefaultRotationEulerY = 0;
        private const float DefaultRotationEulerZ = 0;

        private readonly Transform _chunkParent;
        private readonly Pool<GameObject> _chunkPool;
        private readonly float _chunkSize;
        private readonly float _slopeAngleDeg;
        private readonly float _chunkLength;

        public ChunkProvider(Transform chunkParent, float chunkSize, float slopeAngleDeg)
        {
            _chunkParent = chunkParent;
            _chunkSize = chunkSize;
            _slopeAngleDeg = slopeAngleDeg;
            _chunkPool = new Pool<GameObject>(CreateNewChunk, ResetChunk);

            float slopeAngleRad = Mathf.Deg2Rad * slopeAngleDeg;
            float chunkHeight = Mathf.Tan(slopeAngleRad) * _chunkSize;
            _chunkLength = Mathf.Sqrt((_chunkSize * _chunkSize) + (chunkHeight * chunkHeight));
        }

        private GameObject CreateNewChunk()
        {
            return GameObject.CreatePrimitive(PrimitiveType.Quad);
        }

        private GameObject ResetChunk(GameObject chunk)
        {
            chunk.SetActive(false);
            return chunk;
        }

        public GameObject GetChunk(Vector3 position)
        {
            GameObject chunk = _chunkPool.Lease();
            InitChunk(chunk, position);
            return chunk;
        }

        private void InitChunk(GameObject chunk, Vector3 position)
        {
            Transform chunkTransform = chunk.transform;
            chunkTransform.SetParent(_chunkParent);
            chunkTransform.localPosition = position;

            var chunkRotationEuler = new Vector3(
                DefaultRotationEulerX + _slopeAngleDeg,
                DefaultRotationEulerY,
                DefaultRotationEulerZ);
            chunkTransform.localRotation = Quaternion.Euler(chunkRotationEuler);

            var chunkScale = new Vector3(_chunkSize, _chunkLength, 1);
            chunkTransform.localScale = chunkScale;

            chunk.SetActive(true);
        }

        public void CleanUpChunk(GameObject chunk)
        {
            _chunkPool.Recycle(chunk);
        }
    }
}
