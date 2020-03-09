using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.5f;
    private float _speedMultiplier = 2.0f;

    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;

    [SerializeField]
    private float _fireRate = 0.25f;
    private float _canFire = 0.0f;

    private bool _isTripleShotActive = false;
    private bool _isSpeedBostActive = false;
    private bool _isShieldActive = false;

    [SerializeField]
    private int _lives = 3;

    private SpawnManager _spawnManager;

    [SerializeField]
    private GameObject _shieldVisualizer;

    [SerializeField]
    private int _score = 0;

    private UIManager _uiManager;

    [SerializeField]
    private GameObject[] _engines;
    private bool _isDamaged = false;

    [SerializeField]
    private AudioClip _laserSoundClip;

    private AudioSource _audioSource;


    // Start is called before the first frame update
    void Start()
    {
        this.transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        _uiManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        _audioSource = GetComponent<AudioSource>();

        if (_spawnManager == null)
        {
            Debug.LogError("The spawn manager is NULL");
        }

        if(_uiManager == null)
        {
            Debug.LogError("The UI manager is NULL");
        }

        if(_audioSource == null)
        {
            Debug.LogError("The laser Source is NULL");
        }
        else
        {
            _audioSource.clip = _laserSoundClip;
        }

        _engines[0].gameObject.SetActive(false);
        _engines[1].gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        this.transform.Translate(direction * _speed * Time.deltaTime);

        this.transform.position = new Vector3(this.transform.position.x, Mathf.Clamp(this.transform.position.y, -3.8f, 0), 0);

        if (this.transform.position.x > 11.3f)
        {
            this.transform.position = new Vector3(-11.3f, this.transform.position.y, 0);
        }
        else if (this.transform.position.x < -11.3f)
        {
            this.transform.position = new Vector3(11.3f, this.transform.position.y, 0);
        }
    }

    void FireLaser()
    {

        _canFire = Time.time + _fireRate;

        if (_isTripleShotActive)
        {
            Instantiate(_tripleShotPrefab, this.transform.position, Quaternion.identity);
        }
        else
        {
            // posició del player + una mica de distància (Offset) per simular més real (0.8 a la y)
            Instantiate(_laserPrefab, this.transform.position + new Vector3(0, 1.05f, 0), Quaternion.identity);
        }

        _audioSource.Play();
    }

    public void Damage()
    {
        if(_isShieldActive)
        {
            _isShieldActive = false;
            _shieldVisualizer.SetActive(false);
            return;
        }
        _lives--;

        _audioSource.Play();
        _uiManager.UpdateLives(_lives);
        DamageEngines();

        if (_lives <= 0)
        {
            _spawnManager.onPlayerDeath();
            Destroy(this.gameObject);
            _uiManager.GameOver();
        }
    }

    public void TripleShotPowerup()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerUpRoutine());
    }

    IEnumerator TripleShotPowerUpRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isTripleShotActive = false;
    }


    public void SpeedPowerUp()
    {
        _isSpeedBostActive = true;
        _speed = _speed * _speedMultiplier;
    }

    IEnumerator SpeedBoostRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isSpeedBostActive = false;
        _speed = _speed / _speedMultiplier;
    }

    public void ShieldPowerUp()
    {
        _isShieldActive = true;
        _shieldVisualizer.SetActive(true);
        StartCoroutine(ShieldRoutine());
    }

    IEnumerator ShieldRoutine()
    {
        yield return new WaitForSeconds(5.0f);
        _isShieldActive = false;
        _shieldVisualizer.SetActive(false);
    }

    public void AddScore(int points)
    {
        _score += points;
        _uiManager.UpdateScore(_score);
    }

    private void DamageEngines()
    {
        if (_isDamaged == false)
        {
            int _index = Random.Range(0, 2);
            _engines[_index].SetActive(true);
            _isDamaged = true;
        }
        else
        {
            _engines[0].SetActive(true);
            _engines[1].SetActive(true);
        }
    }
}   
