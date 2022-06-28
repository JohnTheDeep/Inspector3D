using System;
using System.Windows;
using System.Windows.Input;
using Inspector.ViewModel;
using Inspector.Services;

namespace Inspector
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = new MainWindowViewModel(new JsonFileService(),new ModelService(new DialogService(),new JsonFileService()),this);
        }
        private void viewPort3d_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            //Перенести в VM, заменить if else на че то другое s.
            if (e.RightButton == MouseButtonState.Pressed) dragButtonImage.Opacity = 0.2;
            else if (e.MiddleButton == MouseButtonState.Released) rotationButtonImage.Opacity = 1;
            if (e.MiddleButton == MouseButtonState.Pressed) rotationButtonImage.Opacity = 0.2;
            else if(e.RightButton == MouseButtonState.Released) dragButtonImage.Opacity = 1;    
        }
    }
}
