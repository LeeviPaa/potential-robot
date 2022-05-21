using UnityEngine;

namespace PotentialRobot.Terrain.Generation
{
    public interface IChunkProvider
    {
        GameObject GetNewChunk(Vector3 position, float size, Transform parent);
    }
}
