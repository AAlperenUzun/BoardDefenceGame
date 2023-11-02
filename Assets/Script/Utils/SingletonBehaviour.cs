using UnityEngine;

public abstract class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = (T)FindObjectOfType(typeof(T));
            if (Instance == null)
            {
                Debug.LogWarning("An instance of " + typeof(T) +
                                 " is needed in the scene, but there is none.");
            }
            else
            {
                OnAwake();
            }
        }
        else if (Instance != this)
        {
            Destroy(gameObject); // On reload, singleton already set, so destroy duplicate.
        }
    }        

    protected abstract void OnAwake();

    public static T Instance
    {
        get
        {
            if (!_instance)
            {
                _instance = (T)FindObjectOfType(typeof(T));
                if (!_instance)
                {
                    Debug.LogWarning("An instance of " + typeof(T) +
                                     " is needed in the scene, but there is none.");
                }
                else
                {
                    SingletonBehaviour<T> instance = _instance as SingletonBehaviour<T>;
                    instance.OnAwake();
                }
            }

            return _instance;
        }
        set
        {
            _instance = value;
        }
    }
}