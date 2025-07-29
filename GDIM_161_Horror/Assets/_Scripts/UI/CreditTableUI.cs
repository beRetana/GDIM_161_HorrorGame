using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreditTableUI : MonoBehaviour
{
    [SerializeField] private Animator m_AnimatorCredits;
    [SerializeField] private VerticalLayoutGroup m_VerticalGrid;
    [SerializeField] private CreditsInformation m_Information;
    [SerializeField] private Transform m_CategoryTemplate;
    [SerializeField] private Transform m_InfoTemplate;

    private const string START_CREDITS = "START_CREDITS";

    [ContextMenu("Generate Credits Table")]
    private void GenerateInEditor()
    {
#if UNITY_EDITOR
        CreateCategories();
#endif
    }

    public void HideCredits()
    {
        gameObject.SetActive(false);
    }

    public void SetCreditsActive(bool active)
    {
        gameObject.SetActive(active);
        if (active) m_AnimatorCredits.SetTrigger(START_CREDITS);
        else m_AnimatorCredits.ResetTrigger(START_CREDITS);
    }

    private void CreateCategories()
    {
        for (byte index = 0; index < m_Information.CreditsCategories.Length; ++index)
        {
            CreateCreditCell(m_CategoryTemplate, m_Information.CreditsCategories[index].Category);
            PopulateCategory(index);
        }
    }

    private void PopulateCategory(byte index)
    {
        var lines = m_Information.CreditsCategories[index];

        foreach (string text in lines.Lines)
        {
            CreateCreditCell(m_InfoTemplate, text);
        }
    }

    private Transform CreateCreditCell(Transform template, string content)
    {
        Transform container = Instantiate(template, m_VerticalGrid.transform);
        TextMeshProUGUI textMesh;
        if (!container.TryGetComponent<TextMeshProUGUI>(out textMesh)) return null;
        textMesh.text = content;
        return container;
    }
}
