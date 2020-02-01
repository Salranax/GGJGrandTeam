using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    private float _speed = 1f;
    private int _interval = 4;

    public LayerMask hitMask;
    public GameObject thrownObjectPrefab;

    [Header("Enemy Stats")]
    public float throwCooldown;
    public float patrolRange;

    private bool _inCombat = false;

    private float _combatTime;
    private Vector2 _startPosition;
    private Vector2 _patrolPosition;

    private GameObject aggroTarget;
    private float cooldownTimer;

    // Start is called before the first frame update
    void Start()
    {
        _startPosition = transform.position;
        _patrolPosition = new Vector2(_startPosition.x - patrolRange, _startPosition.y);
        Debug.Log(_patrolPosition + " / " + _startPosition); 

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Patrol
        if(!_inCombat){
            if(transform.position.x == _patrolPosition.x){
                _patrolPosition = new Vector2(_patrolPosition.x + (patrolRange * 2), _patrolPosition.y);
                Vector3 tmpLocal = transform.localScale;
                transform.localScale = new Vector3(tmpLocal.x * -1, tmpLocal.y, tmpLocal.z);
                patrolRange *= -1;
            }
            else{
                transform.position = Vector2.MoveTowards(transform.position, _patrolPosition, 1f * Time.deltaTime);
            }
        }

        //Aggro Check
        if (Time.frameCount % _interval == 0) {
            RaycastHit2D[] hitObj = Physics2D.RaycastAll(transform.position, (transform.localScale.x < 0 ? Vector2.left : Vector2.right), 10f, hitMask);
            Debug.DrawRay(transform.position, Vector2.right * (transform.localScale.x < 0 ? Vector2.left : Vector2.right));

            if (hitObj != null) {
                    
                foreach(RaycastHit2D item in hitObj){
                    aggroTarget = item.collider.gameObject;
                    _inCombat = true;
                    _combatTime = 0;
                }
            }
        }

        //Combat
        if(_inCombat){
            _combatTime += Time.deltaTime;
            cooldownTimer += Time.deltaTime;

            if(cooldownTimer >= throwCooldown){
                cooldownTimer = 0;
                GameObject tmp = Instantiate(thrownObjectPrefab, transform.position, Quaternion.identity) as GameObject;
                tmp.GetComponent<Rigidbody2D>().velocity = aggroTarget.transform.position - transform.position;
            }

            if(_combatTime > 4f){
                _inCombat = false;
                //TODO: return to idle
            }
        }

    }
}
