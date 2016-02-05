using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MyNetDiscovery:NetworkDiscovery {
    private GameNetworkManager _gameNW;
    private bool startedClient = false;

    private void Awake() {
        // Init network manager
        _gameNW = GetComponent<GameNetworkManager>();
    }

    public void LateUpdate() {
        if(startedClient) {
            StopBroadcast();
            startedClient = false;
        }
    }

    public override void OnReceivedBroadcast(string fromAddress, string data) {
        if(startedClient)
            return;
        _gameNW.StartClient(fromAddress);
        startedClient = true;
    }
}