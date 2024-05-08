using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VentText : MonoBehaviour
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
            playerVars.canOpenVent = true;
        } else {
            message.enabled = false;
            playerVars.canOpenVent = false;
        }
        if (playerVars.hasScrewdriver) {
            message.text = "<alpha=#FF>[F] Open vent";
        } else {
            if (playerVars.showError) {
                message.text = "<alpha=#88>You need a screwdriver";
            } else {
                message.text = "<alpha=#88>[F] Open vent";
            }
        }
        if (playerVars.vent.isOpen) {
            Destroy(message.gameObject);
        }
    }
}
