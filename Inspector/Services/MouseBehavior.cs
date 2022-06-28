using Inspector.Services.Commands;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Inspector.Services
{
    public class MouseBehavior : Behavior<Panel>
    {
       
        public static readonly DependencyProperty MouseYProperty = DependencyProperty.Register(
        "MouseY", typeof(double), typeof(MouseBehavior), new PropertyMetadata(default(double)));

        public static readonly DependencyProperty MouseXProperty = DependencyProperty.Register(
           "MouseX", typeof(double), typeof(MouseBehavior), new PropertyMetadata(default(double)));

        public static readonly DependencyProperty MouseMoveProperty = DependencyProperty.Register(
         "MouseMoveCommand", typeof(RelayCommand), typeof(MouseBehavior), new PropertyMetadata(new PropertyChangedCallback(MouseMoveCommandChanged)));
        static void MouseMoveCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement el = d as FrameworkElement;
            if (el != null)
            {
                el.MouseMove += el_MouseMove;
            }
        }
        static void el_MouseMove(object sender, MouseEventArgs e)
        {

            FrameworkElement el = (FrameworkElement)sender;
            if (el != null)
            {
                ICommand command = GetMouseMoveCommand(el);
                command.Execute(e);
            }
        }
        public static void SetMouseMoveCommand(UIElement d, ICommand value)
        {
            d.SetValue(MouseMoveProperty, value);
        }

        public static ICommand GetMouseMoveCommand(UIElement el)
        {
            return (ICommand)el.GetValue(MouseMoveProperty);
        }
        public double MouseY
        {
            get => (double)GetValue(MouseYProperty);
            set => SetValue(MouseYProperty, value);
        }

        public double MouseX
        {
            get => (double)GetValue(MouseXProperty); 
            set => SetValue(MouseXProperty, value);
        }
        protected override void OnAttached()
        {
            AssociatedObject.MouseMove += AssociatedObjectOnMouseMove;
        }

        private void AssociatedObjectOnMouseMove(object sender, MouseEventArgs mouseEventArgs)
        {
            var pos = mouseEventArgs.GetPosition(AssociatedObject);
            MouseX = pos.X;
            MouseY = pos.Y;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.MouseMove -= AssociatedObjectOnMouseMove;
        }
    }
}
