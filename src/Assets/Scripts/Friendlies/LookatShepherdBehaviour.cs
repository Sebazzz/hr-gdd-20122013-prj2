<<<<<<< .mine
using UnityEngine;
using System.Collections;
using System;

public class LookatShepherdBehaviour : MonoBehaviour
{
    /// <summary>
    /// The last update.
    /// </summary>
    private float lastUpdate = 0.0F;
    /// <summary>
    /// The update delay in frames.
    /// </summary>
    public float UpdateDelay = 0.5f;
    /// <summary>
    /// The minimum watch distance.
    /// </summary>
    public float minWatchDistance = 20F;
	
	/// <summary>
	/// The minimum angle off set that a lookaway will be triggered.
	/// </summary>
	public float minAngleOffSet =45.0F ;
    /// <summary>
    /// The follow speed.
    /// </summary>
    public int followSpeed = 20;

    private GameObject closestShepherd = null;

    /// <summary>
    /// Finds the closest dog.
    /// </summary>
    void FindClosestDog()
    {
        //FIND SHPEHERD
        GameObject[] shepherd = GameObject.FindGameObjectsWithTag(Tags.Shepherd);
        this.closestShepherd = null;
        if (shepherd.Length == 0)
        {
            return;
        }

        //INITIATE COMPARE
        Vector3 vecmem = shepherd[0].transform.position - transform.position;
        for (int i = 0; i < shepherd.Length; i++)
        {
            Vector3 vec = shepherd[i].transform.position - transform.position;
            if (vec.sqrMagnitude.CompareTo(vecmem.sqrMagnitude) < 1)
            {
                vecmem = vec;
                this.closestShepherd = shepherd[i];
              //  Debug.Log("closest is " +i + "Shepherd");
            }
        }

    }
    
    void Update()
    {
        if (gameObject.GetComponent<SheepIdleBehaviour>().sheepState.Equals(SheepIdleBehaviour.SheepState.active))
        {
            if (this.closestShepherd != null)
            {
                Vector3 distance = this.closestShepherd.transform.position - transform.position;
				if (distance.magnitude < minWatchDistance)
                {
                 
					  Vector3 tmp = closestShepherd.transform.position;
					  tmp.y = transform.position.y;
					  transform.LookAt(tmp) ;
					  transform.RotateAround(Vector3.up,180.0F) ;
              
  
                }
            }
            else if(Time.time - lastUpdate > UpdateDelay)
            {
                //Debug.Log(lastUpdate + "lastupdate" + ", formule dif :" + (Time.time - lastUpdate) + "  > delta * delat " + UpdateDelay);
                //Debug.Log("findingdog");
                this.lastUpdate = Time.time;
                FindClosestDog();
            }
           
        }

    }

}
=======
using UnityEngine;
using System.Collections;
using System;

public class LookatShepherdBehaviour : MonoBehaviour
{
    /// <summary>
    /// The last update.
    /// </summary>
    private float lastUpdate = 0.0F;
    /// <summary>
    /// The update delay in frames.
    /// </summary>
    public float UpdateDelay = 0.5f;
    /// <summary>
    /// The minimum watch distance.
    /// </summary>
    public float minWatchDistance = 20F;
    /// <summary>
    /// The follow speed.
    /// </summary>
    public int followSpeed = 20;

    private GameObject closestShepherd = null;

    /// <summary>
    /// Finds the closest dog.
    /// </summary>
    void FindClosestDog()
    {
        //FIND SHPEHERD
        GameObject[] shepherd = GameObject.FindGameObjectsWithTag(Tags.Shepherd);
        this.closestShepherd = null;
        if (shepherd.Length == 0)
        {
            return;
        }

        //INITIATE COMPARE
        Vector3 vecmem = shepherd[0].transform.position - transform.position;
        for (int i = 0; i < shepherd.Length; i++)
        {
            Vector3 vec = shepherd[i].transform.position - transform.position;
            if (vec.sqrMagnitude.CompareTo(vecmem.sqrMagnitude) < 1)
            {
                vecmem = vec;
                this.closestShepherd = shepherd[i];
              //  Debug.Log("closest is " +i + "Shepherd");
            }
        }

    }
    
    void Update()
    {
        if (gameObject.GetComponent<SheepIdleBehaviour>().sheepState.Equals(SheepIdleBehaviour.SheepState.active))
        {
            if (this.closestShepherd != null)
            {
                Vector3 distance = this.closestShepherd.transform.position - transform.position;
                if (distance.magnitude < minWatchDistance)
                {
                 
                //    float rotation = Vector3.Angle(transform.forward,distance);Debug.Log(rotation + "angle1") ;
                //    float rotation2 = Vector3.Angle(transform.forward,distance);Debug.Log(rotation2 + "angle2" );
                //  Quaternion rotation = transform.AngleAxis(rotation, Vector3.up);
                    distance.x = -distance.x;
                    distance.z = -distance.z;
                    transform.forward = distance;
                }
            }
            else if(Time.time - lastUpdate > UpdateDelay)
            {
                //Debug.Log(lastUpdate + "lastupdate" + ", formule dif :" + (Time.time - lastUpdate) + "  > delta * delat " + UpdateDelay);
                //Debug.Log("findingdog");
                this.lastUpdate = Time.time;
                FindClosestDog();
            }
           
        }

    }

}
>>>>>>> .r346
