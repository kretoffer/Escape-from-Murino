using UnityEngine;

public class UIController : MonoBehaviour
{
    private CameraController _cameraController;
    private GameObject activeMenu = null;

    [SerializeField] private GameObject baseUI;
    [SerializeField] private GameObject[] menus;
    [SerializeField] private KeyCode[] keyCodes;
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            _cameraController = player.GetComponent<CameraController>();
        }
        foreach (var menu in menus)
        {
            menu.SetActive(false);
        }
    }


    void Update()
    {
        for (int i = 0; i < menus.Length; i++)
        {
            if (Input.GetKeyDown(keyCodes[i]))
            {
                if (menus[i] == activeMenu)
                {
                    activeMenu.SetActive(false);
                    baseUI.SetActive(true);
                    activeMenu = null;
                    _cameraController.Activate();
                    continue;
                }

                if (activeMenu != null)
                    continue;

                activeMenu = menus[i];
                activeMenu.SetActive(true);
                baseUI.SetActive(false);
                _cameraController.Deactivate();
            }
        }
    }
}
