using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace __Scripts
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance;
        public static int Ticker;
        public static Action<string> Slide;
    
        [HideInInspector]public bool hasMoved;
        [HideInInspector]public bool hasMovedWithScoreChange;

        [Header("Set in Inspector")]public Color[] fillColors;
        [SerializeField] private GameObject fillPrefab;
        [SerializeField] private Cell2048[] allCells;

        [SerializeField] private Text scoreDisplay;
        [SerializeField] private Text recordDisplay;
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private GameObject menuPanel;
        [SerializeField] private GameObject winningPanel;
        [SerializeField] private int winningScore;

        private Cell2048 _cell2048;
    
        private readonly List<int> _actualMapState = new List<int>();
        private List<int> _previousMapState = new List<int>();

        private const int MinActualMapStateIndex = 0;
        private const int MaxActualMapStateIndex = 15;
        private const int MinPreviousMapStateIndex = 16;
        private const int MaxPreviousMapStateIndex = 31;
    
        private static bool _hasWon;
        private static bool _freeze;
        private static bool _isMoveBack;
        private static bool _saveOldScore;
        private static int _saveGame;
        private static int _saveActualScore;
        private static int _winScore;
        private static int _isGameOver;
        private int _score;
    
        private void OnEnable()
        {
            if (Instance == null)
                Instance = this;
        }
    
        public void Start()
        {
            menuPanel.SetActive(false);
            _freeze = false;
            _saveGame = PlayerPrefs.GetInt("saveGame");

            if (_saveGame == 0 && !_isMoveBack)
            {
                StartSpawnFill();
                StartSpawnFill();
            }

            if (_saveGame == 1 && !_isMoveBack)
            {
                SaveLoad.LoadGame(MinActualMapStateIndex, MaxActualMapStateIndex, fillPrefab, allCells, _actualMapState);
            }
        
            _score = PlayerPrefs.GetInt("score");
            scoreDisplay.text = _score.ToString();
            var record = PlayerPrefs.GetInt("saveRecord");
            recordDisplay.text = record.ToString();
        }

        private void Update()
        {
            if (!_freeze)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    StandardUpdateValues("left");
                }

                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    StandardUpdateValues("right");
                }

                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    StandardUpdateValues("up");
                }

                if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    StandardUpdateValues("down");
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    MoveBack();
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CallMenuPanel();
            }
        }

        private void StandardUpdateValues(string direction)
        {
            Ticker = 0;
            _isGameOver = 0;
            _saveOldScore = false;
            Slide(direction);
            UpdateValuesOfCells();
            SaveLoad.SaveGameCells(_actualMapState, MinActualMapStateIndex, ref _saveGame);
        }
    
        private bool CheckEmptyCells()
        {
            return allCells.Any(t => t.transform.childCount == 0);
        }

        private void DeleteChild()
        {
            foreach (var cell in allCells)
            {
                if (cell.transform.childCount == 0) continue;
                var child = cell.transform.GetChild(0).gameObject;
                Destroy(child);
            }
        }

        private void UpdateValuesOfCells()
        {
            try
            {
                if (hasMoved)
                {
                    _isMoveBack = false;
                    _previousMapState = _actualMapState;
                    SaveLoad.SaveGameCells(_previousMapState, MinPreviousMapStateIndex);

                    if (!hasMovedWithScoreChange)
                    {
                        var oldScore = _score;
                        PlayerPrefs.SetInt("oldScore", oldScore);
                    }
                }

                _actualMapState.Clear();
                foreach (var t in allCells)
                {
                    if (t.transform.childCount == 0)
                        _actualMapState.Add(0);
                    else
                        _actualMapState.Add(t.fill.value);
                }
                hasMoved = false;
                hasMovedWithScoreChange = false;
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public void SpawnFill()
        {
            while (true)
            {
                if (_freeze || !hasMoved || !CheckEmptyCells()) return;
                var whichSpawnIndex = Random.Range(0, allCells.Length);
                if (allCells[whichSpawnIndex].transform.childCount != 0)
                {
                    continue;
                }

                var chance = Random.Range(0f, 1f);
                if (chance > .8f)
                {
                    var tempFill = Instantiate(fillPrefab, allCells[whichSpawnIndex].transform);
                    var tempFillComp = tempFill.GetComponent<Fill2048>();
                    allCells[whichSpawnIndex].GetComponent<Cell2048>().fill = tempFillComp;
                    tempFillComp.FillValueUpdate(4);
                }
                else
                {
                    var tempFill = Instantiate(fillPrefab, allCells[whichSpawnIndex].transform);
                    var tempFillComp = tempFill.GetComponent<Fill2048>();
                    allCells[whichSpawnIndex].GetComponent<Cell2048>().fill = tempFillComp;
                    tempFillComp.FillValueUpdate(2);
                }

                break;
            }
        }
    
        private void StartSpawnFill()
        {
            var whichSpawnIndex = Random.Range(0, allCells.Length);
            if (allCells[whichSpawnIndex].transform.childCount != 0)
            {
                StartSpawnFill();
            }
            var tempFill = Instantiate(fillPrefab, allCells[whichSpawnIndex].transform);
            var tempFillComp = tempFill.GetComponent<Fill2048>();
            allCells[whichSpawnIndex].GetComponent<Cell2048>().fill = tempFillComp;
            tempFillComp.FillValueUpdate(2);
        
        }

        public void ScoreUpdate(int scoreIn)
        {
            if (!_saveOldScore)
            {
                var oldScore = _score;
                PlayerPrefs.SetInt("oldScore", oldScore);
                _saveOldScore = true;
            }
        
            _score += scoreIn;
            scoreDisplay.text = _score.ToString();
            PlayerPrefs.SetInt("score", _score);
        
            var record = PlayerPrefs.GetInt("saveRecord");
            if (record < _score)
            {
                PlayerPrefs.SetInt("saveRecord", _score);
                recordDisplay.text = _score.ToString();
                return;
            }
            recordDisplay.text = record.ToString();
        }

        public void GameOverCheck()
        {
            _isGameOver++;
            if (_isGameOver < 16) return;
            gameOverPanel.SetActive(true);
            _freeze = true;
            _winScore = 0;
            _saveGame = 0;
            PlayerPrefs.SetInt("saveGame", _saveGame);
            PlayerPrefs.SetInt("score", _saveGame);
            PlayerPrefs.SetInt("_winScore", _winScore);
            PlayerPrefs.Save();
        
        }

        public void MoveBack()
        {
            _freeze = false;
            _isMoveBack = true;
            DeleteChild();
            SaveLoad.LoadGame(MinPreviousMapStateIndex, MaxPreviousMapStateIndex, fillPrefab, allCells, _previousMapState);
            _score = PlayerPrefs.GetInt("oldScore");
            scoreDisplay.text = _score.ToString();
            winningPanel.SetActive(false);
            gameOverPanel.SetActive(false);
        }
    
        public void Restart()
        {
            SceneManager.LoadScene("MainScene");
            _saveGame = 0;
            _winScore = 0;
            PlayerPrefs.SetInt("saveGame", _saveGame);
            PlayerPrefs.SetInt("score", _saveGame);
            PlayerPrefs.SetInt("_winScore", _winScore);
            PlayerPrefs.Save();
            _freeze = false;
            _hasWon = false;
            _isMoveBack = false;
        }

        public void WinningCheck(int highestFill)
        {
            if (_score > 37000)
                return;

            _winScore = PlayerPrefs.GetInt("_winScore");
            if (_winScore == 1)
                _hasWon = true;
            if (_hasWon)
                return;

            if (highestFill != winningScore) return;
            _winScore = 1;
            PlayerPrefs.SetInt("_winScore", _winScore);
            PlayerPrefs.Save();
            winningPanel.SetActive(true);
            _hasWon = true;
            _freeze = true;
        }

        public void KeepPlaying()
        {
            winningPanel.SetActive(false);
            _freeze = false;
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        public void CallMenuPanel()
        {
            UpdateValuesOfCells();
            SaveLoad.SaveGameCells(_actualMapState, MinActualMapStateIndex, ref _saveGame);
            if (!menuPanel.activeSelf)
            {
                menuPanel.SetActive(true);
                _freeze = true;
            }
            else
            {
                menuPanel.SetActive(false);
                _freeze = false;
            }
        
        }
    }
}