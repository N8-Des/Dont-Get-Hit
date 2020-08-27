using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform exit;
    public Transform ballSpawn;
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Dodgeball")
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            Vector3 velocity = rb.velocity;
            velocity = Vector3.Reflect(velocity, transform.forward);
            velocity = transform.InverseTransformDirection(velocity);
            velocity = -exit.TransformDirection(velocity);
            rb.velocity = velocity * 0.35f;
            collider.transform.position = exit.position;
        }

    }
}
