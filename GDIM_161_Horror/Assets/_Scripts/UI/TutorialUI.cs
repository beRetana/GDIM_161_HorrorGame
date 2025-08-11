using UnityEngine.UI;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private GameObject m_TutorialPanel;
    [SerializeField] private Button m_BtnOpenTutorial;
    [SerializeField] private Button m_BtnCloseTutorial;

    private void Start()
    {
        m_BtnOpenTutorial.onClick.AddListener(() => m_TutorialPanel.SetActive(true));
        m_BtnCloseTutorial.onClick.AddListener(() => m_TutorialPanel.SetActive(false));
    }
}
