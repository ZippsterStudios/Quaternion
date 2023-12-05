using UnityEngine;
namespace ZippsterStudios.Math
{
    public class Test_Coordinate : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            ECEF ecef = new ECEF(100, 1000, 1000);
            ECEF refECEF = new ECEF(100, 1000, 500);
            Coordinate x = new Coordinate(ecef, refECEF);
            Debug.Log($"ECEF: {x.Ecef}, LLA: {x.Lla}, ENU: {x.Enu}, RAE{x.Rae}");
        }
    }
}