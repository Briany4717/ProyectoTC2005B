using UnityEngine;

[System.Serializable]
public struct LEApplianceRepairData
{
    public string gellyDialogue;    // Diálogo introductorio de Gelly
    public string problemText;      // Texto descriptivo del problema en la ventana
    public int correctToolId;       // ID de la herramienta correcta (0, 1 o 2)
    public string[] tasks;          // Las 3 tareas representativas de la hoja de instrucciones
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
    public float totalMatchDuration = 300f; 

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
        isVictory = false; // Reset
    }
}
