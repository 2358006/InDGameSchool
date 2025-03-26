using Mirror;
using UnityEngine;
using UnityEngine.UI;

internal class SceneScript : NetworkBehaviour
{
    public Text canvasStatusText;
    public PlayerController playerScript;

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

}