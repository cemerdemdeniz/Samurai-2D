using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAttack : MonoBehaviour
{

    private Animator anim;
    public Transform attackPosition;
    public float radiusOfAttack;
    public LayerMask Enemy;

    public int attackDamage = 40;

    private float nextAttackTime=0f;
    public float attackRate=2f;

    public AudioClip attackSound;
    AudioSource _audio;

    void Start()
    {
        anim = GetComponent<Animator>();
        _audio = GetComponent<AudioSource>();

    }
    void Update()
    {
        Attack();
    }
    void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                anim.SetBool("Attack", true);
                nextAttackTime = Time.time + 1f / attackRate;
                

            }
        }
       

        
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPosition.position, radiusOfAttack, Enemy);  
        
       
        

        foreach (Collider2D enemy in hitEnemies)
        {


            if (hitEnemies != null)
            {
                enemy.GetComponent<Enemy>().TakeDamage(attackDamage/8);
                Debug.Log(attackDamage);
                enemy.GetComponent<Enemy>()._movingToWp = false;

            }
            


        }      
        

    }
    

    private void OnDrawGizmosSelected()
    {
        if (attackPosition == null)
            return;
        Gizmos.DrawWireSphere(attackPosition.position, radiusOfAttack);
    }


}


