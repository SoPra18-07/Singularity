using System;

namespace Singularity.Utils
{
    /// <summary>
    /// These are the states for the PlatformPlacer.
    /// </summary>
    public class State3
    {
        private bool mIsFirstState;

        private bool mIsSecondState;

        private bool mIsThirdState;

        public State3(int initialState = 1)
        {
            if (initialState == 1)
            {
                mIsFirstState = true;

            }else if (initialState == 2)
            {
                mIsSecondState = true;
            }
            else if (initialState == 3)
            {
                mIsThirdState = true;
            }
            else
            {
                throw new ArgumentOutOfRangeException(nameof(initialState),
                    initialState,
                    "The given argument has to be between 1 and 3 (inclusive).");
            }
        }


        /// <summary>
        /// Moves this state to its next state and returns the state it was previously in.
        /// </summary>
        /// <returns>The number of the state it was in</returns>
        public int NextState()
        {
            if (mIsFirstState)
            {
                mIsSecondState = true;
                mIsFirstState = false;
                return 1;

            }
            if (!mIsSecondState)
            {

                return 3;
            }
            mIsThirdState = true;
            mIsSecondState = false;

            return 2;
        }

        /// <summary>
        /// Moves this state to its previous state and returns the state it was previously in.
        /// </summary>
        /// <returns>The number of the state it was in</returns>
        public int PreviousState()
        {
            if (mIsSecondState)
            {
                mIsFirstState = true;
                mIsSecondState = false;
                return 2;
            }

            if (mIsThirdState)
            {
                mIsSecondState = true;
                mIsThirdState = false;
                return 3;
            }

            return 1;
        }

        /// <summary>
        /// Gets the current state.
        /// </summary>
        /// <returns>Returns the number of the current state, and -1 if no state is currently active.</returns>
        public int GetState()
        {
            if (mIsFirstState)
            {
                return 1;
            }

            if (mIsSecondState)
            {
                return 2;
            }

            if (mIsThirdState)
            {
                return 3;
            }

            return -1;
        }

        public void MoveTo(int state)
        {
            mIsFirstState = false;
            mIsSecondState = false;
            mIsThirdState = false;

            if (state == 1)
            {
                mIsFirstState = true;

            }
            else if (state == 2)
            {
                mIsSecondState = true;
            }
            else if (state == 3)
            {
                mIsThirdState = true;
            }
        }
    }
}
