using UnityEngine;

namespace PotentialRobot.Terrain.Generation
{
    public interface IChunkProvider
    {
        GameObject GetChunk(Vector3 position, float size, Transform parent);
        void CleanUpChunk(GameObject chunk);
    }
}
