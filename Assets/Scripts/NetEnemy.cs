using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class NetEnemy : MonoBehaviour {
	public int id;
	public int gameID;

    public ThirdPersonCharacter character;
	
	[Header("Movement")]
    public float speed;
    public float acceleration;

    public float stoppingDistance = 1e-3f;

    float currentSpeed = 0;

	Vector3 targetPosition;
	
	public void FixedUpdate(){
        var delta = targetPosition - transform.position;

        if(delta.magnitude > stoppingDistance){
            if (currentSpeed < speed){
                currentSpeed += acceleration * Time.fixedDeltaTime;
            }
            var movement = delta.normalized * currentSpeed;
            character.Move(movement, false, false);
        } else {
            currentSpeed = 0;
            character.Move(Vector3.zero, false, false);
        }
    }

    public void UpdatePosition(Vector3 position)
    {
        targetPosition = position;
    }

	public void Death(){
		GetComponent<Animator>().SetBool("DeathTrigger", true);
	}
}
