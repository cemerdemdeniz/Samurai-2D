using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinColliderPopUp : MonoBehaviour
{
	public GameObject infBallon;
	public Vector3 Offset = new Vector3(1, 1, 0);
	private float DestroyTimeForCoinPopUp = 4f;  
	


	private void Start()
	{
		transform.localPosition += Offset;
		
	}



	private void OnTriggerEnter2D(Collider2D col)
	{
		if (col.tag == "Player")
		{
			Instantiate(infBallon, transform.position, Quaternion.identity);
			DestroyObject(infBallon, DestroyTimeForCoinPopUp);
			Destroy(this.gameObject);
		}

		
	}

	

}
			
