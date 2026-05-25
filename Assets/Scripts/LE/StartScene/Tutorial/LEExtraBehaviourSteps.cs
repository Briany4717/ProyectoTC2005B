using UnityEngine;

public class LEExtraBehaviourSteps : MonoBehaviour
{
    public GameObject gellyBox;
    public GameObject playMenu;
    public void Step2ExtraB()
    {
        gellyBox.SetActive(false);
        playMenu.SetActive(true);
    }
}
