using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartScript : MonoBehaviour
{
    public bool isStart;
    public bool isQuit;
    public Button startbutton;

    // Start is called before the first frame update
    void Start()
    {
        startbutton.onClick.AddListener(Sceneswitch);
    }

    void Sceneswitch()
    {
        Application.LoadLevel(1);
    }
}
