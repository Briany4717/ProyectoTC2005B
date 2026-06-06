using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Category
{
    public string nombre;
}

[System.Serializable]
public class CategoryList
{
    public List<Category> items;
}

[System.Serializable]
public class IDPromptData
{
    public int id_promptcreado;
    public string titulo;
    public string contenido;
}

[System.Serializable]
public class IDPromptDataList
{
    public List<IDPromptData> items;
}

public class PNPromptBehaviour : MonoBehaviour
{
    private const string BaseUrl = "https://127.0.0.1:8999/";

    private static List<string> categoryList = new List<string>();
    private static bool isFetching = false;

    public float speed, xLimit = -11f;
    public string playerTag = "PNPlayer";
    public Renderer objectRenderer;
    public Sprite[] cloudCategories;
    public Sprite _originalSprite, aimCloud;
    public Vector3 scrollOffset = new Vector3(0f, -1.5f, 0f);
    private SpriteRenderer spriteRenderer;
    private int _spriteIndex = -1;
    private GameObject _activeScroll;
    private int id_promptcreado;
    private string _cloudPromptText;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (categoryList.Count == 0 && !isFetching)
        {
            isFetching = true;
            StartCoroutine(GetCategories());
        }
    }

    IEnumerator Start()
    {
        if (cloudCategories != null && cloudCategories.Length > 0)
        {
            _spriteIndex = Random.Range(0, cloudCategories.Length);
            spriteRenderer.sprite = cloudCategories[_spriteIndex];
            _originalSprite = spriteRenderer.sprite;
        }

        yield return new WaitUntil(() => !isFetching);

        if (categoryList != null && categoryList.Count > 0 && _spriteIndex != -1)
        {
            int categoryIndex = _spriteIndex % categoryList.Count;
            yield return StartCoroutine(FetchExternalPrompt(categoryIndex + 1));
        }
    }

    IEnumerator GetCategories()
    {
        using (UnityWebRequest request = UnityWebRequest.Get(BaseUrl + "PromptCategories"))
        {
            request.timeout = 10;
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    string wrapped = "{\"items\":" + request.downloadHandler.text + "}";
                    CategoryList data = JsonUtility.FromJson<CategoryList>(wrapped);
                    categoryList = new List<string>();
                    if (data?.items != null)
                    {
                        foreach (var item in data.items)
                            if (!string.IsNullOrEmpty(item.nombre))
                                categoryList.Add(item.nombre);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError("Error parsing categories: " + e.Message);
                }
            }
            else
            {
                Debug.Log("Error API: " + request.error);
            }
        }
        isFetching = false;
    }

    void Update()
    {
        if (transform.position.x <= xLimit)
        {
            DestroyScroll();
            Destroy(gameObject);
        }
        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }

    public void SetHighlight(bool state)
    {
        if (spriteRenderer == null) return;
        spriteRenderer.sprite = state ? aimCloud : _originalSprite;
    }

    public void SpawnScroll(GameObject prefab)
    {
        if (prefab == null || _activeScroll != null) return;

        Vector3 spawnPosition = transform.position + scrollOffset;
        _activeScroll = Instantiate(prefab, spawnPosition, Quaternion.identity);

        TMP_Text textComponent = _activeScroll.GetComponentInChildren<TMP_Text>();
        if (textComponent != null)
            textComponent.text = _cloudPromptText;
    }

    public void DestroyScroll()
    {
        if (_activeScroll != null)
        {
            Destroy(_activeScroll);
            _activeScroll = null;
        }
    }

    public void savePrompt()
    {
        StartCoroutine(PostSavePrompt());
    }

    IEnumerator PostSavePrompt()
    {
        string json = JsonUtility.ToJson(new SavePromptPayload
        {
            id_usuario = PlayerPrefs.GetInt("id_usuario"),
            id_prompt = id_promptcreado
        });

        using (UnityWebRequest request = new UnityWebRequest(BaseUrl + "savePrompt", "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();
        }
    }

    IEnumerator FetchExternalPrompt(int categoryId)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(BaseUrl + $"promptsByCategory/{categoryId}"))
        {
            request.timeout = 10;
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string wrapped = "{\"items\":" + request.downloadHandler.text + "}";
                IDPromptDataList data = JsonUtility.FromJson<IDPromptDataList>(wrapped);

                if (data?.items != null && data.items.Count > 0)
                {
                    IDPromptData selected = data.items[Random.Range(0, data.items.Count)];
                    id_promptcreado = selected.id_promptcreado;
                    string formattedText = $"{selected.titulo}:{selected.contenido}";

                    if (formattedText.Length > 50)
                        formattedText = formattedText.Substring(0, 47) + "...";

                    _cloudPromptText = formattedText;
                }
                else
                {
                    _cloudPromptText = "No hay prompts disponibles";
                }
            }
            else
            {
                Debug.Log("Error API: " + request.error);
                _cloudPromptText = "Error al cargar prompt";
            }
        }
    }

    [System.Serializable]
    private class SavePromptPayload
    {
        public int id_usuario;
        public int id_prompt;
    }
}
