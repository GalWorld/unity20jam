using UnityEngine;
using System.Collections;

public class BlueBoxController : MonoBehaviour
{
     public Renderer rendBoxBlue;

    public float minIntensity = 20f;
    public float maxIntensity = 200f;
    public float speed = 4f;
    private Material mat;
    public Light pointLightBoxBlue;

    private int _randomValue = 0;
    public bool OnEnable= false;
   
    
    void Start()
    {
        mat = rendBoxBlue.material;
        pointLightBoxBlue.color= Color.yellow;
        StartCoroutine(ChangingBool());
        
        
    }

   void Update()
    {
        if(OnEnable)
        {
            LightChangeColor();
            
        }
        
    }

   void LightChangeColor()
    {
        pointLightBoxBlue.color= Color.yellow;
        float t = Mathf.PingPong(Time.time * speed, 1f);
        float intensity = Mathf.Lerp(minIntensity, maxIntensity, t);
        pointLightBoxBlue.intensity= intensity;
        StartCoroutine(ChangingColor());
        StartCoroutine(ChangingBool());

       
    }

    IEnumerator ChangingColor()
    {
        yield return new WaitForSeconds(3);

        _randomValue= Random.Range(0,6);
        if(_randomValue <=3)
        {
            pointLightBoxBlue.color= Color.red;
            StartCoroutine(OriginalColor());

        }
        else
        {
            pointLightBoxBlue.color= Color.green;
            StartCoroutine(OriginalColor());
        }

        OnEnable= false;
    }

    IEnumerator ChangingBool()
    { yield return new WaitForSeconds(30);
        OnEnable=true;
        
    }

    IEnumerator OriginalColor()
    {
        yield return new WaitForSeconds(5);
        pointLightBoxBlue.color= Color.yellow;

    }
}
