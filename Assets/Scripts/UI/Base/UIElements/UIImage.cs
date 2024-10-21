using UnityEngine;
using UnityEngine.UI;

namespace Game.UI.UIElements
{
    public class UIImage : UIElement
    {
        [SerializeField] protected Image _image;
        
        public Image Image => _image;

        public Vector2 Size => _image.rectTransform.rect.size;

        public RectTransform RectTransform => _image.rectTransform;
        
        
        protected override void Awake()
        {
            base.Awake();
            _image ??= GetComponent<Image>();
        }

        public void SetSprite(Sprite sprite)
        {
            _image.sprite = sprite;
        }

        public void SetSize(Vector2 size)
        {
            _image.rectTransform.sizeDelta = size;
        }
    }
}