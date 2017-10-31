namespace Krista.FM.Client.MDXExpert.Controls
{
    partial class MeasureEditor
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Infragistics.Win.ValueListItem valueListItem1 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem2 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem3 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem4 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem5 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem6 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem7 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem8 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem9 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem10 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem11 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem12 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem13 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem14 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem15 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem16 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem17 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem18 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem19 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem20 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem21 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem22 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem23 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem24 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem25 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem26 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem27 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.ValueListItem valueListItem28 = new Infragistics.Win.ValueListItem();
            Infragistics.Win.Appearance appearance1 = new Infragistics.Win.Appearance();
            Infragistics.Win.UltraWinEditors.SpinEditorButton spinEditorButton1 = new Infragistics.Win.UltraWinEditors.SpinEditorButton();
            this.ucbUnitDisplayType = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ucbValueAlignment = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ucbFormatType = new Infragistics.Win.UltraWinEditors.UltraComboEditor();
            this.ulFormatType = new Infragistics.Win.Misc.UltraLabel();
            this.ulValueAlignment = new Infragistics.Win.Misc.UltraLabel();
            this.ulUnitDisplayType = new Infragistics.Win.Misc.UltraLabel();
            this.uteDigitCount = new Infragistics.Win.UltraWinEditors.UltraTextEditor();
            this.ulDigitCount = new Infragistics.Win.Misc.UltraLabel();
            this.uchThousandDelemiter = new Infragistics.Win.UltraWinEditors.UltraCheckEditor();
            ((System.ComponentModel.ISupportInitialize)(this.ucbUnitDisplayType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ucbValueAlignment)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ucbFormatType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.uteDigitCount)).BeginInit();
            this.SuspendLayout();
            // 
            // ucbUnitDisplayType
            // 
            this.ucbUnitDisplayType.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            valueListItem1.DataValue = ((byte)(0));
            valueListItem1.DisplayText = "Не выводить";
            valueListItem2.DataValue = ((byte)(1));
            valueListItem2.DisplayText = "В столбце показателя";
            valueListItem3.DataValue = ((byte)(2));
            valueListItem3.DisplayText = "В заголовке показателя";
            this.ucbUnitDisplayType.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem1,
            valueListItem2,
            valueListItem3});
            this.ucbUnitDisplayType.Location = new System.Drawing.Point(541, 4);
            this.ucbUnitDisplayType.Name = "ucbUnitDisplayType";
            this.ucbUnitDisplayType.Size = new System.Drawing.Size(209, 21);
            this.ucbUnitDisplayType.TabIndex = 0;
            // 
            // ucbValueAlignment
            // 
            this.ucbValueAlignment.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            valueListItem4.DataValue = ((byte)(0));
            valueListItem4.DisplayText = "По левому краю";
            valueListItem5.DataValue = ((byte)(1));
            valueListItem5.DisplayText = "По центру";
            valueListItem6.DataValue = ((byte)(2));
            valueListItem6.DisplayText = "По правому краю";
            this.ucbValueAlignment.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem4,
            valueListItem5,
            valueListItem6});
            this.ucbValueAlignment.Location = new System.Drawing.Point(165, 35);
            this.ucbValueAlignment.Name = "ucbValueAlignment";
            this.ucbValueAlignment.Size = new System.Drawing.Size(144, 21);
            this.ucbValueAlignment.TabIndex = 1;
            // 
            // ucbFormatType
            // 
            this.ucbFormatType.DropDownStyle = Infragistics.Win.DropDownStyle.DropDownList;
            valueListItem7.DataValue = ((byte)(0));
            valueListItem7.DisplayText = "Автоматический";
            valueListItem8.DataValue = ((byte)(1));
            valueListItem8.DisplayText = "Общий";
            valueListItem9.DataValue = ((byte)(2));
            valueListItem9.DisplayText = "Экспоненциальный";
            valueListItem10.DataValue = ((byte)(3));
            valueListItem10.DisplayText = "Денежный";
            valueListItem11.DataValue = ((byte)(4));
            valueListItem11.DisplayText = "Денежный, тыс.р.";
            valueListItem12.DataValue = ((byte)(5));
            valueListItem12.DisplayText = "Денежный, тыс.р. без деления";
            valueListItem13.DataValue = ((byte)(6));
            valueListItem13.DisplayText = "Денежный, млн.р.";
            valueListItem14.DataValue = ((byte)(7));
            valueListItem14.DisplayText = "Денежный, млн.р. без деления";
            valueListItem15.DataValue = ((byte)(8));
            valueListItem15.DisplayText = "Денежный, млрд.р.";
            valueListItem16.DataValue = ((byte)(9));
            valueListItem16.DisplayText = "Денежный, млрд.р. без деления";
            valueListItem17.DataValue = ((byte)(10));
            valueListItem17.DisplayText = "Процентный";
            valueListItem18.DataValue = ((byte)(11));
            valueListItem18.DisplayText = "Числовой";
            valueListItem19.DataValue = ((byte)(12));
            valueListItem19.DisplayText = "Числовой, тыс.";
            valueListItem20.DataValue = ((byte)(13));
            valueListItem20.DisplayText = "Числовой, млн.";
            valueListItem21.DataValue = ((byte)(14));
            valueListItem21.DisplayText = "Числовой, млрд.";
            valueListItem22.DataValue = ((byte)(15));
            valueListItem22.DisplayText = "Дата время";
            valueListItem23.DataValue = ((byte)(16));
            valueListItem23.DisplayText = "Длинный формат даты";
            valueListItem24.DataValue = ((byte)(17));
            valueListItem24.DisplayText = "Длинный формат времени";
            valueListItem25.DataValue = ((byte)(18));
            valueListItem25.DisplayText = "Короткий формат даты";
            valueListItem26.DataValue = ((byte)(19));
            valueListItem26.DisplayText = "Короткий формат времени";
            valueListItem27.DataValue = ((byte)(20));
            valueListItem27.DisplayText = "Да/Нет";
            valueListItem28.DataValue = ((byte)(21));
            valueListItem28.DisplayText = "Истина/Ложь";
            this.ucbFormatType.Items.AddRange(new Infragistics.Win.ValueListItem[] {
            valueListItem7,
            valueListItem8,
            valueListItem9,
            valueListItem10,
            valueListItem11,
            valueListItem12,
            valueListItem13,
            valueListItem14,
            valueListItem15,
            valueListItem16,
            valueListItem17,
            valueListItem18,
            valueListItem19,
            valueListItem20,
            valueListItem21,
            valueListItem22,
            valueListItem23,
            valueListItem24,
            valueListItem25,
            valueListItem26,
            valueListItem27,
            valueListItem28});
            this.ucbFormatType.Location = new System.Drawing.Point(64, 4);
            this.ucbFormatType.Name = "ucbFormatType";
            this.ucbFormatType.Size = new System.Drawing.Size(245, 21);
            this.ucbFormatType.TabIndex = 2;
            // 
            // ulFormatType
            // 
            this.ulFormatType.AutoSize = true;
            this.ulFormatType.Location = new System.Drawing.Point(0, 11);
            this.ulFormatType.Name = "ulFormatType";
            this.ulFormatType.Size = new System.Drawing.Size(24, 14);
            this.ulFormatType.TabIndex = 3;
            this.ulFormatType.Text = "Тип";
            // 
            // ulValueAlignment
            // 
            this.ulValueAlignment.AutoSize = true;
            this.ulValueAlignment.Location = new System.Drawing.Point(0, 42);
            this.ulValueAlignment.Name = "ulValueAlignment";
            this.ulValueAlignment.Size = new System.Drawing.Size(135, 14);
            this.ulValueAlignment.TabIndex = 4;
            this.ulValueAlignment.Text = "Выравнивание значений";
            // 
            // ulUnitDisplayType
            // 
            this.ulUnitDisplayType.AutoSize = true;
            this.ulUnitDisplayType.Location = new System.Drawing.Point(329, 11);
            this.ulUnitDisplayType.Name = "ulUnitDisplayType";
            this.ulUnitDisplayType.Size = new System.Drawing.Size(179, 14);
            this.ulUnitDisplayType.TabIndex = 5;
            this.ulUnitDisplayType.Text = "Отображение единиц измерения";
            // 
            // uteDigitCount
            // 
            appearance1.TextHAlignAsString = "Right";
            this.uteDigitCount.Appearance = appearance1;
            this.uteDigitCount.ButtonsRight.Add(spinEditorButton1);
            this.uteDigitCount.Location = new System.Drawing.Point(705, 38);
            this.uteDigitCount.Name = "uteDigitCount";
            this.uteDigitCount.Size = new System.Drawing.Size(45, 21);
            this.uteDigitCount.TabIndex = 6;
            this.uteDigitCount.Text = "2";
            // 
            // ulDigitCount
            // 
            this.ulDigitCount.AutoSize = true;
            this.ulDigitCount.Location = new System.Drawing.Point(541, 42);
            this.ulDigitCount.Name = "ulDigitCount";
            this.ulDigitCount.Size = new System.Drawing.Size(141, 14);
            this.ulDigitCount.TabIndex = 7;
            this.ulDigitCount.Text = "Число десятичных знаков";
            // 
            // uchThousandDelemiter
            // 
            this.uchThousandDelemiter.Location = new System.Drawing.Point(329, 42);
            this.uchThousandDelemiter.Name = "uchThousandDelemiter";
            this.uchThousandDelemiter.Size = new System.Drawing.Size(209, 14);
            this.uchThousandDelemiter.TabIndex = 8;
            this.uchThousandDelemiter.Text = "Разделитель групп разрядов";
            // 
            // MeasureEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.uchThousandDelemiter);
            this.Controls.Add(this.ulDigitCount);
            this.Controls.Add(this.uteDigitCount);
            this.Controls.Add(this.ulUnitDisplayType);
            this.Controls.Add(this.ulValueAlignment);
            this.Controls.Add(this.ulFormatType);
            this.Controls.Add(this.ucbFormatType);
            this.Controls.Add(this.ucbValueAlignment);
            this.Controls.Add(this.ucbUnitDisplayType);
            this.MinimumSize = new System.Drawing.Size(0, 66);
            this.Name = "MeasureEditor";
            this.Size = new System.Drawing.Size(755, 66);
            ((System.ComponentModel.ISupportInitialize)(this.ucbUnitDisplayType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ucbValueAlignment)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ucbFormatType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.uteDigitCount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Infragistics.Win.UltraWinEditors.UltraComboEditor ucbUnitDisplayType;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor ucbValueAlignment;
        private Infragistics.Win.UltraWinEditors.UltraComboEditor ucbFormatType;
        private Infragistics.Win.Misc.UltraLabel ulFormatType;
        private Infragistics.Win.Misc.UltraLabel ulValueAlignment;
        private Infragistics.Win.Misc.UltraLabel ulUnitDisplayType;
        private Infragistics.Win.UltraWinEditors.UltraTextEditor uteDigitCount;
        private Infragistics.Win.Misc.UltraLabel ulDigitCount;
        private Infragistics.Win.UltraWinEditors.UltraCheckEditor uchThousandDelemiter;
    }
}
