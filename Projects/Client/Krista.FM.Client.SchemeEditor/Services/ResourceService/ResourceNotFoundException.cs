using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Client.SchemeEditor.Services
{
    /// <summary>
    /// ������������, ����� �������� �������� �� ����� ����� ����������� ������.
    /// </summary>
    public class ResourceNotFoundException : CoreException
    {
        public ResourceNotFoundException(string resource)
            : base("Resource not found : " + resource)
        {
        }
    }
}
