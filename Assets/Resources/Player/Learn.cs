using UnityEngine;
using System.Collections.Generic;

public class Learn : MonoBehaviour
{
    public static Dictionary<string, Skill> library;
    public static List<Learn> learns = new List<Learn>();
    public string skill;
    // Use this for initialization
    void Start()
    {
        learns.Add(this);
        if (library == null)
            InitializeLibrary();
    }
    protected static void InitializeLibrary ()
    {
        library = new Dictionary<string, Skill>();
        library.Add("Jump", new Jump());
        library.Add("BigJump", new BigJump());
    }
    public Skill LearnSkill ()
    {
        return library[skill];
    }
    // Update is called once per frame
}
