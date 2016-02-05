using UnityEngine;

public class GameNetworkManager:MonoBehaviour {
    private const string LOCAL_IP = "127.0.0.1"; // An IP for Network.Connect(), must always be 127.0.0.1
    private const int PORT = 28111; // Local server IP. Must be the same for client and server

    private bool _initResult;
    private bool _connected;

    public GameObject cardboardMain;

    private MyNetManager _nw;
    private MyNetDiscovery _nd;
    
    private void Awake() {
        // Init network manager
        _nw = GetComponent<MyNetManager>();
        _nw.networkAddress = LOCAL_IP;
        _nw.networkPort = PORT;

        _nd = GetComponent<MyNetDiscovery>();
    }

    private void OnGUI() {
        // Don't show GUI once connected
        if(_connected)
            return;
        // Debug
        if(GUI.Button(new Rect(210, 10, 150, 50), "Create server")) {
            _nw.StartHost();
            _connected = true;

            cardboardMain.SetActive(true);
        }
        if(GUI.Button(new Rect(380, 10, 150, 50), "Join server")) {
            _nd.Initialize();
            _nd.StartAsClient();
            _connected = true;

            cardboardMain.SetActive(false);
        }
    }

    public void StartClient(string address) {
        _nw.networkAddress = address;
        _nw.StartClient();
        _connected = true;
        _nd.StopBroadcast();
    }
}