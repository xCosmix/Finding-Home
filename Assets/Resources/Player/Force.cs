using UnityEngine;
using System.Collections;

[System.Serializable]
public class force : System.Object
{
    protected static Rigidbody target;
    protected static MonoBehaviour invoker;

    public string Name;
    public Preset preset;

    [System.Serializable]
    public class Preset : System.Object {
        public AnimationCurve in_curve;
        public AnimationCurve out_curve;
        public float maxForce;
        public float minForce;
        public float in_time;
        public float out_time;
    }

    private float startApply_t = -1.0f;
    private float lastApply_t = -1.0f;
    private float currentApply_t = -1.0f;

    private float startDisolve_t = -1.0f;
    private float currentDisolve_t = -1.0f;

    private float fact;
    private bool atomic;
    private Vector3 dir;

    private Coroutine updater;
    private Vector3 output_vector;
    private Preset backUp;

    private float currentFactor, apply_factor, disolve_factor;
    private float offset;

    private bool stop;

    public void Apply(Vector3 dir, float fact = 1.0f, bool atomic = false)
    {
        EndDisolve();
        if (startApply_t == -1.0f)
            StartApply();

        lastApply_t = Time.time;
        currentApply_t = Mathf.Clamp(Time.time - startApply_t, 0.0f, preset.in_time);
        this.dir = dir;
        this.fact = fact;
        this.atomic = atomic;

        float normalized = currentApply_t / preset.in_time;
        currentFactor = preset.in_curve.Evaluate(normalized + offset);
        apply_factor = currentFactor;
    }
    protected void StartApply()
    {
        offset = GetOffset(20, preset.in_curve, currentFactor);
        startApply_t = Time.time;
        if (invoker == null)
            invoker = new GameObject("Invoker", new System.Type[] { typeof(MonoBehaviour) }).GetComponent<MonoBehaviour>();
        if (updater == null)
            updater = invoker.StartCoroutine(Updater());
        stop = false;
    }
    protected IEnumerator Updater()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();
            if (stop) break;

            if (!atomic)
            {
                if (Time.time - lastApply_t >= Time.fixedDeltaTime)
                {
                    Disolve();
                    if (output_vector == Vector3.zero)
                        break;
                }
            }
            else
            {
                if (apply_factor == 1.0f)
                {
                    Disolve();
                    if (output_vector == Vector3.zero)
                        break;
                }
                else
                {
                    if (Time.time - lastApply_t >= Time.fixedDeltaTime)
                        Apply(dir, fact, atomic);
                }
            }
        }
        Stop();
    }
    protected void Disolve()
    {
        EndApply();
        if (startDisolve_t == -1.0f)
            StartDisolve();

        currentDisolve_t = Mathf.Clamp(Time.time - startDisolve_t, 0.0f, preset.out_time);
        float normalized = currentDisolve_t / preset.out_time;

        currentFactor = preset.out_curve.Evaluate(normalized + offset);
        disolve_factor = currentFactor;
    }
    protected void StartDisolve()
    {
        offset = GetOffset(20, preset.out_curve, currentFactor);
        startDisolve_t = Time.time;
        stop = false;
    }
    protected void EndDisolve()
    {
        startDisolve_t = -1.0f;
        currentDisolve_t = -1.0f;
    }
    protected void EndApply()
    {
        startApply_t = -1.0f;
        currentApply_t = -1.0f;
    }
    protected void Stop()
    {
        EndDisolve();
        EndApply();
        currentFactor = 0.0f;
        updater = null;
        stop = true;
    }
    public void forceStop()
    {
        stop = true;
    }
    public Vector3 output()
    {
        float out_fact = Mathf.Clamp(currentFactor * preset.maxForce * fact, preset.minForce, preset.maxForce);
        output_vector = dir * out_fact;
        return output_vector;
    }
    protected float GetOffset(byte resolution, AnimationCurve target, float value)
    {
        float time = 1.0f / (float)resolution;
        float lower_delta = 2.0f;
        float out_time = 0.0f;
        for (int i = 0; i < resolution; i++)
        {
            float current = time * i;
            float s_value = target.Evaluate(current);
            float delta = Mathf.Abs(value - s_value);
            if (delta <= lower_delta)
            {
                lower_delta = delta;
                out_time = current;
            }
            else
            {
                break;
            }
        }
        return out_time;
    }
    public void SetDirection (Vector3 new_dir)
    {
        dir = new_dir;
    }
    public void SetTime (float in_, float out_)
    {
        if (in_ == preset.in_time && out_ == preset.out_time) return;

        Preset newP = new Preset();
        newP.in_curve = preset.in_curve;
        newP.out_curve = preset.out_curve;
        newP.maxForce = preset.maxForce;
        newP.minForce = preset.minForce;

        newP.in_time = in_;
        newP.out_time = out_;
        ChangePreset(newP);
    }
    public void SetForce(float max, float min)
    {
        if (max == preset.maxForce && min == preset.minForce) return;

        Preset newP = new Preset();
        newP.in_curve = preset.in_curve;
        newP.out_curve = preset.out_curve;
        newP.in_time = preset.in_time;
        newP.out_time = preset.out_time;

        newP.maxForce = max;
        newP.minForce = min;
        ChangePreset(newP);
    }
    protected void ChangePreset (Preset newPreset)
    {
        backUp = preset;
        preset = newPreset;
    }
    public void SetBackUp ()
    {
        if (backUp!=null)
            preset = backUp;
    }
}
