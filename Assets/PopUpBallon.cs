using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpBallon : MonoBehaviour
{
    public Vector3 Offset = new Vector3(1,1,0);
    public float DestroyTime = 3f;
    void Start()
    {
        Destroy(gameObject, DestroyTime);
        transform.localPosition += Offset;
    }

    
    
}
