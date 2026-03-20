#region © 2026 Joe Arrowood (JoeWare)
//
// All rights reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical, or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
#endregion

using System.Collections.Generic;

namespace LogicPuzzle.Core
{
    public sealed class Solution
    {
        public Dictionary<string, Dictionary<string, string>> Assignments
        {
            get;
        }

        public Solution()
        {
            Assignments = new Dictionary<string, Dictionary<string, string>>();
        }
    }
}