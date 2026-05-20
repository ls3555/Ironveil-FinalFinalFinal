using UnityEngine;

public class Movepickup : MonoBehaviour, IInteractable
{
     [SerializeField] private PlayerMove movePrefab;

    private void Awake()
    {

    }

    public void Interact(PlayerMovement player)
    {
        player.EquipMove(movePrefab);
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
