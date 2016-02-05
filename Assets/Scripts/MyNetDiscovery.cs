using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class MyNetDiscovery:NetworkDiscovery {
    private GameNetworkManager _gameNW;

    private void Awake() {
        // Init network manager
        _gameNW = GetComponent<GameNetworkManager>();
    }

    public override void OnReceivedBroadcast(string fromAddress, string data) {
        _gameNW.StartClient(fromAddress);
    }
}