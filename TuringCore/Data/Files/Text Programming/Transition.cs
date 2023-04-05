using System.Text.Json.Serialization;

namespace TuringCore.TextProgramming
{
    public class Transition
    {
        [JsonInclude]
        public float X;
        [JsonInclude]
        public float Y;
        [JsonInclude]
        public string CurrentState;
        [JsonInclude]
        public string TapeValue;
        [JsonInclude]
        public string NewState;
        [JsonInclude]
        public string NewTapeValue;
        [JsonInclude]
        public MoveHeadDirection MoveDirection;

        public Transition()
        {
        }

        public Transition(float x, float y, string currentState, string tapeValue, string newState, string newTapeValue, MoveHeadDirection moveDirection)
        {
            X = x;
            Y = y;
            CurrentState = currentState;
            TapeValue = tapeValue;
            NewState = newState;
            NewTapeValue = newTapeValue;
            MoveDirection = moveDirection;
        }
    }
}
