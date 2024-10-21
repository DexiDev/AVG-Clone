using System.Collections.Generic;
using System.Linq;
using VContainer;
using VContainer.Unity;

namespace Game.Data
{
    public abstract class DataManager<TDataScriptable, TConfig> : IInitializable where TDataScriptable : DataScriptable where TConfig : DataConfig<TDataScriptable>
    {
        protected Dictionary<string, TDataScriptable> _datas = new();
        protected TConfig _config;
        
        protected bool _isInitialize;

        [Inject]
        private void Install(TConfig config)
        {
            _config = config;
        }
        
        public void Initialize()
        {
            if (_isInitialize) return;

            _isInitialize = true;
            
            _datas = _config.Datas?.ToDictionary(key => key.ID, value => value);
            
            Initialized();
        }

        protected virtual void Initialized()
        {
            
        }
        
        public TDataScriptable GetData(string id)
        {
            if(!_isInitialize) Initialize();
            
            if (string.IsNullOrEmpty(id)) return null;

            return _datas.TryGetValue(id, out var data) ? data : null;
        }

        public string[] GetIDAll()
        {
            if(!_isInitialize) Initialize();
            
            return _datas.Keys.ToArray();
        }

        public TDataScriptable[] GetDataAll()
        {
            if(!_isInitialize) Initialize();
            
            return _datas.Values.ToArray();
        }
    }
}