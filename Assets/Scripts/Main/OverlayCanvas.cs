using UnityEngine;

namespace PotentialRobot.Main
{
    public class OverlayCanvas : MonoBehaviour
    {
        [field: SerializeField]
        public Canvas CanvasComponent { get; private set; }

        protected virtual void OnEnable()
        {
            OverlayCanvasManager.Instance.RegisterCanvas(this);
        }

        protected virtual void OnDisable()
        {
            OverlayCanvasManager.Instance.UnregisterCanvas(this);
        }
    }
}
