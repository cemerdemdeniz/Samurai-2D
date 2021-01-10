﻿using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	

	
	
	// private variables below
	
	// store references to components on the gameObject
	Transform _transform;
	Rigidbody2D _rigidbody;
	Animator _animator;
	AudioSource _audio;
	
	// movement tracking
	int _myWaypointIndex = 0; // used as index for My_Waypoints
	float _moveTime; 
	float _vx = 0f;
	bool _moving = true;
	
	// store the layer number the enemy is on (setup in Awake)
	int _enemyLayer;

	// store the layer number the enemy should be moved to when stunned
	int _stunnedLayer;

	int _swordLayer;
	#region Public Variables
	[Range(0f, 10f)]
	public float moveSpeed = 4f;  // enemy move speed when moving
	public int damageAmount = 10; // probably deal a lot of damage to kill player immediately
	public int health = 100;
	public GameObject stunnedCheck; // what gameobject is the stunnedCheck

	public float stunnedTime = 3f;   // how long to wait at a waypoint

	public string stunnedLayer = "StunnedEnemy";  // name of the layer to put enemy on when stunned
	public string playerLayer = "Player";  // name of the player layer to ignore collisions with when stunned


	public bool isStunned = false;  // flag for isStunned

	public GameObject[] myWaypoints; // to define the movement waypoints

	public float waitAtWaypointTime = 1f;   // how long to wait at a waypoint

	public bool loopWaypoints = true; // should it loop through the waypoints
									  // SFXs
	public AudioClip stunnedSFX;
	public AudioClip attackSFX;
	#endregion
	#region Public Variables 2
	public Transform rayCast;
	public LayerMask raycastMask;
	public float RaycastLengt;
	public float attackDistance;
	public float timer;
	#endregion
	#region Private Variables
	private RaycastHit2D hit;
	private GameObject target;
	private float distance; // distance b/w enemy and player
	private bool attackMode;
	private bool inRange; // check if player in range
	private bool cooling; // cool down after attack
	private float intTimer;
	
    #endregion



    void Awake() {
		// get a reference to the components we are going to be changing and store a reference for efficiency purposes
		_transform = GetComponent<Transform> ();
		
		_rigidbody = GetComponent<Rigidbody2D> ();
		if (_rigidbody==null) // if Rigidbody is missing
			Debug.LogError("Rigidbody2D component missing from this gameobject");
		
		_animator = GetComponent<Animator>();
		if (_animator==null) // if Animator is missing
			Debug.LogError("Animator component missing from this gameobject");
		
		_audio = GetComponent<AudioSource> ();
		if (_audio==null) { // if AudioSource is missing
			Debug.LogWarning("AudioSource component missing from this gameobject. Adding one.");
			// let's just add the AudioSource component dynamically
			_audio = gameObject.AddComponent<AudioSource>();
		}

		if (stunnedCheck==null) {
			Debug.LogError("stunnedCheck child gameobject needs to be setup on the enemy");
		}
		
		// setup moving defaults
		_moveTime = 0f;
		_moving = true;
		
		// determine the enemies specified layer
		_enemyLayer = this.gameObject.layer;

		// determine the stunned enemy layer number
		_stunnedLayer = LayerMask.NameToLayer(stunnedLayer);

		intTimer = timer;//store the initial value of timer
		

		// make sure collision are off between the playerLayer and the stunnedLayer
		// which is where the enemy is placed while stunned
		Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(playerLayer), _stunnedLayer,  true) ;
		
	}
	
	// if not stunned then move the enemy when time is > _moveTime
	void Update () {
		if (!isStunned)
		{
			if (Time.time >= _moveTime) {
				EnemyMovement();
				
			} else {
				_animator.SetBool("Moving", false);
			}
		}
		if (health <= 0)
		{
			Destroy(gameObject);
		}
		if (inRange)
		{
			hit = Physics2D.Raycast(rayCast.position, Vector2.left, RaycastLengt, raycastMask);
			RayCastDebugger();
		}
		//when player is Detected
		if (hit.collider != null)
		{
			EnemyLogic();
		}
		else if (hit.collider==null)
		{
			inRange = false;
		}
		if (inRange == false)
		{
			
			StopAttack();
		}

		if (health <= 0)
		{
			Death();
		}

	}

	void EnemyLogic()
	{
		distance = Vector2.Distance(transform.position, target.transform.position);

		if (distance > attackDistance)
		{
			EnemyMovement();
			StopAttack();
		}
		else if ( attackDistance >=distance && cooling == false)
		{
			Attacking();
		}
		if (cooling)
		{
			//anim . set bool attack false
			_animator.SetBool("Attack", false);
		}
	}
	void RayCastDebugger()
	{
		if(distance > attackDistance)
		{
			Debug.DrawRay(rayCast.position, Vector2.left * RaycastLengt, Color.red);
		}
		else if (attackDistance > distance)
		{
			Debug.DrawRay(rayCast.position, Vector2.left * RaycastLengt, Color.blue);
		}
	}
	
	// Move the enemy through its rigidbody based on its waypoints
	void EnemyMovement() {
		// if there isn't anything in My_Waypoints
		if ((myWaypoints.Length != 0) && (_moving)) {
			
			// make sure the enemy is facing the waypoint (based on previous movement)
			Flip (_vx);
			
			// determine distance between waypoint and enemy
			_vx = myWaypoints[_myWaypointIndex].transform.position.x-_transform.position.x;
			
			// if the enemy is close enough to waypoint, make it's new target the next waypoint
			if (Mathf.Abs(_vx) <= 0.05f) {
				// At waypoint so stop moving
				_rigidbody.velocity = new Vector2(0, 0);
				
				// increment to next index in array
				_myWaypointIndex++;
				
				// reset waypoint back to 0 for looping
				if(_myWaypointIndex >= myWaypoints.Length) {
					if (loopWaypoints)
						_myWaypointIndex = 0;
					else
						_moving = false;
				}
				
				// setup wait time at current waypoint
				_moveTime = Time.time + waitAtWaypointTime;
			} else {
				// enemy is moving
				_animator.SetBool("Moving", true);
				
				// Set the enemy's velocity to moveSpeed in the x direction.
				_rigidbody.velocity = new Vector2(_transform.localScale.x * moveSpeed, _rigidbody.velocity.y);
			}
			
		}
	}
	
	// flip the enemy to face torward the direction he is moving in
	void Flip(float _vx) {
		
		// get the current scale
		Vector3 localScale = _transform.localScale;
		
		if ((_vx>0f)&&(localScale.x<0f))
			localScale.x*=-1;
		else if ((_vx<0f)&&(localScale.x>0f))
			localScale.x*=-1;
		
		// update the scale
		_transform.localScale = localScale;
	}

	void Attacking ()
	{
		timer = intTimer;
		attackMode = true;
		_animator.SetBool("Moving", false);
		_animator.SetBool("Attack", true);
	}
	void StopAttack()
	{
		cooling = false;
		attackMode = false;
		_animator.SetBool("Attack", false);
	}
	
	// Attack player
	void OnTriggerEnter2D(Collider2D collision)
	{
		if ((collision.tag == "Player") && !isStunned)
		{
			CharacterController2D player = collision.gameObject.GetComponent<CharacterController2D>();
			if (player.playerCanMove) {
				// Make sure the enemy is facing the player on attack
				Flip(collision.transform.position.x-_transform.position.x);
				
				// attack sound
				playSound(attackSFX);
				
				// stop moving
				_rigidbody.velocity = new Vector2(0, 0);
				
				
				
				// stop to enjoy killing the player
				_moveTime = Time.time + stunnedTime;
			}
		}
		if (collision.gameObject.tag == "Player")
		{
			target = collision.gameObject;
			inRange = true;
		}
		if (collision.gameObject.tag == "Sword")
		{
			
			
		}
		
	}
	
	// if the Enemy collides with a MovingPlatform, then make it a child of that platform
	// so it will go for a ride on the MovingPlatform
	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.tag=="MovingPlatform")
		{
			this.transform.parent = other.transform;
		}
		
		
		
	}
	
	// if the enemy exits a collision with a moving platform, then unchild it
	void OnCollisionExit2D(Collision2D other)
	{
		if (other.gameObject.tag=="MovingPlatform")
		{
			this.transform.parent = null;
		}
	}
	
	// play sound through the audiosource on the gameobject
	void playSound(AudioClip clip)
	{
		//_audio.PlayOneShot(clip);
	}
	
	// setup the enemy to be stunned
	public void Stunned()
	{
		if (!isStunned) 
		{
			isStunned = true;
			
			// provide the player with feedback that enemy is stunned
			playSound(stunnedSFX);
			_animator.SetTrigger("Stunned");
			
			
			// stop moving
			_rigidbody.velocity = new Vector2(0, 0);
			
			// switch layer to stunned layer so no collisions with the player while stunned
			this.gameObject.layer = _stunnedLayer;
			stunnedCheck.layer = _stunnedLayer;

			// start coroutine to stand up eventually
			StartCoroutine (Stand ());
		}
	}
	
	void Death ()
	{
		    StartCoroutine(Diee());
			Destroy(this.gameObject);
		
	}
	
	
	
	// coroutine to unstun the enemy and stand back up
	IEnumerator Stand()
	{
		yield return new WaitForSeconds(stunnedTime); 
		
		// no longer stunned
		isStunned = false;
		
		// switch layer back to regular layer for regular collisions with the player
		this.gameObject.layer = _enemyLayer;
		stunnedCheck.layer = _enemyLayer;
		
		// provide the player with feedback
		_animator.SetTrigger("Stand");
	}
	IEnumerator Diee()
	{
		yield return new WaitForSeconds(2);
		_animator.SetTrigger("Die");
	}
}
