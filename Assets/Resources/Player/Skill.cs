using UnityEngine;
using System.Collections;

public class Skill : System.Object {

    public bool acting;
    public string name;

    public void Actualize ()
    {
        Constant();
        if (Output())
            Action();
    }
    protected bool Output ()
    {
        return Condition() & Input();
    }
    protected virtual bool Condition ()
    {
        return false;
    }
    protected virtual bool Input()
    {
        return false;
    }
    protected virtual void Action ()
    {

    }
    protected virtual void Constant ()
    {

    }
}

//Skills
public class Jump : Skill
{
    public Coroutine jumping;
    public bool bigJump_screen;
    public Jump ()
    {
        name = "Jump";
    }
    protected override bool Input()
    {
        return InputController.buttons["A"].down;
    }
    protected override bool Condition()
    {
        return Player.instance.grounded;
    }
    protected override void Action()
    {
        Jump_Init();
    }
    private void Jump_Init()
    {
        if (jumping == null)
            jumping = Player.instance.StartCoroutine(Jump_Action());
    }
    private IEnumerator Jump_Action()
    {
        byte delay = 3;
        byte bigJumpScreen_frames = 5;
        byte lapse = 20;

        Player.instance.forces[2].SetBackUp();

        for (int i = delay; i > 0; i--)
        {
            yield return new WaitForFixedUpdate();
        }

        Player.instance.forces[2].Apply(Vector3.up, 1.0f, true);

        
        for (int i = bigJumpScreen_frames; i > 0; i--)
        {
            yield return new WaitForFixedUpdate();
        }

        bigJump_screen = true;
        yield return null;
        bigJump_screen = false;

        /*
        if (InputController.buttons["A"].pressing)
            Player.instance.forces[2].SetForce(7.5f, 0.0f);

        */

        for (int i = lapse-delay-2; i > 0; i--)
        {
            yield return new WaitForFixedUpdate();
        }

        jumping = null;
    }
}
public class BigJump : Skill
{
    public Jump jump;
    public BigJump ()
    {
        name = "BigJump";
    }
    protected override bool Input()
    {
        return InputController.buttons["A"].pressing;
    }
    protected override bool Condition()
    {
        if (jump==null)
            jump = (Jump)Player.skills["Jump"];
        return jump.bigJump_screen;
    }
    protected override void Action()
    {
        Player.instance.forces[2].SetForce(7.5f, 0.0f);
    }
}
public class PushPull : Skill
{
    public Coroutine jumping;
    public bool bigJump_screen;
    public PushPull()
    {
        name = "PushPull";
    }
    protected override bool Input()
    {
        return InputController.buttons["A"].down;
    }
    protected override bool Condition()
    {
        return Player.instance.grounded;
    }
    protected override void Action()
    {
        Jump_Init();
    }
    protected override void Constant()
    {
        if (jumping == null)
            Player.instance.forces[2].forceStop();
    }
    private void Jump_Init()
    {
        if (jumping == null)
            jumping = Player.instance.StartCoroutine(Jump_Action());
    }
    private IEnumerator Jump_Action()
    {
        byte delay = 3;
        byte bigJumpScreen_frames = 5;
        byte lapse = 20;

        Player.instance.forces[2].SetBackUp();

        for (int i = delay; i > 0; i--)
        {
            yield return new WaitForFixedUpdate();
        }

        Player.instance.forces[2].Apply(Vector3.up, 1.0f, true);


        for (int i = bigJumpScreen_frames; i > 0; i--)
        {
            yield return new WaitForFixedUpdate();
        }

        bigJump_screen = true;
        yield return null;
        bigJump_screen = false;

        /*
        if (InputController.buttons["A"].pressing)
            Player.instance.forces[2].SetForce(7.5f, 0.0f);

        */

        for (int i = lapse - delay - 2; i > 0; i--)
        {
            yield return new WaitForFixedUpdate();
        }

        jumping = null;
    }
}

