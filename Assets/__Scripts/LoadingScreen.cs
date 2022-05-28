using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace __Scripts
{
    public class LoadingScreen : MonoBehaviour
    {
    
        public GameObject loadingCircle;
        public float circleRotationsPerSecond = 0.1f;
        public Slider bar;

        private Quaternion _loadingCircleRotation;
    
        private void Awake()
        {
            _loadingCircleRotation = loadingCircle.transform.rotation;
        }

        private void Start()
        {
            LoadScene();
        }

        private void Update()
        {
            RollPreloader();
        }

        private void RollPreloader()
        {
            float rotationZ = -(circleRotationsPerSecond * Time.time * 360) % 360f;
            _loadingCircleRotation = Quaternion.Euler(0, 0, rotationZ);
            loadingCircle.transform.rotation = _loadingCircleRotation;
        }

        private void LoadScene()
        {
            StartCoroutine(LoadAsync());
        }

        IEnumerator LoadAsync()
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainScene");

            while (!asyncLoad.isDone)
            {
                bar.value = asyncLoad.progress;
                yield return null;
            }
        }
    }
}
