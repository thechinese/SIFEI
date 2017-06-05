﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using SIF.Visualization.Excel.Core;
using Binding = System.Windows.Data.Binding;
using Rule = SIF.Visualization.Excel.Core.Rules.Rule;
using UserControl = System.Windows.Controls.UserControl;

namespace SIF.Visualization.Excel.View
{
    /// <summary>
    /// Interaction logic for RuleListView.xaml
    /// </summary>
    public partial class RuleListView : UserControl
    {
        internal ListCollectionView RuleView { get; private set; }
        public RuleListView()
        {
            InitializeComponent();
            DataContextChanged += RuleListView_DataContextChanged;
        }

        private void RuleListView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DataContext == null)
                return;
            var binding = new Binding()
            {
                Source = DataModel.Instance.CurrentWorkbook.Rules,
                Mode = BindingMode.OneWay
            };
            RuleListBox.SetBinding(ItemsControl.ItemsSourceProperty, binding);
        }

        //TODO Edit + Delete Click

    }
}
