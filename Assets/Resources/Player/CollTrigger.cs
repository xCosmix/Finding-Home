using UnityEngine;
using System.Collections;

public class CollTrigger : MonoBehaviour {
    public string layer = "none";
    public string tag = "none";
    private int collisions;
    private Collider lastColl;
	void OnTriggerEnter ( Collider col )
    {
        string c_layer = LayerMask.LayerToName(col.gameObject.layer);
        if (!c_layer.Equals(layer) && !layer.Equals("none")) return;
        if (!col.gameObject.tag.Equals(tag) && !tag.Equals("none")) return;
        lastColl = col;
        collisions++;
    }
    void OnTriggerStay ( Collider col )
    {
        lastColl = col;
    }
    void OnTriggerExit ( Collider col )
    {
        string c_layer = LayerMask.LayerToName(col.gameObject.layer);
        if (!c_layer.Equals(layer) && !layer.Equals("none")) return;
        if (!col.gameObject.tag.Equals(tag) && !tag.Equals("none")) return;
        collisions--;
    }

    public bool Colliding ()
    {
        return collisions > 0;
    }
    public Collider LastCollision ()
    {
        return lastColl;
    }
}
