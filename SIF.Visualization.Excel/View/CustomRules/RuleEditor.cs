﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SIF.Visualization.Excel.Core;
using SIF.Visualization.Excel.Core.Rules;
using SIF.Visualization.Excel.Properties;

namespace SIF.Visualization.Excel.View.CustomRules
{
    public partial class RuleEditor : Form
    {
        private static volatile RuleEditor instance;
        private static readonly object syncRoot = new object();
        private static readonly object syncEditor = new object();

        private string[] avaibleConditions =
        {
            "Regex",
            Resources.tl_RuleEditor_Condition_CharacterCount
        };

        private readonly List<Panel> condiPanels = new List<Panel>();
        private Button deleteRowButton;

        public bool edited = false;
        private readonly int panelX = 10;
        private int panelY = 3;
        private int pointX;
        private int pointY;
        private int totalRows;


        /// <summary>
        ///     Singelton
        /// </summary>
        public static RuleEditor Instance
        {
            get
            {
                if (instance == null)
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new RuleEditor();
                    }
                return instance;
            }
        }

        /// <summary>
        ///     Calls the Rule Editor Interface
        /// </summary>
        public void Start()
        {
            lock (syncEditor)
            {
                InitializeComponent();
                SetDesigner();
                RuleCreator.Instance.BlankStart();
                Show();
            }
        }

        /// <summary>
        ///     Shows the Rule Editor Interface with an existing rule
        /// </summary>
        /// <param name="rule"></param>
        public void Open(Rule rule)
        {
            lock (syncEditor)
            {
                InitializeComponent();
                foreach (var existingCondition in rule.Conditions)
                    AddExistingRow(existingCondition);
                SetDesigner();
                UpdateInformations(rule);

                RuleCreator.Instance.OpenRule(rule);
                Show();
            }
        }

        /// <summary>
        ///     Sets the text and Tooltips to localized string, because Visual studio keeps resetting them
        /// </summary>
        private void SetDesigner()
        {
            //Buttons
            CancelButton.Text = Resources.tl_Cancel;
            ConfirmButton.Text = Resources.tl_RuleEditor_Confirm;
            ChooseCellButton.Text = Resources.tl_RuleEditor_CellPicker;
            NewConditionButton.Text = Resources.tl_RuleEditor_NewCondition;
            //Labels
            ConditionLabel.Text = Resources.tl_RuleEditor_Condition;
            RuleNameLabel.Text = "Name";
            RuleAreaLabel.Text = Resources.tl_RuleEditor_RuleArea;
            DescriptionLabel.Text = Resources.tl_RuleEditor_RuleDescription;
            //Tooltips
            ToolTipName.SetToolTip(TooltipLabelName, Resources.tl_RuleEditor_ToolTip_Name);
            ToolTipCellArea.SetToolTip(ToolTipLabelCellArea, Resources.tl_RuleEditor_ToolTip_CellArea);
            ToolTipDescription.SetToolTip(ToolTipLabelDescription, Resources.tl_RuleEditor_ToolTip_Description);
            ToolTipCondition.SetToolTip(TooltipLabelCondition, Resources.tl_RuleEditor_ToolTip_Condition);
        }

        /// <summary>
        ///     Updates the GUI with the details of the rule
        /// </summary>
        /// <param name="rule"></param>
        public void UpdateInformations(Rule rule)
        {
            if (RuleNameTextBox.Text == "")
                RuleNameTextBox.Text = rule.Title;
            if (RuleDescriptionTextBox.Text == "")
                RuleDescriptionTextBox.Text = rule.Description;
            if (rule.RuleCells != null)
            {
                var output = "";
                foreach (var rulecells in rule.RuleCells)
                    output = output + rulecells.Target;
                CellAreaBox.Text = output;
            }
        }


        /// <summary>
        ///     Opens the ConditionPicker Window and saves the current Inputs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NewConditionButton_Click(object sender, EventArgs e)
        {
            try
            {
                RuleCreator.Instance.SetProperties(RuleNameTextBox.Text, RuleDescriptionTextBox.Text);
                End();
                var conditionPicker = new ConditionPicker(RuleCreator.Instance.GetRule());
            }
            catch (Exception f)
            {
                MessageBox.Show(f.ToString());
            }
        }


