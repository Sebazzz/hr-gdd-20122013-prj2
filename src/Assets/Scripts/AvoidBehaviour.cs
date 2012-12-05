using UnityEngine;
using System.Collections;

public class AvoidBehaviour : MonoBehaviour {
    private GameObject inst;

    public float radius = 3.0F;
    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        this.Avoid();
    }


    
    private void Avoid()
    {
        int herders = 0;
        Vector3 averagePosition = new Vector3();
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider hit in colliders)
        {
            if (hit.gameObject != this.gameObject)
            {
                if (hit.CompareTag(Tags.Shepherd))
                {
                    averagePosition += hit.transform.position;
                    herders++;
                }
            }
        }
        if (herders > 0)
        {
            averagePosition /= herders;

            // look at far position in a Smoother Way
            Quaternion lookRotation = Quaternion.LookRotation(transform.position - averagePosition);

            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 2);
        }
    }
}
