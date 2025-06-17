using System.Collections.Generic;
using UnityEngine;

namespace ControllThemAll.Runtime.Gameplay
{
    [RequireComponent(typeof(ParticleSystem))]
    public class Vfx : MonoBehaviour
    {
        [SerializeField] private VfxType type;
        [SerializeField] private List<ParticleSystem> sizeChangingParts;
        [SerializeField] private List<ParticleSystem> colorChangingParts;

        private ParticleSystem root;
        private ParticleSystem[] parts;


        public VfxType Type => type;
        public float Duration => root.main.duration;


        public void Initialize()
        {
            root = GetComponent<ParticleSystem>();
            parts = GetComponentsInChildren<ParticleSystem>(true);
        }

        public void Play()
        {
            gameObject.SetActive(true);
            root.Play();
        }

        public void SetSimulationSpeed(float speed)
        {
            foreach (ParticleSystem part in parts)
            {
                ParticleSystem.MainModule main = part.main;

                main.simulationSpeed = speed;
            }
        }

        public void SetColor(Color color)
        {
            foreach (ParticleSystem part in colorChangingParts)
            {
                ParticleSystem.MainModule main = part.main;

                main.startColor = color;
            }
        }

        public void SetSize(float size)
        {
            foreach (ParticleSystem part in sizeChangingParts)
            {
                ParticleSystem.MainModule main = part.main;

                main.startSize = size;
            }
        }
    }
}