using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace RVP {
    [AddComponentMenu("RVP/Demo Scripts/Performance Stats", 2)]

    // Class for displaying the framerate
    public class PerformanceStats : MonoBehaviour {
        public Text fpsText;
        private float fpsUpdateTime;
        private int frames;

        private void Update() {
            fpsUpdateTime = Mathf.Max(0, fpsUpdateTime - Time.deltaTime);

            if (fpsUpdateTime == 0) {
                fpsText.text = $"FPS: {frames}";
                fpsUpdateTime = 1;
                frames = 0;
            }
            else {
                frames++;
            }
        }

        public void Restart() {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            Time.timeScale = 1;
        }
    }
}