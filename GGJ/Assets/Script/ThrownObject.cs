using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownObject : MonoBehaviour
{
    public int defaultDamage;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player") {
            //TODO: Give Damage
            Destroy(this);
        }
        else if (collision.gameObject.tag == "Ground") {
            Destroy(this);
        }
    }
}
