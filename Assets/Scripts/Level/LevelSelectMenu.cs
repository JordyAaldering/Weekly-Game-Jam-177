using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelectMenu : MonoBehaviour
{
    [SerializeField] private GameObject gridObject;
	[SerializeField] private GameObject levelSelectButtonPrefab;

	private Level[] levels;

	private void Awake()
	{
		PopulateGrid();
	}

	private void PopulateGrid()
	{
		levels = Resources.LoadAll<Level>("Levels");
		int maxLvl = PlayerPrefs.GetInt("MaxLevel", 0);

		for (int i = 0; i < levels.Length; i++) {
			GameObject obj = Instantiate(levelSelectButtonPrefab, gridObject.transform);
			obj.name = $"Level Select Button {i + 1}";
			obj.GetComponentInChildren<TextMeshProUGUI>().text = $"{i + 1}";

			if (i <= maxLvl) {
				Button button = obj.GetComponent<Button>();
				button.interactable = true;
				int lvl = i;
				button.onClick.AddListener(delegate { StartLevel(lvl); });
			}
		}
	}

	public void StartLevel(int lvl)
	{
		PlayerPrefs.SetInt("CurLevel", lvl);
		SceneManager.LoadScene(1);
	}
}
