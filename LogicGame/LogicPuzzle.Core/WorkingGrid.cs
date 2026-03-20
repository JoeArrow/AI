#region © 2026 Joe Arrowood (JoeWare)
//
// All rights reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical, or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
#endregion

using System;
using System.Collections.Generic;
using System.Linq;

namespace LogicPuzzle.Core
{
    public sealed class WorkingGrid
    {
        private readonly Dictionary<string, Category> _categoriesByName;
        private readonly Dictionary<CategoryPairKey, GridMatrix> _matrices;

        // ------------------------------------------------

        public WorkingGrid(List<Category> categories)
        {
            _categoriesByName = categories.ToDictionary(category => category.Name);
            _matrices = new Dictionary<CategoryPairKey, GridMatrix>();

            InitializeMatrices(categories);
        }

        // ------------------------------------------------

        public CellState GetState(string categoryName1, string itemName1, string categoryName2, string itemName2)
        {
            Category category1 = GetCategory(categoryName1);
            Category category2 = GetCategory(categoryName2);

            int itemIndex1 = GetItemIndex(category1, itemName1);
            int itemIndex2 = GetItemIndex(category2, itemName2);

            GridMatrix matrix = GetMatrix(category1.Name, category2.Name);

            if(category1.Name == CreateMatrixKey(category1.Name, category2.Name).FirstCategoryName)
            {
                return matrix.GetCell(itemIndex1, itemIndex2);
            }

            return matrix.GetCell(itemIndex2, itemIndex1);
        }

        // ------------------------------------------------

        public void SetNo( string categoryName1, string itemName1, string categoryName2, string itemName2)
        {
            SetState(categoryName1, itemName1, categoryName2, itemName2, CellState.No);
        }

        // ------------------------------------------------

        public void SetYes( string categoryName1, string itemName1, string categoryName2, string itemName2)
        {
            SetState(categoryName1, itemName1, categoryName2, itemName2, CellState.Yes);
            ApplyYesConstraints(categoryName1, itemName1, categoryName2, itemName2);
        }

        // ------------------------------------------------

        private void SetState( string categoryName1, string itemName1, string categoryName2, string itemName2, CellState newState)
        {
            Category category1 = GetCategory(categoryName1);
            Category category2 = GetCategory(categoryName2);

            int itemIndex1 = GetItemIndex(category1, itemName1);
            int itemIndex2 = GetItemIndex(category2, itemName2);

            GridMatrix matrix = GetMatrix(category1.Name, category2.Name);
            CategoryPairKey key = CreateMatrixKey(category1.Name, category2.Name);

            int rowIndex;
            int columnIndex;

            if(category1.Name == key.FirstCategoryName)
            {
                rowIndex = itemIndex1;
                columnIndex = itemIndex2;
            }
            else
            {
                rowIndex = itemIndex2;
                columnIndex = itemIndex1;
            }

            CellState currentState = matrix.GetCell(rowIndex, columnIndex);

            if(currentState == newState)
            {
                return;
            }

            if(currentState != CellState.Unknown && currentState != newState)
            {
                throw new InvalidOperationException("Contradictory cell assignment.");
            }

            matrix.SetCell(rowIndex, columnIndex, newState);

            Propagate();
        }

        // ------------------------------------------------

        private void ApplyYesConstraints( string categoryName1, string itemName1, string categoryName2, string itemName2)
        {
            Category category1 = GetCategory(categoryName1);
            Category category2 = GetCategory(categoryName2);

            foreach(CategoryItem otherItem in category2.Items)
            {
                if(otherItem.Name != itemName2)
                {
                    SetNo(categoryName1, itemName1, categoryName2, otherItem.Name);
                }
            }

            foreach(CategoryItem otherItem in category1.Items)
            {
                if(otherItem.Name != itemName1)
                {
                    SetNo(categoryName1, otherItem.Name, categoryName2, itemName2);
                }
            }
        }

        // ------------------------------------------------

