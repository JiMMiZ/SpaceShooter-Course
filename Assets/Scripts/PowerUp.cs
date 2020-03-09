using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    private float _speed = 3.0f;

    //ID for powerUp 0=TripleShot; 1=Speed, 2=Shield
    [SerializeField]
    private int _powerUpID;

    [SerializeField]
    private AudioClip _clip;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if(transform.position.y < -5.5f)
        {
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.transform.GetComponent<Player>();

        AudioSource.PlayClipAtPoint(_clip, transform.position);

        if (other.tag == "Player")
        {
            if(player != null)
            {
                switch(_powerUpID)
                {
                    case 0:
                        player.TripleShotPowerup();
                        break;
                    case 1:
                        player.SpeedPowerUp();
                        break;
                    case 2:
                        player.ShieldPowerUp();
                        break;
                }
            }
            Destroy(this.gameObject);
        }
    }
}
