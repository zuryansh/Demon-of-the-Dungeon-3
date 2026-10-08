using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using EditorAttributes;
using static UnityEngine.RuleTile.TilingRuleOutput;

//TODO: Make Melle Attack behavior, repeat untill behavior, make a parent WrapperBehavior for random and conditional behaviors, make generic event behavior;

namespace BossArchitecture {

    [Serializable]
    public abstract class BossPhaseBehavior
    {
        public abstract IEnumerator Execute(Boss boss);
        public virtual void Tick(Boss boss) { }
        public virtual void NotifyHit(Boss boss, Collider2D collider, Vector3 dir) { }
    }

    #region Attack Behaviors

    [Serializable]
    public class BossAttackBehavior : BossPhaseBehavior
    {
        protected ConditionAttackRuntime runtimeATK;

        public override IEnumerator Execute(Boss boss)
        {
            Debug.Log("Generic Boss attack executed");
            yield return new WaitForEndOfFrame();
        }

        public override void Tick(Boss boss)
        {
            if (runtimeATK != null)
            {
                runtimeATK.Tick();
            }
        }

        public override void NotifyHit(Boss boss, Collider2D collider, Vector3 dir)
        {
            if (runtimeATK == null) return;
            Vector3 p = collider.ClosestPoint(boss.transform.position);

            EffectContext context = new(boss.gameObject, collider.gameObject, p, dir);

            foreach (Effect effect in GetAttackData().OnTargetHitEffects)
                effect.Apply(context);


        }

        protected void StartAnimAndEffects(Boss boss, AttackData attackData)
        {
            boss.AnimHelper.ChangeAnimation(attackData.AttackAnimation, priority: attackData.AnimationPriority, forceReplay: true);

            Vector2 effectPos = boss.transform.position;
            EffectContext context = new EffectContext(boss.gameObject, boss.Player.gameObject, effectPos, (Vector3)boss.DirToPlayer);
            foreach (var effect in attackData.OnAttackStartEffects) effect.Apply(context);



        }

        protected virtual void PrepareAttack(Boss boss, AttackData attackData, Func<bool> endFunc)
        {
            runtimeATK = new ConditionAttackRuntime(attackData, endFunc);
            boss.SetStartAttackTime(Time.time);
        }

        protected virtual void OnAttackFinish(Boss boss, ConditionAttackRuntime currentAttack)
        {

            EffectContext context = new EffectContext(boss.gameObject, null, boss.transform.position, (Vector3)boss.DirToPlayer);
            foreach (var effect in currentAttack.Data.OnAttackEndEffects) effect.Apply(context);

            boss.AnimHelper.ChangeAnimation(boss.Data.IdleAnim);
            runtimeATK = null;


        }

        public void Telegraph(Boss boss, AttackData attackData, Vector2 spawnPos, Vector2 direction)
        {
            EffectContext context = new EffectContext(boss.gameObject, boss.Player.gameObject, spawnPos, (Vector3)direction);
            foreach (TelegraphEffect effect in attackData.TelegraphEffects)
            {
                effect.Apply(context);
            }
        }

        protected virtual AttackData GetAttackData() { Debug.LogWarning("DATA REFERENCED ON GENERIC ATTACK BEHAVIOR"); return null; }
    }

    [Serializable]
    public class ChargeAttackBehavior : BossAttackBehavior
    {
        [SerializeField] ChargeAttackData data;

        protected override AttackData GetAttackData()
        {
            return data;
        }

        public override IEnumerator Execute(Boss boss)
        {


            Vector2 chargeDir = boss.DirToPlayer;


            float timeToWait = data.MaxChargeTime + data.TelegraphTime;

            PrepareAttack(boss, data, () => Time.time - boss.AttackStartTime >= timeToWait);

            boss.EWallHit += runtimeATK.SignalCompletion; //attack will complete when it hits a wall

            //telegraph
            Telegraph(boss, data, boss.transform.position, boss.DirToPlayer);
            yield return new WaitForSeconds(data.TelegraphTime);

            StartAnimAndEffects(boss, data);

            //charge
            boss.SetRBVelocity(10 * data.ChargeSpeed * chargeDir);


            yield return new WaitUntil(() => runtimeATK.IsFinished);


            boss.SetRBVelocity(Vector2.zero);
            boss.EWallHit -= runtimeATK.SignalCompletion;
            OnAttackFinish(boss, runtimeATK);

        }
    }

    [Serializable]
    public class JumpAttackBehavior : BossAttackBehavior
    {
        [SerializeField] JumpAttackData data;

        protected override AttackData GetAttackData()
        {
            return data;
        }

