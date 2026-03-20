#region © 2026 Joe Arrowood (JoeWare)
//
// All rights reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical, or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
#endregion

using System;
using System.Linq;
using System.Collections.Generic;

namespace LogicPuzzle.Core
{
    public sealed class WorkingGrid
    {
        private bool _isPropagating;
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
            var category1 = GetCategory(categoryName1);
            var category2 = GetCategory(categoryName2);

            var itemIndex1 = GetItemIndex(category1, itemName1);
            var itemIndex2 = GetItemIndex(category2, itemName2);

            var matrix = GetMatrix(category1.Name, category2.Name);
            var key = CreateMatrixKey(category1.Name, category2.Name);

            if(category1.Name == key.FirstCategoryName)
            {
                return matrix.GetCell(itemIndex1, itemIndex2);
            }

            return matrix.GetCell(itemIndex2, itemIndex1);
        }

        // ------------------------------------------------

        public void SetNo(string categoryName1, string itemName1, string categoryName2, string itemName2)
        {
            SetState(categoryName1, itemName1, categoryName2, itemName2, CellState.No);
        }

        // ------------------------------------------------

        public void SetYes(string categoryName1, string itemName1, string categoryName2, string itemName2)
        {
            SetState(categoryName1, itemName1, categoryName2, itemName2, CellState.Yes);
        }

        // ------------------------------------------------

        private void SetState(string categoryName1, string itemName1, string categoryName2, string itemName2, CellState newState)
        {
            var category1 = GetCategory(categoryName1);
            var category2 = GetCategory(categoryName2);

            var itemIndex1 = GetItemIndex(category1, itemName1);
            var itemIndex2 = GetItemIndex(category2, itemName2);

            var matrix = GetMatrix(category1.Name, category2.Name);
            var key = CreateMatrixKey(category1.Name, category2.Name);

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

            var currentState = matrix.GetCell(rowIndex, columnIndex);

            if(currentState == newState)
            {
                return;
            }

            if(currentState != CellState.Unknown && currentState != newState)
            {
                throw new InvalidOperationException("Contradictory cell assignment.");
            }

            matrix.SetCell(rowIndex, columnIndex, newState);

            ValidateMatrixConsistency();

            if(newState == CellState.Yes)
            {
                ApplyYesConstraints(categoryName1, itemName1, categoryName2, itemName2);
            }

            Propagate();
        }

        // ------------------------------------------------

        private void ApplyYesConstraints(string categoryName1, string itemName1, string categoryName2, string itemName2)
        {
            var category1 = GetCategory(categoryName1);
            var category2 = GetCategory(categoryName2);

            foreach(var otherItem in category2.Items)
            {
                if(otherItem.Name != itemName2)
                {
                    SetNo(categoryName1, itemName1, categoryName2, otherItem.Name);
                }
            }

            foreach(var otherItem in category1.Items)
            {
                if(otherItem.Name != itemName1)
                {
                    SetNo(categoryName1, otherItem.Name, categoryName2, itemName2);
                }
            }
        }

        // ------------------------------------------------

        private void Propagate()
        {
            if(_isPropagating == true)
            {
                return;
            }

            _isPropagating = true;

            try
            {
                bool changed;

                do
                {
                    changed = false;

                    if(ApplySinglePossibilityRule() == true)
                    {
                        changed = true;
                    }

                    if(ApplyCrossCategoryPropagationRule() == true)
                    {
                        changed = true;
                    }

                    if(ApplyCrossCategoryNegativePropagationRule() == true)
                    {
                        changed = true;
                    }

                    ValidateMatrixConsistency();
                }
                while(changed == true);
            }
            finally
            {
                _isPropagating = false;
            }
        }

        // ------------------------------------------------

