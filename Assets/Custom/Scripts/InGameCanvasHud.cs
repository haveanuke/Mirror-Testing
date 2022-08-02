using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class InGameCanvasHud : MonoBehaviour
{
    public GameObject PanelStart;
    public GameObject PanelStop;

    public Button buttonStop;

    public Text serverText;
    public Text clientText;

    public void ButtonStop() //currently does not work
    {
        //stop host if hosting
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        //stop client
        else if (NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopClient();
        }
        else if (NetworkServer.active)
        {
            NetworkManager.singleton.StopServer();
        }

    }

    public void SetupCanvas()
    {
        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            if (NetworkClient.active)
            {
                PanelStart.SetActive(false);
                PanelStop.SetActive(true);
                clientText.text = "Connecting to " + NetworkManager.singleton.networkAddress + "..";
            }
            else
            {
                PanelStart.SetActive(true);
                PanelStop.SetActive(false);
            }
        }
        else
        {
            PanelStart.SetActive(false);
            PanelStop.SetActive(true);

            // server / client status message
            if (NetworkServer.active)
            {
                serverText.text = "Server: active. Transport: " + Transport.activeTransport;
            }
            if (NetworkClient.isConnected)
            {
                clientText.text = "Client: address=" + NetworkManager.singleton.networkAddress;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        buttonStop.onClick.AddListener(ButtonStop);

    }
}
