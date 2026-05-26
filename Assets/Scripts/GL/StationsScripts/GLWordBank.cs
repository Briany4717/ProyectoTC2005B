using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using UnityEngine.Networking;
using System.Collections;
using TMPro;
using Newtonsoft.Json;
using UnityEditor.Experimental.GraphView;

public class GLWordBank : MonoBehaviour
{

    private List<PromptData> promptInfo = new List<PromptData>();
    private bool isLoaded = false;

    public bool IsReady => isLoaded;

    void Update()
    {
        Debug.Log("Prompts jalados: " + promptInfo.Count);
    }

    IEnumerator GetPromptsFromAPI()
    {
        string JSONurl = "https://127.0.0.1:8081/glotonesPromptInformation";

        UnityWebRequest web = UnityWebRequest.Get(JSONurl);
        web.certificateHandler = new ForceAcceptAll();
        yield return web.SendWebRequest();

        if (web.result != UnityWebRequest.Result.Success)
            UnityEngine.Debug.Log("Error API: " + web.error);
        else
        {
            promptInfo = JsonConvert.DeserializeObject<List<PromptData>>(web.downloadHandler.text);
            workingPrompts = new List<PromptData>(promptInfo);
            Shuffle(workingPrompts);
        }

        isLoaded = true;
    }

    private List<PromptData> workingPrompts = new List<PromptData>();

    void Awake()
    {
        StartCoroutine(GetPromptsFromAPI());
    }

    private void Shuffle(List<PromptData> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int random = Random.Range(i, list.Count);

            // subtitute with the random int position
            var temporary = list[i];
            list[i] = list[random];
            list[random] = temporary;
        }
    }



    public PromptData GetPrompt()
    {
        PromptData newPrompt = new PromptData();

        if (workingPrompts.Count != 0)
        {
            newPrompt = workingPrompts.Last();
            workingPrompts.Remove(newPrompt);
        }

        return newPrompt;
    }



}
