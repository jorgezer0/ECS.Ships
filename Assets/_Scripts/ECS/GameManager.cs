using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Ships.ECS
{
    public class GameManager : MonoBehaviour
    {
        #region GAME_MANAGER_STUFF
        public static GameManager GM;

        public NativeArray<Entity> entities = new NativeArray<Entity>();

        private Camera cam;

        [Header("Simulation Settings")]
        public float topBound;
        public float bottonBound;
        public float leftBound;
        public float rightBound;
        public Transform point;
        public float rotationSpeed;

        [Header("Enemy Settings")]
        public GameObject enemyShipPrefabA;
        public GameObject enemyShipPrefabB;
        public float enemySpeed = 1f;

        [Header("Spawn Settings")]
        public int enemyShipCount = 1;
        public int enemyShipIncrement = 1;

        [Header("Mother Ships")]
        public Transform mShip1;
        public Transform mShip2;

        float fps;
        int count;
        #endregion

        EntityManager manager;

        private void Awake()
        {
            GM = this;
        }

        private void Start()
        {
            cam = Camera.main;
            Debug.Log(GM);
            manager = World.Active.GetOrCreateManager<EntityManager>();
            AddShips(enemyShipCount, 1);
            AddShips(enemyShipCount, 2);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                AddShips(enemyShipIncrement, 1);
                AddShips(enemyShipIncrement, 2);
            }

            fps = 1 / Time.deltaTime;
            /*
            if (Input.GetMouseButtonUp(0))
            {
                var ray = cam.ScreenPointToRay(Input.mousePosition);
                var pt = ray.origin + ray.direction;
                point = new Vector3(pt.x, 0, pt.z) * 100;
                Debug.Log(point);
            }
            */
        }

        void AddShips(int amount, int team)
        {
            NativeArray<Entity> entities = new NativeArray<Entity>(amount, Allocator.Temp);
            if (team == 1)
            {
                manager.Instantiate(enemyShipPrefabA, entities);
            } else if (team == 2)
            {
                manager.Instantiate(enemyShipPrefabB, entities);
            }
            
            for (int i = 0; i < amount; i++)
            {
                float xVal = UnityEngine.Random.Range(leftBound, rightBound);
                float yVal = UnityEngine.Random.Range(-100f, 100f);
                float zVal = UnityEngine.Random.Range(0f, 60f);

                if (team == 1)
                {
                    manager.SetComponentData(entities[i], new Position { Value = new float3(xVal, yVal, topBound + zVal) });
                    manager.SetComponentData(entities[i], new Rotation { Value = new quaternion(0, 1, 0, 0) });
                    manager.AddComponentData(entities[i], new Team_A { });
                }                            
                else if (team == 2)          
                {                            
                    manager.SetComponentData(entities[i], new Position { Value = new float3(xVal, yVal, bottonBound - zVal) });
                    manager.SetComponentData(entities[i], new Rotation { Value = new quaternion(0, 0, 0, 0) });
                    manager.AddComponentData(entities[i], new Team_B { });
                }
                manager.SetComponentData(entities[i], new MoveSpeed { Value = enemySpeed });
            }

            entities.Dispose();

            count += amount;
        }

        void OnGUI()
        {
            GUI.TextArea(new Rect(10, 10, 150, 100), fps.ToString() + "\n\n" + count.ToString() + "\n\n" + entities.Length.ToString());            
        }
    }

}
