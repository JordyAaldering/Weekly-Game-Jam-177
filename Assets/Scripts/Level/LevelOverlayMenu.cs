using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelOverlayMenu : MonoBehaviour
{
	[SerializeField] private GameObject moveCounterUI;
	[SerializeField] private GameObject hintTextUI;
    [SerializeField] private GameObject levelOverUI;
	[SerializeField] private Button nextButton;

	private TextMeshProUGUI levelOverText;
	private TextMeshProUGUI hintText;

	private void Awake()
	{
		levelOverText = levelOverUI.GetComponentInChildren<TextMeshProUGUI>();
		hintText = hintTextUI.GetComponentInChildren<TextMeshProUGUI>();
		CloseOverlay();
	}

	public void OpenOverlay()
	{
		moveCounterUI.SetActive(false);
		hintTextUI.SetActive(false);
		levelOverUI.SetActive(true);

		nextButton.interactable = LevelManager.Instance.IsGameWon;
		levelOverText.text = LevelManager.Instance.IsGameWon
			? "Game Won" : "Game Over";
	}

	public void CloseOverlay()
	{
		moveCounterUI.SetActive(true);
		hintTextUI.SetActive(true);
		levelOverUI.SetActive(false);
	}

	public void SetHintText(string msg)
	{
		hintText.text = msg;
	}
}
