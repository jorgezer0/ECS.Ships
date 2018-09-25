using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {
    
	// Update is called once per frame
	void Update () {
        Vector3 pos = transform.position;
        pos += transform.forward * GameManagerClassic.GM.enemySpeed * Time.deltaTime;

        if (pos.z < GameManagerClassic.GM.bottonBound)
            pos.z = GameManagerClassic.GM.topBound;

        transform.position = pos;
    }
}
