using System.Collections.Generic;
using UnityEngine;

public class LENetworkManager : MonoBehaviour
{
    // Pool local de prueba para alimentar el juego en lo que tus endpoints están listos
    private readonly LEApplianceRepairData[] mockDatabasePool = new LEApplianceRepairData[]
    {
        new LEApplianceRepairData { 
            gellyDialogue = "¡Cielos, este tostador echa chispas! Manos a la obra.", 
            problemText = "Cortocircuito severo en las resistencias térmicas.", 
            correctToolId = 1, 
            tasks = new string[] { "Remover carcasa exterior", "Soldar cable de cobre central", "Calibrar termostato análogo" }
        },
        new LEApplianceRepairData { 
            gellyDialogue = "Vaya, esta licuadora huele a quemado... ¡Comencemos la reparación!", 
            problemText = "El motor central está bloqueado por óxido acumulado.", 
            correctToolId = 0, 
            tasks = new string[] { "Desmontar cuchillas inferiores", "Aplicar lubricante dieléctrico", "Alinear escobillas del motor" }
        },
        new LEApplianceRepairData { 
            gellyDialogue = "¡El refrigerador no enfría nada! Revisemos esto rápido.", 
            problemText = "Fuga de gas refrigerante en el condensador trasero.", 
            correctToolId = 2, 
            tasks = new string[] { "Parchar tubería de aluminio", "Inyectar carga de gas R134a", "Testear presostato digital" }
        },
        new LEApplianceRepairData { 
            gellyDialogue = "Este microondas hace un ruido terrible... ¡Manos a la obra!", 
            problemText = "El magnetrón está suelto debido a vibraciones.", 
            correctToolId = 0, 
            tasks = new string[] { "Ajustar tornillos de anclaje", "Verificar diodo de alta tensión", "Limpiar guía de ondas mica" }
        },
        new LEApplianceRepairData { 
            gellyDialogue = "¡La cafetera se está desbordando! A reparar se ha dicho.", 
            problemText = "Obstrucción masiva de sarro en la caldera de paso.", 
            correctToolId = 1, 
            tasks = new string[] { "Descalcificar ductos internos", "Reemplazar junta de goma rota", "Probar válvula de seguridad" }
        }
    };

    /// <summary>
    /// Simula la petición al endpoint. Elige 5 elementos aleatorios del pool sin repetir.
    /// </summary>
    public void FetchMatchData()
    {
        List<LEApplianceRepairData> shuffleList = new List<LEApplianceRepairData>(mockDatabasePool);
        LEApplianceRepairData[] selectedData = new LEApplianceRepairData[5];

        // Algoritmo Fisher-Yates de barajado ultra rápido (0 allocs en loops cortos)
        for (int i = 0; i < selectedData.Length; i++)
        {
            int randomIndex = Random.Range(i, shuffleList.Count);
            LEApplianceRepairData temp = shuffleList[i];
            shuffleList[i] = shuffleList[randomIndex];
            shuffleList[randomIndex] = temp;
            
            selectedData[i] = shuffleList[i];
        }

        // Guardamos los 5 datos aleatorios de la partida en el contenedor inmortal
        LEGameSessionData.Instance.currentMatchData = selectedData;
        Debug.Log("🔌 [Mock API] 5 Configuraciones de reparación descargadas con éxito y listas.");
    }
}
