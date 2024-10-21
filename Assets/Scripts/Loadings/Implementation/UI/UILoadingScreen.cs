using Game.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Loadings.UI
{
    public class UILoadingScreen : UIScreen
    {
        [SerializeField] private Image _progressBar;

        private int _countTask = 0;
        private int _currentTask = 0; 
        
        private void OnEnable()
        {
            _countTask = 0;
            _currentTask = 0;
        }

        public void AddTask(int count = 1)
        {
            _countTask += count;
        }

        public void CompletedTask(int count = 1)
        {
            _currentTask += count;
        }

        public void SetProgress(float taskProgress)
        {
            var progress = ((float)_currentTask / _countTask) + (taskProgress / _countTask);
            _progressBar.fillAmount = progress;
        }

        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}