        private bool ApplySinglePossibilityRule()
        {
            foreach(var entry in _matrices)
            {
                var key = entry.Key;
                var matrix = entry.Value;

                for(int row = 0; row < matrix.RowCount; row++)
                {
                    var unknownCount = 0;
                    var lastUnknownColumn = -1;
                    var hasYes = false;

                    for(int column = 0; column < matrix.ColumnCount; column++)
                    {
                        var state = matrix.GetCell(row, column);

                        if(state == CellState.Yes)
                        {
                            hasYes = true;
                            break;
                        }

                        if(state == CellState.Unknown)
                        {
                            unknownCount++;
                            lastUnknownColumn = column;
                        }
                    }

                    if(hasYes == false && unknownCount == 1)
                    {
                        var firstCategory = GetCategory(key.FirstCategoryName);
                        var secondCategory = GetCategory(key.SecondCategoryName);

                        var itemName1 = firstCategory.Items[row].Name;
                        var itemName2 = secondCategory.Items[lastUnknownColumn].Name;

                        SetYes(firstCategory.Name, itemName1, secondCategory.Name, itemName2);
                        return true;
                    }
                }

                for(int column = 0; column < matrix.ColumnCount; column++)
                {
                    var unknownCount = 0;
                    var lastUnknownRow = -1;
                    var hasYes = false;

                    for(int row = 0; row < matrix.RowCount; row++)
                    {
                        var state = matrix.GetCell(row, column);

                        if(state == CellState.Yes)
                        {
                            hasYes = true;
                            break;
                        }

                        if(state == CellState.Unknown)
                        {
                            unknownCount++;
                            lastUnknownRow = row;
                        }
                    }

                    if(hasYes == false && unknownCount == 1)
                    {
                        var firstCategory = GetCategory(key.FirstCategoryName);
                        var secondCategory = GetCategory(key.SecondCategoryName);

                        var itemName1 = firstCategory.Items[lastUnknownRow].Name;
                        var itemName2 = secondCategory.Items[column].Name;

                        SetYes(firstCategory.Name, itemName1, secondCategory.Name, itemName2);
                        return true;
                    }
                }
            }

            return false;
        }

        // ------------------------------------------------

