using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Categorization;


/// Gestor centralizado para realizar peticiones a la API utilizando el patrón Singleton.

public class ApiManager : MonoBehaviour
{
    private static ApiManager _instance;

    
    /// Instancia única del ApiManager.
    
    public static ApiManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<ApiManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("ApiManager(AutoCreated)");
                    _instance = go.AddComponent<ApiManager>();
                }
            }
            return _instance;
        }
    }

    
    /// Configura el Singleton y evita que el objeto se destruya al cargar nuevas escenas.
    
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private readonly string baseUrl = "https://127.0.0.1:8081/";

    public void Get(string endpoint, Action<string> onSuccess, Action<string> onError, string customBaseUrl = null)
    {
        StartCoroutine(GetCoroutine(endpoint, onSuccess, onError, customBaseUrl));
    }


    private IEnumerator GetCoroutine(string endpoint, Action<string> onSuccess, Action<string> onError, string customBaseUrl = null)
    {
        string url = (customBaseUrl ?? baseUrl) + endpoint;

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            request.certificateHandler = new ForceAcceptAll();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                onError?.Invoke(request.error);
            }
            else
            {
                onSuccess?.Invoke(request.downloadHandler.text);
            }
        }
    }

    public void Post(string endpoint, string jsonData, Action<string> onSuccess, Action<string> onError, string customBaseUrl = null)
    {
        StartCoroutine(PostCoroutine(endpoint, jsonData, onSuccess, onError, customBaseUrl));
    }

    private IEnumerator PostCoroutine(string endpoint, string jsonData, Action<string> onSuccess, Action<string> onError, string customBaseUrl = null)
    {
        string url = (customBaseUrl ?? baseUrl) + endpoint;

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();

            request.SetRequestHeader("Content-Type", "application/json");

            request.certificateHandler = new ForceAcceptAll();

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError ||
                request.result == UnityWebRequest.Result.ProtocolError)
            {
                onError?.Invoke(request.error);
            }
            else
            {
                onSuccess?.Invoke(request.downloadHandler.text);
            }
        }
    }

}
