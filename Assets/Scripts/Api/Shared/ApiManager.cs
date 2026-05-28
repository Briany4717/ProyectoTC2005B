using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.Categorization;

/// <summary>
/// Gestor centralizado para realizar peticiones a la API utilizando el patrón Singleton.
/// </summary>
public class ApiManager : MonoBehaviour
{
    private static ApiManager _instance;

    /// <summary>
    /// Instancia única del ApiManager.
    /// </summary>
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

    /// <summary>
    /// Configura el Singleton y evita que el objeto se destruya al cargar nuevas escenas.
    /// </summary>
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

    private readonly String baseUrl = "https://127.0.0.1:8081/";

    /// <summary>
    /// Inicia una petición GET a la API.
    /// </summary>
    /// <param name="endpoint">Ruta del endpoint a consultar.</param>
    /// <param name="onSuccess">Acción a ejecutar si la petición es exitosa.</param>
    /// <param name="onError">Acción a ejecutar si ocurre un error.</param>
    public void Get(string endpoint, Action<string> onSuccess, Action<string> onError)
    {
        StartCoroutine(GetCoroutine(endpoint, onSuccess, onError));
    }

    /// <summary>
    /// Corrutina que maneja la lógica interna de la petición GET.
    /// </summary>
    private IEnumerator GetCoroutine(string endpoint, Action<string> onSucces, Action<string> onError)
    {
        string url = baseUrl + endpoint;

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
                onSucces?.Invoke(request.downloadHandler.text);
            }
        }
    }

    /// <summary>
    /// Inicia una petición POST a la API enviando datos en formato JSON.
    /// </summary>
    /// <param name="endpoint">Ruta del endpoint a consultar.</param>
    /// <param name="jsonData">Datos a enviar en formato JSON.</param>
    /// <param name="onSucces">Acción a ejecutar si la petición es exitosa.</param>
    /// <param name="onError">Acción a ejecutar si ocurre un error.</param>
    public void Post(string endpoint, string jsonData, Action<string> onSucces, Action<string> onError)
    {
        StartCoroutine(PostCoroutine(endpoint, jsonData, onSucces, onError));
    }

    /// <summary>
    /// Corrutina que maneja la lógica interna de la petición POST.
    /// </summary>
    private IEnumerator PostCoroutine(string endpoint, string jsonData, Action<string> onSucces, Action<string> onError)
    {
        string url = baseUrl + endpoint;

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
                onSucces?.Invoke(request.downloadHandler.text);
            }
        }
    }

}
