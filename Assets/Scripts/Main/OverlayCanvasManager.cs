using UnityEngine;
using System.Collections.Generic;

namespace PotentialRobot.Main
{
    public class OverlayCanvasManager : MonoBehaviour
    {
        public static OverlayCanvasManager Instance { get; private set; }

        [SerializeField]
        private Camera _overlayCamera;

        private readonly List<OverlayCanvas> _activeCanvases = new List<OverlayCanvas>();

        private void Awake()
        {
            Instance = this;
        }

        public void RegisterCanvas(OverlayCanvas canvas)
        {
            if (canvas == null)
            {
                return;
            }
            for (int i = 0; i < _activeCanvases.Count; i++)
            {
                if (_activeCanvases[i] == canvas)
                {
                    return;
                }
            }
            _activeCanvases.Add(canvas);
            canvas.transform.SetParent(transform);
            canvas.CanvasComponent.worldCamera = _overlayCamera;
        }

        public void UnregisterCanvas(OverlayCanvas canvas)
        {
            for (int i = 0; i < _activeCanvases.Count; i++)
            {
                if (_activeCanvases[i] == canvas)
                {
                    _activeCanvases.RemoveAt(i);
                    if (canvas != null)
                    {
                        if (canvas.transform.parent && transform.parent == transform)
                        {
                            canvas.transform.SetParent(null);
                        }
                        canvas.CanvasComponent.worldCamera = null;
                    }
                    return;
                }
            }
        }
    }
}
