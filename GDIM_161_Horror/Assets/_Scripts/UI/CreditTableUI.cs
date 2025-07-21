using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Toolbars;

public class CreditTableUI : MonoBehaviour
{
    [SerializeField] private HorizontalLayoutGroup m_HorizontalGrid;
    [SerializeField] private CreditsInformation m_Information;
    [SerializeField] private Transform m_CategoryTemplate;
    [SerializeField] private Transform m_InfoTemplate;

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

    public void DisplayCredits()
    {
        gameObject.SetActive(true);
    }

    private void CreateCategories()
    {
        for (byte index = 0; index < m_Information.CreditsCategories.Length; ++index)
        {
            Transform category = Instantiate(m_CategoryTemplate, m_HorizontalGrid.transform);
            PopulateCategory(category, index);
        }
    }

    private void PopulateCategory(Transform container, byte index)
    {
        var information = m_Information.CreditsCategories[index];
        Transform category = CreateCreditCell(container, information.Category);
        TextMeshProUGUI content = category.GetComponent<TextMeshProUGUI>();
        content.fontSize = 48f;
        content.color = Color.red;
        content.alignment = TextAlignmentOptions.Center;
        content.fontStyle = FontStyles.Bold | FontStyles.Italic;

        foreach (string text in information.Lines)
        {
            CreateCreditCell(container, text);
        }
    }

    private Transform CreateCreditCell(Transform parent, string content)
    {
        Transform container = Instantiate(m_InfoTemplate, parent);
        TextMeshProUGUI textMesh;
        if (!container.TryGetComponent<TextMeshProUGUI>(out textMesh)) return null;
        textMesh.text = content;
        return container;
    }
}
