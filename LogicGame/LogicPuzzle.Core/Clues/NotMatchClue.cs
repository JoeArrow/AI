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
    public sealed class NotMatchClue : absClue
    {
        public string CategoryName1
        {
            get;
        }

        // ------------------------------------------------

        public string ItemName1
        {
            get;
        }

        // ------------------------------------------------

        public string CategoryName2
        {
            get;
        }

        // ------------------------------------------------

        public string ItemName2
        {
            get;
        }

        // ------------------------------------------------

        public NotMatchClue(string clueText,
                            string categoryName1,
                            string itemName1,
                            string categoryName2,
                            string itemName2)
            : base(clueText)
        {
            if(string.IsNullOrWhiteSpace(categoryName1) == true)
            {
                throw new ArgumentException("Category name cannot be null or blank.",
                                            nameof(categoryName1));
            }

            if(string.IsNullOrWhiteSpace(itemName1) == true)
            {
                throw new ArgumentException("Item name cannot be null or blank.",
                                            nameof(itemName1));
            }

            if(string.IsNullOrWhiteSpace(categoryName2) == true)
            {
                throw new ArgumentException("Category name cannot be null or blank.",
                                            nameof(categoryName2));
            }

            if(string.IsNullOrWhiteSpace(itemName2) == true)
            {
                throw new ArgumentException("Item name cannot be null or blank.",
                                            nameof(itemName2));
            }

            if(string.Equals(categoryName1, categoryName2, StringComparison.Ordinal) == true)
            {
                throw new ArgumentException("A clue must use two different categories.");
            }

            ItemName1 = itemName1;
            ItemName2 = itemName2;
            CategoryName1 = categoryName1;
            CategoryName2 = categoryName2;
        }

        // ------------------------------------------------

        public override void Apply(WorkingGrid grid)
        {
            if(grid == null)
            {
                throw new ArgumentNullException(nameof(grid));
            }

            grid.SetNo(CategoryName1,
                       ItemName1,
                       CategoryName2,
                       ItemName2);
        }
    }
}