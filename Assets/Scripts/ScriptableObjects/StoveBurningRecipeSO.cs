using UnityEngine;

[CreateAssetMenu()]
public class StoveBurningRecipeSO : ScriptableObject {
    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public float burningTimerMax;


}
