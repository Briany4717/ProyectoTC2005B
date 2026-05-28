using UnityEngine;

public class LEExtraBehaviourSteps : MonoBehaviour
{
    public GameObject gellyBox;
    public GameObject playMenu;
    public GameObject playBtn;
    public void Step2ExtraB()
    {
        gellyBox.SetActive(false);
        playMenu.SetActive(true);
    }

    public void FinalStepExtraB()
    {
    }
}
