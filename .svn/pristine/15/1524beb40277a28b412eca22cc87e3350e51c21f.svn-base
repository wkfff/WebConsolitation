Ext.ns('App');

App.FxProgressManagerXHeader = 'X-Progress-TaskId';

/*
* Цепляется за события beforeajaxrequest, ajaxrequestcomplete и ajaxrequestexception,
* и создает окно индикации выполнения запроса, если для запроса задан параметр fxProgress.
*/
App.FxProgressManager = function () {
    /*
    * @private Множество выполняемых запросов за которыми ведется наблюдение.
    * Key - Id запроса (taskId), Value - окно индикации запроса App.ProgressWindow.
    */
    var activeRequests = {};

    /*
    * @private Создает идентификатор запроса.
    */
    var createTaskId = function () {
        var minNumber = 100,
            maxNumber = 1000000000;
        return minNumber + Math.floor(Math.random() * maxNumber);
    };

    var beginAjaxRequest = function (extraParams, o) {
        if (extraParams.fxProgressConfig) {
            var config = Ext.apply({}, extraParams.fxProgressConfig, { closeBy100Percent: o.isUpload });
            delete extraParams.fxProgressConfig;
            var taskId = createTaskId();
            activeRequests[taskId] = new App.ProgressWindow(config).start(taskId);
            if (o.isUpload) {
                // Для запросов передающих/получающих файлы, идентификатор задача передаем
                // через параметры, т.к. нет возможности задать заголовки для iframe
                extraParams[App.FxProgressManagerXHeader] = taskId;
            } else {
                o.headers[App.FxProgressManagerXHeader] = taskId;
            }
        }
    };

    var endAjaxRequest = function (success, extraParams, o) {
        var taskId;

        if (!o) {
            return;
        }
        
        if (o.isUpload) {
            taskId = extraParams[App.FxProgressManagerXHeader];
        } else {
            taskId = o.headers[App.FxProgressManagerXHeader];
        }

        if (!taskId) {
            return;
        }

        var pw = activeRequests[taskId];
        if (pw) {
            pw.close();
            delete activeRequests[taskId];
        }
    };

    // Перехватываем все вызовы BasicForm.submit
    Ext.form.BasicForm.prototype.submit = Ext.form.BasicForm.prototype.submit.createSequence(function () {
        Ext.apply(arguments[0], { isUpload: false, headers: {} });
        beginAjaxRequest(Ext.apply(this.baseParams || {}, arguments[0].params), arguments[0]);
    });

    Ext.Ajax.on('requestcomplete', function (conn, response, options) {
        if (options.headers['X-Progress-Status']) {
            return;
        }

        endAjaxRequest(true, {}, Ext.apply({}, options, { isUpload: false, headers: {} }));
    }, this);

    Ext.Ajax.on('requestexception', function (conn, response, options) {
        endAjaxRequest(false, {}, Ext.apply({}, options, { isUpload: false, headers: {} }));
    }, this);

    Ext.net.DirectEvent.on('beforeajaxrequest', function (control, eventType, action, extraParams, o) {
        beginAjaxRequest(extraParams, o);
    });

    Ext.net.DirectEvent.on('ajaxrequestcomplete', function (response, result, control, eventType, action, extraParams, o) {
        endAjaxRequest(true, extraParams, o);
    });

    Ext.net.DirectEvent.on('ajaxrequestexception', function (response, result, control, eventType, action, extraParams, o) {
        endAjaxRequest(false, extraParams, o);
    });
} ();

/*
* Окно индикации хода выполнения запроса.
* Параметры конфигурации: 
*   message - Заголовок операции.
*   canHide - может ли индикатор быть свернут, по умолчанию false.
*   interval - интервал опроса, по умолчаию 500 мс.
*   progressUrl - url менеджера фоновых процессов, по умолчанию '/Progress/Get'.
*/
App.ProgressWindow = function (config) {
    var that = this;

    Ext.apply(that, config, {
        operationTitle: 'Параметр operationTitle не задан',
        canHide: false,
        interval: 500,
        progressUrl: '/Progress/Get'
    });

    var progressCallback = function (status) {
        that.progress.updateProgress(status.Percentage / 100, status.Text, true);
        that.fireEvent('statusChanged', status, this);

        // Если производится загрузка файла с сервера в iframe, то закрываем окно 
        // при достижении 100%. Это вынужденный костыль, т.к. в случае успешной загрузки 
        // файла не стабатывает собылие load у iframe, во всех остальных случаях собылие 
        // срабатывает и закрытие окна произойдет штатным способом.
        if (status.Percentage == 100 && that.closeBy100Percent) {
            that.close();
        }
    };

    var internalProgressCallback = function () {
        that.timerId = window.setTimeout(internalProgressCallback, that.interval);
        Ext.Ajax.request({
            url: that.progressUrl,
            method: 'POST',
            headers: {
                'X-Progress-TaskId': that.taskId,
                'X-Progress-Status': true
            },
            success: function (status) {
                progressCallback.call(that.callbackScope, Ext.decode(status.responseText));
            }
        });
    };

    that.start = function (taskId) {
        that.taskId = taskId;
        that.timerId = window.setTimeout(internalProgressCallback, that.interval);

        return that;
    };

    that.label = new Ext.form.Label({ text: that.message });
    that.progress = new Ext.ProgressBar({ boxMaxHeight: 20, style: { marginTop: '10px'} });

    var buttons = [];
    if (that.canHide) {
        buttons = [{
            text: 'Скрыть',
            handler: this.onHide,
            scope: that
        }];
    }

    App.ProgressWindow.superclass.constructor.call(that, {
        title: 'Индикатор операции',
        iconCls: 'feed-icon',
        autoHeight: true,
        width: 500,
        resizable: true,
        plain: true,
        modal: true,
        autoScroll: true,
        closeAction: 'hide',
        hidden: false,
        border: false,
        padding: 5,

        buttons: buttons,

        items: [that.label, that.progress]
    });

    that.on('beforehide', function (c) {
        return c.canHide;
    });

    that.addEvents({
        statusChanged: true,
        completed: true
    });
};

/*
* Окно индикации хода выполнения запроса.
*/
Ext.extend(App.ProgressWindow, Ext.Window, {
    show: function () {
        if (this.rendered) {
        }
        App.ProgressWindow.superclass.show.apply(this, arguments);
    },

    onHide: function () {
        if (!this.hidden) {
            this.hide();
            window.ProgressMgr.attachIndicator(this);
        }
    },

    close: function () {
        this.taskId = 0;
        window.clearTimeout(this.timerId);

        this.canHide = true;
        App.ProgressWindow.superclass.close.apply(this, arguments);
    },

    getKey: function () {
        return this.progressFx._taskId;
    }
});