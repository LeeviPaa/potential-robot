using UnityEngine;

[CreateAssetMenu(menuName = "UIStyle/PropertyAsset/Padding", fileName = "UIStyle_Padding.asset")]
public class UIStyle_Padding : ScriptableObject
{
    [SerializeField]
    private float _left;
    public float Left => _left;
    [SerializeField]
    private float _right;
    public float Right => _right;
    [SerializeField]
    private float _top;
    public float Top => _top;
    [SerializeField]
    private float _bottom;
    public float Bottom => _bottom;
}
