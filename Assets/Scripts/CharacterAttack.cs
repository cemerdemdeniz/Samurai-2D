using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAttack : MonoBehaviour
{   
    public float attackRangeX;
    public float attackRangeY;


    private Animator anim;
    public Transform attackPosition;
   
    public float attackRange;
    public LayerMask whatIsEnemy;
    public int damage;
    public AudioClip attackSound;
    AudioSource _audio;

     void Start()
    {
        anim = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>(); 

    }
     void Update()
    {
        
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                
                Attack();
                PlaySound(attackSound);
                
            }
            
        }
        
    }
    void Attack()
    {
        anim.SetTrigger("Attack");
        Collider2D[] enemiesToDamage = Physics2D.OverlapBoxAll(attackPosition.position, new Vector2(attackRangeX, attackRangeY), 0, whatIsEnemy);
        foreach (Collider2D enemy in enemiesToDamage)
        {
            Debug.Log("We hit" + enemy.name);
        }
        

    }
    void PlaySound(AudioClip clip)
    {
        _audio.PlayOneShot(clip);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(attackPosition.position,new Vector3(attackRangeX,attackRangeY,1));
    }

}


