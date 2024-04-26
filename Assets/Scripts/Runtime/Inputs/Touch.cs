using UnityEngine;

namespace TS.Inputs
{
    public class Touch
    {
        public Vector2 Position { get; private set; }
        public Vector2 PreviousPosition { get; private set; }
        public TouchPhase Phase { get; private set; }

        public Touch(Vector2 position, TouchPhase phase)
        {
            Position = position;
            PreviousPosition = position;
            Phase = phase;
        }
    }
}