        /// <summary>
        ///     Adds existing Conditions to the ConditionPanel
        /// </summary>
        /// <param name="existingCondition"></param>
        private void AddExistingRow(Condition existingCondition)
        {
            pointX = NewConditionButton.Location.X;
            pointY = NewConditionButton.Location.Y;
            totalRows = condiPanels.Count;

            // Creates Panel where everything is contained
            var condiPanel = new Panel();
            ConditionPanel.Controls.Add(condiPanel);
            condiPanels.Add(condiPanel);
            condiPanel.Location = new Point(panelX, panelY);
            condiPanel.Name = "panel" + totalRows;
            condiPanel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            condiPanel.Size = new Size(570, 50);
            condiPanel.BackColor = SystemColors.ControlDark;
            condiPanel.Padding = new Padding(10);
            panelY = panelY + 55;

            var condiButton = new Button();
            condiPanel.Controls.Add(condiButton);
            condiButton.Text = existingCondition.Name;
            condiButton.Size = new Size(150, 30);
            condiButton.AutoSize = true;
            condiButton.Anchor = AnchorStyles.Right;
            condiButton.Location = new Point(10, 10);
            condiButton.Click += condiButton_Click;
            // condition bearbeiten Button Event, param: condition


            // Creates the delete Button for the Panel
            deleteRowButton = new Button();
            condiPanel.Controls.Add(deleteRowButton);
            deleteRowButton.Margin = new Padding(10);
            deleteRowButton.Location = new Point(500, 10);
            deleteRowButton.Name = "delete" + totalRows;
            deleteRowButton.Size = new Size(30, 30);
            deleteRowButton.Image = Resources.delete;
            deleteRowButton.Click += deleteRowButton_Click;

            // Moves down newConditionButton 
            NewConditionButton.Location = new Point(pointX, pointY + 55);
        }

        /// <summary>
        ///     Opens existing Condition
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void condiButton_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            foreach (var condition in RuleCreator.Instance.GetRule().Conditions)
                if (condition.Name == button.Text)
                {
                    var conditionPicker = new ConditionPicker(condition);
                }
        }

        /// <summary>
        ///     Gets the panel where the Events was triggered and deletes its content
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void deleteRowButton_Click(object sender, EventArgs e)
        {
            try
            {
                var button = sender as Button;
                var parent = button.Parent as Panel;
                parent.Dispose();
            }
            catch
            {
                MessageBox.Show(e.ToString());
            }
        }


        /// <summary>
        ///     Checks and commits the Data with the RuleCreator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfirmButton_Click(object sender, EventArgs e)
        {
            // Check for Rulecells
            if (CheckInputs())
            {
                RuleCreator.Instance.SetProperties(RuleNameTextBox.Text, RuleDescriptionTextBox.Text);

                var newRule = RuleCreator.Instance.End();
                if (newRule != null)
                {
                    DataModel.Instance.CurrentWorkbook.Rules.Add(newRule);
                    End();
                }
                else
                {
                    MessageBox.Show("keine RuleCells");
                }
            }
        }

        /// <summary>
        ///     Checks the Rule Editor for empty or invalid inputs
        /// </summary>
        /// <returns></returns>
        private bool CheckInputs()
        {
            if (RuleCreator.Instance.GetRule().Conditions.Count <= 0)
            {
                MessageBox.Show(Resources.tl_RuleEditor_NoCondition);
                return false;
            }

            if (RuleNameTextBox.Text == "")
            {
                MessageBox.Show(Resources.tl_RuleEditor_NoName);
                return false;
            }
            if (RuleCreator.Instance.GetRule().RuleCells.Count <= 0)
            {
                MessageBox.Show(Resources.tl_RuleEditor_NoRuleCells);
                return false;
            }
            if (RuleDescriptionTextBox.Text == "")
            {
                MessageBox.Show(Resources.tl_RuleEditor_NoDescription);
                return true;
            }
            return true;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            //check for edited
            RuleCreator.Instance.End();
            End();
        }

        /// <summary>
        ///     Opens the CellChooser Window and saves current Input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChooseCellButton_Click(object sender, EventArgs e)
        {
            RuleCreator.Instance.SetProperties(RuleNameTextBox.Text, RuleDescriptionTextBox.Text);
            End();
            var cellpicker = new CellPickerWF();
        }

        public void End()
        {
            if (instance == null)
            {
            }
            lock (syncEditor)
            {
                instance = null;
                Close();
                Dispose();
            }
        }
    }
}