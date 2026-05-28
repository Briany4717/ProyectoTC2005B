using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LENetworkManager : MonoBehaviour
{
    [Header("API Server Configuration")]
    [SerializeField] private string baseUrl = "http://127.0.0.1:8000";

    public static Dictionary<int, APITool> ToolsCache = new Dictionary<int, APITool>();

    #region DTOs de tu API Real
    [System.Serializable]
    public struct APIProblem
    {
        public int id_problema;
        public string nombre;
        public string problema;
        public string dialogo;
        public int solucion;
    }

    [System.Serializable]
    public struct APITask
    {
        public int id_tarea;
        public string desc_tarea;
    }

    [System.Serializable]
    public struct APITool
    {
        public int id_herramienta;
        public string nombre_herramienta;
        public string descripcion_herramienta;
    }

    private class JsonArrayWrapper<T>
    {
        public T[] items;
    }
    #endregion

    public void FetchMatchData()
    {
        StartCoroutine(DownloadMatchDataRoutine());
    }

    private IEnumerator DownloadMatchDataRoutine()
    {
        // 1. Descarga e indexación de Herramientas en Caché Global
        using (UnityWebRequest wwwTools = UnityWebRequest.Get($"{baseUrl}/LE/herramientas"))
        {
            wwwTools.certificateHandler = new AcceptAllCertificates();
            yield return wwwTools.SendWebRequest();
            
            if (wwwTools.result == UnityWebRequest.Result.Success)
            {
                string json = "{\"items\":" + wwwTools.downloadHandler.text + "}";
                APITool[] toolsList = JsonUtility.FromJson<JsonArrayWrapper<APITool>>(json).items;
                ToolsCache.Clear();
                for (int i = 0; i < toolsList.Length; i++) ToolsCache[toolsList[i].id_herramienta] = toolsList[i];
            }
        }

        // 2. Descarga del Pool de 15 Problemas
        APIProblem[] problemsPool = null;
        using (UnityWebRequest wwwProblems = UnityWebRequest.Get($"{baseUrl}/LE/problemas"))
        {
            
            wwwProblems.certificateHandler = new AcceptAllCertificates();            
            yield return wwwProblems.SendWebRequest();

            if (wwwProblems.result == UnityWebRequest.Result.Success)
            {
                string json = "{\"items\":" + wwwProblems.downloadHandler.text + "}";
                problemsPool = JsonUtility.FromJson<JsonArrayWrapper<APIProblem>>(json).items;
            }
            else yield break;
        }

        // 3. Descarga del Pool de 15 Tareas
        APITask[] tasksPool = null;
        using (UnityWebRequest wwwTasks = UnityWebRequest.Get($"{baseUrl}/LE/tareas"))
        {
            wwwTasks.certificateHandler = new AcceptAllCertificates();
            yield return wwwTasks.SendWebRequest();
            
            if (wwwTasks.result == UnityWebRequest.Result.Success)
            {
                string json = "{\"items\":" + wwwTasks.downloadHandler.text + "}";
                tasksPool = JsonUtility.FromJson<JsonArrayWrapper<APITask>>(json).items;
            }
            else yield break;
        }

        // ====================================================================
        // ⚙️ ENSAMBLAJE MATRICIAL DE SUB-PASOS (0 Allocations de bucle)
        // Mapeamos los 15 pares de forma paralela 1-to-1 en tramos de 3 (⌐■_■)
        // ====================================================================
        if (problemsPool != null && tasksPool != null && problemsPool.Length >= 15 && tasksPool.Length >= 15)
        {
            LEApplianceRepairData[] compiledMatchData = new LEApplianceRepairData[5];

            for (int i = 0; i < 5; i++)
            {
                compiledMatchData[i].steps = new LETaskStepData[3];

                for (int j = 0; j < 3; j++)
                {
                    // Índice secuencial del pool de 0 a 14
                    int poolIndex = (i * 3) + j; 

                    compiledMatchData[i].steps[j] = new LETaskStepData
                    {
                        gellyDialogue = problemsPool[poolIndex].dialogo,
                        problemText = problemsPool[poolIndex].problema,
                        correctToolId = problemsPool[poolIndex].solucion,
                        taskDescription = tasksPool[poolIndex].desc_tarea
                    };
                }
            }

            LEGameSessionData.Instance.currentMatchData = compiledMatchData;
            Debug.Log("🚀 [API] Matriz de 15 sub-pasos entrelazada y cargada con éxito.");
        }
    }
}
