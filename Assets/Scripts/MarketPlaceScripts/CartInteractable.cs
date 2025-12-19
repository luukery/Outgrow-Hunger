using UnityEngine;
using UnityEngine.UI;

public class CartInteractable : MonoBehaviour
{
    public GameObject popupPanel;
    public Button ContinueButton;
    
    public void Interact()
    {
        //get the scenescroller compoment
        SceneScroller sceneScroller = FindObjectOfType<SceneScroller>();
        sceneScroller.NextScene();
    }
}
