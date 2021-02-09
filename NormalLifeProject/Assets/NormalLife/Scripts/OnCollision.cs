using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnCollision : MonoBehaviour
{
    public Character mc;
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            return;
        if (collision.gameObject.CompareTag("Ground"))
            return;
        mc.OnCharacterColliderHit(collision.collider);
    }
}
