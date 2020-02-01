using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterHealth : MonoBehaviour
{

    [Header("Player Settings")]
    public float Health;

    public event System.Action OnPlayerDeath;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetDamage(float Damage)
    {
        Health -= Damage;
        if (Health <= 0f)
        {
            if(OnPlayerDeath != null)
            {
                OnPlayerDeath();
            }
            gameObject.SetActive(false);
        }
    }
}