        public override IEnumerator Execute(Boss boss)
        {


            float timeToWait = data.JumpTime + data.StartLeapAfter + 0.1f + data.TelegraphTime;
            PrepareAttack(boss, data, () => Time.time - boss.AttackStartTime >= timeToWait);

            Vector2 endPos = boss.Player.transform.position - boss.transform.position;
            endPos = Vector2.ClampMagnitude(endPos, data.MaxJumpDist) + boss.transform.position.ToV2();

            //telegraph
            Telegraph(boss, data, endPos, boss.DirToPlayer);
            yield return new WaitForSeconds(data.TelegraphTime);

            StartAnimAndEffects(boss, data);

            yield return new WaitForSeconds(data.StartLeapAfter); //wait for animation to get to the jump point 

            boss.RB.DOJump(endPos, data.JumpPower, 1, data.JumpTime).SetEase(data.EaseType)
                .OnComplete(() => boss.AnimHelper.ChangeAnimation(data.LandAnim));


            yield return new WaitUntil(() => runtimeATK.IsFinished);
            OnAttackFinish(boss, runtimeATK);
        }



    }

    [Serializable]
    public class ShootAttackBehavior : BossAttackBehavior
    {
        [SerializeField] ProjectileAttackData data;

        public override IEnumerator Execute(Boss boss)
        {

            int spawnedProjectiles = 0;
            PrepareAttack(boss, data, () => spawnedProjectiles >= data.NoOfProj);
            float randomOffset = UnityEngine.Random.Range(-data.RandomAngleOffset, data.RandomAngleOffset);
            Vector2 dirToFire = Quaternion.Euler(0, 0, randomOffset) * boss.DirToPlayer;

            //telegraph
            Telegraph(boss, data, boss.transform.position, dirToFire);
            yield return new WaitForSeconds(data.TelegraphTime);

            StartAnimAndEffects(boss, data);

            for (int i = 0; i < data.NoOfProj; i++)
            {

                Vector2 dir = GetSpreadDir(dirToFire, i, data.CoverAngle, data.NoOfProj, 0);// random offset is applied to dir to fire;
                Vector2 spawnPos = boss.transform.position;

                Quaternion spawnRot = Quaternion.FromToRotation(data.ProjectilePrefab.transform.right, dir);
                //Quaternion spawnRot = Quaternion.identity;
                Projectile proj = MonoBehaviour.Instantiate(data.ProjectilePrefab, spawnPos, spawnRot);
                proj.Sender = boss.transform;
                proj.Launch(dir * data.ProjSpeed);

                spawnedProjectiles++;
                if (data.TimeBetweenProj > 0f) yield return new WaitForSeconds(data.TimeBetweenProj);

            }

            Vector3 GetSpreadDir(Vector2 forward, int index, float angle, int count, float offset)
            {
                if (count == 1) return forward.normalized;

                float angleStep = Mathf.Approximately(angle, 360f) ? angle / count : angle / (count - 1); //centers it for non 360 angles

                return Quaternion.Euler(0, 0, angleStep * index + offset - angle / 2) * forward;

            }

            yield return new WaitUntil(() => runtimeATK.IsFinished);

            OnAttackFinish(boss, runtimeATK);

        }

        protected override AttackData GetAttackData()
        {
            return data;
        }

    }

    [Serializable]
    public class SummonMinionsBehavior : BossAttackBehavior
    {
        [SerializeField] SummonAttackData data;
        [SerializeField] float timeBetweenSummons=0.1f;
        [SerializeField] bool waitUntilAllMinionsDead=false;

        protected override AttackData GetAttackData()
        {
            return data;
        }

        public override IEnumerator Execute(Boss boss)
        {
            int minionCount = data.GetRandomCount();
            float timeToWait = minionCount * data.TelegraphTime + (minionCount - 1) * timeBetweenSummons;


            PrepareAttack(boss, data, () => Time.time - boss.AttackStartTime >= timeToWait);

            StartAnimAndEffects(boss, data);

            //summon
            for (int i = 0; i < minionCount; i++)
            {
                Vector2 spawnPos = GetSpawnPos(boss);
                //telegraph
                Telegraph(boss, data, spawnPos, boss.DirToPlayer);
                yield return new WaitForSeconds(data.TelegraphTime);

                Spawn(boss, spawnPos, data.GetRandomMinion());
                if (i < minionCount - 1)
                    yield return new WaitForSeconds(timeBetweenSummons);
            }

            if (waitUntilAllMinionsDead)
            {
                yield return new WaitUntil(() => (boss.AllMinionsDead())); //runtimr atk condition is not neeede since the attack will finish after summoning 
            }

            OnAttackFinish(boss, runtimeATK);

        }

        Vector2 GetSpawnPos(Boss boss)
        {
            //make it avoid the boss itself
            Vector2 spawnCenter = (data.HasGlobalPosition) ? data.GlobalSpawnPosition : boss.transform.position;
            return spawnCenter +
                   (UnityEngine.Random.insideUnitCircle.normalized * (UnityEngine.Random.Range(data.SpawnBounds.x,data.SpawnBounds.y)));
        }

