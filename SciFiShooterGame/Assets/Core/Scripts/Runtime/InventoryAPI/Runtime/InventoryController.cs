using System.Collections.Generic;
using System.Linq;
using Modules.InventoryAPI.Runtime;
using UnityEngine;

namespace Core.Scripts.Runtime.InventoryAPI.Runtime
{
    public class InventoryController : MonoBehaviour
    {
        public List<Panel> PanelList = new List<Panel>();

        private void Start()
        {
            PanelList ??= GetComponents<Panel>().ToList();
        }
    }
}