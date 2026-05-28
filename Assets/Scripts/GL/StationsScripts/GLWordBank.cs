using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Networking;
using System.Collections;
using TMPro;
using Newtonsoft.Json;
using UnityEditor.Experimental.GraphView;

/// <summary>
/// Gestiona la carga y distribución de palabras o frases obtenidas desde una API.
/// </summary>
public class GLWordBank : MonoBehaviour
{
    private List<PromptData> promptInfo = new List<PromptData>();
    private bool isLoaded = false;
    private List<PromptData> workingPrompts = new List<PromptData>();

    public bool IsReady => isLoaded;

    /// <summary>
    /// Registra en consola el total de prompts cargados.
    /// </summary>
    void Update()
    {
        Debug.Log("Prompts jalados: " + promptInfo.Count);
    }

    /// <summary>
    /// Obtiene información de prompts consumiendo una API externa.
    /// </summary>
    private void GetPromptsFromAPI()
    {
        ApiManager.Instance.Get("glotonesPromptInformation",
        onSuccess: (jsonRespone) =>
        {
            promptInfo = JsonConvert.DeserializeObject<List<PromptData>>(jsonRespone);
            workingPrompts = new List<PromptData>(promptInfo);
            Shuffle(workingPrompts);
            isLoaded = true;
        },
        onError: (error) =>
        {
            Debug.LogError("Error API en GLWordBank: " + error);
        }
        );
    }

    /// <summary>
    /// Inicia la carga de datos de la API al despertar el objeto.
    /// </summary>
    void Awake()
    {
        GetPromptsFromAPI();
    }

    /// <summary>
    /// Mezcla de forma aleatoria una lista de prompts.
    /// </summary>
    private void Shuffle(List<PromptData> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int random = Random.Range(i, list.Count);
            var temporary = list[i];
            list[i] = list[random];
            list[random] = temporary;
        }
    }

    /// <summary>
    /// Retorna un prompt disponible de la lista activa y lo remueve.
    /// </summary>
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