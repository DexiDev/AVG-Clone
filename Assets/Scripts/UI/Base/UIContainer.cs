using UnityEngine;
using VContainer;

namespace Game.UI
{
    public class UIContainer : MonoBehaviour
    {
        [SerializeField] private UIElementType _containerType;

        private UIManager _uiManager;
        
        public UIElementType ContainerType => _containerType;

        [Inject]
        private void Construct(UIManager uiManager)
        {
            _uiManager = uiManager;
        }

        private void Awake()
        {
            _uiManager.RegisterContainer(this);
        }

        private void OnEnable()
        {
            _uiManager.RegisterContainer(this);
        }

        private void OnDisable()
        {
            _uiManager.UnregisterContainer(this);
        }
    }
}