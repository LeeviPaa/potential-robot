using UnityEngine;

namespace PotentialRobot.Terrain.Generation
{
    public interface IChunkProvider
    {
        GameObject GetChunk(Vector3 position);
        void CleanUpChunk(GameObject chunk);
    }
}
