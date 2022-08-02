using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Mirror;

public class SceneScript : NetworkBehaviour
{
    public Text canvasStatusText;
    public PlayerScript playerScript;
    public SceneReference sceneReference;
    public Text canvasAmmoText;
    public InputField inputFieldMessage;

    [SyncVar(hook = nameof(OnStatusTextChanged))]
    public string statusText;

    void OnStatusTextChanged(string _Old, string _New)
    {
        //called from syncvar to update textbox for all players
        canvasStatusText.text = statusText;
    }

    public void UIAmmo(int _value)
    {
        canvasAmmoText.text = "Ammo: " + _value;
    }
    public void ButtonSendMessage()
    {
        if (playerScript != null)
            playerScript.CmdSendPlayerMessage(inputFieldMessage.text);
    }

    public void ButtonChangeScene()
    {
        if (isServer)
        {
            Scene scene = SceneManager.GetActiveScene();
            if (scene.name == "SampleScene")
            {
                NetworkManager.singleton.ServerChangeScene("SceneTwo");
            }
            else
            {
                NetworkManager.singleton.ServerChangeScene("SampleScene");
            }
        }
        else
        {
            Debug.Log("Not host");
        }
    }
}
