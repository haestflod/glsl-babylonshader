using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace glsl_babylon.classes
{
    public class Watcher
    {
        /// <summary>
        /// The converter used to convert files after IO update event happens
        /// </summary>
        private IConverter m_converter;

        public Watcher(IConverter a_converter)
        {
            m_converter = a_converter;
        }
    }
}
