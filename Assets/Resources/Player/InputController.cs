using UnityEngine;
using System.Collections.Generic;

public class InputController : MonoBehaviour {

    public static Dictionary<string, Button> buttons = new Dictionary<string, Button>();
    public static Dictionary<string, Axis> axes = new Dictionary<string, Axis>();

    public class Button : System.Object
    {
        public bool pressing;
        public bool down;
        public bool up;
        string name;
        public Button(string name)
        {
            this.name = name;
        }
        public void Actualize()
        {
            pressing = Input.GetButton(name);
            down = Input.GetButtonDown(name);
            up = Input.GetButtonUp(name);
        }
    }
    public class Axis : System.Object
    {
        public float value;
        string name;
        public Axis (string name)
        {
            this.name = name;
        }
        public void Actualize ()
        {
            value = Input.GetAxis(name);
        }
    }
    void Awake ()
    {
        buttons.Add("A", new Button("A"));
        axes.Add("H_L", new Axis("Horizontal"));
    }
	// Update is called once per frame
	void Update () {
	    foreach (KeyValuePair<string, Button> b in buttons)
        {
            b.Value.Actualize();
        }
        foreach (KeyValuePair<string, Axis> a in axes)
        {
            a.Value.Actualize();
        }
	}
}
