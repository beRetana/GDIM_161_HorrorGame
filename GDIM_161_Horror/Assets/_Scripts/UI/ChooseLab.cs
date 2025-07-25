using System;
using UnityEngine;
using UnityEngine.UI;

public class ChooseLab : MonoBehaviour
{
    [SerializeField] private LabChooser[] m_LabButtons;
    [SerializeField] private Color m_selected;
    [SerializeField] private Color m_Unselected;

    [Serializable]
    private struct LabChooser
    {
        public string Name;
        public Button Button;
    }

    private void Start()
    {
        foreach (var labButton in m_LabButtons)
        {
            labButton.Button.onClick.AddListener(() => PickLab(labButton.Name));
        }
    }

    private void PickLab(string labName)
    {
        foreach(var labButton in m_LabButtons)
        {
            if (labButton.Name == labName)
            {
                labButton.Button.image.color = m_selected;
                NewNetworkManager.NewSingleton.SetGameSceneName(labName);
            }
            else
            {
                labButton.Button.image.color = m_Unselected;
            }
        }
    }
}
