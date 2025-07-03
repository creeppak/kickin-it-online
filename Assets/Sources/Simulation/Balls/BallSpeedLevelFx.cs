using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using R3;
using UnityEngine;
using UnityEngine.Events;
using VContainer;

namespace KickinIt.Simulation.Balls
{
    internal class BallSpeedLevelFx : MonoBehaviour
    {
        [Serializable]
        public class SpeedLevelConfig
        {
            public int stepThreshold;
            public ParticleSystem particles;
            public UnityEvent onTriggered;
        }
        
        [SerializeField] private List<SpeedLevelConfig> levelConfigs;
        [SerializeField] private float minSpeedToShowTrail = 10f;
        
        private BallMovement _ballMovement;
        private List<SpeedLevelConfig> _sortedLevelConfigs;
        private SpeedLevelConfig _activeConfig;

        [Inject]
        private void Construct(BallMovement ballMovement)
        {
            _ballMovement = ballMovement;
        }

        private void Awake()
        {
            _sortedLevelConfigs = levelConfigs
                .OrderBy(config => config.stepThreshold)
                .ToList();
        }

        private void Start()
        {
            _ballMovement.MaxSpeedStepObservable
                .Subscribe(step =>
                {
                    var targetConfig = FetchTargetConfig(step);

                    if (targetConfig == _activeConfig)
                    {
                        return;
                    }

                    ActivateConfig(targetConfig);
                })
                .AddTo(this);
        }

        private void Update()
        {
            if (_activeConfig == null)
            {
                return;
            }
            
            var shouldEmit = _ballMovement.Velocity.magnitude > minSpeedToShowTrail;
            var fx = _activeConfig.particles;
            
            if (shouldEmit && !fx.isPlaying)
            {
                fx.Play();
            }
            else if (!shouldEmit && fx.isPlaying)
            {
                fx.Stop();
            }
        }

        private void ActivateConfig(SpeedLevelConfig config)
        {
            if (_activeConfig != null)
            {
                _activeConfig.particles.Stop();
            }
            
            _activeConfig = config;
            
            if (_activeConfig != null)
            {
                _activeConfig.particles.Play();
                _activeConfig.onTriggered.Invoke();
            }
        }

        [CanBeNull]
        private SpeedLevelConfig FetchTargetConfig(int step)
        {
            SpeedLevelConfig config = null;

            foreach (var iConfig in _sortedLevelConfigs)
            {
                if (step < iConfig.stepThreshold)
                {
                    break;
                }
                
                config = iConfig;
            }

            return config;
        }
    }
}