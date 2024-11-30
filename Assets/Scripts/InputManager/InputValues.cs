using UnityEngine;

namespace InputController
{
    public struct InputValues
    {
        public Vector2 MoveInput { get; internal set; }
        public bool JumpInput { get; internal set; }
        public bool CrounchDiveInput { get; internal set; }
        public bool InteractInput { get; internal set; }
        public bool GhostViewInput { get; internal set; }
        public bool SprintInput { get; internal set; }
        public bool Poltergeist { get; internal set; }

        public Vector2 PoltergeistXZAxis { get; internal set; }
        public bool SelectDeselectInput { get; internal set; }
        public float PoltergeistYAxis { get; internal set; }
        public bool Cancel { get; internal set; }

        public void ResetInputs()
        {
            MoveInput = Vector2.zero;
            JumpInput = false;
            CrounchDiveInput = false;
            InteractInput = false;
            GhostViewInput = false;
            SprintInput = false;

            PoltergeistXZAxis = Vector2.zero;
            SelectDeselectInput = false;
            PoltergeistYAxis = 0;
            Cancel = false;
        }
    }
}
