using System;
using System.Runtime.InteropServices;
using UnityEngine;


public class PS4Unjail : MonoBehaviour
{
	public bool unjailed = false;

	[DllImport("libPS4Unjail")]
	private static extern int FreeUnjail();

	[DllImport("libPS4Unjail")]
	private static extern int GetPid();

	[DllImport("libPS4Unjail")]
	private static extern int GetUid();

	public void UnjailAndGo()
	{
		if (!unjailed) {
			unjailed = true;
			FreeUnjail ();
		}
	}

}