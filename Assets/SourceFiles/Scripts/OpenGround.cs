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
        else
        {
            float originalZLeft= -14.89f;
            Vector3 originPosition= new Vector3(_groundLeft.position.x, _groundLeft.position.y, originalZLeft );
            _groundLeft.position= originPosition;

            float originalZRight= 15.50f;
            Vector3 _originPositionRight = new Vector3(_groundRight.position.x, _groundRight.position.y, originalZRight);
            _groundRight.position= _originPositionRight;
        }
        
        
    }
}
