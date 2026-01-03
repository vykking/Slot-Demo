using UnityEngine;

public class Test1 : MonoBehaviour
{
    public bool check = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
     if(check == true)
        {
            Debug.Log("true");
        }

    else { return; }

    }
}
