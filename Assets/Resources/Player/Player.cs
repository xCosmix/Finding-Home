using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour {

    public static Player instance;

    public float fieldOfView = 1.0f;
    public float learnSpeed = 3.0f;
    public bool grounded = false;
    public bool topped = false;
    public float slopeLimit = 0.5f;
    public force[] forces;
    public LayerMask groundLayerMask;
    private int dir;
    private Vector3 currentNormal;
    public float currentSlope;
    private Collider my_collider;
    private Rigidbody my_rigid;
    private CollTrigger my_grounded;
    private CollTrigger my_topped;
    private GameObject learnIcon;
    private SpriteRenderer learnProcess_render;
    private Sprite[] process_sprites;

    private Learn currentLearning;
    private float currentLearnTime;

    public static Dictionary<string, Skill> skills = new Dictionary<string, Skill>();
   
    public int get_dir () { return dir; }

	void Start () {

        process_sprites = Resources.LoadAll<Sprite>("Player/LearningProcess");
        learnProcess_render = GameObject.Find("LearnProcess").GetComponent<SpriteRenderer>();
        instance = FindObjectOfType<Player>();
        my_rigid = GetComponent<Rigidbody>();
        my_collider = GetComponent<Collider>();
        CollTrigger[] ct = GetComponentsInChildren<CollTrigger>();
        foreach (CollTrigger cti in ct)
        {
            if (cti.gameObject.name == "Ground_CT")
                my_grounded = cti;
            if (cti.gameObject.name == "Top_CT")
                my_topped = cti;
        }
        learnIcon = GameObject.Find("LearnIcon");
        learnIcon.SetActive(false);
        //Set Input Controller
        InputController controller = FindObjectOfType<InputController>();
        if (controller == null)
            new GameObject("InputController", new System.Type[] { typeof(InputController) });
	}
	
	// Update is called once per frame
	void Update () {
        Skills();
        Watch();
	}
    private void Skills ()
    {
        foreach (KeyValuePair<string, Skill> s in skills)
        {
            s.Value.Actualize();
        }
    }
    private void Watch ()
    {
        int mountToLearn = 0;
        foreach (Learn l in Learn.learns)
        {
            float dist = (l.transform.position - transform.position).sqrMagnitude;
            if (dist < fieldOfView)
            {
                if (!AlreadyLearned(l.LearnSkill()))
                {
                    Learning(l);
                    mountToLearn++;
                }
            }
        }
        if (mountToLearn == 0) {
            currentLearnTime = 0.0f;
            learnIcon.SetActive(false);
        }
    }
    private void Learning (Learn l)
    {
        learnIcon.SetActive(true);
        if (currentLearning != l)
        {
            currentLearnTime = 0.0f;
            currentLearning = l;
        }
        currentLearnTime += Time.deltaTime;
        float counter_time = learnSpeed / 8.0f;
        int process_sprite_index = Mathf.RoundToInt(currentLearnTime / counter_time);
        process_sprite_index = Mathf.Clamp(process_sprite_index, 0, 7);
        learnProcess_render.sprite = process_sprites[7-process_sprite_index];
        if (currentLearnTime >= learnSpeed)
            AdquireSkill(l);
    }
    private void AdquireSkill (Learn l)
    {
        Debug.Log(l.skill + " Learned!");
        GUI.instance.NewAbility(l.LearnSkill());
        skills.Add(l.LearnSkill().name, l.LearnSkill());
        learnIcon.SetActive(false);
    }
    private bool AlreadyLearned (Skill l)
    {
        return skills.ContainsKey(l.name);
    }
    void FixedUpdate ()
    {
        Grounded();
        Topped();
        Move();
        if (!grounded)
            AirStep();
        else
            GroundedStep();
        ApplyAll();
    }
    private void ApplyAll ()
    {
        Vector3 outVel = Vector3.zero;
        foreach (force f in forces)
            outVel += f.output();
        my_rigid.velocity = outVel;
    }
    private void Move ()
    {
        float hl = InputController.axes["H_L"].value;
        if (hl > 0.6f)
        {
            forces[0].Apply(new Vector3(hl, 0.0f, 0.0f));
            dir = 1;
        }
        else if (hl < -0.6f)
        {
            forces[1].Apply(new Vector3(hl, 0.0f, 0.0f));
            dir = -1;
        }
    }
    private void GroundedStep ()
    {
        NotSlideFriction();
    }
    private void NotSlideFriction ()
    {
        Vector3 friction_dir = Vector3.Reflect(forces[3].output(), currentNormal)*-1;
        forces[4].Apply(friction_dir); //Set gravity dir
    }
    private void LandStep ()
    {
       //forces[0].SetBackUp();
       //forces[1].SetBackUp();
    }
    private void AirStep ()
    {
        Gravity();
        if (topped)
            ToppedStep();
    }
    private void TakeOffStep ()
    {
        //forces[0].SetForce(1.0f, 0.0f);
        //forces[1].SetTime(1.0f, 0.0f);
    }
    private void ToppedStep ()
    {
        forces[2].forceStop();
    }
    private void Grounded ()
    {
        bool lastGrounded = grounded;
        bool stepping = my_grounded.Colliding();
        grounded = GroundInfo(stepping);

        if (lastGrounded != grounded && grounded)
            LandStep();
        else if (lastGrounded != grounded)
            TakeOffStep();
    }
    private bool GroundInfo (bool stepping)
    {
        if (!stepping)
            return false;
        Collider ground = my_grounded.LastCollision();
        RaycastHit hit=new RaycastHit();
        if (Physics.Linecast(transform.position, new Vector3(transform.position.x, ground.bounds.center.y, ground.bounds.center.z), out hit, groundLayerMask))
        {
            currentNormal = hit.normal;
            currentSlope = (180.0f - Vector3.Angle(currentNormal, Vector3.down)) / 180.0f;
        }
        if (currentSlope < slopeLimit)
            return true;
        return false;
    }
    private void Topped()
    {
        topped = my_topped.Colliding();
    }
    private void Gravity ()
    {
        forces[3].Apply(Vector3.down);
    }
    void OnCollisionEnter (Collision coll)
    {
     
    }
    void OnCollisionExit (Collision coll)
    {

    }
}
