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
    public sealed class Category :ICategory
    {
        public string Name
        {
            set;  get;
        }

        public List<CategoryItem> Items
        {
            get;
        }

        public Category(string name, List<CategoryItem> items)
        {
            Name = name;
            Items = items;
        }
    }
}
