using UnityEngine;

[System.Serializable]
public struct LETaskStepData
{
    public string gellyDialogue;       // Diálogo único de Gelly para ESTA tarea
    public string problemText;         // Explicación de la avería técnica de ESTE paso
    public int correctToolId;          // Herramienta reglamentaria para ESTE paso (0, 1 o 2)
    public string taskDescription;     // El texto que se tachará en la hoja de instrucciones
}

[System.Serializable]
public struct LEApplianceRepairData
{
    // Ahora cada aparato contiene estrictamente sus 3 sub-pasos dinámicos (⌐■_■)
    public LETaskStepData[] steps; 
}

public class LEGameSessionData
{
    private static LEGameSessionData instance;
    public static LEGameSessionData Instance => instance ??= new LEGameSessionData();

    public float remainingTime;
    public Sprite currentApplianceSprite;
    public int repairedCount;
    public int discardedCount;
    public int globalStrikes;
    public int totalSpawnedLimit = 5;
    public LEApplianceRepairData[] currentMatchData;
    public int currentMatchDataIndex = 0;
    public bool isGameInProgress = false; 
    public int totalSpawnedCount; 
    public bool isVictory = false; 
    public float totalMatchDuration; 

    public void ResetSession(float durationSeconds)
    {
        remainingTime = durationSeconds;
        totalMatchDuration = durationSeconds;
        currentApplianceSprite = null;
        repairedCount = 0;
        discardedCount = 0;
        globalStrikes = 0;
        currentMatchDataIndex = 0;
        currentMatchData = null;
        isGameInProgress = false;
        totalSpawnedCount = 0;
        isVictory = false;
    }
}
