using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameManager:NetworkBehaviour {
    public void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)) {
            if(isServer) {
                NetworkManager.singleton.StopHost();
                SceneManager.LoadScene("Game");
            } else {
                NetworkManager.singleton.client.Disconnect();
                SceneManager.LoadScene("Game");
            }
        }
    }
}