        private void InitializeMatrices(List<Category> categories)
        {
            for(int i = 0; i < categories.Count; i++)
            {
                for(int j = i + 1; j < categories.Count; j++)
                {
                    Category firstCategory = categories[i];
                    Category secondCategory = categories[j];

                    CategoryPairKey key = new CategoryPairKey(
                        firstCategory.Name,
                        secondCategory.Name);

                    _matrices[key] = new GridMatrix(
                        firstCategory.Items.Count,
                        secondCategory.Items.Count);
                }
            }
        }

        // ------------------------------------------------

        private Category GetCategory(string categoryName)
        {
            if(_categoriesByName.TryGetValue(categoryName, out Category category) == false)
            {
                throw new ArgumentException(
                    $"Unknown category: {categoryName}",
                    nameof(categoryName));
            }

            return category;
        }

        // ------------------------------------------------

        private int GetItemIndex(Category category, string itemName)
        {
            for(int i = 0; i < category.Items.Count; i++)
            {
                if(category.Items[i].Name == itemName)
                {
                    return i;
                }
            }

            throw new ArgumentException(
                $"Unknown item '{itemName}' in category '{category.Name}'.",
                nameof(itemName));
        }

        // ------------------------------------------------

        private GridMatrix GetMatrix(string categoryName1, string categoryName2)
        {
            CategoryPairKey key = CreateMatrixKey(categoryName1, categoryName2);

            if(_matrices.TryGetValue(key, out GridMatrix matrix) == false)
            {
                throw new InvalidOperationException(
                    $"No matrix exists for '{categoryName1}' and '{categoryName2}'.");
            }

            return matrix;
        }

        // ------------------------------------------------

        private CategoryPairKey CreateMatrixKey(string categoryName1, string categoryName2)
        {
            return new CategoryPairKey(categoryName1, categoryName2);
        }

        // ------------------------------------------------

        private void Propagate()
        {
            bool changed;

            do
            {
                changed = ApplySinglePossibilityRule();
            }
            while(changed);
        }

        // ------------------------------------------------

        private bool ApplySinglePossibilityRule()
        {
            bool anyChange = false;

            foreach(KeyValuePair<CategoryPairKey, GridMatrix> entry in _matrices)
            {
                GridMatrix matrix = entry.Value;

                // ----------
                // Check rows

                for(int row = 0; row < matrix.RowCount; row++)
                {
                    int unknownCount = 0;
                    int lastUnknownColumn = -1;

                    for(int column = 0; column < matrix.ColumnCount; column++)
                    {
                        CellState state = matrix.GetCell(row, column);

                        if(state == CellState.Unknown)
                        {
                            unknownCount++;
                            lastUnknownColumn = column;
                        }

                        if(state == CellState.Yes)
                        {
                            unknownCount = 0;
                            break;
                        }
                    }

                    if(unknownCount == 1)
                    {
                        matrix.SetCell(row, lastUnknownColumn, CellState.Yes);
                        ApplyYesFromMatrix(entry.Key, row, lastUnknownColumn);
                        anyChange = true;
                    }
                }

                // -------------
                // Check columns

                for(int column = 0; column < matrix.ColumnCount; column++)
                {
                    int unknownCount = 0;
                    int lastUnknownRow = -1;

                    for(int row = 0; row < matrix.RowCount; row++)
                    {
                        CellState state = matrix.GetCell(row, column);

                        if(state == CellState.Unknown)
                        {
                            unknownCount++;
                            lastUnknownRow = row;
                        }

                        if(state == CellState.Yes)
                        {
                            unknownCount = 0;
                            break;
                        }
                    }

                    if(unknownCount == 1)
                    {
                        matrix.SetCell(lastUnknownRow, column, CellState.Yes);
                        ApplyYesFromMatrix(entry.Key, lastUnknownRow, column);
                        anyChange = true;
                    }
                }
            }

            return anyChange;
        }

        // ------------------------------------------------

        private void ApplyYesFromMatrix(CategoryPairKey key, int row, int column)
        {
            Category firstCategory = GetCategory(key.FirstCategoryName);
            Category secondCategory = GetCategory(key.SecondCategoryName);

            string item1 = firstCategory.Items[row].Name;
            string item2 = secondCategory.Items[column].Name;

            ApplyYesConstraints(firstCategory.Name, item1, secondCategory.Name, item2);
        }
    }
}
