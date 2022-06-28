using das = GalaSoft.MvvmLight.Command;
using Inspector.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Media3D;
using System.Windows.Media;
using HelixToolkit.Wpf;
using System.Windows.Threading;
using System.Windows.Controls;
using Inspector.Services.Interfaces;
using Inspector.ViewModel.Base;
using Inspector.Services.Commands;

namespace Inspector.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region Properties
        private Point _mousePt;
        public Point mousePt
        {
            get => _mousePt;
            set => Set(ref _mousePt, value);
        }

        private double _panelX;
        public double PanelX
        {
            get => _panelX;
            set => Set(ref _panelX, value);
        }

        private double _panelY;
        public double PanelY
        {
            get => _panelY;
            set => Set(ref _panelY, value);
        }

        private ModelElement _selectedModel;
        public ModelElement SelectedModel
        {
            get => _selectedModel;
            set => Set(ref _selectedModel, value);
        }

        private bool _autoRotatePopUpState = false;
        public bool AutoRotatePopUpState
        {
            get => _autoRotatePopUpState;
            set => Set(ref _autoRotatePopUpState, value);
        }

        private ModelVisual3D _visualModel;
        public ModelVisual3D VisualModel
        {
            get => _visualModel;
            set => Set(ref _visualModel, value);
        }

        private ObservableCollection<ModelElement> _elements;
        public ObservableCollection<ModelElement> Elements
        {
            get => _elements;
            set => Set(ref _elements, value);
        }

        private Model3DGroup _myModel;
        public Model3DGroup MyModel
        {
            get => _myModel;
            set
            {
                Set(ref _myModel, value);
                ModelCollectionHandler(this, new EventArgs());
            }
        }
        #endregion

        #region Boolean
        private bool CanSelect = true;

        private bool IsHiden = false;
        #endregion

        #region Variables
        //Rotation
        private double angle = 0;
        public double RotateSpeed;
        private DispatcherTimer autoRotateTimer;
        private DispatcherTimer dispatcherTimer;

        //Selected elements
        private DiffuseMaterial selectedModelMaterial = new DiffuseMaterial(new SolidColorBrush(Colors.Black));
        private GeometryModel3D selectedGeometryModel;
        private Material standartMaterial;
        private GeometryModel3D hitGeometry;

        //Other
        private GridLength _column2Length;
        private readonly Window _mainWindow;

        //Services
        private readonly IFileService _fileService;
        private readonly IModelService _modelService;

        //Events
        public event EventHandler ModelCollectionHandler;
        #endregion

        #region Commands
        private RelayCommand _autoRotatePopUpStateCommand;
        public RelayCommand AutoRotatePopUpStateCommand
        {
            get
            {
                return _autoRotatePopUpStateCommand ?? (_autoRotatePopUpStateCommand = new RelayCommand(obj =>
                {
                    if (AutoRotatePopUpState)
                        AutoRotatePopUpState = false;
                    else AutoRotatePopUpState = true;
                }));
            }
        }

        private RelayCommand _setAutoRotateSpeedCommand;
        public RelayCommand SetAutoRotateSpeedCommand
        {
            get
            {
                return _setAutoRotateSpeedCommand ?? (_setAutoRotateSpeedCommand = new RelayCommand(obj =>
                {
                    SetAutoRotateSpeed();
                }));
            }
        }

        private RelayCommand _hideOrOpenTabCommand = null;
        public RelayCommand HideOrOpenTabCommand
        {
            get
            {
                return _hideOrOpenTabCommand ?? (_hideOrOpenTabCommand = new RelayCommand(obj =>
                {
                    dispatcherTimer.Start();
                }));
            }
        }

        private RelayCommand _openModelCommand = null;
        public RelayCommand OpenModelCommand
        {
            get
            {
                return _openModelCommand ?? (_openModelCommand = new RelayCommand(obj =>
                {
                    MyModel = _modelService.open();
                }));
            }
        }

        private RelayCommand _viewPort3dMouseMoveCommand = null;
        public RelayCommand ViewPort3dMouseMoveCommand
        {
            get
            {
                return _viewPort3dMouseMoveCommand ?? (_viewPort3dMouseMoveCommand = new RelayCommand(obj =>
                {
                    viewPort3d_MouseMove();
                }
                ));
            }
        }

        private RelayCommand _viewPofrt3d_PreviewMouseDown = null;
        public RelayCommand ViewPofrt3d_PreviewMouseDown
        {
            get
            {
                return _viewPofrt3d_PreviewMouseDown ?? (_viewPofrt3d_PreviewMouseDown = new RelayCommand(obj =>
                {
                    viewPort3d_PreviewMouseDown();
                }
                ));
            }
        }
        #endregion

        public MainWindowViewModel(IFileService fileService = null, IModelService modelService = null, Window window = null)
        {
            this._fileService = fileService;
            this._modelService = modelService;
            this._mainWindow = window;
            this.SetTimer();

            _column2Length = new GridLength(((MainWindow)_mainWindow).column2.Width.Value);

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            dispatcherTimer.Tick += TimerNewTick;

            ModelCollectionHandler += FillCollection;
        }

        /// <summary>
        /// Метод для отслеживания позиции курсора на модели и ее элементах
        /// </summary>
        public HitTestResultBehavior HTResult(HitTestResult rawresult)
        {
            if ((rawresult as RayMeshGeometry3DHitTestResult) != null)
            {
                string name = (rawresult as RayMeshGeometry3DHitTestResult)?.ModelHit?.GetName();

                if (name != "" && Elements != null)
                {
                    foreach (var v in Elements)
                    {
                        if (v.ModelName == name?.ToString())
                        {
                            SelectedModel = v;
                            ListBox lb = ((MainWindow)_mainWindow).modelElementsList;
                            foreach (var s in lb.Items)
                            {
                                if (s == v)
                                    lb.SelectedValue = s;
                            }
                        }
                    }
                }
            }
            else SelectedModel = null;
            return HitTestResultBehavior.Stop;

        }
        /// <summary>
        /// Метод для отслеживания клика курсора на модели и ее элементах
        /// </summary>
        public HitTestResultBehavior HTResultClick(HitTestResult rawResult)
        {
            hitGeometry = (rawResult as RayMeshGeometry3DHitTestResult)?.ModelHit as GeometryModel3D;
            if (hitGeometry != null)
            {
                if (CanSelect)
                {
                    standartMaterial = hitGeometry.Material;
                    selectedGeometryModel = hitGeometry;
                    changeMaterialToSelectedMouse();
                }
                else { CanSelect = true; changeMaterialToDefaultMouse(); }
            }
            return HitTestResultBehavior.Stop;
        }

        private void TimerNewTick(object sender, EventArgs e)
        {
            ColumnDefinition column2 = ((MainWindow)_mainWindow).column2;

            if (IsHiden)
            {
                column2.Width = new GridLength((double)column2.Width.Value + 5);
                if (column2.Width.Value == _column2Length.Value)
                {
                    dispatcherTimer.Stop();
                    IsHiden = false;
                }
            }
            else
            {
                column2.Width = new GridLength((double)column2.Width.Value - 5);
                if (column2.Width.Value == 0)
                {
                    dispatcherTimer.Stop();
                    IsHiden = true;
                }
            }
        }

        private void autoRotateTimer_Tick(object sender, EventArgs e)
        {
            if (this.angle >= 360) this.angle = 0;

            this.angle = this.angle + RotateSpeed;
            if (MyModel.Transform is RotateTransform3D rotateTransform3 && rotateTransform3.Rotation is AxisAngleRotation3D rotation)
                rotation.Angle = this.angle;
            else
            {
                MyModel.Transform = new RotateTransform3D()
                {
                    Rotation = new AxisAngleRotation3D()
                    {
                        Axis = new Vector3D(0, 1, 0),
                        Angle = this.angle,
                    }
                };
            }
        }
        private void hitParametersHover()
        {
            mousePt = (new Point(PanelX, PanelY));
            PointHitTestParameters ptParams = new PointHitTestParameters(mousePt);
            VisualTreeHelper.HitTest(((MainWindow)_mainWindow).viewPort3d, null, HTResult, ptParams);
        }

        private void hitParametersClick()
        {
            mousePt = (new Point(PanelX, PanelY));
            PointHitTestParameters ptParams = new PointHitTestParameters(mousePt);
            VisualTreeHelper.HitTest(((MainWindow)_mainWindow).viewPort3d, null, HTResultClick, ptParams);
        }

        public void SetAutoRotateSpeed()
        {
            if (MyModel != null)
            {
                if (((MainWindow)_mainWindow).autoRotateSlider.Value != default(double)) { this.autoRotateTimer.Start(); RotateSpeed = ((MainWindow)_mainWindow).autoRotateSlider.Value; ((MainWindow)_mainWindow).autoRotatePopUp.IsOpen = false; }
                if (((MainWindow)_mainWindow).autoRotateSlider.Value == default(double)) { this.autoRotateTimer.Stop(); RotateSpeed = default(double); }
            }
        }
        public void SetTimer()
        {
            this.autoRotateTimer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(10) };
            this.autoRotateTimer.Tick += autoRotateTimer_Tick;
        }

        private void viewPort3d_MouseMove() => hitParametersHover();

        private void viewPort3d_PreviewMouseDown() => hitParametersClick();

        public void ChangeSelectedModel(ref GeometryModel3D selectModl, Material material) => selectModl.Material = material;

        public void changeMaterialToSelectedMouse() { ChangeSelectedModel(ref selectedGeometryModel, selectedModelMaterial); CanSelect = false; }

        public void changeMaterialToDefaultMouse() { ChangeSelectedModel(ref selectedGeometryModel, standartMaterial); CanSelect = true; }

        private void FillCollection(object sender, EventArgs e) => Elements = _modelService.FillModelElementsCollection(MyModel);

    }
}
