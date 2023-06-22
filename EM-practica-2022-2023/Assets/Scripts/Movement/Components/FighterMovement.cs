using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;
using UnityEngine.Serialization;

namespace Movement.Components
{
    [RequireComponent(typeof(Rigidbody2D)),
     RequireComponent(typeof(Animator)),
     RequireComponent(typeof(NetworkObject))]
    public sealed class FighterMovement : NetworkBehaviour, IMoveableReceiver, IJumperReceiver, IFighterReceiver
    {
        public float speed = 1.0f;
        public float jumpAmount = 1.0f;
        

        private Rigidbody2D _rigidbody2D;
        private Animator _animator;
        private NetworkAnimator _networkAnimator;
        private Transform _feet;
        private LayerMask _floor;
        private Vector3 _direction = Vector3.zero;
        NetworkVariable<Vector3> direccion = new();
        private bool _grounded = true;
        public Vida vida;
        public string nombre;
        [SerializeField] public NetworkVariable<float> vidaActual = new NetworkVariable<float>();
        //[SerializeField] public NetworkVariable<string> nombreActual = new NetworkVariable<string>();



        private static readonly int AnimatorSpeed = Animator.StringToHash("speed");
        private static readonly int AnimatorVSpeed = Animator.StringToHash("vspeed");
        private static readonly int AnimatorGrounded = Animator.StringToHash("grounded");
        private static readonly int AnimatorAttack1 = Animator.StringToHash("attack1");
        private static readonly int AnimatorAttack2 = Animator.StringToHash("attack2");
        private static readonly int AnimatorHit = Animator.StringToHash("hit");
        private static readonly int AnimatorDie = Animator.StringToHash("die");

        public void Awake()
        {
            direccion.OnValueChanged += direccionCambiada;
        }

        void Start()
        {
            _rigidbody2D = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _networkAnimator = GetComponent<NetworkAnimator>();

            _feet = transform.Find("Feet");
            _floor = LayerMask.GetMask("Floor");

            vida = GameObject.FindObjectOfType<Vida>();
            //nombre = GameObject.Find("UI").GetComponent<UIManager>().playerName;
            inicializarPersonajeServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        public void inicializarPersonajeServerRpc()
        {
            vidaActual.Value = vida.getVidaMax();
            //nombreActual.Value = "POAPA";
        }

        void Update()
        {
            if (!IsOwner) return;
            AnimacionesServerRpc();
        }

        [ServerRpc]
        void AnimacionesServerRpc()
        {
            _grounded = Physics2D.OverlapCircle(_feet.position, 0.1f, _floor);
            _animator.SetFloat(AnimatorSpeed, this.direccion.Value.magnitude);
            _animator.SetFloat(AnimatorVSpeed, this._rigidbody2D.velocity.y);
            _animator.SetBool(AnimatorGrounded, this._grounded);
        }

        void FixedUpdate()
        {
            _rigidbody2D.velocity = new Vector2(direccion.Value.x, _rigidbody2D.velocity.y);
        }

        private void direccionCambiada(Vector3 direccionAnterior, Vector3 direccionNueva)
        {
            _direction = direccionNueva;
        }

        [ClientRpc]
        public void actualizarDireccionClientRpc(bool lookingRight)
        {
            transform.localScale = new Vector3(lookingRight ? 1 : -1, 1, 1);
        }

        public void Move(IMoveableReceiver.Direction direction)
        {

            MoveServerRpc(direction);
        }

        public void Jump(IJumperReceiver.JumpStage stage)
        {
            JumpServerRpc(stage);
        }

        public void Attack1()
        {
            Attack1ServerRpc();
        }

        public void Attack2()
        {
            Attack2ServerRpc();
        }

        public void TakeHit(float damage)
        {
            TakeHitServerRpc(damage);
        }

        public void Die()
        {
            DieServerRpc();
        }

        [ServerRpc]
        public void MoveServerRpc(IMoveableReceiver.Direction direction)
        {
            if (direction == IMoveableReceiver.Direction.None)
            {
                this.direccion.Value = Vector3.zero;
                return;
            }

            bool lookingRight = direction == IMoveableReceiver.Direction.Right;
            direccion.Value = (lookingRight ? 1f : -1f) * speed * Vector3.right;
            actualizarDireccionClientRpc(lookingRight);
        }

        [ServerRpc]
        public void JumpServerRpc(IJumperReceiver.JumpStage stage)
        {
            switch (stage)
            {
                case IJumperReceiver.JumpStage.Jumping:
                    if (_grounded)
                    {
                        float jumpForce = Mathf.Sqrt(jumpAmount * -2.0f * (Physics2D.gravity.y * _rigidbody2D.gravityScale));
                        _rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                    }
                    break;
                case IJumperReceiver.JumpStage.Landing:
                    break;
            }
        }

        [ServerRpc]
        public void Attack1ServerRpc()
        {
            _networkAnimator.SetTrigger(AnimatorAttack1);
        }

        [ServerRpc]
        public void Attack2ServerRpc()
        {
            _networkAnimator.SetTrigger(AnimatorAttack2);
        }

        [ServerRpc(RequireOwnership = false)]
        public void TakeHitServerRpc(float damage)
        {
            _networkAnimator.SetTrigger(AnimatorHit);
            TakeHitClientRpc(damage);
        }
        [ClientRpc]
        public void TakeHitClientRpc(float damage)
        {
            float v = vida.getVidaNueva();
            vida.setVidaNueva(v - damage);
        }

        [ServerRpc(RequireOwnership = false)]
        public void DieServerRpc()
        {
            _networkAnimator.SetTrigger(AnimatorDie);
            Invoke("DieClientRpc", 0.85f);
        }

        [ClientRpc]
        public void DieClientRpc()
        {
            foreach (Transform hijo in transform.parent)
            {
                Destroy(hijo.gameObject);
            }
        }

        public float GetVida()
        {
            return vida.getVidaNueva(); ;
        }
    }
}