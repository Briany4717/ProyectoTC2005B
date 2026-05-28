using System.Collections;
using UnityEngine;

public class PNGoldenBehaviour : MonoBehaviour
{
    public float speed, coinSpawnIntervalMin = 0.4f, coinSpawnIntervalMax = 1.2f, xLimit = -11F;
    public int maxCoins = 5;
    public GameObject PNCoin;

    void Start()
    {
        StartCoroutine(SpawnCoins());
    }

    void Update()
    {
        if(transform.position.x <= xLimit)
            Destroy(gameObject);
        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }

    IEnumerator SpawnCoins()
    {
        int spawned = 0;
        while (spawned < maxCoins)
        {
            yield return new WaitForSeconds(Random.Range(coinSpawnIntervalMin, coinSpawnIntervalMax));
            Instantiate(PNCoin, transform.position, Quaternion.identity);
            spawned++;
        }
    }
}
