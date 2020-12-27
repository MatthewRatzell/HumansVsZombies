using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager1 : MonoBehaviour
{
    //list to hold all of the zombies
    public List<Zombie> zombies = new List<Zombie>();
    public List<Human> humansList = new List<Human>();
    public List<Obstacles> obstacleList = new List<Obstacles>();

    //prefab objects for our 3 types
    public Zombie zombiePrefab;
    public Human humanPrefab;
    public Obstacles obstaclePrefab;

    //floor information to communicate with other scritps
    public MeshRenderer floor;
    public float edgeDistance = 1f;
    Bounds floorbounds;
    float min;
    float max;

    //a boolean that may be used one day
    bool isGameOver;

    //so i can access futureposition objects
    public GameObject humansFuturePosition;
    public GameObject zombiesFuturePosition;

    // Start is called before the first frame update
    void Start()
    {
        floorbounds = floor.bounds;

        max = floorbounds.max.x-2;
        min = max * (-1);
        isGameOver = false;

        //spawning in our obstacle
    }

    // Update is called once per frame
    void Update()
    {
        //only when zombies are all dead and the game is still going does this work
        if (zombies.Count == 0 && isGameOver != true)
        {

            this.SpawnInSomeZombies(1);

        }

        if (humansList.Count == 0 && isGameOver != true && obstacleList.Count == 0)
        {
            this.SpawnInSomeHumans(3);

        }
        //these should only spawn in once and obstacle list will never be empty 
        if(isGameOver != true && obstacleList.Count == 0)
        {
            this.spawnInObstacles(5);
        }

        
    }

    //helper method for spawning in zombies
    void SpawnInSomeZombies(int numberOfZombies)
    {

        for (int i = 0; i< numberOfZombies; i++)
        {
            Zombie zombie = Instantiate(zombiePrefab, new Vector3(Random.Range(max, min), 0f, Random.Range(max, min)),Quaternion.identity);
            zombies.Add(zombie);
        }
    }
    //helper method for spawning in humans
    void SpawnInSomeHumans(int numberOfHumans)
    {
        for (int i = 0; i < numberOfHumans; i++)
        {
            Human human = Instantiate(humanPrefab, new Vector3(Random.Range(max, min), 0f, Random.Range(max, min)), Quaternion.identity);
            humansList.Add(human);
        }
    }
    //helper method for spawning in obstacles
    void spawnInObstacles(int numberOfObstacle)
    {
        for (int i = 0; i < numberOfObstacle; i++)
        {
            Obstacles obstacle = Instantiate(obstaclePrefab, new Vector3(Random.Range(max, min), 1.5f, Random.Range(max, min)), Quaternion.identity);
            obstacleList.Add(obstacle);
        }
    }
    
    //two helpful methods for returning a list, this was added later in devolopement so this method may not be leveraged everywhere
    public List<Human> ReturnAllHumans()
    {
        return humansList;
    }
    public List<Zombie> ReturnAllZombies()
    {
        return zombies;
    }

    public void spawnInSingularZombie()
    {
        SpawnInSomeZombies(1);
    }

    public void spawnInSingularHuman()
    {
        SpawnInSomeHumans(1);
    }
}
