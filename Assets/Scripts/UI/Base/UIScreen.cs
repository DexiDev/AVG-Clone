using UnityEngine;

namespace Game.UI
{
    [RequireComponent(typeof(Canvas))]
    public class UIScreen : UIElement
    {
        [SerializeField] protected RectTransform _root;
    }
}