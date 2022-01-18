//单例父类

using UnityEngine;

namespace ThinkUtils
{
    public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                    _instance = FindObjectOfType<T>();
                if (_instance == null)
                {
                    var obj = new GameObject(typeof(T).Name + "_Instance");
                    _instance = obj.AddComponent<T>();
                }
                return _instance;
            }
        }
        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                DestroyImmediate(this.gameObject);
                return;
            }
            _instance = this as T;
        }
    }
}
