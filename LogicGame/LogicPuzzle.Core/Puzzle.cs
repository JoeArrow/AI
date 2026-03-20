#region © 2026 Joe Arrowood (JoeWare)
//
// All rights reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical, or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using LogicPuzzle.Core.Clues;

namespace LogicPuzzle.Core
{
    [ExcludeFromCodeCoverage]
    public sealed class Puzzle : IPuzzle
    {
        public string Title
        {
            get;
        }

        // ------------------------------------------------

        public IReadOnlyList<IClue> Clues
        {
            get;
        }

        // ------------------------------------------------

        public IReadOnlyList<ICategory> Categories
        {
            get;
        }

        // ------------------------------------------------

        public Puzzle(List<ICategory> categories)
        {
            if(categories == null)
            {
                throw new ArgumentNullException(nameof(categories));
            }

            Categories = new List<ICategory>(categories).AsReadOnly();
        }
    }
}