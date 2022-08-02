using UnityEngine;
using UnityEngine.UI;
using Mirror;


public class CanvasHUD : MonoBehaviour
{
    public GameObject PanelStart;
    public GameObject PanelStop;

    public Button buttonHost, buttonServer, buttonClient, buttonStop;

    public InputField inputFieldAddress;

    public Text serverText;
    public Text clientText;

    // Start is called before the first frame update
    private void Start()
    {
        if (NetworkManager.singleton.networkAddress != "localhost")
        {
            inputFieldAddress.text = NetworkManager.singleton.networkAddress;
        }

        inputFieldAddress.onValueChanged.AddListener(delegate { ValueChangeCheck(); });

        buttonHost.onClick.AddListener(ButtonHost);
        buttonServer.onClick.AddListener(ButtonServer);
        buttonClient.onClick.AddListener(ButtonClient);
        buttonStop.onClick.AddListener(ButtonStop);

        SetupCanvas();
    }

    public void ValueChangeCheck()
    {
        NetworkManager.singleton.networkAddress = inputFieldAddress.text;
    }

    public void ButtonHost()
    {
        NetworkManager.singleton.StartHost();
        SetupCanvas();
    }
    public void ButtonServer()
    {
        NetworkManager.singleton.StartServer();
        SetupCanvas();
    }
    public void ButtonClient()
    {
        NetworkManager.singleton.StartClient();
        SetupCanvas();
    }
    public void ButtonStop()
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

        SetupCanvas();
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
    // Update is called once per frame
    void Update()
    {
        
    }
}
