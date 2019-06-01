using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private Transform _motor;
    [SerializeField] private Transform _dir;
    public float Speed;

    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {


        var dir = (_motor.position - _dir.position);
        
        _rigidBody.AddForce((_motor.position - _dir.position) * Speed, ForceMode.Acceleration);

    }
}
