using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Human : Vehicle

{ 
    public float detectionRange = 5f;


    public float fireSpeed = 2f;

    //using this for when the human gets whooped
    Vector3 newZombiePosition;



    protected override void CalculateSteeringForces()
    {
        Vector3 ultimateForce = Vector3.zero;

        //using gameobject and using conditional to make sure the human only flees when any of the zombies 
        if (gameManager.humansList.Count != 0 && gameManager.zombies.Count != 0)
        {
            foreach (Zombie z in gameManager.zombies)
            {
                if (Vector3.Distance(position, z.transform.position) < detectionRange)
                {
                    ultimateForce += Evade(z,3)*4;
                }
                else
                {
                    ultimateForce += wander();
                }
            }
        }

        //obstacle avoidance logic 
        foreach (Obstacles o in gameManager.obstacleList)
        {
            ultimateForce += AvoidObstacle(o) * 5;
        }
        //seperation
        ultimateForce += Seperate<Human>(gameManager.ReturnAllHumans());
        //boundary checking
        ultimateForce+=this.StayInBounds();
        //making sure we clamp to a max force
        ultimateForce = Vector3.ClampMagnitude(ultimateForce, maxForce);
        //applying force to our vehicle
        ApplyForce(ultimateForce);
    }


    protected override void Update()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager1>();
        //going through our collisions now
        this.CheckCollisions();
        base.Update();

        radius = 1.0f;
    }

    private void CheckCollisions()
    {
        if (gameManager.humansList.Count != 0)
        {
            foreach (Zombie z in gameManager.zombies)
            {
                if (CircleCollision(this.gameObject, z.transform.gameObject))
                {
                    //making sure we remove the human from our list
                    gameManager.humansList.Remove(gameManager.humansList[z.indexOfClosestHumanInInt]);
                    //saving the position of our object before we destroy it
                    newZombiePosition = gameObject.transform.position;
                    //creating our new zombie at the correct postion
                    gameManager.zombies.Add(Instantiate(gameManager.zombiePrefab, newZombiePosition, Quaternion.identity));
                    //deleting this object good bye little buddy
                    Destroy(gameObject);
                    break;
                }

            }

        }

    }
    protected Vector3 GenerateRandomVector()
    {
        Vector3 vector = new Vector3(Random.Range(-50f, 50f), 1.02f, Random.Range(-50f, 50f));
        return vector;
    }
    
    //this method is only used in our human class because it works hand in hand with pursue
    protected override void OnRenderObject()
    {
        if (drawGizmos)
        {
            base.OnRenderObject();
            material5.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Vertex(this.position);
            GL.Vertex(this.GetFuturePosition(2));
            GL.End();
        }
        
    }

}

