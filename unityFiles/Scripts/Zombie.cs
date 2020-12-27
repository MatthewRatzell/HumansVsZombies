using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Vehicle
{
    //all 3 fields for determining and remembering the closest human
    private float indexOfClosestHuman;
    private float distanceOfClosestHuman;
    public int indexOfClosestHumanInInt;

    protected override void CalculateSteeringForces()
    {
        Vector3 ultimateForce = Vector3.zero;
        //only one human at the moment so we will just refer to that part of the list
        distanceOfClosestHuman = 1000;
        indexOfClosestHumanInInt = 50;
        if (gameManager.humansList.Count != 0)
        {

            //finding the closest human
            for (int i = 0; i < gameManager.humansList.Count; i++)
            {
                //calculating the distance 
                float distance = (gameManager.humansList[i].transform.position - this.position).magnitude;
                if (distance < distanceOfClosestHuman)
                {
                    distanceOfClosestHuman = distance;
                    indexOfClosestHuman = i;
                }
            }
            indexOfClosestHumanInInt = (int)indexOfClosestHuman;
            //now that we have found the closest human we are going to seek them
            if(indexOfClosestHumanInInt != 50)
            {
                ultimateForce += Pursue(gameManager.humansList[indexOfClosestHumanInInt], 2);
            }

        }
        else
        {
            ultimateForce += wander();
        }

        //obstacle avoidance logic 
        foreach (Obstacles o in gameManager.obstacleList)
        {
            ultimateForce += AvoidObstacle(o) * 5;
        }
        //in bounds
        ultimateForce += this.StayInBounds();
        //making sure our obstacles dont intersect
        ultimateForce += Seperate<Zombie>(gameManager.ReturnAllZombies());

        ultimateForce = Vector3.ClampMagnitude(ultimateForce, maxForce);

        ApplyForce(ultimateForce);
    }

    //nothing really special going on in here
    protected override void Update()
    {
        GameObject zombiesFuturePosition;
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager1>();
        base.Update();
    }


    protected override void OnRenderObject()
    {
        if (drawGizmos)
        {
            base.OnRenderObject();
            //draws the line to our 
            if (gameManager.humansList.Count != 0 && indexOfClosestHumanInInt != 50)
            {
                material1.SetPass(0);
                // Draws one line
                GL.Begin(GL.LINES);
                // Begin to draw lines
                GL.Vertex(this.transform.position);
                // First endpoint of this line
                GL.Vertex(gameManager.humansList[indexOfClosestHumanInInt].transform.position);
                // Second endpoint of this line
                GL.End();
            }

            material4.SetPass(0);
            GL.Begin(GL.LINES);
            GL.Vertex(this.position);
            GL.Vertex(GetFuturePosition(2));
            GL.End();

        }


    }


}
