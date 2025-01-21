using System.Collections;
using System.Collections.Generic;
using Game.Scripts.Infrastructure.Bootstrapper;
using PG;
using UnityEngine;

namespace Game.Scripts.LevelElements.Car
{
    public class CarWheelsTrailController
    {
        private CarVfxView _carVfxView;
        
        const float OffsetHitHeightForTrail = 0.05f;
        
        Queue<TrailRenderer> FreeTrails = new Queue<TrailRenderer>(); //Free trail pool
        private Dictionary<WheelView, TrailRenderer> ActiveTrails { get; set; }
        
        private WheelView[] _wheels;
        private CarView _carView;

        public CarWheelsTrailController(CarView carView)
        {
            _carView = carView;
            _carVfxView = carView.CarVfxView;
            _wheels = carView.Wheels;
            ActiveTrails = new Dictionary<WheelView, TrailRenderer> ();
            
            foreach (var wheel in _wheels)
            {
                ActiveTrails.Add (wheel, null);
            }
        }
        public void UpdateTrail (bool hasSlip)
        {
            foreach (var wheel in _wheels)
            {
                var trail = ActiveTrails[wheel];

                if (hasSlip)
                {
                    if (trail == null)
                    {
                        //Get free or create trail.
                        trail = GetTrail (wheel.transform.position + (wheel.transform.up * (-wheel.Radius + OffsetHitHeightForTrail)));
                        trail.transform.SetParent (wheel.transform);
                        ActiveTrails[wheel] = trail;
                    }
                    else
                    {
                        //Move the trail to the desired position
                        trail.transform.position = wheel.transform.position + (wheel.transform.up * (-wheel.Radius + OffsetHitHeightForTrail));
                    }
                }
                else if (ActiveTrails[wheel] != null)
                {
                    //Set trail as free.
                    Debug.Log("Set trail as free");
                    SetTrailAsFree (trail);
                    trail = null;
                    ActiveTrails[wheel] = trail;
                }
            }
        }
        void ResetAllTrails ()
        {
            TrailRenderer trail;
            for (int i = 0; i < _wheels.Length; i++)
            {
                trail = ActiveTrails[_wheels[i]];
                if (trail)
                {
                    SetTrailAsFree (trail);
                    trail = null;
                    ActiveTrails[_wheels[i]] = trail;
                }
            }
        }

        /// <summary>
        /// Get first free trail and set start position.
        /// </summary>
        public TrailRenderer GetTrail (Vector3 startPos)
        {
            TrailRenderer trail = null;
            if (FreeTrails.Count > 0)
            {
                trail = FreeTrails.Dequeue ();
            }
            else
            {
                trail = Object.Instantiate (_carVfxView.TrailRef, _carView.transform.parent);
            }

            trail.transform.position = startPos;
            trail.gameObject.SetActive (true);
            trail.Clear ();

            return trail;
        }

        /// <summary>
        /// Set trail as free and wait life time.
        /// </summary>
        public void SetTrailAsFree (TrailRenderer trail)
        {
            CoroutineRunner.Instance.StartCoroutine(WaitVisibleTrail (trail));
        }

        /// <summary>
        /// The trail is considered busy until it disappeared.
        /// </summary>
        private IEnumerator WaitVisibleTrail (TrailRenderer trail)
        {
            trail.transform.SetParent (_carView.transform.parent);
            yield return new WaitForSeconds (trail.time);
            trail.Clear ();
            trail.gameObject.SetActive (false);
            FreeTrails.Enqueue (trail);
        }
    }
}