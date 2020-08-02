using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelCreator))]
public class LevelEditor : Editor
{
    
    #region Private

    private readonly string[] _toolbars = {"Entrance Points", "Exit Points", "Obstacles"};
    private int _selectedToolBar = 0;

    #endregion
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        
        var levelCreator = (LevelCreator) target;

        #region HelpBox

        EditorGUILayout.HelpBox("To start level creation press \"Create New Level\" button. OR\n" +
                                "To update an existing level, you drag and drop the level to Creating Level field.", MessageType.Info);
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("When a new level is created, you must update the max level number " +
                                "on the game Manager object in the hierarchy.", MessageType.Info);


        #endregion
        
        #region Create Level Field
        
        GUILayout.BeginHorizontal();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("creatingLevel"));
        EditorGUI.BeginDisabledGroup(levelCreator.isEditingLevel);
        if (GUILayout.Button("Create New Level"))
        {
            levelCreator.CreateNewLevel();
            levelCreator.isCreatingNewLevel = true;
        }

        if (levelCreator.creatingLevel == null)
        {
            levelCreator.isCreatingNewLevel = false;
        }
        EditorGUI.EndDisabledGroup();
        GUILayout.EndHorizontal();
        EditorGUILayout.Space();
        
        #endregion

        if (levelCreator.creatingLevel != null && !levelCreator.isCreatingNewLevel) 
        {
            #region HelpBox

            EditorGUILayout.HelpBox("You attempt to update a level! \n" +
                                    "If you do not update the level, data of the existing level will be lost.", MessageType.Warning);

            #endregion
        }
            
        if (levelCreator.creatingLevel != null)
        {
            #region Create Objects

            if (GUILayout.Button("Create Points")) levelCreator.CreatePoints();
            if (GUILayout.Button("Create Obstacle")) levelCreator.CreateObstacles();
            EditorGUILayout.Space();

            #endregion

            #region Save/Update

            GUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(levelCreator.creatingLevel == null || levelCreator.isEditingLevel);
            if (GUILayout.Button("Save Level")) levelCreator.SaveLevel();
            EditorGUI.EndDisabledGroup();

            EditorGUI.BeginDisabledGroup(!levelCreator.isEditingLevel);
            if (GUILayout.Button("Update Level")) levelCreator.UpdateLevel();
            EditorGUI.EndDisabledGroup();
            GUILayout.EndHorizontal();
            EditorGUILayout.Space();
            
            #endregion

            #region HelpBox

            EditorGUILayout.HelpBox(
                            "After saving, you need to drag and drop the new level to the specified path." +
                            "( MANAGERS/GameManager/GameController -> Level )",
                            MessageType.Info);

            #endregion
        }
        
        if (!levelCreator.isCreatingNewLevel)
        {
            #region Load

            EditorGUI.BeginDisabledGroup(levelCreator.creatingLevel == null);
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Load Level"))
            {
                if (levelCreator.creatingLevel != null) levelCreator.LoadLevelObject();
                else Debug.LogWarning("Please drag and drop a level you want to load.");
            }

            if (GUILayout.Button("Cancel Loading"))
            {
                levelCreator.creatingLevel = null;
                levelCreator.isEditingLevel = false;
            }

            GUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();
            EditorGUILayout.Space();
            #endregion
        }

        #region Clear
        
        if (GUILayout.Button("Clear")) levelCreator.Clear();
        EditorGUILayout.Space();
        #endregion

        #region HelpBox

        EditorGUILayout.HelpBox(
            "You can create obstacles, entrance, and exit points. After the creation object, " +
            "you must relocate these objects. When you finish creating, press \"Save Level\" button, " +
            "then level will be saved.",
            MessageType.Info);

        #endregion
        

        if (levelCreator.creatingLevel != null)
        {
            #region Object Toolbar

            _selectedToolBar = GUILayout.Toolbar(_selectedToolBar, _toolbars);
            switch (_selectedToolBar)
            {
                default:
                case 0:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("startGameObjects"));
                    break;
                case 1:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("targetGameObjects"));
                    break;
                case 2:
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("obstaclesGameObjects"));
                    break;
            }

            #endregion
        }

        serializedObject.ApplyModifiedProperties();
    }
}
