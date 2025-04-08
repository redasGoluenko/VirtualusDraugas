using UnityEngine;

namespace IdyllicFantasyNature
{
    public class PlayerMovement : MonoBehaviour
    {
        [Range(1f, 20f)]
        [SerializeField] private float _movementSpeed;
        [Tooltip("run multiplier of the movement speed")]
        [Range(1f, 20f)]
        [SerializeField] private float _runMultiplier;
        [SerializeField] private float _gravity = -9.81f;
        [Range(1f, 20f)]
        [SerializeField] private float _jumpHeight;      

        private CharacterController characterController;
        Vector3 _controllerVelocity;

        public bool PhoonMode = false;

        // Start is called before the first frame update
        void Start()
        {
            characterController = GetComponent<CharacterController>();
        }

        // Update is called once per frame
        void Update()
        {
            // stops the y velocity when player is on the ground and the velocity has reached 0
            if (characterController.isGrounded && _controllerVelocity.y < 0)
            {
                _controllerVelocity.y = 0;
            }

            // get the movement input
            float moveX = Input.GetAxis("Horizontal");
            float moveZ = Input.GetAxis("Vertical");

            // moves the controller in the desired direction on the x- and z-axis
            Vector3 movement = transform.right * moveX + transform.forward * moveZ;
            characterController.Move(movement * _movementSpeed * Time.deltaTime);

            // gravity affects the controller on the y-axis
            _controllerVelocity.y += _gravity * Time.deltaTime;

            // moves the controller on the y-axis
            characterController.Move(_controllerVelocity * Time.deltaTime);

            // the controller is able to jump when on the ground
            if (Input.GetButton("Jump") && characterController.isGrounded)
            {
                _controllerVelocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
            }

            // the controller is able to run
            if (Input.GetKey(KeyCode.LeftShift) || PhoonMode)
            {
                characterController.Move(movement * Time.deltaTime * _runMultiplier);
            }

            // keep adding to movement speed while spacebar is being held and reset if its released
            if (Input.GetKey(KeyCode.Space) && PhoonMode)
            {
                _movementSpeed += Time.deltaTime;
                _jumpHeight += Time.deltaTime / 4;
            }
            else
            {
                _movementSpeed = 4f;
                _jumpHeight = 1.5f;
            }
            
        }
    }
}
