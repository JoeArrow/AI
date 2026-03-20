#region © 2026 Joe Arrowood (JoeWare)
//
// All rights reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical, or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
#endregion

namespace LogicPuzzle.Core
{
    public sealed class CategoryPairKey
    {
        public string FirstCategoryName
        {
            get;
        }

        // ------------------------------------------------

        public string SecondCategoryName
        {
            get;
        }

        // ------------------------------------------------

        public CategoryPairKey(string firstCategoryName, string secondCategoryName)
        {
            if(string.CompareOrdinal(firstCategoryName, secondCategoryName) <= 0)
            {
                FirstCategoryName = firstCategoryName;
                SecondCategoryName = secondCategoryName;
            }
            else
            {
                FirstCategoryName = secondCategoryName;
                SecondCategoryName = firstCategoryName;
            }
        }

        // ------------------------------------------------

        public override bool Equals(object obj)
        {
            CategoryPairKey other = obj as CategoryPairKey;

            if(other == null)
            {
                return false;
            }

            return FirstCategoryName == other.FirstCategoryName
                && SecondCategoryName == other.SecondCategoryName;
        }

        // ------------------------------------------------

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = (hash * 23) + FirstCategoryName.GetHashCode();
                hash = (hash * 23) + SecondCategoryName.GetHashCode();
                return hash;
            }
        }
    }
}
