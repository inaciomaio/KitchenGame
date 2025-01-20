using UnityEngine;

public class StoveCounterVisual : MonoBehaviour {
    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private GameObject stoveOnGameObject;
    [SerializeField] private GameObject particlesGameObject;

    private void Start() {
        stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
    }

    private void StoveCounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e) {
        bool showVisuals = e.state == StoveCounter.State.Cooking || e.state == StoveCounter.State.Cooked;
        stoveOnGameObject.SetActive(showVisuals);
        particlesGameObject.SetActive(showVisuals);
    }
}