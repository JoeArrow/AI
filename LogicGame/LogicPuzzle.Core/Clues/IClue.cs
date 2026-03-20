#region © 2026 Joe Arrowood (JoeWare)
//
// All rights reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical, or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
#endregion

namespace LogicPuzzle.Core.Clues
{
    public interface IClue
    {
        string ClueText { get; }

        void Apply(WorkingGrid grid);
    }
}
