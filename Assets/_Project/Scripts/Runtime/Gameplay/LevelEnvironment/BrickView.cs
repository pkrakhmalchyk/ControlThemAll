using UnityEngine;

namespace ControllThemAll.Runtime.Gameplay
{
    [RequireComponent(typeof(Renderer))]
    [RequireComponent(typeof(Collider))]
    public class BrickView : MonoBehaviour
    {
        [SerializeField] private string gameplayLayer;

        private MaterialPropertyBlock materialPropertyBlock;
        private Renderer brickRenderer;

        private readonly int colorId = Shader.PropertyToID("_BaseColor");


        public string GameplayLayer => gameplayLayer;


        public void Initialize()
        {
            brickRenderer = GetComponent<Renderer>();
            materialPropertyBlock = new MaterialPropertyBlock();
        }

        public void SetColor(Color color)
        {
            materialPropertyBlock.SetColor(colorId, color);

            brickRenderer.SetPropertyBlock(materialPropertyBlock);
        }

        public void SetGameplayLayer(string gameplayLayer)
        {
            this.gameplayLayer = gameplayLayer;
        }

        public Bounds GetBounds()
        {
            return GetComponent<Collider>().bounds;
        }
    }
}
