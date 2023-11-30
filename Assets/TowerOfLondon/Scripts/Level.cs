using UnityEngine;

namespace TowerOfLondon.Scripts {
    [CreateAssetMenu(fileName = "Level", menuName = "XR2Learn/Tower of London/Level", order = 1)]
    public class Level : ScriptableObject {
        public Vector3 redStart;
        public Vector3 greenStart;
        public Vector3 blueStart;

        public Vector3 redTarget;
        public Vector3 greenTarget;
        public Vector3 blueTarget;

        public int maxMoves;
        public int time;

        public int difficulty;
    }
}