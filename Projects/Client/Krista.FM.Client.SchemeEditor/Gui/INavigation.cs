using System;
using System.Collections.Generic;
using System.Text;

namespace Krista.FM.Client.SchemeEditor.Gui
{
    /// <summary>
    /// Обеспечивает установку состояния клиента.
    /// </summary>
    public interface INavigation
    {
        // Этот метод уже был, там добавлена проверка, перехода с навигатора.
        void SelectObject(string name, bool isCorrectName);
    }
}