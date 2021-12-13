using UnityEngine;

public class CarSelection : MonoBehaviour
{
   
    
    public GameObject Visual
    {
        get { return _visual; }
    }

    public CarSettings carSettings;
    [SerializeField] private GameObject _visual;

    public void Activate()
    {
        _visual.SetActive(true);
    }
    
    public void Deactivate()
    {
        _visual.SetActive(false);
    }
}

