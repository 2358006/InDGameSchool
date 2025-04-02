using Mirror;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//internal class SceneScript : NetworkBehaviour
public class SceneScript : NetworkBehaviour
{
    public Text canvasStatusText;
    public PlayerController playerScript;

    public SceneRefer sceneReference;

    public Text canvasAmmoText;

    public void UIAmmo(int _value) => canvasAmmoText.text = $"Ammo: {_value}";


    [SyncVar(hook = nameof(OnStatusTextChanged))]
    public string statusText;

    void OnStatusTextChanged(string _Old, string _New)
    {
        canvasStatusText.text = statusText;
    }

    public void ButtonSendMessage()
    {
        if (playerScript != null)
            playerScript.CmdSendPlayerMessage();
    }
    public void ButtonChangeScene()
    {
        if (isServer)
        {
            var scene = SceneManager.GetActiveScene();
            if (scene.name.Equals("MyScene"))
            {
                NetworkManager.singleton.ServerChangeScene("MyOtherScene");
            }
            else
            {
                NetworkManager.singleton.ServerChangeScene("MyScene");
            }

        }
        else
        {
            Debug.Log("You are not host");
        }
    }

    void Awake()
    {

    }
}