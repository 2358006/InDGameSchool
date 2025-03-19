using UnityEngine;
public class OldInputTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Key Pressed");
        }

        if (Input.GetKeyUp(KeyCode.S))
        {
            Debug.Log("Key Released");
        }

        if (Input.GetKey(KeyCode.S))
        {
            Debug.Log("Key Hold");
        }
    }
}
