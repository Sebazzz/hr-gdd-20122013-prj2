using UnityEngine;
using System.Collections;

public class SelectionManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(1))
        {
            ShootRay();
        }
	}

    void ShootRay()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 point = hit.point;
            // make sure objects dont move on the Y axis
            point.y = 1;

            ActionWalk aw = new ActionWalk(point);
            ObjectManager.getInstance().action(aw);

            // Detect what has been clicked on
            if (hit.collider.gameObject.tag == "enterable")
            {
                ActionBuilding ab = new ActionBuilding(hit.collider.gameObject);
                ObjectManager.getInstance().action(ab);
            } 
        }

    }
}
