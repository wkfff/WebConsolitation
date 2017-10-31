using System;

namespace Krista.FM.Server.Dashboards.Core.QueryGenerators
{
    /// <summary>
    /// Генератор конструкции Descendants для задания множества элементов
    /// </summary>
    public class DescendantsGenerator
    {
        private string dimension;
        private string rootName;
        private string levelName;
        private string flag;

        public string Dimension
        {
            get { return dimension; }
        }

        /// <summary>
        /// Генератор конструкции Descendants
        /// </summary>
        /// <param name="dimension">измерение элементов</param>
        /// <param name="rootName">корневой элемент</param>
        /// <param name="levelName">нижний уровень потомков</param>
        /// <param name="flag">флаг</param>
        public DescendantsGenerator(string dimension, string rootName, string levelName, string flag)
        {
            this.dimension = dimension;
            this.rootName = rootName;
            this.levelName = levelName;
            this.flag = flag;
        }

        public override string ToString()
        {
            return String.Format("Descendants({0}.[{1}], {0}.[{2}], {3})", dimension, rootName, levelName, flag);
        }
    }
}
