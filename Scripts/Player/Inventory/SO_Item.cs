using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    [CreateAssetMenu(menuName = "Inventory Item Data")]
    public class SO_Item : ScriptableObject
    {
        public string id;
        public new string name;
        public bool is_holdable;
        public bool placeable;
        public Sprite inventory_icon;
        public GameObject item_prefab;

        public bool deals_damage;
        public float damage;
        public float range_scaling;
        public bool is_ranged;
        public float attack_delay;
        public bool follow_camera;
    }
}
