using System.Collections;
using UnityEngine;

public class CloudSpawner : MonoBehaviour
{
    public GameObject PNPromptCloud, PNGoldenCloud, PNStormCloud;
    public float minHeight = -2f, maxHeight = 4f;
    public float maxStateDuration = 5f;
    public float timeToSpawnMin = 1f, timeToSpawnMax = 3f;
    public PNGUIController guiController;

    private enum CloudState { Prompt, Golden, Storm }

    void Start()
    {
        StartCoroutine(StateMachine());
    }

    IEnumerator StateMachine()
    {
        CloudState currentState; 

        for(int i = 0; i < 3; i++)
        {
            currentState = (CloudState)i;
            if (guiController != null) guiController.ChangeSkyAsset((int)currentState);
            if (PNSFXController.Instance != null) PNSFXController.Instance.PlayMusic((int)currentState);
            float stateDuration = 8f;

            yield return StartCoroutine(SpawnForDuration(currentState, stateDuration));
            PlayerPrefs.SetInt("CloudState", (int)currentState);
        }
        while (true)
        {
            currentState = (CloudState)Random.Range(0, 3);
            if (guiController != null) guiController.ChangeSkyAsset((int)currentState);
            if (PNSFXController.Instance != null) PNSFXController.Instance.PlayMusic((int)currentState);
            float stateDuration = Random.Range(1f, maxStateDuration);
            yield return StartCoroutine(SpawnForDuration(currentState, stateDuration));

            currentState = (CloudState)Random.Range(0, 3);
            PlayerPrefs.SetInt("CloudState", (int)currentState);
        }
    }

    IEnumerator SpawnForDuration(CloudState state, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float delay = Random.Range(timeToSpawnMin, timeToSpawnMax);
            delay = Mathf.Min(delay, duration - elapsed);

            yield return new WaitForSeconds(delay);
            elapsed += delay;

            if (elapsed < duration)
                SpawnCloud(state);
        }
    }

    void SpawnCloud(CloudState state)
    {
        GameObject prefab = null;
        switch (state)
        {
            case CloudState.Prompt: prefab = PNPromptCloud; break;
            case CloudState.Golden: prefab = PNGoldenCloud; break;
            case CloudState.Storm:  prefab = PNStormCloud;  break;
        }

        Vector3 spawnPos = new Vector3(
            transform.position.x,
            transform.position.y + Random.Range(minHeight, maxHeight),
            0f
        );

        Instantiate(prefab, spawnPos, Quaternion.identity);
    }
}