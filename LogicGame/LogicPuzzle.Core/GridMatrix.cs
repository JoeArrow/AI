#region © 2026 Joe Arrowood (JoeWare)
//
// All rights reserved. Reproduction or transmission in whole or in part, in
// any form or by any means, electronic, mechanical, or otherwise, is prohibited
// without the prior written consent of the copyright owner.
//
#endregion

using System;

namespace LogicPuzzle.Core
{
    public sealed class GridMatrix
    {
        private readonly CellState[,] _cells;

        public int RowCount
        {
            get;
        }

        // ------------------------------------------------

        public int ColumnCount
        {
            get;
        }

        // ------------------------------------------------

        public GridMatrix(int rowCount, int columnCount)
        {
            RowCount = rowCount;
            ColumnCount = columnCount;
            _cells = new CellState[rowCount, columnCount];

            for(int row = 0; row < rowCount; row++)
            {
                for(int column = 0; column < columnCount; column++)
                {
                    _cells[row, column] = CellState.Unknown;
                }
            }
        }

        // ------------------------------------------------

        public CellState GetCell(int row, int column)
        {
            ValidateCoordinates(row, column);
            return _cells[row, column];
        }

        // ------------------------------------------------

        public void SetCell(int row, int column, CellState state)
        {
            ValidateCoordinates(row, column);
            _cells[row, column] = state;
        }

        // ------------------------------------------------

        private void ValidateCoordinates(int row, int column)
        {
            if(row < 0 || row >= RowCount)
            {
                throw new ArgumentOutOfRangeException(nameof(row));
            }

            if(column < 0 || column >= ColumnCount)
            {
                throw new ArgumentOutOfRangeException(nameof(column));
            }
        }
    }
}