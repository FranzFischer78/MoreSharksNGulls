using RaftModLoader;
﻿using UnityEngine;
using HMLLibrary;
using HarmonyLib;
using System.Collections;
using System.Runtime.CompilerServices;
using System;

public class MoreSharksNGulls : Mod
{
    private int sharkCountBaseGame = 1;
    private int seagullMaxCountBaseGame = 2;
    private int seagullMaxCountModded;
	private int maxSharkCountModded;
	private Network_Host_Entities NetworkHostEntities;
	private bool inWorld = false;

	public void Start()
    {
        //NetworkHostEntities = FindObjectOfType<Network_Host_Entities>();
        //sharkCountBaseGame = NetworkHostEntities.SharkCount;
        //seagullMaxCountBaseGame = (int)Traverse.Create(NetworkHostEntities).Field("seagullMaxCount").GetValue();
        
		Debug.Log("Mod MoreSharksNGulls has been loaded!");
	}

	public void ExtraSettingsAPI_Load() // Occurs when the API loads the mod's settings
	{
		ApplySettings();
	}

	public void ExtraSettingsAPI_SettingsClose() // Occurs when user closes the settings menu
	{
		ApplySettings();
	}



	private void ChangeMaxSeagullCount(int newCount)
    {
		if (FindObjectOfType<Network_Host_Entities>() != null)
		{
			Traverse.Create(FindObjectOfType<Network_Host_Entities>()).Field("seagullMaxCount").SetValue(newCount);
		}
		else
		{
			Debug.Log("MoreSharksNGulls|INFO: Change will be applied once in-game!");
		}
	}

	private void ApplySettings()
	{
		if (ExtraSettingsAPI_Loaded)
		{
			seagullMaxCountModded = Convert.ToInt32(ExtraSettingsAPI_GetSliderValue("maxSeagullCount"));
			maxSharkCountModded = Convert.ToInt32(ExtraSettingsAPI_GetSliderValue("maxSharkCount"));
			ChangeMaxSeagullCount(seagullMaxCountModded);
		}
	}

	public override void WorldEvent_WorldLoaded()
	{
		ApplySettings();
		inWorld = true;
		NetworkHostEntities = FindObjectOfType<Network_Host_Entities>();
		StartCoroutine(sharkSpawner());
	}

	public override void WorldEvent_WorldUnloaded()
	{
		inWorld = false;
		StopCoroutine(sharkSpawner());
	}

	IEnumerator sharkSpawner()
	{
		while (true)
		{
			if (inWorld)
			{
				if (NetworkHostEntities.SharkCount < maxSharkCountModded)
				{
					if (Raft_Network.IsHost)
					{
						AI_NetworkBehaviour spawned = ComponentManager<Network_Host_Entities>.Value.CreateAINetworkBehaviour(AI_NetworkBehaviourType.Shark, RAPI.GetLocalPlayer().transform.position + RAPI.GetLocalPlayer().transform.forward * 90);
						NetworkHostEntities.SharkCount++;
					}
				}
			}
			yield return new WaitForSeconds(20f);
		}
	}


	public void OnModUnload()
    {
		if (inWorld)
		{
			NetworkHostEntities = FindObjectOfType<Network_Host_Entities>();
			NetworkHostEntities.SharkCount = sharkCountBaseGame;
			NetworkHostEntities.SeagullCount = seagullMaxCountBaseGame;
		}
		Debug.Log("Mod MoreSharksNGulls has been unloaded!");
    }


	//Extra Settings API
	static bool ExtraSettingsAPI_Loaded = false;
	// Use to get the current value from a Slider type setting
	// This method returns the value of the slider rounded according to the mod's setting configuration
	[MethodImpl(MethodImplOptions.NoInlining)]
	public static float ExtraSettingsAPI_GetSliderValue(string SettingName) => 0;
}