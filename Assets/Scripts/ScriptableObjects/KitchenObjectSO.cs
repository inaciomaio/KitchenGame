using UnityEngine;

[CreateAssetMenu()]
public class KitchenObjectSO : ScriptableObject {
    // In this instance it's public because the tutor doesn't ever write directly to a scriptable object, the ideal way is still to make it a serializedfield and create a get function 
    public Transform prefab;
    public Sprite sprite;
    public string objectName;
}
