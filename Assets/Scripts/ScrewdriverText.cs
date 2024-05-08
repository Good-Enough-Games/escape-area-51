using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScrewdriverText : MonoBehaviour
{
    public Transform player; // the player
    public float enableDistance = 10; // distance for text to appear
    private TextMeshProUGUI message; // the text

    private PlayerController playerVars; // access to PlayerController variables
    // Start is called before the first frame update
    void Start()
    {
        message = GetComponent<TextMeshProUGUI>();
        playerVars = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= enableDistance) {
            message.enabled = true;
            playerVars.canCollectScrewdriver = true;
        } else {
            message.enabled = false;
            playerVars.canCollectScrewdriver = false;
        }
        if (playerVars.hasScrewdriver) {
            Destroy(gameObject);
        }
    }
}
