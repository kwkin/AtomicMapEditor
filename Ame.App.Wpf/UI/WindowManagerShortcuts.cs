using Ame.Infrastructure.Events;
using Ame.Infrastructure.Messages;
using Ame.Infrastructure.Models;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ame.App.Wpf.UI
{
    public class WindowManagerShortcuts
    {
        #region fields

        private IEventAggregator eventAggregator;
        private AmeSession session;

        #endregion fields


        #region constructor

        public WindowManagerShortcuts(IEventAggregator eventAggregator, AmeSession session)
        {
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator");
            this.session = session ?? throw new ArgumentNullException("session");

            this.NewProjectCommand = new DelegateCommand(() => NewProject());
            this.NewMapCommand = new DelegateCommand(() => NewMap());
            this.OpenProjectCommand = new DelegateCommand(() => OpenProject());
            this.OpenMapCommand = new DelegateCommand(() => OpenMap());
            this.SaveFileCommand = new DelegateCommand(() => SaveFile());
            this.SaveAsFileCommand = new DelegateCommand(() => SaveAsFile());
            this.ExportFileCommand = new DelegateCommand(() => ExportFile());
            this.ExportAsFileCommand = new DelegateCommand(() => ExportAsFile());
            this.CloseFileCommand = new DelegateCommand(() => CloseFile());
            this.UndoCommand = new DelegateCommand(() => Undo());
            this.RedoCommand = new DelegateCommand(() => Redo());
            this.CutCommand = new DelegateCommand(() => CutSelection());
            this.CopyCommand = new DelegateCommand(() => CopySelection());
            this.PasteCommand = new DelegateCommand(() => PasteClipboard());
            this.SampleViewCommand = new DelegateCommand(() => SampleView());
            this.ZoomInCommand = new DelegateCommand(() => ZoomIn());
            this.ZoomOutCommand = new DelegateCommand(() => ZoomOut());
            this.FitMapToWindowCommand = new DelegateCommand(() => FitMapToWindow());
        }

        #endregion constructor


        #region properties

        public ICommand NewProjectCommand { get; set; }
        public ICommand NewMapCommand { get; set; }
        public ICommand OpenProjectCommand { get; set; }
        public ICommand OpenMapCommand { get; set; }
        public ICommand SaveFileCommand { get; private set; }
        public ICommand SaveAsFileCommand { get; private set; }
        public ICommand ExportFileCommand { get; private set; }
        public ICommand ExportAsFileCommand { get; private set; }
        public ICommand CloseFileCommand { get; private set; }
        public ICommand UndoCommand { get; private set; }
        public ICommand RedoCommand { get; private set; }
        public ICommand CutCommand { get; private set; }
        public ICommand CopyCommand { get; private set; }
        public ICommand PasteCommand { get; private set; }
        public ICommand SampleViewCommand { get; private set; }
        public ICommand ZoomInCommand { get; private set; }
        public ICommand ZoomOutCommand { get; private set; }
        public ICommand FitMapToWindowCommand { get; private set; }

        #endregion properties


        #region methods

        public void NewProject()
        {
            Console.WriteLine("New project");
        }

        public void NewMap()
        {
            Console.WriteLine("New map");
        }

        public void OpenProject()
        {
            Console.WriteLine("Open project");
        }

        public void OpenMap()
        {
            Console.WriteLine("Open map");
        }

        public void SaveFile()
        {
            Console.WriteLine("Save file");
        }

        public void SaveAsFile()
        {
            Console.WriteLine("Save as file");
        }
        public void ExportFile()
        {
            Console.WriteLine("Export file");
        }

        public void ExportAsFile()
        {
            Console.WriteLine("Export as file");
        }

        public void CloseFile()
        {
            Console.WriteLine("Close");
        }

        public void Undo()
        {
            Console.WriteLine("Undo ");
        }

        public void Redo()
        {
            Console.WriteLine("Redo ");
        }

        public void CutSelection()
        {
            Console.WriteLine("Cut Selection");
        }

        public void CopySelection()
        {
            Console.WriteLine("Copy Selection");
        }

        public void PasteClipboard()
        {
            Console.WriteLine("Paste Clipboard");
        }

        public void SampleView()
        {
            Console.WriteLine("Sample View");
        }

        public void ZoomIn()
        {
            NotificationMessage<ViewNotification> message = new NotificationMessage<ViewNotification>(ViewNotification.ZoomInDocument);
            this.eventAggregator.GetEvent<NotificationEvent<ViewNotification>>().Publish(message);
        }

        public void ZoomOut()
        {
            NotificationMessage<ViewNotification> message = new NotificationMessage<ViewNotification>(ViewNotification.ZoomOutDocument);
            this.eventAggregator.GetEvent<NotificationEvent<ViewNotification>>().Publish(message);
        }

        public void FitMapToWindow()
        {
            Console.WriteLine("Fit Map To Window");
        }

        #endregion methods
    }
}
