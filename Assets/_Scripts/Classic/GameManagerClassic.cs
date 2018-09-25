using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerClassic : MonoBehaviour {

    #region GAME_MANAGER_STUFF

    public static GameManagerClassic GM;

    [Header("Simulation Settings")]
    public float topBound;
    public float bottonBound;
    public float leftBound;
    public float rightBound;

    [Header("Enemy Settings")]
    public GameObject enemyShipPrefab;
    public float enemySpeed = 1f;

    [Header("Spawn Settings")]
    public int enemyShipCount = 1;
    public int enemyShipIncrement = 1;

    float fps;
    int count;
    #endregion

    private void Awake()
    {
        GM = this;
    }

    // Use this for initialization
    void Start () {
        AddShips(enemyShipCount);
	}

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Space)) || (Input.GetMouseButtonDown(0)))
            AddShips(enemyShipIncrement);

        fps = 1 / Time.deltaTime;
    }

    void AddShips(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            float xVal = Random.Range(leftBound, rightBound);
            float zVal = Random.Range(0f, 10f);

            Vector3 pos = new Vector3(xVal, 0f, zVal + topBound);
            Quaternion rot = Quaternion.Euler(0f, 180f, 0f);

            var obj = Instantiate(enemyShipPrefab, pos, rot) as GameObject;

        }
        count += amount;
    }

    void OnGUI()
    {
        GUI.TextArea(new Rect(10, 10, 150, 100), fps.ToString() + "\n\n" + count.ToString());
    }
}
