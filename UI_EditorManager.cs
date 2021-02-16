using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager_Editor : MonoBehaviour
{
    public EditorManager editorManager;

    public GameObject preEditPanel;
    [System.Serializable]
    public struct PreEdit
    {
        public GameObject selectLevelPanel;
        [System.Serializable]
        public struct SelectLevel
        {
            public GameObject buttonsPanel;
            [System.Serializable]
            public struct Buttons
            {
                public Button createLevelButton;
                public Button editLevelButton;
                public Button deleteLevelButton;
                public Button backButton;
            }
            public Buttons buttons;

            public GameObject levelListContent;
        }
        public SelectLevel selectLevel;

        public GameObject createLevelPanel;
        [System.Serializable]
        public struct CreateLevel
        {
            public Button backButton;
            public Dropdown selectMapDropdown;
            public Button nextButton;
        }
        public CreateLevel createLevel;
    }
    public PreEdit preEdit;

    public GameObject editPanel;
    [System.Serializable]
    public struct Edit
    {
        public GameObject editLevelPanel;
        [System.Serializable]
        public struct EditLevel
        {
            public GameObject enemySelectorPanel;
            [System.Serializable]
            public struct EnemySelector
            {
                public GameObject topPanel;
                [System.Serializable]
                public struct Top
                {
                    public Dropdown selectAmountDropdown;
                    public Dropdown selectTypeDropdown;
                    public Dropdown selectPathDropdown;
                    public Dropdown selectWaveDropdown;
                }
                public Top top;

                public GameObject buttomPanel;
                [System.Serializable]
                public struct Bottom
                {
                    public Button addPathButton;
                    public Button deletePathButton;
                    public Button addWaveButton;
                    public Button deleteWaveButton;
                }
                public Bottom bottom;
            }
            public EnemySelector enemySelector;

            public GameObject enemyListPanel;
            [System.Serializable]
            public struct EnemyList
            {
                public GameObject enemyListContent;
                public Button addToQueueButton;
                public Button removeFromQueueButton;
            }
            public EnemyList enemyList;

            public Dropdown actionSelectorDropdown;
            public Button backButton;
            public Button saveButton;
        }
        public EditLevel editLevel;

        public GameObject createPathPanel;
        [System.Serializable]
        public struct CreatePath
        {
            public Button doneButton;
        }
        public CreatePath createPath;

        public GameObject saveLevelPanel;
        [System.Serializable]
        public struct SaveLevel
        {
            public GameObject inputFieldsPanel;
            [System.Serializable]
            public struct InputFields
            {
                public InputField levelNameInputField;
                public InputField startingGoldInputField;
                public InputField startingHPInputField;
            }
            public InputFields inputFields;

            public Button backButton;
            public Button doneButton;
        }
        public SaveLevel saveLevel;
    }
    public Edit edit;

    #region PreEditMethods
    public void CreateLevelBtn_Buttons_SelectLevel_PreEdit()
    {
        preEdit.selectLevelPanel.SetActive(false);
        preEdit.createLevelPanel.SetActive(true);
    }

    public void EditLevelBtn_Buttons_SelectLevel_PreEdit()
    {
        preEditPanel.SetActive(false);
        editPanel.SetActive(true);

        //CHECK
        editorManager.LoadLevelToEdit();
    }

    public void DeleteLevelBtn_Buttons_SelectLevel_PreEdit()
    {
        //TO DO: implement
    }

    public void BackBtn_Buttons_SelectLevel_PreEdit()
    {
        SceneManager.LoadScene("Menu");
    }

    public void BackBtn_CreateLevel_PreEdit()
    {
        preEdit.createLevelPanel.SetActive(false);
        preEdit.selectLevelPanel.SetActive(true);

        //CHECK
        editorManager.ResetEditor();
    }

    public void SelectMapDropdown_CreateLevel_PreEdit(int index)
    {
        //CHECK
        editorManager.SelectMapToPlace(index);
    }

    public void NextBtn_CreateLevel_PreEdit()
    {
        preEdit.createLevelPanel.SetActive(false);
        preEdit.selectLevelPanel.SetActive(true);
        preEditPanel.SetActive(false);
        editPanel.SetActive(true);
    }
    #endregion

    #region EditMethods
    public void SelectAmountDropdown_Top_EnemySelector_EditLevel_Edit()
    {
        //CHECK
        editorManager.SetSelectedAmount();
    }

    public void SelectTypeDropdown_Top_EnemySelector_EditLevel_Edit(int index)
    {
        //CHECK
        editorManager.SetSelectedTypeIndex(index);
    }

    public void SelectPathDropdown_Top_EnemySelector_EditLevel_Edit(int index)
    {
        //CHECK
        editorManager.SetSelectedPathIndex(index);
    }

    public void SelectWaveDropdown_Top_EnemySelector_EditLevel_Edit(int index)
    {
        //CHECK
        editorManager.SetSelectedWaveIndex(index);
    }

    public void AddPathBtn_Bottom_EnemySelector_EditLevel_Edit()
    {
        edit.editLevelPanel.SetActive(false);
        edit.createPathPanel.SetActive(true);

        //CHECK
        editorManager.CreateNewPath();
    }

    public void DeletePathBtn_Bottom_EnemySelector_EditLevel_Edit()
    {
        //CHECK
        editorManager.DeleteSelectedPath();
    }

    public void AddWaveBtn_Bottom_EnemySelector_EditLevel_Edit()
    {
        //CHECK
        editorManager.CreateNewWave();
    }

    public void DeleteWaveBtn_Bottom_EnemySelector_EditLevel_Edit()
    {
        //CHECK
        editorManager.DeleteSelectedWave();
    }
    #endregion

    void Start()
    {
        if (GameObject.FindObjectOfType<EditorManager>() != null)
        {
            editorManager = GameObject.FindObjectOfType<EditorManager>();
        }
    }
}
