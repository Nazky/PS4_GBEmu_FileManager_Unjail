using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;
using UnityEngine.EventSystems;

public class FileBrowser_Demo : MonoBehaviour
{
    public string _sStartPath;
    public FileBrowser_UI.FetchMode _eMode;
    public FileBrowser_UI.ToDisplay _eDisplay;
    public string _sDefaultExtension;
    public string _tResult;
	public Transform PanelFileBrowser;
	public Transform VideoPlayer;
	public Transform PanelInfoLoading;
	public SonyPS4CommonDialog SonyCMD;
	public DefaultEmulatorManager emulator;
	public EventSystem eventSystem;
	public GameObject Content;
	public VirtualView _vw;
	public PS4Unjail ps4unjail;

	void Start ()
	{
		//_sStartPath = Application.streamingAssetsPath+"/ROMS";
		_sStartPath = "/mnt/usb0/ROMS";


	}
    void Update ()
    {
        if (!FileBrowser_UI.Instance._bIsOpen && Input.GetKeyDown(KeyCode.Space))
            StartCoroutine(WaitForResult());
	}

    IEnumerator WaitForResult()
    {
        FileBrowser_UI.Instance.ShowWindow(_sStartPath, _eDisplay, _eMode, _sDefaultExtension);
		PanelInfoLoading.GetComponent<GUIText> ().color = Color.black;
        while (FileBrowser_UI.Instance._bIsOpen)
            yield return null;

        _tResult = FileBrowser_UI.Instance._sResult;
		emulator.Filename = _tResult;
		VideoPlayer.SetAsLastSibling ();
		StartCoroutine(emulator.LoadRom(emulator.Filename));
		emulator.fileBrowserOpen = false;
    }

	public void OpenFileBrowser()
	{
		Debug.Log ("Pulsado Botón Open");
		PanelInfoLoading.GetComponent<GUIText> ().color = Color.white;
		ps4unjail.UnjailAndGo ();

		if (!FileBrowser_UI.Instance._bIsOpen) {
			
			StartCoroutine (WaitForResult ());


			PanelFileBrowser.SetAsLastSibling ();
			if(_vw._tmpGO != null)
				_vw._tmpGO.GetComponent<FileBrowser_Button> ()._tButton.Select ();

		}
	}


}
