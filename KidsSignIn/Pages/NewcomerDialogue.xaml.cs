﻿using FirstFloor.ModernUI.Windows.Controls;
using System;
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

namespace KidsSignIn.Pages
{
    /// <summary>
    /// Interaction logic for NewcomerDialogue.xaml
    /// </summary>
    public partial class NewcomerDialogue : ModernDialog
    {
        public NewcomerDialogue()
        {
            InitializeComponent();

            DataContext = new NewcomerDialogueViewModel();

            // define the dialog buttons
            this.Buttons = new Button[] { this.OkButton, this.CancelButton };
        }
    }

    

}
