using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Ame.Infrastructure.Core;
using Ame.Infrastructure.Events;
using ICSharpCode.AvalonEdit.Utils;
using Prism.Commands;
using Prism.Events;
using Xceed.Wpf.AvalonDock;

namespace Ame.Modules.Docks.Core
{
    public class DockLayoutViewModel
    {
        #region fields

        private const string LayoutFileName = "Layout.config";
        private ILayoutViewModel layoutParent = null;
        private IEventAggregator ea;

        #endregion fields


        #region constructor & destructer

        public DockLayoutViewModel(ILayoutViewModel parent, IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }
            this.layoutParent = parent;
            this.ea = eventAggregator;
        }

        #endregion constructor & destructer


        #region properties

        public string CurrentLayout { get; private set; }

        private DelegateCommand _SaveLayoutToStringCommand = null;
        public ICommand SaveLayoutToStringCommand
        {
            get
            {
                if (this._SaveLayoutToStringCommand == null)
                {
                    this._SaveLayoutToStringCommand = new DelegateCommand(
                        this.SaveLayoutToString,
                        () => !this.layoutParent.IsBusy);
                }
                return this._SaveLayoutToStringCommand;
            }
        }

        private DelegateCommand _LoadLayoutFromStringCommand = null;
        public ICommand LoadLayoutFromStringCommand
        {
            get
            {
                if (this._LoadLayoutFromStringCommand == null)
                {
                    this._LoadLayoutFromStringCommand = new DelegateCommand(
                        this.LoadLayoutFromString,
                        () => !this.layoutParent.IsBusy && !string.IsNullOrEmpty(this.CurrentLayout));
                }
                return this._LoadLayoutFromStringCommand;
            }
        }

        private DelegateCommand<object> _LoadLayoutCommand = null;
        public ICommand LoadLayoutCommand
        {
            get
            {
                if (this._LoadLayoutCommand == null)
                {
                    this._LoadLayoutCommand = new DelegateCommand<object>((p) =>
                    {
                        DockingManager docManager = p as DockingManager;
                        if (docManager == null)
                        {
                            return;
                        }

                        try
                        {
                            this.LoadLayoutFromFile();
                        }
                        catch
                        {
                        }
                        this.layoutParent.IsBusy = false;
                    });
                }
                return this._LoadLayoutCommand;
            }
        }

        private DelegateCommand<object> _SaveLayoutCommand = null;
        public ICommand SaveLayoutCommand
        {
            get
            {
                if (this._SaveLayoutCommand == null)
                {
                    this._SaveLayoutCommand = new DelegateCommand<object>((p) =>
                    {
                        string xmlLayout = p as string;
                        if (xmlLayout == null)
                        {
                            return;
                        }

                        this.SaveLayoutToFile(xmlLayout);
                    });
                }

                return this._SaveLayoutCommand;
            }
        }

        #endregion properties


        #region methods

        #region LoadLayout

        private void LoadLayoutFromString()
        {
            if (string.IsNullOrEmpty(this.CurrentLayout) == true)
            {
                return;
            }

            NotificationMessage<string> notification = new NotificationMessage<string>(
                this.CurrentLayout,
                MessageIds.LoadWorkspaceLayout);
            this.ea.GetEvent<NotificationEvent<string>>().Publish(notification);
        }

        private void LoadLayoutFromFile()
        {
            try
            {
                this.layoutParent.IsBusy = true;
                string layoutFile = Path.Combine(this.layoutParent.AppDataDirectory, DockLayoutViewModel.LayoutFileName);
                if (!File.Exists(layoutFile))
                {
                    throw new FileNotFoundException("Layout file not found");
                }

                Task taskToProcess = Task.Factory.StartNew<string>((stateObj) =>
                {
                    string xml = string.Empty;
                    try
                    {
                        using (FileStream fs = new FileStream(layoutFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                        {
                            using (StreamReader reader = FileReader.OpenStream(fs, Encoding.Default))
                            {
                                xml = reader.ReadToEnd();
                            }
                        }
                    }
                    catch (OperationCanceledException operationCanceledExp)
                    {
                        throw operationCanceledExp;
                    }
                    catch (Exception exp)
                    {
                        throw exp;
                    }
                    return xml;
                }, null).ContinueWith(ant =>
                {
                    try
                    {
                        Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            NotificationMessage<string> notification = new NotificationMessage<string>(ant.Result, MessageIds.LoadWorkspaceLayout);
                            this.ea.GetEvent<NotificationEvent<string>>().Publish(notification);
                        }),
                        DispatcherPriority.Background);
                    }
                    catch (AggregateException aggExp)
                    {
                        throw new Exception("Error loading layout.", aggExp);
                    }
                });
            }
            catch (FileNotFoundException fileNotFoundExp)
            {
                throw fileNotFoundExp;
            }
            catch (Exception exp)
            {
                throw exp;
            }
            finally
            {
                this.layoutParent.IsBusy = false;
            }
        }

        #endregion LoadLayout

        #region SaveLayout

        private void SaveLayoutToString()
        {
            NotificationActionMessage<string> notification = new NotificationActionMessage<string>(
                MessageIds.SaveWorkspaceLayout,
                (result) =>
                {
                    this.layoutParent.IsBusy = true;
                    CommandManager.InvalidateRequerySuggested();
                    this.CurrentLayout = result;
                    this.layoutParent.IsBusy = false;
                });
            this.ea.GetEvent<NotificationActionEvent<string>>().Publish(notification);
        }

        private void SaveLayoutToFile(string xmlLayout)
        {
            if (xmlLayout == null)
            {
                return;
            }

            string layoutFile = System.IO.Path.Combine(this.layoutParent.AppDataDirectory, DockLayoutViewModel.LayoutFileName);
            File.WriteAllText(layoutFile, xmlLayout);
        }

        #endregion SaveLayout

        #endregion methods
    }
}
