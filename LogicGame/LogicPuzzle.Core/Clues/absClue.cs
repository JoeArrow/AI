#region © 2026 Joe Arrowood (JoeWare)
//
// All rights reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical, or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
#endregion

using System;

namespace LogicPuzzle.Core.Clues
{
    public abstract class absClue : IClue
    {
        protected absClue(string clueText)
        {
            if(string.IsNullOrWhiteSpace(clueText) == true)
            {
                throw new ArgumentException("Clue text cannot be null or blank.",
                                            nameof(clueText));
            }

            ClueText = clueText;
        }

        // ------------------------------------------------

        public string ClueText { get; }

        // ------------------------------------------------

        public abstract void Apply(WorkingGrid grid);

        // ------------------------------------------------

        public override string ToString()
        {
            return ClueText;
        }
    }
}