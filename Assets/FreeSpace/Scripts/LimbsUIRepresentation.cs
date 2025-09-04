using System.Collections;
using System.Collections.Generic;
using Base;
using TMPro;
using Units.Health;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FreeSpace.Scripts
{
    public class LimbsUIRepresentation : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;
        
        private Dictionary<LimbType, ImageBar> _limbs = new ();

        private List<LimbType> _loadingLimbs = new ();

        private void Start()
        {
            _label.text = Localization.UILimbsPanelName;
        }

        public void UpdateUI(Dictionary<LimbType, float> limbs)
        {
            foreach (var limbType in limbs.Keys)
            {
                if (!_limbs.ContainsKey(limbType))
                    StartCoroutine(CreateImageBarFor(limbType, limbs[limbType]));
                else
                    _limbs[limbType].UpdateUI(Localization.LimbsNames[limbType], limbs[limbType]);
            }
        }

        private IEnumerator CreateImageBarFor(LimbType limbType, float limb, float timeOut = 30)
        {
            if (_loadingLimbs.Contains(limbType))
                yield break;
            
            _loadingLimbs.Add(limbType);
            var ap = Addressables.InstantiateAsync(AddressablesNames.UIImageBar);
            while (!ap.IsDone && timeOut > 0)
            {
                timeOut -= Time.deltaTime;
                yield return null;
            }

            if (ap.IsDone)
            {
                var res = ap.Result.GetComponent<ImageBar>();
                res.transform.SetParent(transform);
                res.transform.localScale = Vector3.one;
                res.UpdateUI(Localization.LimbsNames[limbType], limb);
                _limbs.TryAdd(limbType, res);
            }
            
            _loadingLimbs.Remove(limbType);
        }
    }
}