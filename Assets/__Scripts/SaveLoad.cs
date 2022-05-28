using System.Collections.Generic;
using UnityEngine;

namespace __Scripts
{
    public class SaveLoad : MonoBehaviour
    {
        public static void SaveGameCells(IEnumerable<int> actualMapState, int minIndex, ref int saveGame)
        {
            saveGame = 1;
            PlayerPrefs.SetInt("saveGame", saveGame);
            PlayerPrefs.Save();
        
            var counter = minIndex;
            foreach (var cellVal in actualMapState)
            {
                PlayerPrefs.SetInt($"{counter}", cellVal);
                PlayerPrefs.Save();
                counter++;
            }
        }
    
        public static void SaveGameCells(List<int> previousMapState, int minIndex)
        {
            var counter = minIndex;
            foreach (var cellVal in previousMapState)
            {
                PlayerPrefs.SetInt($"{counter}", cellVal);
                PlayerPrefs.Save();
                counter++;
            }
        }

        private static void LoadGameCells(int minIndex, int maxIndex, List<int> mapState)
        {
            mapState.Clear();
            for (var i = minIndex; i <= maxIndex; i++)
            {
                mapState.Add(PlayerPrefs.GetInt($"{i}"));
            }
        }

        public static void LoadGame(int minIndex, int maxIndex, GameObject fillPrefab, Cell2048[] allCells, List<int> mapState)
        {
            LoadGameCells(minIndex, maxIndex, mapState);
            var whichSpawnIndex = 0;
            foreach (var cellsVal in mapState)
            {
                if (cellsVal != 0)
                {
                    var tempFill = Instantiate(fillPrefab, allCells[whichSpawnIndex].transform);
                    var tempFillComp = tempFill.GetComponent<Fill2048>();
                    allCells[whichSpawnIndex].GetComponent<Cell2048>().fill = tempFillComp;
                    tempFillComp.FillValueUpdate(cellsVal);
                    whichSpawnIndex++;
                }

                else
                {
                    whichSpawnIndex++;
                }
            }
        }
    }
}
