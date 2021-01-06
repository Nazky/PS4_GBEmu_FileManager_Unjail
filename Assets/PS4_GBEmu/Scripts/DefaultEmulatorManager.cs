using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;


using UnityGB;

public class DefaultEmulatorManager : MonoBehaviour
{
	public string Filename;
	public Renderer ScreenRenderer;

	public static bool up;
	public static bool down;
	public static bool left;
	public static bool right;

	public FileBrowser_Demo filebrowser;
	public bool fileBrowserOpen = false;

	private float lastX, lastY;
	private bool gameLoaded = false;

	private bool isFirstLoad=true;

	float elapsedTime = 0.0f;

	void Awake()
	{
		//Application.targetFrameRate = 60;
	}
	public EmulatorBase Emulator
	{
		get;
		private set;
	}
		
	private Dictionary<KeyCode, EmulatorBase.Button> _keyMapping;

	// Use this for initialization
	void Start()
	{

		up = down = left = right = false;
		lastX =    lastY = 0;

		// Init Keyboard mapping
		_keyMapping = new Dictionary<KeyCode, EmulatorBase.Button>();
		_keyMapping.Add(KeyCode.UpArrow, EmulatorBase.Button.Up);
		_keyMapping.Add(KeyCode.DownArrow, EmulatorBase.Button.Down);
		_keyMapping.Add(KeyCode.LeftArrow, EmulatorBase.Button.Left);
		_keyMapping.Add(KeyCode.RightArrow, EmulatorBase.Button.Right);
		_keyMapping.Add(KeyCode.Z, EmulatorBase.Button.A);
		_keyMapping.Add(KeyCode.X, EmulatorBase.Button.B);
		_keyMapping.Add(KeyCode.Space, EmulatorBase.Button.Start);
		_keyMapping.Add(KeyCode.LeftShift, EmulatorBase.Button.Select);


		//GetComponent<PS4Unjail> ().Unjail ();

		// Load emulator
		IVideoOutput drawable = new DefaultVideoOutput();
		IAudioOutput audio = GetComponent<DefaultAudioOutput>();
		ISaveMemory saveMemory = new DefaultSaveMemory();
		Emulator = new Emulator(drawable, audio, saveMemory);
		ScreenRenderer.material.mainTexture = ((DefaultVideoOutput) Emulator.Video).Texture;

		gameObject.GetComponent<AudioSource>().enabled = false;
		StartCoroutine(LoadRom(Filename));
	}

	void Update()
	{
		if (gameLoaded) {

			if (Input.GetButton ("L1")) {
				filebrowser.OpenFileBrowser ();
				fileBrowserOpen = true;
			}

			if (!fileBrowserOpen) {
				if (Input.GetButtonDown ("Circle"))
					Emulator.SetInput (EmulatorBase.Button.Start, true);
				if (Input.GetButtonUp ("Circle"))
					Emulator.SetInput (EmulatorBase.Button.Start, false);

				if (Input.GetButtonDown ("X"))
					Emulator.SetInput (EmulatorBase.Button.B, true);
				if (Input.GetButtonUp ("X"))
					Emulator.SetInput (EmulatorBase.Button.B, false);
		

				if (Input.GetButtonDown ("Square"))
					Emulator.SetInput (EmulatorBase.Button.A, true);
				if (Input.GetButtonUp ("Square"))
					Emulator.SetInput (EmulatorBase.Button.A, false);

				if (Input.GetButtonDown ("Triangle"))
					Emulator.SetInput (EmulatorBase.Button.Select, true);
				if (Input.GetButtonUp ("Triangle"))
					Emulator.SetInput (EmulatorBase.Button.Select, false);
		

				if (Input.GetAxis ("DpadX") != 0) {
					float DPadX = Input.GetAxisRaw ("DpadX");

					if (DPadX == 1) { 
						Emulator.SetInput (EmulatorBase.Button.Right, true); 
						lastX = DPadX;
					} else { 
						Emulator.SetInput (EmulatorBase.Button.Right, false); 
						lastX = 0;
					}

					if (DPadX == -1) {
						Emulator.SetInput (EmulatorBase.Button.Left, true);
						lastX = DPadX;
					} else { 
						Emulator.SetInput (EmulatorBase.Button.Left, false);
						lastX = 0;
					}


				} else {
			
					lastX = 0; 
					Emulator.SetInput (EmulatorBase.Button.Right, false); 
					Emulator.SetInput (EmulatorBase.Button.Left, false); 
				}

				if (Input.GetAxis ("DpadY") != 0) {
					float DPadY = Input.GetAxisRaw ("DpadY");
					if (DPadY == 1) {
						Emulator.SetInput (EmulatorBase.Button.Up, true);
					} else {
						Emulator.SetInput (EmulatorBase.Button.Up, false);
					}
					if (DPadY == -1) {
						Emulator.SetInput (EmulatorBase.Button.Down, true);
					} else {
						Emulator.SetInput (EmulatorBase.Button.Down, false);
					}

					lastY = DPadY;
				} else { 
					lastY = 0; 
					Emulator.SetInput (EmulatorBase.Button.Up, false);
					Emulator.SetInput (EmulatorBase.Button.Down, false);
				}
	
 
				foreach (KeyValuePair<KeyCode, EmulatorBase.Button> entry in _keyMapping) {
					if (Input.GetKeyDown (entry.Key))
						Emulator.SetInput (entry.Value, true);
					else if (Input.GetKeyUp (entry.Key))
						Emulator.SetInput (entry.Value, false);
				}

				if (Input.GetKeyDown (KeyCode.T)) {
					byte[] screenshot = ((DefaultVideoOutput)Emulator.Video).Texture.EncodeToPNG ();
					File.WriteAllBytes ("./screenshot.png", screenshot);
					Debug.Log ("Screenshot saved.");
				}
			}
		}
	}



	public IEnumerator LoadRom(string filename)
	{
		string path = "";
		if (isFirstLoad) {
			path = System.IO.Path.Combine (Application.streamingAssetsPath + "/ROMS", filename);
			isFirstLoad = false;
		}
		else
			path = filename;
		Debug.Log("Loading ROM from " + path + ".");

		if (!File.Exists (path)) {
			Debug.LogError("File couldn't be found.");
			yield break;
		}

		WWW www = new WWW("file://" + path);
		yield return www;

		if (string.IsNullOrEmpty(www.error))
		{
			Emulator.LoadRom(www.bytes);
			StartCoroutine(Run());
		} else
			Debug.LogError("Error during loading the ROM.\n" + www.error);
	}

	private IEnumerator Run()
	{
		gameObject.GetComponent<AudioSource>().enabled = true;
		gameLoaded = true;
		while (true)
		{
			// Run

			//if (elapsedTime > 0.01f) {
				Emulator.RunNextStep ();
			//	elapsedTime = 0.0f;
			//}
			//else
			//	elapsedTime += Time.deltaTime;

			yield return null;
		}
	}
}
