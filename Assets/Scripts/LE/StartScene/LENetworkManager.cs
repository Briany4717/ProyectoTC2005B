using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LENetworkManager : MonoBehaviour
{
    [Header("API Server Configuration")]
    [Tooltip("URL base")]
    [SerializeField] private string baseUrl = "https://127.0.0.1:8000";

    public static Dictionary<int, APITool> ToolsCache = new Dictionary<int, APITool>();

    #region DTOs de Deserialización JSON
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
        Debug.Log("[API] Iniciando descarga de telemetría de partida...");

        // 1. PETICIÓN A: Herramientas globales (Para alimentar tus descripciones dinámicas)
        using (UnityWebRequest wwwTools = UnityWebRequest.Get($"{baseUrl}/LE/herramientas"))
        {
            wwwTools.certificateHandler = new AcceptAllCertificates();
            yield return wwwTools.SendWebRequest();

            if (wwwTools.result == UnityWebRequest.Result.Success)
            {
                string json = "{\"items\":" + wwwTools.downloadHandler.text + "}";
                APITool[] toolsList = JsonUtility.FromJson<JsonArrayWrapper<APITool>>(json).items;
                
                ToolsCache.Clear();
                for (int i = 0; i < toolsList.Length; i++)
                {
                    ToolsCache[toolsList[i].id_herramienta] = toolsList[i];
                }
                Debug.Log($"[API] {ToolsCache.Count} Herramientas encontradas.");
            }
            else
            {
                Debug.LogError($"Error al traer herramientas: {wwwTools.error}");
            }
        }

        // 2. PETICIÓN B: Pool de 15 Problemas Aleatorios
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
            else
            {
                Debug.LogError($"Error al traer problemas: {wwwProblems.error}");
                yield break;
            }
        }

        // 3. PETICIÓN C: Pool de 15 Tareas Aleatorias
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
            else
            {
                Debug.LogError($"Error al traer tareas: {wwwTasks.error}");
                yield break;
            }
        }

        if (problemsPool != null && tasksPool != null && problemsPool.Length >= 5 && tasksPool.Length >= 15)
        {
            LEApplianceRepairData[] compiledMatchData = new LEApplianceRepairData[5];

            for (int i = 0; i < 5; i++)
            {
                compiledMatchData[i] = new LEApplianceRepairData
                {
                    gellyDialogue = problemsPool[i].dialogo,
                    problemText = problemsPool[i].problema,
                    correctToolId = problemsPool[i].solucion,
                    
                    // Empacamos las tareas de 3 en 3 de forma secuencial: [0,1,2], [3,4,5]...
                    tasks = new string[]
                    {
                        tasksPool[i * 3].desc_tarea,
                        tasksPool[(i * 3) + 1].desc_tarea,
                        tasksPool[(i * 3) + 2].desc_tarea
                    }
                };
            }

            // Inyectamos la configuración real en el contenedor inmortal entre escenas
            LEGameSessionData.Instance.currentMatchData = compiledMatchData;
            Debug.Log("[API] Partida armada con datos de la db");
        }
        else
        {
            Debug.LogError("La API retornó registros insuficientes para armar la partida (Se necesitan min. 5 problemas y 15 tareas).");
        }
    }
}
