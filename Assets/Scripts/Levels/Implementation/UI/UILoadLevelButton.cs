using Game.UI;
using VContainer;

namespace Game.Levels.UI
{
    public class UILoadLevelButton : UIButton
    {
        private LevelManager _levelManager;
        
        [Inject]
        private void Install(LevelManager levelManager)
        {
            _levelManager = levelManager;
        }

        protected override void OnClickHandler()
        {
            base.OnClickHandler();
            
            _levelManager.LoadCurrentLevel();
        }
    }
}