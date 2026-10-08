using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


namespace BossArchitecture
{

    [RequireComponent(typeof(AnimationHelper), typeof(Rigidbody2D))]
    public class Boss : MonoBehaviour
    {

        public Vector2 DirToPlayer => (player.transform.position - transform.position).normalized;
        public float AttackStartTime => attackStartTime;
        public AnimationHelper AnimHelper => animHelper;
        public Player Player => player;
        public BossData Data => bossData;
        public LayerMask WallLayer => wallLayer;
        public event Action EWallHit;
        public Rigidbody2D RB => rb;

        [SerializeField] protected BossData bossData;
        [SerializeField] protected AnimationHelper animHelper;
        [SerializeField] protected BossPhase currentPhase;
        [SerializeField] protected Hitbox[] hitBoxes;
        [SerializeField] protected LayerMask wallLayer;
        [SerializeField] bool phaseChangeQueued = false;

        BossPhase queuedPhase;
        Rigidbody2D rb;
        float attackStartTime = 0f;
        Health health;
        Player player;
        List<GameObject> minions = new();

        Coroutine phaseRoutine;
        BossPhaseBehavior currentBehavior;



        private void Awake()
        {
            if (animHelper == null) animHelper = GetComponent<AnimationHelper>();
            rb = GetComponent<Rigidbody2D>();
            health = GetComponent<Health>();
        }

        private void Start()
        {
            player = Player.Instance;
            StartPhase(bossData.Phases[0]);
        }


        private void Update()
        {
            currentBehavior?.Tick(this);
            CheckPhaseChange(health.CurHealth, health.MaxHealth);
        }

        public void CheckPhaseChange(float currentHealth, float maxHealth)
        {
            float ratio = currentHealth / maxHealth;

            if (ratio <= currentPhase.PhaseEndPoint)
            {
                //Phase Change
                AnimHelper.ChangeAnimation(currentPhase.PhaseChangeAnim, priority: 2);
                int nextIndex = bossData.Phases.IndexOf(currentPhase) + 1;
                if (nextIndex < bossData.Phases.Count)
                {
                    QueuePhaseChange(bossData.Phases[nextIndex]);
                }
            }
        }

        public virtual void OnHitTaken(EffectContext context)
        {
            foreach (Effect effect in bossData.OnHitEffects)
            {
                effect.Apply(context);
            }
        }




        void StartPhase(BossPhase phase)
        {
            if (phaseRoutine != null)
            {
                StopCoroutine(phaseRoutine);
            }

            currentPhase = phase;
            phaseRoutine = StartCoroutine(RunPhase(phase));

        }

        IEnumerator RunPhase(BossPhase phase)
        {
            foreach (var b in phase.EnterBehaviors)
            {
                currentBehavior = b;
                yield return b.Execute(this);
            }
            foreach (var b in phase.MainBehaviors)
            {
                currentBehavior = b;
                yield return b.Execute(this);

                if (phaseChangeQueued)
                {
                    yield return ChangePhase(queuedPhase); //we might run into an issue where the boss is idle and dosent check this 
                    yield break;
                }
            }

        }



        public void NotifyAttackHit(Collider2D collider, Vector3 dir)
        {
            if (collider.gameObject.IsInLayerMask(WallLayer))
            {
                EWallHit?.Invoke();
            }
            currentBehavior.NotifyHit(this, collider, dir);
        }

        private void OnEnable()
        {
            if (hitBoxes.Length > 0)
                foreach (Hitbox hitbox in hitBoxes) hitbox.EOnHitDetect += NotifyAttackHit;
        }

        private void OnDisable()
        {
            if (hitBoxes.Length > 0)
                foreach (Hitbox hitbox in hitBoxes) hitbox.EOnHitDetect -= NotifyAttackHit;
        }

        private IEnumerator ChangePhase(BossPhase nextPhase)
        {
            BossPhase oldPhase = currentPhase;

            phaseChangeQueued = false;
            queuedPhase = null;

            foreach (var behavior in oldPhase.ExitBehaviors)
            {
                currentBehavior = behavior;
                yield return behavior.Execute(this);
            }

            StartPhase(nextPhase);


        }

        public void SetRBVelocity(Vector2 val)
        {
            rb.linearVelocity = val;
        }

        public void QueuePhaseChange(BossPhase phase)
        {
            if (phaseChangeQueued)
                return;

            queuedPhase = phase;
            phaseChangeQueued = true;
        }

        public void SetStartAttackTime(float val) { attackStartTime = val; }

        public bool HasQueuedPhaseChange() => phaseChangeQueued;

        public bool AllMinionsDead() => minions.Count == 0;

        public void RegisterMinion(GameObject obj) { minions.Add(obj); }
        public void UnregisterMinion(GameObject obj) { minions.Remove(obj); }
    }
}
