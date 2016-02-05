using UnityEngine;
using System.Collections;
#if UNITY_ANDROID
using LostPolygon.AndroidBluetoothMultiplayer;
#endif
using UnityEngine.Networking;

public class GameNetworkManager:MonoBehaviour {
    private const string LOCAL_IP = "127.0.0.1"; // An IP for Network.Connect(), must always be 127.0.0.1
    private const int PORT = 28111; // Local server IP. Must be the same for client and server

    private bool _initResult;
    private bool _connected;

    private MyNetManager _nw;
    private MyNetDiscovery _nd;

#if !UNITY_ANDROID
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
        }
        if(GUI.Button(new Rect(380, 10, 150, 50), "Join server")) {
            _nd.Initialize();
            _nd.StartAsClient();
            _connected = true;
        }
    }

    public void StartClient(string address) {
        _nw.networkAddress = address;
        _nw.StartClient();
        _connected = true;
        _nd.StopBroadcast();
    }
#endif

#if UNITY_ANDROID
    private BluetoothMultiplayerMode _desiredMode = BluetoothMultiplayerMode.None;

    private void Awake() {
        // Setting the UUID. Must be unique for every application
        _initResult = AndroidBluetoothMultiplayer.Initialize("c39f5ff4-c717-11e5-9912-ba0be0483c18");

        // Init network manager
        _nw = GetComponent<NetworkManager>();
        _nw.networkAddress = LOCAL_IP;
        _nw.networkPort = PORT;

        // Registering the event delegates
        AndroidBluetoothMultiplayer.ListeningStarted += OnBluetoothListeningStarted;
        AndroidBluetoothMultiplayer.ListeningStopped += OnBluetoothListeningStopped;
        AndroidBluetoothMultiplayer.AdapterEnabled += OnBluetoothAdapterEnabled;
        AndroidBluetoothMultiplayer.AdapterEnableFailed += OnBluetoothAdapterEnableFailed;
        AndroidBluetoothMultiplayer.AdapterDisabled += OnBluetoothAdapterDisabled;
        AndroidBluetoothMultiplayer.DiscoverabilityEnabled += OnBluetoothDiscoverabilityEnabled;
        AndroidBluetoothMultiplayer.DiscoverabilityEnableFailed += OnBluetoothDiscoverabilityEnableFailed;
        AndroidBluetoothMultiplayer.ConnectedToServer += OnBluetoothConnectedToServer;
        AndroidBluetoothMultiplayer.ConnectionToServerFailed += OnBluetoothConnectionToServerFailed;
        AndroidBluetoothMultiplayer.DisconnectedFromServer += OnBluetoothDisconnectedFromServer;
        AndroidBluetoothMultiplayer.ClientConnected += OnBluetoothClientConnected;
        AndroidBluetoothMultiplayer.ClientDisconnected += OnBluetoothClientDisconnected;
        AndroidBluetoothMultiplayer.DevicePicked += OnBluetoothDevicePicked;
    }

    // Don't forget to unregister the event delegates!
    private void OnDestroy() {
        AndroidBluetoothMultiplayer.ListeningStarted -= OnBluetoothListeningStarted;
        AndroidBluetoothMultiplayer.ListeningStopped -= OnBluetoothListeningStopped;
        AndroidBluetoothMultiplayer.AdapterEnabled -= OnBluetoothAdapterEnabled;
        AndroidBluetoothMultiplayer.AdapterEnableFailed -= OnBluetoothAdapterEnableFailed;
        AndroidBluetoothMultiplayer.AdapterDisabled -= OnBluetoothAdapterDisabled;
        AndroidBluetoothMultiplayer.DiscoverabilityEnabled -= OnBluetoothDiscoverabilityEnabled;
        AndroidBluetoothMultiplayer.DiscoverabilityEnableFailed -= OnBluetoothDiscoverabilityEnableFailed;
        AndroidBluetoothMultiplayer.ConnectedToServer -= OnBluetoothConnectedToServer;
        AndroidBluetoothMultiplayer.ConnectionToServerFailed -= OnBluetoothConnectionToServerFailed;
        AndroidBluetoothMultiplayer.DisconnectedFromServer -= OnBluetoothDisconnectedFromServer;
        AndroidBluetoothMultiplayer.ClientConnected -= OnBluetoothClientConnected;
        AndroidBluetoothMultiplayer.ClientDisconnected -= OnBluetoothClientDisconnected;
        AndroidBluetoothMultiplayer.DevicePicked -= OnBluetoothDevicePicked;
    }

    private void OnGUI() {
        // Don't show GUI once connected
        if(_connected)
            return;
        // If initialization was successfull, showing the buttons
        if(_initResult) {
            // If there is no current Bluetooth connectivity
            BluetoothMultiplayerMode currentMode = AndroidBluetoothMultiplayer.GetCurrentMode();
            if(currentMode == BluetoothMultiplayerMode.None) {
                if(GUI.Button(new Rect(210, 10, 150, 50), "Create server")) {
                    // If Bluetooth is enabled, then we can do something right on
                    if(AndroidBluetoothMultiplayer.GetIsBluetoothEnabled()) {
                        AndroidBluetoothMultiplayer.RequestEnableDiscoverability(120);
                        AndroidBluetoothMultiplayer.StartServer(PORT);
                    } else {
                        // Otherwise we have to enable Bluetooth first and wait for callback
                        _desiredMode = BluetoothMultiplayerMode.Server;
                        AndroidBluetoothMultiplayer.RequestEnableDiscoverability(120);
                    }
                }

                if(GUI.Button(new Rect(370, 10, 150, 50), "Connect to server")) {
                    // If Bluetooth is enabled, then we can do something right on
                    if(AndroidBluetoothMultiplayer.GetIsBluetoothEnabled()) {
                        AndroidBluetoothMultiplayer.ShowDeviceList(); // Open device picker dialog
                    } else {
                        // Otherwise we have to enable Bluetooth first and wait for callback
                        _desiredMode = BluetoothMultiplayerMode.Client;
                        AndroidBluetoothMultiplayer.RequestEnableBluetooth();
                    }
                }
            }
        }
    }

    #region Bluetooth events

    private void OnBluetoothListeningStarted() {
        Debug.Log("Event - ListeningStarted");

        // Starting Unity networking server if Bluetooth listening started successfully
        _nw.StartHost();
        _connected = true;
    }

    private void OnBluetoothListeningStopped() {
        Debug.Log("Event - ListeningStopped");

        // For demo simplicity, stop server if listening was canceled
        AndroidBluetoothMultiplayer.Stop();
        Application.Quit();
    }

    private void OnBluetoothDevicePicked(BluetoothDevice device) {
        Debug.Log("Event - DevicePicked: " + device.Name);

        // Trying to connect to a device user had picked
        AndroidBluetoothMultiplayer.Connect(device.Address, PORT);
    }

    private void OnBluetoothClientDisconnected(BluetoothDevice device) {
        Debug.Log("Event - ClientDisconnected: " + device.Name);
    }

    private void OnBluetoothClientConnected(BluetoothDevice device) {
        Debug.Log("Event - ClientConnected: " + device.Name);
    }

    private void OnBluetoothDisconnectedFromServer(BluetoothDevice device) {
        Debug.Log("Event - DisconnectedFromServer: " + device.Name);

        // Stopping Unity networking on Bluetooth failure
        _nw.StopHost();
    }

    private void OnBluetoothConnectionToServerFailed(BluetoothDevice device) {
        Debug.Log("Event - ConnectionToServerFailed: " + device.Name);
    }

    private void OnBluetoothConnectedToServer(BluetoothDevice device) {
        Debug.Log("Event - ConnectedToServer: " + device.Name);

        // Trying to negotiate a Unity networking connection, 
        // when Bluetooth client connected successfully
        _nw.StartClient();
        _connected = true;
    }

    private void OnBluetoothAdapterDisabled() {
        Debug.Log("Event - AdapterDisabled");
    }

    private void OnBluetoothAdapterEnableFailed() {
        Debug.Log("Event - AdapterEnableFailed");
    }

    private void OnBluetoothAdapterEnabled() {
        Debug.Log("Event - AdapterEnabled");

        // Resuming desired action after enabling the adapter
        switch(_desiredMode) {
        case BluetoothMultiplayerMode.Server:
            AndroidBluetoothMultiplayer.StartServer(PORT);
            break;
        case BluetoothMultiplayerMode.Client:
            AndroidBluetoothMultiplayer.ShowDeviceList();
            break;
        }

        _desiredMode = BluetoothMultiplayerMode.None;
    }

    private void OnBluetoothDiscoverabilityEnableFailed() {
        Debug.Log("Event - DiscoverabilityEnableFailed");
    }

    private void OnBluetoothDiscoverabilityEnabled(int discoverabilityDuration) {
        Debug.Log(string.Format("Event - DiscoverabilityEnabled: {0} seconds", discoverabilityDuration));
    }

    #endregion Bluetooth events

    #region Network events

    private void OnPlayerDisconnected(NetworkPlayer player) {
        Debug.Log("Player disconnected: " + player.GetHashCode());
    }

    private void OnFailedToConnect(NetworkConnectionError error) {
        Debug.Log("Can't connect to the networking server");

        // Stopping all Bluetooth connectivity on Unity networking disconnect event
        AndroidBluetoothMultiplayer.Stop();
        Application.Quit();
    }

    private void OnDisconnectedFromServer() {
        Debug.Log("Disconnected from server");

        // Stopping all Bluetooth connectivity on Unity networking disconnect event
        AndroidBluetoothMultiplayer.Stop();
        Application.Quit();
    }

    private void OnConnectedToServer() {
        Debug.Log("Connected to server");
    }

    private void OnServerInitialized() {
        Debug.Log("Server initialized");
    }

    #endregion Network events

#endif
}