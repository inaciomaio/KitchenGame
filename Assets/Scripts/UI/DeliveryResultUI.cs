using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeliveryResultUI : MonoBehaviour {

    private const string POPUP = "Popup";
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Color successColor;
    [SerializeField] private Color failColor;
    [SerializeField] private Sprite sucessSprite;
    [SerializeField] private Sprite failSprite;

    private Animator animator;

    private void Awake() {
        animator = GetComponent<Animator>();
    }

    private void Start() {
        animator.SetTrigger(POPUP);
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;

        gameObject.SetActive(false);
    }

    private void DeliveryManager_OnRecipeSuccess(object sender, System.EventArgs e) {
        gameObject.SetActive(true);
        animator.SetTrigger(POPUP);
        backgroundImage.color = successColor;
        iconImage.sprite = sucessSprite;
        messageText.text = "DELIVERY\nSUCCESS!";
    }

    private void DeliveryManager_OnRecipeFailed(object sender, System.EventArgs e) {
        gameObject.SetActive(true);
        animator.SetTrigger(POPUP);
        backgroundImage.color = failColor;
        iconImage.sprite = failSprite;
        messageText.text = "DELIVERY\nFAILED!";
    }

}
