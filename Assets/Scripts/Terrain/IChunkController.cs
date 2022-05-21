using UnityEngine;

namespace PotentialRobot.Terrain
{
    public interface IChunkController
    {
        void UpdateChunkVisibility(Vector3 viewerPosition);
    }
}
