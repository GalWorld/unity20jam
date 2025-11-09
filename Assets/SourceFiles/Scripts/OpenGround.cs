using UnityEngine;

public class OpenGround : MonoBehaviour
{
    public Transform _groundLeft;
    public Transform _groundRight;
    private float _speedGroundOpen= 2f;
    public bool _onPlatform= false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        OpenFloor();
        
    }

    public void OpenFloor()
    {
        if(_onPlatform== true)
        {
            _groundLeft.Translate(-_groundLeft.forward* _speedGroundOpen* Time.deltaTime);
        _groundRight.Translate(_groundRight.forward* _speedGroundOpen* Time.deltaTime);
            
        }
        
        
    }
}
