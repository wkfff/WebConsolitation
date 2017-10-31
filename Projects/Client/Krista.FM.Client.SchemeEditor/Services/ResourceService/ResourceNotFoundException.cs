using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Client.SchemeEditor.Services
{
    /// <summary>
    /// Генерируется, когда менеджер ресурсов не может найти запрошенный ресурс.
    /// </summary>
    public class ResourceNotFoundException : CoreException
    {
        public ResourceNotFoundException(string resource)
            : base("Resource not found : " + resource)
        {
        }
    }
}
