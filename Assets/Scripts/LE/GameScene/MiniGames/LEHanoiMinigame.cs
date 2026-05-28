using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LEHanoiMinigame : MonoBehaviour
{
    [Header("Manager Connection")]
    [SerializeField] private LERepairManager repairManager;

    [Header("UI Canvas Panels")]
    [SerializeField] private GameObject minigamePanel;
    [SerializeField] private TextMeshProUGUI announcementTextMesh;
    [SerializeField] private Transform dragOverlayLayer; 

    [Header("Towers (Pegs)")]
    public Transform[] towerPegs; 

    [Header("Disks Objects")]
    [SerializeField] private RectTransform[] allDisks; 

    private int activeDisksCount = 3;
    private Transform selectedSourcePeg = null;
    private bool isMinigameActive = false;

    public bool IsMinigameActive => isMinigameActive;
    public LERepairManager RepairManager => repairManager;
    public Transform DragOverlayLayer => dragOverlayLayer;

    void Awake()
    {
        if (minigamePanel != null) minigamePanel.SetActive(false);
    }

    public void StartMinigame()
    {
        if (minigamePanel != null) minigamePanel.SetActive(true);
        isMinigameActive = true;
        selectedSourcePeg = null;

        ConfigureDifficulty();
        ResetHanoiPuzzle();
    }

    private void ConfigureDifficulty()
    {
        int difficulty = PlayerPrefs.GetInt("LE_Minigames_Difficulty", 1);
        if (difficulty == 0) activeDisksCount = 2;      
        else if (difficulty == 1) activeDisksCount = 3; 
        else if (difficulty == 2) activeDisksCount = 4; 

        announcementTextMesh.text = $"Torre de Hanoi: {activeDisksCount} Piezas";
    }

    private void ResetHanoiPuzzle()
    {
        for (int i = 0; i < allDisks.Length; i++)
        {
            if (allDisks[i] != null) allDisks[i].gameObject.SetActive(false);
        }

        for (int i = activeDisksCount - 1; i >= 0; i--)
        {
            if (allDisks[i] != null)
            {
                allDisks[i].SetParent(towerPegs[0], false);
                allDisks[i].gameObject.SetActive(true);
                allDisks[i].SetAsLastSibling(); 

                // Injectamos o actualizamos el componente de arrastre individual en caliente
                LEHanoiDisk diskScript = allDisks[i].GetComponent<LEHanoiDisk>() ?? allDisks[i].gameObject.AddComponent<LEHanoiDisk>();
                diskScript.SetupDisk(this, allDisks[i]);
            }
        }
        announcementTextMesh.text = "Selecciona o arrastra un disco...";
    }

    public void OnClickTowerPeg(int pegIndex)
    {
        if (!isMinigameActive || repairManager.currentState == LERepairManager.RepairState.Paused) return;

        Transform clickedPeg = towerPegs[pegIndex];

        if (selectedSourcePeg == null)
        {
            if (clickedPeg.childCount > 0)
            {
                selectedSourcePeg = clickedPeg;
                announcementTextMesh.text = "Mover a la torre...";
                SetDiskVisualState(GetTopDisk(selectedSourcePeg), isSelected: true);
            }
            return;
        }

        ExecuteMoveRules(selectedSourcePeg, clickedPeg);
    }

    public void ExecuteMoveRules(Transform sourcePeg, Transform targetPeg)
    {
        RectTransform sourceDisk = GetTopDisk(sourcePeg);

        if (sourceDisk == null) return;

        if (sourcePeg == targetPeg)
        {
            SetDiskVisualState(sourceDisk, isSelected: false);
            selectedSourcePeg = null;
            announcementTextMesh.text = "Selección cancelada.";
            return;
        }

        RectTransform targetDisk = GetTopDisk(targetPeg);

        if (targetDisk == null || GetDiskSize(sourceDisk) < GetDiskSize(targetDisk))
        {
            SetDiskVisualState(sourceDisk, isSelected: false);
            sourceDisk.SetParent(targetPeg, false);
            sourceDisk.SetAsLastSibling(); 

            selectedSourcePeg = null;
            announcementTextMesh.text = "¡Buen movimiento!";
            CheckWinCondition();
        }
        else
        {
            SetDiskVisualState(sourceDisk, isSelected: false);
            selectedSourcePeg = null;
            announcementTextMesh.text = "<color=red>¡Movimiento Inválido!</color>";
        }
    }

    public RectTransform GetTopDisk(Transform peg)
    {
        if (peg.childCount == 0) return null;
        return peg.GetChild(peg.childCount - 1) as RectTransform;
    }

    public int GetDiskSize(RectTransform disk)
    {
        for (int i = 0; i < allDisks.Length; i++)
        {
            if (allDisks[i] == disk) return i;
        }
        return 999;
    }

    public void SetDiskVisualState(RectTransform disk, bool isSelected)
    {
        if (disk == null) return;
        CanvasGroup cg = disk.GetComponent<CanvasGroup>() ?? disk.gameObject.AddComponent<CanvasGroup>();
        cg.alpha = isSelected ? 0.65f : 1.0f;
        disk.localScale = isSelected ? Vector3.one * 1.08f : Vector3.one;
    }

    private void CheckWinCondition()
    {
        if (towerPegs[2].childCount == activeDisksCount)
        {
            isMinigameActive = false;
            announcementTextMesh.text = "¡TORRE COMPLETADA!";
            StartCoroutine(EndMinigameRoutine(won: true));
        }
    }

    private IEnumerator EndMinigameRoutine(bool won)
    {
        yield return new WaitForSeconds(1.3f);
        if (minigamePanel != null) minigamePanel.SetActive(false);
        if (won) repairManager.SimulateWinMinigame();
        else repairManager.RegisterMinigameStrikeFailure();
    }
}
