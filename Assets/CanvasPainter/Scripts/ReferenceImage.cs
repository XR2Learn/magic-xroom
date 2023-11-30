using UnityEngine;

namespace CanvasPainter
{
    [CreateAssetMenu(fileName = "ReferenceImage", menuName = "ScriptableObjects/CanvasPainter/Reference Image", order = 1)]
    public class ReferenceImage : ScriptableObject
    {
        [SerializeField] private Texture2D image;
        [SerializeField] private int brushSize;
        [SerializeField] private int timeLimit;
        [SerializeField] private int foulLimit;
        [SerializeField] private int coverageTarget;

        public Texture2D Image => image;
        public int BrushSize => brushSize;
        public int TimeLimit => timeLimit;
        public int FoulLimit => foulLimit;
        public int CoverageTarget => coverageTarget;
    }
}