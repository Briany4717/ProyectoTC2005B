using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LEDebrisPhysics : MonoBehaviour
{
    private Vector3 velocity;
    private float gravity = 1500f;
    private RectTransform rectTransform;
    private float screenBottomLimit = -700f;

    public void InitializeDebris(Sprite sprite, Vector3 spawnPosition)
    {
        rectTransform = GetComponent<RectTransform>();
        GetComponent<Image>().sprite = sprite;
        
        transform.position = spawnPosition;
        rectTransform.localScale = Vector3.one;

        float randomX = Random.Range(-350f, 350f);
        float randomY = Random.Range(400f, 850f);
        velocity = new Vector3(randomX, randomY, 0f);

        rectTransform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
    }

    void Update()
    {
        velocity.y -= gravity * Time.deltaTime;

        rectTransform.anchoredPosition += (Vector2)(velocity * Time.deltaTime);

        if (rectTransform.anchoredPosition.y < screenBottomLimit)
        {
            Destroy(gameObject);
        }
    }
}
