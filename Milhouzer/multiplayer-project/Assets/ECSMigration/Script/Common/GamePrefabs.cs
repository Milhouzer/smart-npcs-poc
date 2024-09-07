using Unity.Entities;
using UnityEngine;

namespace Milhouzer.Netcode
{
    public struct GamePrefabs : IComponentData
    {
        // public Entity Champion;
        // public Entity Minion;
        // public Entity GameOverEntity;
        // public Entity RespawnEntity;
    }

    public class UIPrefabs : IComponentData
    {
        public GameObject HealthBar;
        public GameObject SkillShot;
    }
}