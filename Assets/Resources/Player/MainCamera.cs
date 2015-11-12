using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {
    public GameObject target;
    public Vector3 offset;
    public bool zOffset;
    public Vector2 staticFocus;
    public float speed;
    private Rect staticFocus_world;
    private Vector3 goalPoint;
    private Vector3 delta;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        //if (!OnFocus())
            Move();
        delta = goalPoint - transform.position;
        delta.z = zOffset ? delta.z : 0.0f;
        transform.position += delta * Time.fixedDeltaTime * speed;
	}
    protected bool OnFocus ()
    {
        Vector3 goal = (transform.position - offset);
        staticFocus_world.xMin = goal.x - staticFocus.x;
        staticFocus_world.xMax = goal.x + staticFocus.x;
        staticFocus_world.yMin = goal.y - staticFocus.y;
        staticFocus_world.yMax = goal.y + staticFocus.y;
        return staticFocus_world.Contains(target.transform.position);
    }
    protected void Move ()
    {
        Vector3 set_offset = offset;
        set_offset.x *= Player.instance.get_dir();
        goalPoint = (target.transform.position + set_offset);
    }
}
