using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.Characters.ThirdPerson;

public class PlayerController : MonoBehaviour
{

    public Camera cam;

    public NavMeshAgent agent;

    public ThirdPersonCharacter character;
    // Use this for initialization
    void Start()
    {
        //agent.updatePosition = false;
        agent.updateRotation = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1)){
            var target = UseAbility(1);
            StartCoroutine(Turn(target));
            GetComponent<Animator>().SetTrigger("Attack");
        }
        if (Input.GetMouseButton(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, LayerMask.NameToLayer("Player")))
            {
                agent.SetDestination(hit.point);
            }
        }

        if (agent.remainingDistance > agent.stoppingDistance)
        {
            character.Move(agent.desiredVelocity, false, false);
        }
        else
        {
            character.Move(Vector3.zero, false, false);
        }

        UpdatePosition();
    }

    void UpdatePosition()
    {
        using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
        using (System.IO.BinaryWriter bw = new System.IO.BinaryWriter(ms))
        {
            Vector3 desPosition = transform.position;
            if (agent.remainingDistance > agent.stoppingDistance)
            {
                desPosition += agent.desiredVelocity;
            }
            bw.Write((int)(NetMessage.UpdatePosition));
            bw.Write(desPosition);
            Networking.Instance.SendUnReliable(ms.ToArray());
        }
    }

    Vector3 UseAbility(int type){
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 100f, LayerMask.NameToLayer("Player")))
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            using (System.IO.BinaryWriter bw = new System.IO.BinaryWriter(ms))
            {
                bw.Write((int)(NetMessage.UseAbility));
                bw.Write(type);
                bw.Write(hit.point);
                Networking.Instance.SendReliable(ms.ToArray());
            }
            return hit.point;
        }
        return Vector3.zero;
    }

    IEnumerator Turn(Vector3 target){
        var direction = (target - transform.position);
        direction = Vector3.ProjectOnPlane(direction, Vector3.up).normalized;
        while (Mathf.Abs(Quaternion.Angle(transform.rotation,
            transform.rotation * Quaternion.FromToRotation(transform.forward, direction))) > 10)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation,
                transform.rotation * Quaternion.FromToRotation(transform.forward, direction),
                800f * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    void Hit(Object obj){

    }
}
