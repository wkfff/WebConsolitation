using System;
using System.Diagnostics;

using Krista.FM.Server.Common;
using Krista.FM.Server.Users;

using Krista.FM.ServerLibrary;

namespace Krista.FM.Server.Scheme.Classes
{
	/// <summary>
	/// ����� ����� ������ ��� ��������� ������� �����.
	/// </summary>
    [DebuggerStepThrough]
    internal abstract class CommonObject : KeyIdentifiedObject, ICommonObject
	{
        // ��� �������
        private string name;
		// �������� �������
        private string description = String.Empty;
        // �������� �������
        private string caption = String.Empty;
		// XML ������������ �������
        private string xmlConfig = String.Empty;

		/// <summary>
		/// ���������� �������� ������
		/// </summary>
		/// <param name="key"></param>
		/// <param name="owner"></param>
		/// <param name="name">��� �������</param>
        /// <param name="state"></param>
        public CommonObject(string key, ServerSideObject owner, string name, ServerSideObjectStates state)
            : base(key, owner, state)
		{
            this.name = name;
		}

        /// <summary>
        /// ����������� ������� � ������� ����� ����
        /// </summary>
        public virtual void RegisterObject(string objectName, string objectCaption, SysObjectsTypes sysObjectsType)
        {
            try
            {
                ((UsersManager)SchemeClass.Instance.UsersManager).RegisterSystemObject(objectName, objectCaption, sysObjectsType);
            }
            catch (Exception e)
            {
                Trace.WriteLine(String.Format("��������� ���������������� ������ {0}: {1}", objectName, e));
            }
        }

        /// <summary>
        /// �������� ������� � ������� ����� ����
        /// </summary>
        /// <param name="objectName"></param>
        public virtual void UnRegisterObject(string objectName)
        {
            try
            {
                ((UsersManager)SchemeClass.Instance.UsersManager).UnregisterSystemObject(objectName);
            }
            catch (Exception e)
            {
                Trace.WriteLine(String.Format("�� ������� ������� ������ {0}: {1}", objectName, e));
            }
		}

		/// <summary>
		/// XML ������������ �������
		/// </summary>
		public virtual string Configuration
		{
            [DebuggerStepThrough]
            get { return xmlConfig; }
            [DebuggerStepThrough]
            set { xmlConfig = value; }
		}

		#region ���������� ICommonObject

        /// <summary>
		/// ���������� ���������� ��� �������
		/// </summary>
        public virtual string Name 
		{
            [DebuggerStepThrough]
            get { return name; }
            set
            {
				// ������� �� �� ������, ��...
                if (!(this is Document) && !(this is Package) && !(this is EntityAssociation)) 
                    ScriptingEngine.ScriptingEngineHelper.CheckDBName(value);
                name = value;
            }
        }

        /// <summary>
        /// ������ ��� �������.
        /// </summary>
        public virtual string FullName
        {
            [DebuggerStepThrough]
            get { return name; }
        }

        /// <summary>
        /// ���������� ������������ ������� (������ ����).
        /// ��� �������� ����� ������� ������ 2.4.1 ���������� �������.
        /// </summary>
        public override string ObjectOldKeyName
        {
            get { return FullName; }
        }

        /// <summary>
        /// ������ ��� ������� � ���� ������
        /// </summary>
        public virtual string FullDBName
        {
            [DebuggerStepThrough]
            get { return FullName.Replace('.', '_'); }
        }

        /// <summary>
		/// ���������� ��������� ������������ �������
		/// </summary>
		public virtual bool IsValid
		{ 
			get	{ return true; }
		}
		
		/// <summary>
		/// ������� ������������ ������� ��������� � ����������
		/// </summary>
		public virtual string Caption
		{
            [DebuggerStepThrough]
            get { return caption; }
			set	{ caption = value; }
		}

        /// <summary>
		/// ��������� �������� ������� ��������� � ����������
		/// </summary>
		public virtual string Description
		{
            [DebuggerStepThrough]
            get { return description; }
			set	
            { 
                description = value;
                if (!String.IsNullOrEmpty(description))
                    if (description[description.Length - 1] != '.')
                        description += '.';
            }
		}

        /// <summary>
        /// XML ������������ �������
        /// </summary>
        public virtual string ConfigurationXml
        {
            get { return String.Empty; }
        }

        #endregion
    }
}
