#region © 2026 Joe Arrowood (JoeWare)
//
// All rights reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical, or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
#endregion

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace LogicPuzzle.Core
{
    [ExcludeFromCodeCoverage]
    public sealed class Puzzle : IPuzzle
    {
        public List<ICategory> Categories
        {
            get;
        }

        public Puzzle(List<ICategory> categories)
        {
            Categories = categories;
        }
    }
}