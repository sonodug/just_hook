using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace Entities.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Player : MonoBehaviour
    {
        [SerializeField] private Hook _hook;
        [SerializeField] private Transform _hookPivot;
        [SerializeField] private float _healthCount;
        [SerializeField] private float _damage;
        [SerializeField] private float _diedDelayTransition;

        private Rigidbody2D _rigidbody;
        private float _maxHealth;
        private int _overallScoreAmount;
        private int _currentGemsCollected;
        private int _gemsCollectToFinish; //GameManager should initialize this and others values

        private SpriteRenderer _renderer;

        public float Damage => _damage;

        public event UnityAction LevelScoreChanged;
        //public event UnityAction<float, float> HealthChanged;
        public event UnityAction HealthChanged;
        public event UnityAction<Transform> ControlPointChanged;
        public event UnityAction Died;

        private void Awake()
        {
            Instantiate(_hook, _hookPivot);

            _rigidbody = GetComponent<Rigidbody2D>();
            _currentGemsCollected = 0;
        }

        private void Start()
        {
            _renderer = GetComponent<SpriteRenderer>();

            //DontDestroyOnLoad(gameObject);
            _maxHealth = _healthCount;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.TryGetComponent<Gem>(out Gem gem))
            {
                _currentGemsCollected++;

                if (gem.enabled == true)
                    LevelScoreChanged?.Invoke();
            
                gem.gameObject.SetActive(false);
                gem.enabled = false;
            }
            else if (collision.gameObject.TryGetComponent<ControlPoint>(out ControlPoint controlPoint))
            {
                ControlPointChanged?.Invoke(controlPoint.transform);
            }
        }

        private async void OnCollisionEnter2D(Collision2D collision)
        {
            //ref
            if (collision.gameObject.TryGetComponent<Obstacle>(out Obstacle obstacle))
            {
                _rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
                await Fade();
                //HealthChanged?.Invoke(0, _maxHealth);
                Died?.Invoke();
                await Delay();
                _rigidbody.constraints = RigidbodyConstraints2D.None;
                _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
            }
        }

        public async void ApplyDamage(float damage)
        {
            _healthCount -= 1;
            //HealthChanged?.Invoke(_health, _maxHealth);
            HealthChanged?.Invoke();

            if (_healthCount <= 0)
            {
                _rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
                await Fade();
                //HealthChanged?.Invoke(0, _maxHealth);
                Died?.Invoke();
                await Delay();
                _rigidbody.constraints = RigidbodyConstraints2D.None;
                _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
                
                _healthCount = _maxHealth;
                
            }
            else
            {
                _rigidbody.AddForce(new Vector2(Random.Range(-1, 1), 1) * 200);
            }
        }

        private async Task Fade()
        {
            Color color = _renderer.color;

            for (float alpha = _diedDelayTransition; alpha >= 0f; alpha -= 0.01f)
            {
                color.a = alpha;
                _renderer.color = color;

                await Task.Yield();
            }
        }

        public async Task Unfade()
        {
            Color color = _renderer.color;

            for (float alpha = 0f; alpha <= _diedDelayTransition; alpha += 0.01f)
            {
                color.a = alpha;
                _renderer.color = color;

                await Task.Yield();
            }
        }
        
        private async Task Delay()
        {
            for (float a = 0f; a <= 1f; a += 0.005f)
            {
                await Task.Yield();
            }
        }
    }
}