        void Spawn(Boss boss, Vector2 spawnPoint, EnemyBrain minion)
        {


            EnemyBrain enemy = MonoBehaviour.Instantiate(minion, spawnPoint, Quaternion.identity);

            //enemy.SetSpawner(Brain.ParentSpawner);// sets for the room

            boss.RegisterMinion(enemy.gameObject);

            enemy.EOnDeath += ( (EnemyBrain enemy)=> OnSpawnRemoved(boss,enemy)); //sets for itself

        }

        void OnSpawnRemoved(Boss boss,EnemyBrain enemy)
        {
           boss.UnregisterMinion(enemy.gameObject);
        }

    }
    #endregion



    [Serializable]
    public class DebugBehavior : BossPhaseBehavior
    {
        [SerializeField] string message;
        public override IEnumerator Execute(Boss boss)
        {
            Debug.Log(message);
            yield return new WaitForEndOfFrame();
        }
    }

    [Serializable]
    public class RandomBehavior : BossPhaseBehavior
    {
        [SerializeReference, SubclassSelector]
        private List<BossPhaseBehavior> behaviors = new();

        private BossPhaseBehavior currentBehavior;

        public override IEnumerator Execute(Boss boss)
        {
            if (behaviors == null || behaviors.Count == 0)
                yield break;

            currentBehavior = behaviors[UnityEngine.Random.Range(0, behaviors.Count)];

            yield return currentBehavior.Execute(boss);

            currentBehavior = null;
        }

        public override void Tick(Boss boss)
        {
            currentBehavior?.Tick(boss);
        }

        public override void NotifyHit(Boss boss, Collider2D collider, Vector3 dir)
        {
            currentBehavior?.NotifyHit(boss, collider, dir);
        }

    }

    [Serializable]
    public class RepeatBehavior : BossPhaseBehavior
    {
        [SerializeReference, SubclassSelector]
        private List<BossPhaseBehavior> behaviors = new();

        private BossPhaseBehavior currentBehavior;


        public override void Tick(Boss boss)
        {
            currentBehavior?.Tick(boss);
        }

        public override void NotifyHit(Boss boss, Collider2D collider, Vector3 dir)
        {
            currentBehavior?.NotifyHit(boss, collider, dir);
        }

        public override IEnumerator Execute(Boss boss)
        {
            if (behaviors == null || behaviors.Count == 0)
                yield break;

            while (!boss.HasQueuedPhaseChange())
            {
                foreach (var behavior in behaviors)
                {
                    currentBehavior = behavior;

                    yield return behavior.Execute(boss);

                    if (boss.HasQueuedPhaseChange())
                        break;
                }
            }

            currentBehavior = null;
        }
    }

    [Serializable]
    public class WaitBehavior : BossPhaseBehavior
    {
        [SerializeField] float waitTime;
        public override IEnumerator Execute(Boss boss)
        {
            boss.AnimHelper.ChangeAnimation(boss.Data.IdleAnim);
            yield return new WaitForSeconds(waitTime);
        }
    }

    [Serializable]
    public class SetVulnerability : BossPhaseBehavior
    {
        [SerializeField] bool val;
        [SerializeField, ShowField(nameof(val))] AnimationClip vulnerableAnim;
        public override IEnumerator Execute(Boss boss)
        {
            if (boss.TryGetComponent(out Health health))
            {
                if (val) boss.AnimHelper.ChangeAnimation(Animator.StringToHash(vulnerableAnim.name), priority: 1);
                else boss.AnimHelper.ChangeAnimation(boss.Data.IdleAnim);
                health.SetVulnerability(val);
            }

            yield return new WaitForEndOfFrame();
        }
    }

    [Serializable]
    public class ConditionalBehavior : BossPhaseBehavior
    {
        [SerializeReference, SubclassSelector] BossPhaseBehavior behavior;
        [SerializeReference, SubclassSelector] BossCondition condition;

        private BossPhaseBehavior currentBehavior;

        public override IEnumerator Execute(Boss boss)
        {
            if (behavior == null) yield break;

            if (condition.Evaluate(boss))
            {
                currentBehavior = behavior;
                yield return behavior.Execute(boss);
                currentBehavior = null;
            }
        }

        public override void Tick(Boss boss)
        {
            currentBehavior?.Tick(boss);
        }

        public override void NotifyHit(Boss boss, Collider2D collider, Vector3 dir)
        {
            currentBehavior?.NotifyHit(boss, collider, dir);
        }
    }



    #region Conditions

    [Serializable]
    public abstract class BossCondition
    {
        public abstract bool Evaluate(Boss boss);
    }

    [Serializable]
    public class AllMinionsDeadCondition : BossCondition
    {
        public override bool Evaluate(Boss boss)
        {
            return boss.AllMinionsDead();
        }
    }

    #endregion

}