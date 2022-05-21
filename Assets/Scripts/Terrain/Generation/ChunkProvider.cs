using PotentialRobot.Common;
using UnityEngine;

namespace PotentialRobot.Terrain.Generation
{
    public class ChunkProvider : IChunkProvider
    {
        private static readonly Vector3 s_defaultRotationEuler = new Vector3(90f, 0f, 0f);

        private readonly float _chunkSize;
        private readonly Transform _chunkParent;
        private readonly Pool<GameObject> _chunkPool;

        public ChunkProvider(Transform chunkParent, float chunkSize)
        {
            _chunkParent = chunkParent;
            _chunkSize = chunkSize;
            _chunkPool = new Pool<GameObject>(CreateNewChunk, ResetChunk);
        }

        public GameObject GetChunk(Vector3 position)
        {
            GameObject chunk = _chunkPool.Lease();
            InitChunk(chunk, position);
            return chunk;
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

        private void InitChunk(GameObject chunk, Vector3 position)
        {
            Transform chunkTransform = chunk.transform;
            chunkTransform.SetParent(_chunkParent);
            chunkTransform.localRotation = Quaternion.Euler(s_defaultRotationEuler);
            chunkTransform.localPosition = position;
            chunkTransform.localScale = Vector3.one * _chunkSize;
            chunk.SetActive(true);
        }

        public void CleanUpChunk(GameObject chunk)
        {
            _chunkPool.Recycle(chunk);
        }
    }
}
