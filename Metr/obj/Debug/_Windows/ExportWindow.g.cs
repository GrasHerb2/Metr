﻿#pragma checksum "..\..\..\_Windows\ExportWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "005F06D16295820F5B2CAE0FB9B2B980E1FDD2438EFD8D51FBAD4E5DB3907127"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using Metr.Classes;
using Metr._Windows;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Metr._Windows {
    
    
    /// <summary>
    /// ExportWindow
    /// </summary>
    public partial class ExportWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 17 "..\..\..\_Windows\ExportWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ProgressBar loadingProgress;
        
        #line default
        #line hidden
        
        
        #line 20 "..\..\..\_Windows\ExportWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox TableTypeCmB;
        
        #line default
        #line hidden
        
        
        #line 23 "..\..\..\_Windows\ExportWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox SearchUseChB;
        
        #line default
        #line hidden
        
        
        #line 28 "..\..\..\_Windows\ExportWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox OriginTableCmB;
        
        #line default
        #line hidden
        
        
        #line 36 "..\..\..\_Windows\ExportWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.CheckBox ObjSortChB;
        
        #line default
        #line hidden
        
        
        #line 40 "..\..\..\_Windows\ExportWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.DataGrid ColumnsDG;
        
        #line default
        #line hidden
        
        
        #line 59 "..\..\..\_Windows\ExportWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button saveBtn;
        
        #line default
        #line hidden
        
        
        #line 60 "..\..\..\_Windows\ExportWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button delBtn;
        
        #line default
        #line hidden
        
        
        #line 61 "..\..\..\_Windows\ExportWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button expBtn;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Metr;component/_windows/exportwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\_Windows\ExportWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.loadingProgress = ((System.Windows.Controls.ProgressBar)(target));
            return;
            case 2:
            this.TableTypeCmB = ((System.Windows.Controls.ComboBox)(target));
            
            #line 20 "..\..\..\_Windows\ExportWindow.xaml"
            this.TableTypeCmB.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.TableTypeCmB_SelectionChanged);
            
            #line default
            #line hidden
            return;
            case 3:
            this.SearchUseChB = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 4:
            this.OriginTableCmB = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 5:
            this.ObjSortChB = ((System.Windows.Controls.CheckBox)(target));
            return;
            case 6:
            this.ColumnsDG = ((System.Windows.Controls.DataGrid)(target));
            return;
            case 7:
            this.saveBtn = ((System.Windows.Controls.Button)(target));
            
            #line 59 "..\..\..\_Windows\ExportWindow.xaml"
            this.saveBtn.Click += new System.Windows.RoutedEventHandler(this.saveBtn_Click);
            
            #line default
            #line hidden
            return;
            case 8:
            this.delBtn = ((System.Windows.Controls.Button)(target));
            
            #line 60 "..\..\..\_Windows\ExportWindow.xaml"
            this.delBtn.Click += new System.Windows.RoutedEventHandler(this.delBtn_Click);
            
            #line default
            #line hidden
            return;
            case 9:
            this.expBtn = ((System.Windows.Controls.Button)(target));
            
            #line 61 "..\..\..\_Windows\ExportWindow.xaml"
            this.expBtn.Click += new System.Windows.RoutedEventHandler(this.expBtn_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

