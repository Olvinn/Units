using UnityEngine;

namespace Base
{
    public class Singleton<T> : MonoBehaviour, ISingleton<T>  where T : class
    {
        public static T Instance;
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this as T;
                // transform.SetParent(null);
                // DontDestroyOnLoad(gameObject);
            }
            else
            {
                Debug.LogWarning($"Singleton instance of {typeof(T)} is already exist");
                Destroy(gameObject);
            }
        }
    }
}
