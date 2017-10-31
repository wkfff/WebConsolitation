Extension = {
    View: {
        // Возвращает объект рабочей области главного окна
        getWorkbench: function () {
            return parent.Workbench || parent.parent.Workbench || parent.parent.parent.Workbench || parent.parent.parent.parent.Workbench; ;
        }
    }
};
