﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;

namespace OpcodeTools
{
	public partial class Form1 : Form
	{
        FormulasBase f;

        bool IgnoreTextChanged = false;

		public Form1()
		{
			InitializeComponent();
		}

		private void OpcodeDecBox_TextChanged(object sender, EventArgs e)
		{
            if (IgnoreTextChanged)
                return;

			uint opcode;
			if(!UInt32.TryParse(opcodeDecBox.Text, out opcode))
				return;

            opcodeHexBox.Text = String.Format("{0:X}", opcode);
			updateValuesForOpcode(opcode);
		}

        private void OpcodeHex_TextChanged(object sender, EventArgs e)
        {
            try 
	        {
                opcodeDecBox.Text = Convert.ToUInt32(opcodeHexBox.Text, 16).ToString();
	        }
	        catch (Exception)
	        {
	        }
        }
        private void CleanValues()
        {
            IgnoreTextChanged = true;
            cryptedBox.Text = specialBox.Text = offsetBox.Text = authBox.Text = "";
            IgnoreTextChanged = false;
        }

		void updateValuesForOpcode(uint opcode)
		{
            // start clean
            CleanValues();

            IgnoreTextChanged = true;
            if (f.IsAuthOpcode(opcode))
            {
                uint auth = f.CalcAuthFromOpcode(opcode);
                authBox.Text = auth.ToString();
            }
            else if (f.IsSpecialOpcode(opcode))
            {
                uint specialHandlerNum = f.CalcSpecialFromOpcode(opcode);
                specialBox.Text = String.Format("{0:x}", specialHandlerNum);
            }
            else if (f.IsNormalOpcode(opcode))
            {
                uint crypt = f.CalcCryptedFromOpcode(opcode);
                uint offset = f.CalcOffsetFromOpcode(opcode);
			    cryptedBox.Text = crypt.ToString();
			    offsetBox.Text = offset.ToString();
            }
            IgnoreTextChanged = false;
		}

        private void specialBox_TextChanged(object sender, EventArgs e)
        {
            if (IgnoreTextChanged)
                return;
            try
            {
                SetOpcode(f.CalcOpcodeFromSpecial(Convert.ToUInt32(specialBox.Text, 16)));
            }
            catch (Exception)
            {
            }
        }

        private void offsetBox_TextChanged(object sender, EventArgs e)
        {
            if (IgnoreTextChanged)
                return;
            try
            {
                SetOpcode(f.CalcOpcodeFromOffset(Convert.ToUInt32(offsetBox.Text)));

            }
            catch (Exception)
            {
            }
        }

        private void authBox_TextChanged(object sender, EventArgs e)
        {
            if (IgnoreTextChanged)
                return;
            try
            {
                SetOpcode(f.CalcOpcodeFromAuth(Convert.ToUInt32(authBox.Text)));
            }
            catch (Exception)
            {
            }
        }

        private void SetOpcode(uint opcode)
        {
            IgnoreTextChanged = true;
            opcodeHexBox.Text = String.Format("{0:X}", opcode);
            opcodeDecBox.Text = opcode.ToString();
            IgnoreTextChanged = false;
        }

        private void Versions_SelectedIndexChanged(object sender, EventArgs e)
        {
            f = (FormulasBase)Versions.SelectedItem;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (t.IsSubclassOf(typeof(FormulasBase)))
                {
                    object formulas = t.GetConstructor(new Type[] { }).Invoke(new Object[] { });
                    Versions.Items.Add(formulas);
                }
            }
            // choose the newest by default
            Versions.SelectedIndex = Versions.Items.Count - 1;
        }
	}
}
