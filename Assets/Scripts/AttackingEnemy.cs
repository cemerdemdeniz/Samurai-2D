using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackingEnemy : MonoBehaviour
{
    Animator anim;
    RaycastHit2D ray;
    bool seenPlayer = false ;
    public Vector3 attackInRange;
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    
    void Update()
    {
        Debug.DrawRay(transform.position, attackInRange,Color.green);
    }
}
