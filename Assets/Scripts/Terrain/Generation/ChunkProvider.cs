using PotentialRobot.Common;
using UnityEngine;

namespace PotentialRobot.Terrain.Generation
{
    public class ChunkProvider : IChunkProvider
    {
        private static readonly Vector3 s_defaultRotationEuler = new Vector3(90f, 0f, 0f);

        private readonly Pool<GameObject> _chunkPool;

        public ChunkProvider()
        {
            _chunkPool = new Pool<GameObject>(CreateNewChunk, ResetChunk);
        }

        public GameObject GetChunk(Vector3 position, float size, Transform parent)
        {
            GameObject chunk = _chunkPool.Lease();
            InitChunk(chunk, position, size, parent);
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

        private void InitChunk(GameObject chunk, Vector3 position, float size, Transform parent)
        {
            Transform chunkTransform = chunk.transform;
            chunkTransform.SetParent(parent);
            chunkTransform.localRotation = Quaternion.Euler(s_defaultRotationEuler);
            chunkTransform.localPosition = position;
            chunkTransform.localScale = Vector3.one * size;
            chunk.SetActive(true);
        }

        public void CleanUpChunk(GameObject chunk)
        {
            _chunkPool.Recycle(chunk);
        }
    }
}
