using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GUI : MonoBehaviour {
    public Dictionary<string, Image> images = new Dictionary<string, Image>();
    public Dictionary<string, Text> texts = new Dictionary<string, Text>();
    public AnimationCurve alphaCurve;
    public static GUI instance;
	// Use this for initialization
	void Start () {
        Image[] imgs = FindObjectsOfType<Image>(); 
	    foreach (Image i in imgs)
        {
            images.Add(i.name, i);
        }
        Text[] txts = FindObjectsOfType<Text>();
        foreach (Text t in txts)
        {
            texts.Add(t.name, t);
        }
        if (instance == null)
            instance = FindObjectOfType<GUI>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public void NewAbility (Skill s)
    {
        StartCoroutine(Animation_Show(s));
    }
    protected IEnumerator Animation_Show(Skill s)
    {
        float currentT = 0.0f;
        Color c=new Color();
        string inputString = s.name;

        Image targetImg = images[inputString + "_input"];
        Text targetTx = texts[inputString + "_name"];

        images["NewSkill"].gameObject.SetActive(true);
        targetImg.gameObject.SetActive(true);
        targetTx.gameObject.SetActive(true);

        while (currentT < alphaCurve.length)
        {
            currentT += Time.deltaTime;
            c= images["NewSkill"].color;
            c.a = alphaCurve.Evaluate(currentT);
            images["NewSkill"].color = c;
            targetImg.color = c;
            targetTx.color = c;
            yield return null;
        }
        yield return new WaitForSeconds(3.0f);
        while (currentT > 0.0f)
        {
            currentT -= Time.deltaTime;
            c = images["NewSkill"].color;
            c.a = alphaCurve.Evaluate(currentT);
            images["NewSkill"].color = c;
            targetImg.color = c;
            targetTx.color = c;
            yield return null;
        }
        c.a = 0.0f;
        images["NewSkill"].color = c;
        targetImg.color = c;
        targetTx.color = c;

        images["NewSkill"].gameObject.SetActive(false);
        targetImg.gameObject.SetActive(false);
        targetTx.gameObject.SetActive(false);
    }
}
