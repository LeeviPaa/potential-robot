using UnityEngine;

namespace PotentialRobot.Terrain.Generation
{
    public class ChunkProvider : IChunkProvider
    {
        private static readonly Vector3 s_defaultRotationEuler = new Vector3(90f, 0f, 0f);

        public GameObject GetChunk(Vector3 position, float size, Transform parent)
        {
            GameObject chunk = CreateNewChunk();
            InitChunk(chunk, position, size, parent);
            return chunk;
        }

        private GameObject CreateNewChunk()
        {
            return GameObject.CreatePrimitive(PrimitiveType.Quad);
        }

        private void InitChunk(GameObject chunk, Vector3 position, float size, Transform parent)
        {
            Transform chunkTransform = chunk.transform;
            chunkTransform.SetParent(parent);
            chunkTransform.localRotation = Quaternion.Euler(s_defaultRotationEuler);
            chunkTransform.position = position;
            chunkTransform.localScale = Vector3.one * size;
        }

        public void CleanUpChunk(GameObject chunk)
        {
            Object.Destroy(chunk);
        }
    }
}
