using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
    public GameObject Prefab;
    // Start is called before the first frame update
    public void Create()
    {
        var thisGameObject=Instantiate(Prefab,transform);
        var demoPosition=Camera.main.transform.position;
        demoPosition.z = 0;
        thisGameObject.transform.position = demoPosition;
    }
    public void Create(Vector3 position)
    {
        var thisGameObject = Instantiate(Prefab, transform);
        thisGameObject.transform.position = position;
    }
}