        private bool ApplyCrossCategoryPropagationRule()
        {
            foreach(var sourceCategory in _categoriesByName.Values)
            {
                foreach(var bridgeCategory in _categoriesByName.Values)
                {
                    foreach(var targetCategory in _categoriesByName.Values)
                    {
                        if(sourceCategory.Name == bridgeCategory.Name)
                        {
                            continue;
                        }

                        if(sourceCategory.Name == targetCategory.Name)
                        {
                            continue;
                        }

                        if(bridgeCategory.Name == targetCategory.Name)
                        {
                            continue;
                        }

                        foreach(var sourceItem in sourceCategory.Items)
                        {
                            foreach(var bridgeItem in bridgeCategory.Items)
                            {
                                if(GetState(sourceCategory.Name, sourceItem.Name, bridgeCategory.Name, bridgeItem.Name) != CellState.Yes)
                                {
                                    continue;
                                }

                                foreach(var targetItem in targetCategory.Items)
                                {
                                    if(GetState(bridgeCategory.Name, bridgeItem.Name, targetCategory.Name, targetItem.Name) != CellState.Yes)
                                    {
                                        continue;
                                    }

                                    if(GetState(sourceCategory.Name, sourceItem.Name, targetCategory.Name, targetItem.Name) == CellState.Unknown)
                                    {
                                        SetYes(sourceCategory.Name, sourceItem.Name, targetCategory.Name, targetItem.Name);
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        // ------------------------------------------------

        private bool ApplyCrossCategoryNegativePropagationRule()
        {
            foreach(var sourceCategory in _categoriesByName.Values)
            {
                foreach(var bridgeCategory in _categoriesByName.Values)
                {
                    foreach(var targetCategory in _categoriesByName.Values)
                    {
                        if(sourceCategory.Name == bridgeCategory.Name)
                        {
                            continue;
                        }

                        if(sourceCategory.Name == targetCategory.Name)
                        {
                            continue;
                        }

                        if(bridgeCategory.Name == targetCategory.Name)
                        {
                            continue;
                        }

                        foreach(var sourceItem in sourceCategory.Items)
                        {
                            foreach(var bridgeItem in bridgeCategory.Items)
                            {
                                var sourceBridgeState = GetState(
                                    sourceCategory.Name,
                                    sourceItem.Name,
                                    bridgeCategory.Name,
                                    bridgeItem.Name);

                                foreach(var targetItem in targetCategory.Items)
                                {
                                    var bridgeTargetState = GetState(
                                        bridgeCategory.Name,
                                        bridgeItem.Name,
                                        targetCategory.Name,
                                        targetItem.Name);

                                    var sourceTargetState = GetState(
                                        sourceCategory.Name,
                                        sourceItem.Name,
                                        targetCategory.Name,
                                        targetItem.Name);

                                    if(sourceTargetState != CellState.Unknown)
                                    {
                                        continue;
                                    }

                                    // --------------------------
                                    // A = B and B != C => A != C

                                    if(sourceBridgeState == CellState.Yes && bridgeTargetState == CellState.No)
                                    {
                                        SetNo(
                                            sourceCategory.Name,
                                            sourceItem.Name,
                                            targetCategory.Name,
                                            targetItem.Name);

                                        return true;
                                    }

                                    // --------------------------
                                    // A != B and B = C => A != C

                                    if(sourceBridgeState == CellState.No && bridgeTargetState == CellState.Yes)
                                    {
                                        SetNo(
                                            sourceCategory.Name,
                                            sourceItem.Name,
                                            targetCategory.Name,
                                            targetItem.Name);

                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        // ------------------------------------------------

        private void InitializeMatrices(List<Category> categories)
        {
            for(int i = 0; i < categories.Count; i++)
            {
                for(int j = i + 1; j < categories.Count; j++)
                {
                    var firstCategory = categories[i];
                    var secondCategory = categories[j];

                    var key = new CategoryPairKey(firstCategory.Name, secondCategory.Name);

                    _matrices[key] = new GridMatrix(firstCategory.Items.Count, secondCategory.Items.Count);
                }
            }
        }

        // ------------------------------------------------

        private Category GetCategory(string categoryName)
        {
            if(_categoriesByName.TryGetValue(categoryName, out var category) == false)
            {
                throw new ArgumentException($"Unknown category: {categoryName}", nameof(categoryName));
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

            throw new ArgumentException($"Unknown item '{itemName}' in category '{category.Name}'.", nameof(itemName));
        }

        // ------------------------------------------------

        private GridMatrix GetMatrix(string categoryName1, string categoryName2)
        {
            var key = CreateMatrixKey(categoryName1, categoryName2);

            if(_matrices.TryGetValue(key, out var matrix) == false)
            {
                throw new InvalidOperationException($"No matrix exists for '{categoryName1}' and '{categoryName2}'.");
            }

            return matrix;
        }

        // ------------------------------------------------

        private CategoryPairKey CreateMatrixKey(string categoryName1, string categoryName2)
        {
            return new CategoryPairKey(categoryName1, categoryName2);
        }

        // ------------------------------------------------

        private void ValidateMatrixConsistency()
        {
            foreach(var entry in _matrices)
            {
                var key = entry.Key;
                var matrix = entry.Value;

                for(int row = 0; row < matrix.RowCount; row++)
                {
                    var yesCount = 0;
                    var unknownCount = 0;

                    for(int column = 0; column < matrix.ColumnCount; column++)
                    {
                        var state = matrix.GetCell(row, column);

                        if(state == CellState.Yes)
                        {
                            yesCount++;
                        }

                        if(state == CellState.Unknown)
                        {
                            unknownCount++;
                        }
                    }

                    if(yesCount > 1)
                    {
                        throw new InvalidOperationException(
                            $"Row contradiction in matrix '{key.FirstCategoryName}' x '{key.SecondCategoryName}'.");
                    }

                    if(yesCount == 0 && unknownCount == 0)
                    {
                        throw new InvalidOperationException(
                            $"Row has no possible match in matrix '{key.FirstCategoryName}' x '{key.SecondCategoryName}'.");
                    }
                }

                for(int column = 0; column < matrix.ColumnCount; column++)
                {
                    var yesCount = 0;
                    var unknownCount = 0;

                    for(int row = 0; row < matrix.RowCount; row++)
                    {
                        var state = matrix.GetCell(row, column);

                        if(state == CellState.Yes)
                        {
                            yesCount++;
                        }

                        if(state == CellState.Unknown)
                        {
                            unknownCount++;
                        }
                    }

                    if(yesCount > 1)
                    {
                        throw new InvalidOperationException(
                            $"Column contradiction in matrix '{key.FirstCategoryName}' x '{key.SecondCategoryName}'.");
                    }

                    if(yesCount == 0 && unknownCount == 0)
                    {
                        throw new InvalidOperationException(
                            $"Column has no possible match in matrix '{key.FirstCategoryName}' x '{key.SecondCategoryName}'.");
                    }
                }
            }
        }
    }
}