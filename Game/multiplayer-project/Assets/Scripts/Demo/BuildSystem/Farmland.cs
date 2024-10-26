using UnityEngine;
using Milhouzer.Core.BuildSystem.ContextActions;
using Milhouzer.Core.BuildSystem;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Milhouzer.Game.BuildSystem
{
    /// <summary>
    /// IStateObject implementations should be stateless and rely only of static informations such as tables and update rules
    /// </summary>
    public class Farmland : MonoBehaviour, IStateObject, IContextActionHandler
    {
        readonly string _seedPattern = @"^FARMLAND\.([^.]+)\.";

        [HideInInspector]
        public string UID { get; set; }

        public void Destroy()
        {
            Destroy(gameObject);
        }

        public void PlantSeed(Seed seed)
        {
            BuildManager.Instance.RequestBuildUpdate(this, UID + "." + seed.Name);
        }

        public void WaterPlant() {
            List<string> splitStates = UID.Split('.').ToList();
            BuildManager.Instance.RequestBuildUpdate(this, GetNextWaterState(splitStates));
        }

        /// <summary>
        /// HarvestPlant removes the plant on the farmland.
        /// </summary>
        internal void HarvestPlant()
        {
            // string seed = FindElement(_seedPattern, UID);
            BuildManager.Instance.RequestBuildUpdate(this, "FARMLAND");
        }

        string GetNextWaterState(List<string> current) {
            if(!int.TryParse(current[^1], out int result)) {
                current.Add("1");
            }else{
                current[^1] = (result + 1).ToString();
            }

            return string.Join(".", current);
        }

        string FindElement(string pattern, string input){
            
			Regex regex = new Regex(pattern);
			Match match = regex.Match(input);
			if (match.Success) return match.Groups[1].Value;

            return "";
        }
    }
}
