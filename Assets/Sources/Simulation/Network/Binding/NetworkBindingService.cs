using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using R3;
using UnityEngine;

namespace KickinIt.Simulation.Synchronization
{
    public class NetworkBindingService : IDisposable
    {
        private static readonly List<Event> ScheduledEvents = new(16);
        
        public static bool Active { get; private set; }
        
        public static void PushSpawned(Type type, NetworkBinder component)
        {
            ScheduledEvents.Add(new Event { Type = type, Component = component });
        }
        
        private class Event
        {
            public Type Type { get; set; }
            public NetworkBinder Component { get; set; }
        }

        private abstract class Subscription
        {
            public abstract void Invoke(NetworkBinder component);
        }
        
        private class Subscription<TComponent> : Subscription where TComponent : NetworkBinder
        {
            public Subject<TComponent> ReactiveSubject { get; set; }
            
            public override void Invoke(NetworkBinder component)
            {
                ReactiveSubject.OnNext((TComponent)component);
            }
        }
        
        private readonly Dictionary<Type, Subscription> _subscriptions = new();
        
        private CancellationTokenSource _cts;
        private IDisposable _hookSubscription;

        public NetworkBindingService()
        {
            if (Active)
            {
                throw new InvalidOperationException("Another NetworkBindingService is already active.");
            }

            Active = true;
        }

        public void Dispose()
        {
            _hookSubscription?.Dispose();
            _cts?.Cancel();
            ScheduledEvents.Clear();
            Active = false;
        }

        public void Attach(EarlyNetworkUpdateHook hook)
        {
            _hookSubscription?.Dispose(); // reset subscription just in case

            _hookSubscription = hook.EarlyNetworkUpdate
                .Subscribe(_ => FlushScheduledEvents());
        }

        public Observable<TComponent> Track<TComponent>() 
            where TComponent : NetworkBinder
        {
            var type = typeof(TComponent);
            
            if (_subscriptions.TryGetValue(type, out var subscription))
            {
                return ((Subscription<TComponent>)subscription).ReactiveSubject;
            }
            
            var newSubscription = new Subscription<TComponent>
            {
                ReactiveSubject = new Subject<TComponent>()
            };
            
            _subscriptions[type] = newSubscription;
            
            return newSubscription.ReactiveSubject;
        }

        private void FlushScheduledEvents()
        {
            if (ScheduledEvents.Count == 0) return;
                
            var orderedEvents = ScheduledEvents.OrderBy(entry => entry.Component.DispatchOrder);
                
            foreach (var scheduledEntry in orderedEvents)
            {
                var type = scheduledEntry.Type;
                    
                if (!_subscriptions.TryGetValue(type, out var subscription)) continue;

                try
                {
                    subscription.Invoke(scheduledEntry.Component);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
            }
                
            ScheduledEvents.Clear();
        }
    }
}