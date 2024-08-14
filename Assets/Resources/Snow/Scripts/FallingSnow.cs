using System.Collections.Generic;
using UnityEngine;

namespace Rito
{
    public class FallingSnow : MonoBehaviour
    {
        private ParticleSystem ps;
        private List<ParticleCollisionEvent> colEventList;
        private TerrainSnowPainter terrainSnowPainter;

        [SerializeField]
        private float snowAmount = 0.01f;
        [SerializeField]
        private float snowRadius = 1f;

        private void Awake()
        {
            ps = GetComponent<ParticleSystem>();
            colEventList = new List<ParticleCollisionEvent>(100);
            terrainSnowPainter = FindObjectOfType<TerrainSnowPainter>();
        }

        private void OnParticleCollision(GameObject other)
        {
            // 터레인에 눈 쌓기
            if (terrainSnowPainter != null && other.GetComponent<Terrain>() != null)
            {
                int numColEvents = ps.GetCollisionEvents(other, colEventList);
                for (int i = 0; i < numColEvents; i++)
                {
                    terrainSnowPainter.AddSnow(colEventList[i].intersection, snowRadius, snowAmount);
                }
            }
        }
    }
}

/*using System.Collections.Generic;
using UnityEngine;

namespace Rito
{
    /// <summary> 
    /// 파티클 - 바닥에 눈 쌓기
    /// </summary>
    public class FallingSnow : MonoBehaviour
    {
        private ParticleSystem ps;
        private List<ParticleCollisionEvent> colEventList;
        private GameObject cachedTargetGO;
        private GroundSnowPainter snowPainter;
        private TerrainSnowPainter terrainSnowPainter;

        [SerializeField]
        private float snowAmount = 0.01f;
        [SerializeField]
        private float snowRadius = 1f;

        private void Awake()
        {
            ps = GetComponent<ParticleSystem>();
            colEventList = new List<ParticleCollisionEvent>(100);
            terrainSnowPainter = FindObjectOfType<TerrainSnowPainter>();
        }

        private void OnParticleCollision(GameObject other)
        {
            if (other.GetComponent<Terrain>() != null)
            {
                // 터레인에 눈 쌓기
                if (terrainSnowPainter != null)
                {
                    int numColEvents = ps.GetCollisionEvents(other, colEventList);
                    for (int i = 0; i < numColEvents; i++)
                    {
                        terrainSnowPainter.AddSnow(colEventList[i].intersection, snowRadius, snowAmount);
                    }
                }
            }
            else
            {
                // 일반 오브젝트에 눈 쌓기
                if (other != cachedTargetGO)
                {
                    cachedTargetGO = other;
                    snowPainter = other.GetComponent<GroundSnowPainter>();
                }

                if (snowPainter == null || !snowPainter.isActiveAndEnabled)
                    return;

                int numColEvents = ps.GetCollisionEvents(other, colEventList);
                for (int i = 0; i < numColEvents; i++)
                {
                    snowPainter.PileSnow(colEventList[i].intersection);
                }
            }
        }
    }
}*/