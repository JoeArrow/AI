using System.Collections.Generic;
using Practical.AI.UnsupervisedLearning.Clustering;

namespace Practical.AI.UnsupervisedLearning
{
    public class DataSet
    {
        public List<Element> Objects { get; set; }
 
        public DataSet()
        {
            Objects = new List<Element>();
        }

        public void Load(List<Element> objects)
        {
            Objects = new List<Element>(objects);
        }
    }
}
