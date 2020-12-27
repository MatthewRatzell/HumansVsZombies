using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Vehicle : MonoBehaviour
{
    //public GameObject targetHuman;
    public GameManager1 gameManager;

    //vectors needed to control our vehicle
    public Vector3 position;
    protected Vector3 direction;
    public Vector3 velocity;
    protected Vector3 acceleration;
    protected Vector3 forward;
    protected Vector3 right;
    private Vector3 upVector;
    public float timeScale = 2f;

    //allows us to manipulate certain stats of the vehicles
    [Min(0.0001f)]
    public float mass = 1;
    public float radius = -1f;
    public float maxSpeed = 1f;
    public float maxForce = 2f;

    //holds materials for debug Lines
    public Material material1;
    public Material material2;
    public Material material3;
    public Material material4;
    public Material material5;


    //allows us to leverage when debug lines are drawn
    protected bool drawGizmos;

    //used for seperation
    public float safeDistance = 4f;
    [Min(.1f)]
    public float personalSpace = 2f;
    public float distanceThreshold = .1f;

    //used for using wander
    public float wanderAngle = 25f;
    protected float angleChange = 1f;

    //variable used to help make sure our drawgizmos are working
    protected bool shouldGizmosDrawn;
    // Start is called before the first frame update
    void Start()
    {
        upVector = new Vector3(0, 1, 0);
        //setting all of our starting data
        position = transform.position;
        direction = Vector3.right;
        velocity = Vector3.zero;
        acceleration = Vector3.zero;

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
        this.CalculateSteeringForces();
        velocity += acceleration * Time.deltaTime;
        //making sure we dont exceed our maxSpeed
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
        position += velocity * Time.deltaTime;
        transform.position = position;
        direction = velocity.normalized;

        if (direction != Vector3.zero)
        {
            //every frame making sure we update our forward and our right vector
            forward = velocity.normalized;
            right = Vector3.Cross(forward.normalized, upVector.normalized);
            transform.rotation = Quaternion.LookRotation(direction);
            
        }

        acceleration = Vector3.zero;

        this.DrawGizmosSetter();
        if (Input.GetKeyDown(KeyCode.D))
        {

            drawGizmos = !drawGizmos;

        }
        if (drawGizmos)
        {
            
        }

        position.y = 0f;
    }
    //applys a force to our vehicle
    public void ApplyForce(Vector3 force)
    {
        acceleration += force / mass;
    }
    //applys friction to our vehicle
    protected void ApplyFriction(float coeff)
    {
        Vector3 friction = velocity * -1;
        friction.Normalize();
        friction *= coeff;
        acceleration += friction;
    }
    //applys gravity to out vehicle
    protected void ApplyGravity(Vector3 gravityForce)
    {
        acceleration += gravityForce;
    }



    //makes sure object stays in bounds
    protected Vector3 StayInBounds()
    {
        Bounds floorbounds = gameManager.floor.bounds;

        Vector3 max = floorbounds.max;
        Vector3 min = floorbounds.min;

        float edgeDistance = gameManager.edgeDistance;

        Vector3 inBoundsVelocity = Vector3.zero;

        if (position.x > (max.x - edgeDistance))
        {
            Vector3 wallPosition = new Vector3(max.x - edgeDistance, 0, position.z);

            float sqrDist = Vector3.SqrMagnitude(wallPosition - position);

            Vector3 desiredVelocity = new Vector3(-maxSpeed, 0, 0);
            desiredVelocity = desiredVelocity.normalized * sqrDist;

            inBoundsVelocity += desiredVelocity;
        }
        if (position.x < (min.x + edgeDistance))
        {
            Vector3 wallPosition = new Vector3(min.x + edgeDistance, 0, position.z);

            float sqrDist = Vector3.SqrMagnitude(wallPosition - position);

            Vector3 desiredVelocity = new Vector3(maxSpeed, 0, 0);
            desiredVelocity = desiredVelocity.normalized * sqrDist;

            inBoundsVelocity += desiredVelocity;
        }
        if (position.z > (max.z - edgeDistance))
        {
            Vector3 wallPosition = new Vector3(position.x, 0, max.z - edgeDistance);

            float sqrDist = Vector3.SqrMagnitude(wallPosition - position);

            Vector3 desiredVelocity = new Vector3(0, 0, -maxSpeed);
            desiredVelocity = desiredVelocity.normalized * sqrDist;

            inBoundsVelocity += desiredVelocity;
        }
        if (position.z < (min.z + edgeDistance))
        {
            Vector3 wallPosition = new Vector3(position.x, 0, min.z + edgeDistance);

            float sqrDist = Vector3.SqrMagnitude(wallPosition - position);

            Vector3 desiredVelocity = new Vector3(0, 0, maxSpeed);
            desiredVelocity = desiredVelocity.normalized * sqrDist;

            inBoundsVelocity += desiredVelocity;
        }

        Vector3 inBoundsForce = inBoundsVelocity - velocity;
        return inBoundsVelocity;

    }
    protected abstract void CalculateSteeringForces();
    //used when seeking a specific vector
    public Vector3 Seek(Vector3 targetPosition)
    {
        //calculate desired velocity
        //its a vector from our position to our targets poosition
        Vector3 desiredVelocity = targetPosition - position;

        //set desired = max speed
        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        //calculate the seek steering force
        Vector3 seekingForce = desiredVelocity - velocity;

        //return steerng force
        return seekingForce;
    }
    //used when seeking a gameobject
    public Vector3 Seek(GameObject targetObject)
    {
        return this.Seek(targetObject.transform.position);
    }
    //used when fleeing from a gameobjects position
    public Vector3 Flee(Vector3 targetPosition)
    {
        //calculate desired velocity
        //it is a vector AWAY from our  target position
        //target position to our location
        Vector3 desiredVelocity = position - targetPosition;

        //set desired = max speed
        desiredVelocity = desiredVelocity.normalized * maxSpeed;

        //calculate the fleeing steering force
        Vector3 fleeingForce = desiredVelocity - velocity;

        //return steerng force
        return fleeingForce;
    }
    //used when fleeing away from a gameobjects 
    public Vector3 Flee(GameObject targetObject)
    {
        return this.Flee(targetObject.transform.position);
    }
    //better version of flee
    public Vector3 Evade(Vehicle targetObject, float timescale)
    {
        //calculating desired velocity 
        Vector3 desiredVelocity = position - targetObject.GetFuturePosition(timescale);
        //setting desired to the max speed
        desiredVelocity = desiredVelocity.normalized * maxSpeed;
        //calculating the evading steering force
        Vector3 evadingForce = desiredVelocity - velocity;
        return evadingForce;
    }
    //better version of seek
    public Vector3 Pursue(Vehicle targetObject, float timescale)
    {
        //calculating desired velocity 
        Vector3 desiredVelocity = targetObject.GetFuturePosition(timescale) - position;

        float distance = desiredVelocity.magnitude;
        if (distance < distanceThreshold)
        {
            //scaling our desired because of where we are at
            desiredVelocity = desiredVelocity.normalized * Mathf.Lerp(0, maxSpeed, distance / distanceThreshold);
        }
        else
        {
            //setting desired to the max speed
            desiredVelocity = desiredVelocity.normalized * maxSpeed;
        }

        //calculating the evading steering force
        Vector3 pursueingForce = desiredVelocity - velocity;
        return pursueingForce;
    }
    //retrieves the future position of a vehicle based on how many seconds in the future is given
    public Vector3 GetFuturePosition(float timescale)
    {
        return position + (velocity * timeScale);
    }
    //keeps gameobjects away from each other
    public Vector3 Seperate<T>(List<T> vehicles) where T : Vehicle
    {
        Vector3 desiredVelocity = Vector3.zero;
        foreach (T other in vehicles)
        {
            if (other == this)
            {
                continue;
            }
            float sqrDist = Vector3.SqrMagnitude(other.position - position);

            if (sqrDist < personalSpace * personalSpace && sqrDist > 0f)
            {
                Vector3 otherVehicleSeperation = position - other.position;
                otherVehicleSeperation = otherVehicleSeperation.normalized * (personalSpace / sqrDist);
                desiredVelocity += otherVehicleSeperation;
            }
        }
        Vector3 seperationForce = desiredVelocity - velocity;

        return seperationForce;
    }
    public Vector3 AvoidObstacle(Vector3 targetposition, float radiusOfObstacle)
    {
        //check if the obstaclke is behind me
        Vector3 meTooObstacle = targetposition - position;

        if (Vector3.Dot(forward, meTooObstacle) < 0)
        {
            return Vector3.zero;
        }

        //check if there is a potential collision (if the obstacle is too far from the left or the right)
        float rightMeTooObstacle = Vector3.Dot(right, meTooObstacle);
        if (Mathf.Abs(rightMeTooObstacle) > radiusOfObstacle + radius)
        {
            return Vector3.zero;
        }

        //check if the obstacle is in range
        float distance = meTooObstacle.sqrMagnitude - Mathf.Pow(radiusOfObstacle, 2);
        if (distance > (safeDistance * safeDistance))
        {
            return Vector3.zero;
        }

        //weight the steering force based on how close we are
        float weight = 0;
        if (distance <= 0)
        {
            weight = float.MaxValue;
        }
        else
        {
            weight = (safeDistance * safeDistance) / distance;
        }
        //clamping our weight so it is in an accepable range
        weight = Mathf.Min(weight, 10000);
        Vector3 desiredVeolicty = Vector3.zero;

        if (rightMeTooObstacle < 0)
        {
            //if obstacle is on the left we want to steer right
            desiredVeolicty = right * maxSpeed;
        }
        else
        {
            desiredVeolicty = right * -maxSpeed;
        }
        //if obstacle is on the right we want to steer left

        //calculate our steering force from our desiored velocity
        Vector3 steeringForce = (desiredVeolicty - velocity) * weight;
        //return out steering force
        return steeringForce;
    }
    //takes gameobject and leverages the other method we made
    public Vector3 AvoidObstacle(Obstacles obstacleToAvoid)
    {
        return this.AvoidObstacle(obstacleToAvoid.transform.position, obstacleToAvoid.radius);
    }
    //our collision detecting 
    public bool CircleCollision(GameObject gameObjectOne, GameObject gameObjectTwo)
    {
        //using built in method
        Vector3 distanceVector = gameObjectOne.transform.position - gameObjectTwo.transform.position;
        float distance = distanceVector.magnitude;

        float gameObjectOneRadius = gameObjectOne.GetComponent<BoxCollider>().bounds.extents.magnitude;
        float gameObjectTwoRadius = gameObjectTwo.GetComponent<BoxCollider>().bounds.extents.magnitude;
        float combinedRadius = gameObjectOneRadius + gameObjectTwoRadius;

        if (distance < combinedRadius)
        {

            return true;
        }
        else
        {
            return false;
        }
    }

    protected Vector3 wander()
    {
        Vector3 circleCenter;
        circleCenter = direction;
        //2 is the distance the circle is from us
        circleCenter= circleCenter*2f;

        //calculating displacement force
        Vector3 displacement = new Vector3(1, 0, 1);
        //the 1 is the radius of the circle
        displacement = displacement * 1.5f;
        //randomly changing vector direction by changing angle
        this.setAngle(displacement, wanderAngle);

        //making sure our wanderangle changes every fram
        wanderAngle += (Random.Range(20, 30) * angleChange) - (angleChange * .5f);

        Vector3 wanderForce = circleCenter + displacement;

        return wanderForce;
    }
    //changing the direction angle sligtly each time 
    public void setAngle(Vector3 distance,float angle)
    {
        float dist = distance.magnitude;
        distance.x = Mathf.Cos(angle) * dist;
        distance.y = Mathf.Sin(angle) * dist;
    }

    protected virtual void OnRenderObject()
    {
        if (drawGizmos)
        {
            material3.SetPass(0);
            // Draws one line
            GL.Begin(GL.LINES);
            // Begin to draw lines
            GL.Vertex(this.position);
            // First endpoint of this line
            GL.Vertex(this.position + forward);
            // Second endpoint of this line
            GL.End();

            material2.SetPass(0);
            // Draws one line
            GL.Begin(GL.LINES);
            // Begin to draw lines
            GL.Vertex(this.position);
            // First endpoint of this line
            GL.Vertex(this.position + right);
            // Second endpoint of this line
            GL.End();
        }
        
    }

    //allows us to make sure when a human becomes a zombie they remember if they should be drawing lines or not
    protected void DrawGizmosSetter()
    {
        shouldGizmosDrawn = false;
        foreach (Zombie z in gameManager.zombies)
        {
            if(z.drawGizmos == true)
            {
                shouldGizmosDrawn = true;
                break;
            }
            else
            {
                shouldGizmosDrawn = false;
            }

                
        }
        //if any zombies are drawing gizmos make sure all zombies are
        if (shouldGizmosDrawn)
        {
            foreach(Zombie z in gameManager.zombies)
            {
                drawGizmos = true;
            }
            foreach(Human h in gameManager.humansList)
            {
                drawGizmos = true;
            }
        }
        else
        {

        }
    }
}


