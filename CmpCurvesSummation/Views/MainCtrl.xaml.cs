﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CmpCurvesSummation.ViewModels;

namespace CmpCurvesSummation.Views
{
    /// <summary>
    /// Interaction logic for MainCtrl.xaml
    /// </summary>
    public partial class MainCtrl : UserControl
    {
        private MainCtrlViewModel _viewModel;


        public MainCtrl()
        {
            InitializeComponent();
            _viewModel = new MainCtrlViewModel();
            DataContext = _viewModel;

            SetUpEvents();
            SetUpAutoSummation(false);
        }

        private void SetUpAutoSummation(bool auto)
        {
            OptionsControl.ViewModel.AutoSummation = auto;
        }

        private void SetUpEvents()
        {
            ToolbarControl.ViewModel.FileOpened += ProcessingControl.ViewModel.OnFileLoaded;
            ToolbarControl.ViewModel.FileOpened += OptionsControl.ViewModel.OnFileLoaded;
            ProcessingControl.ViewModel.RawCmpDataProcessed += CmpScanControl.ViewModel.OnRawCmpDataProcessed;
            ProcessingControl.ViewModel.RawCmpDataProcessed += SummedOverCurveScanControl.ViewModel.OnRawCmpDataProcessed;
            ProcessingControl.ViewModel.RawCmpDataProcessed += OptionsControl.ViewModel.OnRawCmpDataProcessed;
            SummedOverCurveScanControl.ViewModel.HodographDrawClick += CmpScanControl.ViewModel.OnHodographDrawClick;
            SummedOverCurveScanControl.ViewModel.HodographDrawClick += LayersInfoControl.ViewModel.OnHodographDrawClick;
            LayersInfoControl.ViewModel.DeleteClick += CmpScanControl.ViewModel.OnDeleteClick;
            LayersInfoControl.ViewModel.DeleteClick += SummedOverCurveScanControl.ViewModel.OnDeleteClick;
            OptionsControl.ViewModel.SummationClick += SummedOverCurveScanControl.ViewModel.OnSummationClick;
            OptionsControl.ViewModel.AutoSumCheckEvent += SummedOverCurveScanControl.ViewModel.OnAutoSummationChange;
            OptionsControl.ViewModel.PaletteChanged += CmpScanControl.ViewModel.OnPaletteChanged;
            OptionsControl.ViewModel.PaletteChanged += SummedOverCurveScanControl.ViewModel.OnPaletteChanged;
            OptionsControl.ViewModel.StepDistanceChanged += CmpScanControl.ViewModel.OnStepDistanceChanged;
            OptionsControl.ViewModel.StepTimeChanged += CmpScanControl.ViewModel.OnStepTimeChanged;
        }
    }
}