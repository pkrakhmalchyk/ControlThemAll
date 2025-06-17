using System;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ControllThemAll.Runtime.Gameplay
{
    public class Ability : IDisposable
    {
        private readonly List<IAbilityCondition> conditions;
        private readonly List<IAbilityComponent> components;

        private CancellationTokenSource activityCts;
        private bool isFulfilled = false;


        public Ability(
            IEnumerable<IAbilityCondition> conditions,
            IEnumerable<IAbilityComponent> components)
        {
            this.conditions = new List<IAbilityCondition>(conditions.Count());
            this.components = new List<IAbilityComponent>(components.Count());

            foreach (IAbilityCondition condition in conditions)
            {
                this.conditions.Add(condition);
            }

            foreach (IAbilityComponent component in components)
            {
                this.components.Add(component);
            }
        }

        public void SetActive(bool active)
        {
            if (active)
            {
                activityCts = new CancellationTokenSource();

                ExecuteIfFulfilled(activityCts.Token).Forget();
            }
            else
            {
                activityCts?.Cancel();
                activityCts?.Dispose();

                activityCts = null;
            }
        }

        public void Dispose()
        {
            foreach (IAbilityComponent component in components)
            {
                if (component is IDisposable disposableComponent)
                {
                    disposableComponent.Dispose();
                }
            }

            foreach (IAbilityCondition condition in conditions)
            {
                if (condition is IDisposable disposableCondition)
                {
                    disposableCondition.Dispose();
                }
            }

            activityCts?.Cancel();
            activityCts?.Dispose();

            activityCts = null;
        }


        private async UniTask ExecuteIfFulfilled(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                bool areAllConditionsFulfilled = AreAllConditionsFulfilled();

                if (!isFulfilled && areAllConditionsFulfilled)
                {
                    isFulfilled = true;

                    ExecuteAllComponents(true);
                }
                else if (isFulfilled && !areAllConditionsFulfilled)
                {
                    isFulfilled = false;

                    ExecuteAllComponents(false);
                }

                await UniTask.Yield(cancellationToken);
            }

            ExecuteAllComponents(false);
        }

        private bool AreAllConditionsFulfilled()
        {
            return conditions.All(condition => condition.IsFulfilled());
        }

        private void ExecuteAllComponents(bool execute)
        {
            foreach (IAbilityComponent component in components)
            {
                component.Execute(execute);
            }
        }
    }
}