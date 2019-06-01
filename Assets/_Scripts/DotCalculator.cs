using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotCalculator : MonoBehaviour
{

    public Transform p1;
    public Transform p2;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DotLogger(p1, p2);
        DotLogger(p2, p1);

    }

    private void DotLogger(Transform p1, Transform p2)
    {
        var dot = Vector3.Dot(p1.forward, Vector3.Normalize(p2.position - p1.position));
        Debug.Log(p1.name + ": " + dot);
        Debug.DrawLine(p1.position, p2.position);
    }
}
