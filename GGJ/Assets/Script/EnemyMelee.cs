using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float Health;
    public float Speed;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector2(1f, 0f) * Speed*Time.deltaTime);
    }

    public void GetDamage(float Damage)
    {
        Health -= Damage;
        if (Health <= 0f)
        {

            gameObject.SetActive(false);
        }
    }
}
