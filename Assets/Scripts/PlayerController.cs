using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class PlayerController : MonoBehaviourPun
{

    [Header("info")]
    public int id;

    [Header("Stats")]
    public float moveSpeed;
    public float JumpForce;

    [Header("Components")]
    public Rigidbody rig;
    public Player photonplayer;
    [Header("CameraInfo")]
    public Camera caminfo;

    [PunRPC]
    public void Initalize(Player player)
    {
        id = player.ActorNumber;
        photonplayer = player;

        GameManger.instance.players[id - 1] = this;

        if (!photonView.IsMine)
        {
            GetComponentInChildren<Camera>().gameObject.SetActive(false);
            rig.isKinematic = true;
        }
    }

    private void Start()
    {
        rig = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        Move();

        if (Input.GetKeyDown(KeyCode.Space))
            Tryjump();
    }
    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");


        Vector3 dir = (caminfo.transform.forward * z + caminfo.transform.right * x)*moveSpeed;
        dir.y = rig.velocity.y;

        rig.velocity = dir;
    }
    void Tryjump()
    {
        Ray ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, 1.5f))
        {
            rig.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
        }
    }
}
