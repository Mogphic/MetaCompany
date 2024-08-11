using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceConsole : MonoBehaviour
{
    public static SequenceConsole instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
        }
        
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
