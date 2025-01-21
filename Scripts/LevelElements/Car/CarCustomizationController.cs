using System;
using UnityEngine;

namespace Game.Scripts.LevelElements.Car
{
    public class CarCustomizationController : MonoBehaviour
    {
        [SerializeField] private CarCustomizationMesh[] _carCustomizationMeshes;

        public void SetCarMaterial(Material newMaterial)
        {
            foreach (var carCustomizationMesh in _carCustomizationMeshes)
            {
                Material[] materials = carCustomizationMesh.Renderer.materials;
                var materialIndex = carCustomizationMesh.MaterialIndex;
                
                if (materialIndex >= 0 && materialIndex < materials.Length)
                {
                    // Заменяем материал по индексу
                    materials[materialIndex] = newMaterial;

                    // Применяем изменения
                    carCustomizationMesh.Renderer.materials = materials;
                }
            }

        }
    }

    [Serializable]
    public class CarCustomizationMesh
    {
        public MeshRenderer Renderer;
        public int MaterialIndex;
    }
}