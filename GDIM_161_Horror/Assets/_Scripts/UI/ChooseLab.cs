using UnityEngine;
using UnityEngine.UI;

public class ChooseLab : MonoBehaviour
{
    [SerializeField] private Button m_SmallGameScene;
    [SerializeField] private Button m_BigGameScene;
    [SerializeField] private Button m_BuildGameScene;
    [SerializeField] private string m_BuildName;
    [SerializeField] private string m_SmallName;
    [SerializeField] private string m_BigName;
    [SerializeField] private Color m_selected;
    [SerializeField] private Color m_Unselected;

    private void Start()
    {
        m_SmallGameScene.onClick.AddListener(SetSmallScene);
        m_BigGameScene.onClick.AddListener(SetBigScene);
        m_BuildGameScene.onClick.AddListener(SetBuildScene);
    }

    private void SetSmallScene()
    {
        m_SmallGameScene.image.color = m_selected;
        m_BigGameScene.image.color = m_Unselected;
        m_BuildGameScene.image.color = m_Unselected;
        (NewNetworkManager.singleton as NewNetworkManager).SetGameSceneName(m_SmallName);
    }

    private void SetBigScene()
    {
        m_BigGameScene.image.color = m_selected;
        m_SmallGameScene.image.color = m_Unselected;
        m_BuildGameScene.image.color = m_Unselected;
        (NewNetworkManager.singleton as NewNetworkManager).SetGameSceneName(m_BigName);
    }

    private void SetBuildScene()
    {
        m_BigGameScene.image.color = m_Unselected;
        m_SmallGameScene.image.color = m_Unselected;
        m_BuildGameScene.image.color = m_selected;
        (NewNetworkManager.singleton as NewNetworkManager).SetGameSceneName(m_BuildName);
    }
}
