using UnityEngine;
using System.Collections;

public class BlueBoxController : MonoBehaviour
{
     public Renderer rendBoxBlue;

    public float minIntensity = 20f;
    public float maxIntensity = 400f;
    public float speed = 4f;
    private Material mat;
    public Light pointLightBoxBlue;

    private int _randomValue = 0;
    public bool OnEnable= false;

    public GameObject _plusTone;
    public GameObject _minusTone;

    public Transform BoxBlue;
    private float _heightOffset = 2.5f;

   
    
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
        OnEnable= false;
        

       
    }

    IEnumerator ChangingColor()
    {
        _randomValue= Random.Range(0,6);
        yield return new WaitForSeconds(5);

        
        if(_randomValue <=3)
        {
            Vector3 offset= new Vector3(BoxBlue.transform.position.x, BoxBlue.transform.position.y+ _heightOffset, BoxBlue.transform.position.z );
            pointLightBoxBlue.color= Color.red;
            StartCoroutine(OriginalColor());
            Instantiate(_minusTone, offset,Quaternion.identity);

        }
        else
        {
            Vector3 offset2= new Vector3(BoxBlue.transform.position.x, BoxBlue.transform.position.y+ _heightOffset, BoxBlue.transform.position.z);
            pointLightBoxBlue.color= Color.green;
            StartCoroutine(OriginalColor());
            Instantiate(_plusTone, offset2, Quaternion.identity);
        }

        
        StartCoroutine(ChangingBool());
    }

    IEnumerator ChangingBool()
    { yield return new WaitForSeconds(10);
        OnEnable=true;
        
    }

    IEnumerator OriginalColor()
    {
        yield return new WaitForSeconds(5);
        pointLightBoxBlue.color= Color.yellow;

    }
}
