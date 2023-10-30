using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelChange : MonoBehaviour
{
    [SerializeField] private Transform pricePanel;
    [SerializeField] private TMP_Text currentIndicatorsText;
    [SerializeField] private RectTransform notificationPanel;
    private List<Button> indicatorButtons =  new List<Button>();
    private List<GameObject> indicatorPanels = new List<GameObject>();
    void Start()
    {
        string btnName = "";
        string panelName = "";
        foreach (Transform obj in pricePanel)
        {
            if (obj.tag == "IndicatorsPanel")
                indicatorPanels.Add(obj.gameObject);
            else if (obj.tag == "IndicatorsButton")
                indicatorButtons.Add(obj.GetComponent<Button>());
        }
        foreach(Button btn in indicatorButtons)//����Ʈ�� ���Ե� ��ư ��
        {
            btnName = btn.name.Substring(0, btn.name.Length - 6);
            foreach (GameObject panel in indicatorPanels)//�г� ����Ʈ�� �гΰ�
            {
                panelName = panel.name;
                if (panelName.Contains(btnName))//��ư�� �̸��� ���ٸ�
                    btn.onClick.AddListener(() => IndicatorsButtonClick(panel));//��ư �̺�Ʈ�� �Ҵ��Ѵ�.
            }
            
        }
    }
    private void IndicatorsButtonClick(GameObject panel)
    {
        foreach(GameObject obj in indicatorPanels)
        {
            obj.SetActive(false);
        }
        panel.SetActive(true);
        currentIndicatorsText.text = panel.name.Substring(0, panel.name.Length - 5);
    }
    public void NotificationPanelButton()
    {
        if (notificationPanel.anchoredPosition.x < 1000)
            notificationPanel.anchoredPosition = new Vector2(5000, 0);
        else
            notificationPanel.anchoredPosition = new Vector2(0, 0);
    }
}
