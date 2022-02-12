using System.Collections.Generic;
using System.Linq;
using GUI.Actors.Hero.Stats;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GUI.Physics.Canvas
{
    public class CanvasRaycaster : MonoBehaviour
    {
        public Image loadingBar;
        
        private GraphicRaycaster _raycaster;

        private PointerEventData _pointerEventData;

        private EventSystem _eventSystem;
        
        private float _mouseDownCount;
        
        private void Start()
        {
            _raycaster = GetComponent<GraphicRaycaster>();
            _eventSystem = GetComponent<EventSystem>();
        }

        private void Update()
        {
            if (Input.GetMouseButtonUp(0))
                ResetProgressBar();
            
            if (Input.GetMouseButton(0))
            {
                _pointerEventData = new PointerEventData(_eventSystem) {position = Input.mousePosition};

                var results = new List<RaycastResult>();
                _raycaster.Raycast(_pointerEventData, results);

                foreach (var result in results.Where(result => result.gameObject.CompareTag("HeroOption") && result.gameObject.GetComponent<Button>().interactable))
                {
                    _mouseDownCount += Time.deltaTime;
                    loadingBar.fillAmount = _mouseDownCount / 2f;
                    if (_mouseDownCount >= 2f)
                    {
                        HeroSelectionStats.Instance.ShowHeroStatistics(result.gameObject.GetComponent<HeroSelectionStats>());
                        ResetProgressBar();
                    }
                }
            }
        }
        
        private void ResetProgressBar()
        {
            _mouseDownCount = 0f;
            loadingBar.fillAmount = _mouseDownCount;
        }
    }
}
