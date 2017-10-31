using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Krista.FM.Client.OLAPStructures;
using Microsoft.AnalysisServices;
using CommandType=Krista.FM.Client.OLAPStructures.CommandType;

namespace Krista.FM.Client.OLAPAdmin
{
    public partial class WizardCreatePatch : Form
    {
        private Microsoft.AnalysisServices.Server server = null;
        private Dictionary<string, ObjectForScript> items;

        public WizardCreatePatch()
        {
            InitializeComponent();
        }


        public WizardCreatePatch(Microsoft.AnalysisServices.Server server, Dictionary<string, ObjectForScript> items)
            : this()
        {
            this.server = server;
            this.items = items;

            Initialize();

            databases.Enabled = false;
            dvsList.Enabled = false;
        }

        private void Initialize()
        {
            var temp = new Dictionary<string, ObjectForScript>(); 
            foreach (ObjectForScript obj in items.Values)
            {
                // если измереню делаем альтер, то подцепляем зависимые кубы
                if (obj.Obj is Dimension && obj.CommandType == CommandType.alter)
                {
                    foreach (Cube cube in ((Dimension)obj.Obj).Parent.Cubes)
                    {
                        if (cube.Dimensions.ContainsName(obj.Obj.Name) && !temp.ContainsKey(cube.Name))
                            temp.Add(cube.Name, new ObjectForScript(CommandType.alter, cube, string.Format("{0}(зависимый)", cube.Name)));
                    }
                }
            }

            foreach (KeyValuePair<string, ObjectForScript> objectForScript in temp)
            {
                if (!items.ContainsKey(objectForScript.Key))
                    items.Add(objectForScript.Key, objectForScript.Value);
            }
            objectsList.Items.Clear();

            foreach (KeyValuePair<string, ObjectForScript> item in items)
            {
                objectsList.Items.Add(
                    String.Format("{0}, {1}, {2}", item.Key, item.Value.ObjectType, item.Value.CommandType));
            }

            databases.Items.Clear();
            if (server != null)
            {
                foreach (Database databas in server.Databases)
                {
                    databases.Items.Add(String.Format("{0}", databas.Name));
                }
            }
        }

        private void btCreate_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked && dvsList.SelectedItems.Count != 0)
            {
                foreach (object o in dvsList.SelectedItems)
                {
                    ObjectForScript obj = new ObjectForScript(CommandType.alter, server.Databases[databases.SelectedItem.ToString()].DataSourceViews[o.ToString()], "Представление данных");    
                    items.Add(obj.Obj.Name, obj);
                }
            }

            ICommand command = new ComplexScriptCommand(items);
            command.Execute();

            Close();
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                databases.Enabled = true;
                dvsList.Enabled = true;
            }
            else
            {
                databases.Enabled = false;
                dvsList.Enabled = false;
            }
        }

        private void databases_SelectedIndexChanged(object sender, EventArgs e)
        {
            ListBox list = sender as ListBox;

            if (list != null)
            {
                string s = list.SelectedItem.ToString();

                dvsList.Items.Clear();

                foreach (DataSourceView view in server.Databases[s].DataSourceViews)
                {
                    dvsList.Items.Add(String.Format("{0}", view.Name));
                }
            }
        }
    }
}