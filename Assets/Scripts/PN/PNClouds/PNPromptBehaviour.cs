using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

public class PNPromptBehaviour : MonoBehaviour
{
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
    string _cloudPromptText;
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
        bool done = false;
        ApiManager.Instance.Get("PromptCategories",
            (json) => {
                try {
                    var rawData = JsonConvert.DeserializeObject<List<Category>>(json);
                    categoryList = new List<string>();
                    foreach(var item in rawData) 
                    {
                        if (!string.IsNullOrEmpty(item.nombre))
                            categoryList.Add(item.nombre);
                    }
                } catch (JsonException e) {
                    Debug.LogError("Error: " + e.Message);
                }
                done = true;
            },
            (error) => {
                Debug.Log("Error API: " + error);
                done = true;
            },
            "https://127.0.0.1:8999/"
        );

        yield return new WaitUntil(() => done);
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
        {
            textComponent.text = _cloudPromptText;
        }
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
        var payload = new { id_usuario = PlayerPrefs.GetInt("id_usuario"), id_prompt = id_promptcreado };
        string json = JsonConvert.SerializeObject(payload);

        ApiManager.Instance.Post("savePrompt", json, 
            (response) => {},
            (error) => Debug.Log("Error saving prompt: " + error),
            "https://127.0.0.1:8999/"
        );
    }

    IEnumerator FetchExternalPrompt(int categoryId)
    {
        bool done = false;
        ApiManager.Instance.Get($"promptsByCategory/{categoryId}",
            (json) => {
                List<IDPromptData> prompts = JsonConvert.DeserializeObject<List<IDPromptData>>(json);
                
                if (prompts != null && prompts.Count > 0)
                {
                    IDPromptData selected = prompts[Random.Range(0, prompts.Count)];
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
                done = true;
            },
            (error) => {
                Debug.Log("Error API: " + error);
                _cloudPromptText = "Error al cargar prompt";
                done = true;
            },
            "https://127.0.0.1:8999/"
        );

        yield return new WaitUntil(() => done);
    }
}