using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class GameProgressWindow : EditorWindow
{
	public Dictionary<string, string> objectives;
	public List<bool> objectivesCompleted;

	public int maxGoals;
	public int currentGoals;

	public string playerPrefsName = "Game Progress Window";

	bool showAddObjectivesMenu = false;
	bool showObjectives = true;
	bool showFinished = true;

	//Private variables
	string objecttiveName = "Objective Name";
	string objecttiveDescription = "Objective Description";

	int maxSizeOptions;

	Vector2 scrollPos;

	bool fold;

	[MenuItem("Window/Game Progress Window")]
	public static void ShowWindow()
	{
		GetWindow<GameProgressWindow>("Game Progress");
	}

	private void Awake()
	{
		objectives = new Dictionary<string, string>();
		objectivesCompleted = new List<bool>();
	}

	private void OnFocus()
	{
		playerPrefsName = PlayerPrefs.GetString("PrefsName");
		maxGoals = PlayerPrefs.GetInt(playerPrefsName + "_MaxGoals");
		currentGoals = PlayerPrefs.GetInt(playerPrefsName + "_CurrentGoals");
	}

	private void OnGUI()
	{
		GUILayout.BeginVertical();
		GUILayout.Space(10);

		List<GUILayoutOption> expandedTextLabels = new List<GUILayoutOption>();
		expandedTextLabels.Add(GUILayout.ExpandWidth(true));

		GUILayout.Label("		Software Developement Progress", expandedTextLabels.ToArray());

		GUILayout.Space(10);

		GUILayout.EndVertical();

		GUILayout.BeginHorizontal();

		float currGoalTemp = currentGoals;
		float maxGoalTemp = maxGoals;

		float percentageCompleted = (currGoalTemp / maxGoalTemp) * 100;

		GUILayout.Label("   ");
		EditorGUILayout.Slider(percentageCompleted, 0, 100.0f);

		GUILayout.Label("%");
		GUILayout.EndHorizontal();

		GUILayout.BeginVertical();

		GUILayout.Space(10);

		GUILayout.Label("		" + currentGoals + " completed goals out of " + maxGoals + " goals!", expandedTextLabels.ToArray());

		GUILayout.Space(20);

		GUILayout.EndVertical();

		GUILayout.BeginHorizontal();
		playerPrefsName = GUILayout.TextField(playerPrefsName);

		if (GUILayout.Button("Update Player Prefs Name"))
		{
			PlayerPrefs.SetString("PrefsName", playerPrefsName);
		}
		GUILayout.EndHorizontal();

		GUILayout.Space(10);

		GUIContent f = new GUIContent();
		f.text = "Show Add New Objectives Menu";
		showAddObjectivesMenu = EditorGUILayout.BeginFoldoutHeaderGroup(showAddObjectivesMenu, f);

		if (showAddObjectivesMenu)
		{
			GUILayout.Space(5);
			//Show Add Objectives Components
			GUILayout.BeginHorizontal();
			GUILayout.Label("Name ");
			objecttiveName = GUILayout.TextField(objecttiveName);
			GUILayout.EndHorizontal();

			GUILayout.BeginVertical();
			GUILayout.Label("Description ");
			objecttiveDescription = GUILayout.TextArea(objecttiveDescription);
			GUILayout.EndVertical();

			GUILayout.Space(5);
			if (GUILayout.Button("Add New Objective"))
			{
				PlayerPrefs.SetString(playerPrefsName, PlayerPrefs.GetString(playerPrefsName) + ">" + objecttiveName + ":" + objecttiveDescription + ":false");
			}
		}

		EditorGUILayout.EndFoldoutHeaderGroup();

		List<GUILayoutOption> options = new List<GUILayoutOption>();

		//Showing Objectives

		//EditorGUILayout.BeginHorizontal();
		showObjectives = EditorGUILayout.BeginToggleGroup("Show Objectives", showObjectives);
		//showFinished = EditorGUILayout.BeginToggleGroup("Show Finished Tasks", showFinished);
		//EditorGUILayout.EndHorizontal();

		List<GUILayoutOption> tempOpt = new List<GUILayoutOption>();
		tempOpt.Add(GUILayout.ExpandWidth(true));
		tempOpt.Add(GUILayout.ExpandHeight(true));

		scrollPos = GUILayout.BeginScrollView(scrollPos, tempOpt.ToArray());
		
		if (showObjectives)
		{
			if (PlayerPrefs.GetString(playerPrefsName) != "")
			{
				objectives = new Dictionary<string, string>();
				objectivesCompleted = new List<bool>();

				string[] values = PlayerPrefs.GetString(playerPrefsName).Split(">"[0]);

				int i = 0;
				foreach (var item in values)
				{
					if (item.Contains(":") && !objectives.ContainsKey(item.Split(":"[0])[0]))
					{
						objectives.Add(item.Split(":"[0])[0], item.Split(":"[0])[1]);

						options.Add(GUILayout.ExpandWidth(true));
						options.Add(GUILayout.ExpandHeight(false));

						GUIContent d = new GUIContent();
						d.text = item.Split(":"[0])[0];

						GUIContent dd = new GUIContent();
						dd.text = item.Split(":"[0])[1];

						GUILayout.Box(d, options.ToArray());

						objectivesCompleted.Add(bool.Parse(item.Split(":"[0])[2]));

						GUILayout.Box(dd, options.ToArray());

						GUIContent g = new GUIContent();
						g.text = "Show More Options";

						bool foldd = bool.Parse(PlayerPrefs.GetString("boolValue" + i) == "" ? "true" : PlayerPrefs.GetString("boolValue" + i));

						if (objectivesCompleted[i])
						{
							//Has been completed
							EditorGUILayout.HelpBox("Completed", MessageType.Info);
						}
						else
						{
							if (dd.text.ToLower().Contains("bug"))
							{
								//Has been completed
								EditorGUILayout.HelpBox("BUG", MessageType.Error);
							}

							if (dd.text.ToLower().Contains("fix"))
							{
								//Has been completed
								EditorGUILayout.HelpBox("FIX", MessageType.Error);
							}

							if (dd.text.ToLower().Contains("add"))
							{
								//Has been completed
								EditorGUILayout.HelpBox("ADD", MessageType.Warning);
							}

							if (dd.text.ToLower().Contains("optimize"))
							{
								//Has been completed
								EditorGUILayout.HelpBox("OPTIMIZE", MessageType.Warning);
							}
						}

						foldd = EditorGUILayout.BeginFoldoutHeaderGroup(foldd, g);
						PlayerPrefs.SetString("boolValue" + i, foldd + "");

						if (foldd)
						{
							GUILayout.BeginHorizontal();

							if (!objectivesCompleted[i])
							{
								if (GUILayout.Button("Mark As Finished"))
								{
									objectivesCompleted[i] = true;

									currentGoals++;
								}
							}
							else
							{
								if (GUILayout.Button("Mark As UnFinished"))
								{
									objectivesCompleted[i] = false;

									currentGoals--;
								}
							}

							if (GUILayout.Button("Remove"))
							{
								objectives.Remove(item.Split(":"[0])[0]);
							}

							GUILayout.EndHorizontal();
						}

						EditorGUILayout.EndFoldoutHeaderGroup();

						GUILayout.Space(15);

						i++;
					}
				}
			}

			//Update player prefs
			int ii = 0;
			PlayerPrefs.SetString(playerPrefsName, "");
			foreach (var item in objectives)
			{
				PlayerPrefs.SetString(playerPrefsName, PlayerPrefs.GetString(playerPrefsName) + ">" + item.Key + ":" + item.Value + ":" + objectivesCompleted[ii]);

				ii++;
			}

			maxSizeOptions = objectives.Count;
			objectives = new Dictionary<string, string>();
			objectivesCompleted = new List<bool>();
		}
		
		GUILayout.EndScrollView();
		
		EditorGUILayout.EndToggleGroup();

		//Only update max goals if value is larger
		PlayerPrefs.SetInt(playerPrefsName + "_MaxGoals", maxGoals < maxSizeOptions ? maxSizeOptions : currentGoals <= maxSizeOptions ? maxSizeOptions : maxGoals);
		PlayerPrefs.SetInt(playerPrefsName + "_CurrentGoals", currentGoals);
	}
}
