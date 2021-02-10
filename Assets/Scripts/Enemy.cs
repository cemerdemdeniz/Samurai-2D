using UnityEngine;
using System.Collections;


public class Enemy : MonoBehaviour {




	public GameObject EnemyDiePopUpBallonPrefab;
	public GameObject EnemyBloodParticular; 
	
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
	public int maxhealth = 100;
	int currentHealth;
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
	public bool _movingToWp;

	#region
	public Transform rayCast;
	public LayerMask raycastMask;
	public float raycastLength;
	public float _moveSpeed;
	public float timer;
    
	public float attackDistance;
	public float attackRange;

	#endregion

	private RaycastHit2D hit;
	private GameObject target;
	private float distance;
	private bool attackMode;
	private bool inRange;
	private bool cooling;
	private float intTimer;

	private float attackRate = 2f;
	private float nextAttackTime = 0f;







	void Awake() {
		// get a reference to the components we are going to be changing and store a reference for efficiency purposes
		_transform = GetComponent<Transform> ();		
		_rigidbody = GetComponent<Rigidbody2D> ();		
		_animator = GetComponent<Animator>();		
		_audio = GetComponent<AudioSource> ();		
		_audio = gameObject.AddComponent<AudioSource>();		
		
		// setup moving defaults
		_moveTime = 0f;
		_moving = true;
		_movingToWp = true;

		intTimer = timer;

		
		// determine the enemies specified layer
		_enemyLayer = this.gameObject.layer;

		// determine the stunned enemy layer number
		_stunnedLayer = LayerMask.NameToLayer(stunnedLayer);

		// make sure collision are off between the playerLayer and the stunnedLayer
		// which is where the enemy is placed while stunned
		Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(playerLayer), _stunnedLayer,  true) ;

		currentHealth = maxhealth;
		
	}
	
	// if not stunned then move the enemy when time is > _moveTime
	void Update () {

		

		if (_movingToWp)
		{
			
			if (!isStunned)
			{
				if (Time.time >= _moveTime)
				{
					EnemyMovement();
				}
			}
		}
		else
		{
			_animator.SetBool("Moving", false);
		}


		if (inRange)
		{

			hit = Physics2D.Raycast(rayCast.position, Vector2.left, raycastLength,raycastMask);
			RaycastDebugger();

		}

		if(hit.collider != null)
		{
			EnemyLogic();
		}else if (hit.collider == null)
		{
			inRange = false;
		}

		//if ( inRange == false)
		//{
		//	_movingToWp = false;
		//	StopAttack();
		//}


		



	}

	void RaycastDebugger()
	{
		if(distance > attackDistance)
		{
			Debug.DrawRay(rayCast.position, Vector2.left * raycastLength, Color.red);
		}else if ( attackDistance > distance)
		{
			Debug.DrawRay(rayCast.position, Vector2.left * raycastLength, Color.green);
		}
	}
	
	public void TakeDamage(int damage)
	{
		
		currentHealth -= damage;
		
		_animator.SetTrigger("Hurt");
				
		if (currentHealth <= 0)
		{
			var _effectAfterDeath = Camera.main.GetComponent<RipplePostProcessor>();
			_effectAfterDeath.SceenEffectAfterDeath();
			var _blood = Instantiate(EnemyBloodParticular, transform.position, Quaternion.identity);
			Instantiate(EnemyDiePopUpBallonPrefab, transform.position,Quaternion.identity);
			_animator.SetBool("Die", true);			
			StartCoroutine(Diee(3));
			Destroy(this.gameObject, 3);
			Destroy(_blood.gameObject, 3);

			
			
			
			
		}
	}

	public void EnemyMovement()
	{
		if ((myWaypoints.Length != 0) && (_moving))
		{

			// make sure the enemy is facing the waypoint (based on previous movement)
			Flip(_vx);

			// determine distance between waypoint and enemy
			_vx = myWaypoints[_myWaypointIndex].transform.position.x - _transform.position.x;

			// if the enemy is close enough to waypoint, make it's new target the next waypoint
			if (Mathf.Abs(_vx) <= 0.05f)
			{
				// At waypoint so stop moving
				_rigidbody.velocity = new Vector2(0, 0);

				// increment to next index in array
				_myWaypointIndex++;

				// reset waypoint back to 0 for looping
				if (_myWaypointIndex >= myWaypoints.Length)
				{
					if (loopWaypoints)
						_myWaypointIndex = 0;
					else
						_moving = false;
				}


			_moveTime = Time.time + waitAtWaypointTime;
			}
			else
			{
				// enemy is moving
				_animator.SetBool("Moving", true);

				// Set the enemy's velocity to moveSpeed in the x direction.
				_rigidbody.velocity = new Vector2(_transform.localScale.x* moveSpeed, _rigidbody.velocity.y);
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


	void EnemyLogic()
	{
		distance = Vector2.Distance(transform.position, target.transform.position);
		if (distance > attackDistance)
		{
			_movingToWp = true;
			EnemyMovement();
			StopAttack();
		}
		else if ( attackDistance >= distance && cooling == false)
		{
			Attack();
		}
		if (cooling)
		{
			_animator.SetBool("Attack", false);
		}

	} 

	void Attack()
	{
		
		if (Time.time > nextAttackTime)
		{

			_animator.SetBool("Attack", true);
			nextAttackTime = Time.time + 5f / attackRate;
			
		}
		timer = intTimer;
		attackMode = true;
		_movingToWp = false;
		
	}

	void StopAttack()
	{
		cooling = false;
		attackMode = false;
		_animator.SetBool("Attack", false);
	}

	
	
	
	void OnTriggerEnter2D(Collider2D collision)
	{
		
		if ((collision.tag == "GroundCheck") && !isStunned)
		{
			CharacterController2D player = collision.gameObject.GetComponent<CharacterController2D>();
			if (player.playerCanMove) {
				// Make sure the enemy is facing the player on attack
				Flip(collision.transform.position.x-_transform.position.x);
				
				// attack sound
				PlaySound(attackSFX);
				
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
		
		
		
		
	}
	
	void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.tag=="MovingPlatform")
		{
			this.transform.parent = other.transform;
		}
		
		
		
	}
	
	
	void OnCollisionExit2D(Collision2D other)
	{
		if (other.gameObject.tag=="MovingPlatform")
		{
			this.transform.parent = null;
		}
	}
	
	
	void PlaySound(AudioClip clip)
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
			PlaySound(stunnedSFX);
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
	IEnumerator Diee(float delay)
	{
		Destroy(_rigidbody);
		
		isStunned = true;
		this.transform.GetComponent<BoxCollider2D>().enabled = false;
		this.transform.GetComponent<CircleCollider2D>().enabled = false;
		this.transform.GetChild(1).gameObject.SetActive(false);
		
		yield return new WaitForSeconds(delay);
		
		
		




	}

	
}
