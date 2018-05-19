using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class NetPlayer : MonoBehaviour
{

    public TextMeshPro text;
    public Transform textTransform;
    public ThirdPersonCharacter character;
    public int id;
    string nickname;

    public string Nickname
    {
        get
        {
            return nickname;
        }
        set
        {
            nickname = value;
            text.text = value;
        }
    }

    [Header("Movement")]
    public float speed;
    public float acceleration;

    public float stoppingDistance = 1e-3f;

    float currentSpeed = 0;

    Vector3 targetPosition;

    public void Update()
    {
        textTransform.LookAt(Camera.main.transform);
    }

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
}
