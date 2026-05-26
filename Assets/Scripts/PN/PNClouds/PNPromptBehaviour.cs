using UnityEngine;
using TMPro;

public class PNPromptBehaviour : MonoBehaviour
{
    public enum CloudCategory
    {Code,Finance,Writing,Marketing}

    public float speed, xLimit = -11f;
    public string playerTag = "PNPlayer";
    public Renderer objectRenderer;
    public Sprite[] cloudCategories;
    public Sprite _originalSprite, aimCloud;
    public Vector3 scrollOffset = new Vector3(0f, -1.5f, 0f);
    private SpriteRenderer spriteRenderer;
    private CloudCategory currentCategory;
    private GameObject _activeScroll;
    private string _cloudPromptText = ""; 

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (cloudCategories != null && cloudCategories.Length > 0)
        {
            int randomIndex = Random.Range(0, cloudCategories.Length);
            spriteRenderer.sprite = cloudCategories[randomIndex];
            _originalSprite = spriteRenderer.sprite;

            currentCategory = (CloudCategory)(randomIndex % System.Enum.GetValues(typeof(CloudCategory)).Length);
            _cloudPromptText = FetchExternalPrompt(currentCategory, randomIndex);
        }
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

    private string FetchExternalPrompt(CloudCategory category, int index)
    {
        return $"[{category}] Prompt #{index}";
    }
}