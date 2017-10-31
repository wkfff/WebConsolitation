﻿// Определяет интерфейс взаимодействия представления с родительским окном.
// Предоставляет возможность из родительского окна определять изменились ли
// данные в дочернем представлении и выполнять сохранение данных.
ViewPersistence = {

    // Определяет, все ли поля корректно заполнены в представлении.
    isValid: function () { return true; },

    // Определяет, что в представлении данные были изменены и еще не сохранены.
    isDirty: function () { return false; },

    // Определяет, что в представлении данные были изменены и необходимо
    // обновить данные родительской формы
    isChanged: false,

    // Выполняет сохранение данных в представлении.
    save: function (reload) { },

    // Событие вызывается после сохранения данных формы в представлении.
    onSave: function () { },

    // Выполняет обновление содержимого представления
    refresh: function () { },
    
    asynSave: false
};